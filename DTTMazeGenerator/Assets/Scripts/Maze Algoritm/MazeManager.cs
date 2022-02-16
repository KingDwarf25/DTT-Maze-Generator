using DTTMazeGenerator.MazeGeneration.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using DTTMazeGenerator.AR;
#endif
#if UNITY_STANDALONE_WIN
using DTTMazeGenerator.Cameras;
#endif


namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        /// <summary>
        /// Manages various functionalities that a maze uses, like grid generation and resetting the entire maze.
        /// </summary>
        public class MazeManager : MonoBehaviour
        {
            public static MazeManager Instance;

#if UNITY_STANDALONE_WIN
            FrustrumCamera m_frustrumcamera;
#endif
            [SerializeField] GameObject m_cellprefab;
            [SerializeField] Color m_basiccellcolor;
            

            readonly int m_maxgridsize = 250; //Maximum size of maze. Do not change the maximum value, this is what the user wants.
            int m_wantedgridsizeX = 10, m_wantedgridsizeY = 10; //Minimum size of maze. Do not change the minimum value, this is what the user wants.
            int m_wantedbeginpointX = 0, m_wantedbeginpointY = 0;

            //Pool
            Queue<GameObject> m_cellobjects;

            //Grid
            Cell[,] m_cellgrid;
            Vector2 m_currentgridsize;

            MazeGenerator m_mazegenerator;

            //Generation speed modifier
            float m_iterationmodifier;

#if UNITY_ANDROID
            ARManager m_ARmanager; 
            bool m_isgeneratinggrid;
#endif
            void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                }
                else
                {
                    Destroy(gameObject);
                }

                m_cellgrid = new Cell[m_maxgridsize, m_maxgridsize];
                m_cellobjects = new Queue<GameObject>();
                m_currentgridsize = new Vector2();

#if UNITY_STANDALONE_WIN
                m_frustrumcamera = FindObjectOfType<FrustrumCamera>();
#endif
#if UNITY_ANDROID
                m_ARmanager = FindObjectOfType<ARManager>();
#endif
                InstantiateObjectpooling();
            }

            /// <summary>
            /// Creates an object pool with cells equal to the maximum grid size.
            /// </summary>
            void InstantiateObjectpooling()
            {
                for (int x = 0; x < m_maxgridsize; x++)
                {
                    for (int y = 0; y < m_maxgridsize; y++)
                    {
                        GameObject cellobj = Instantiate(m_cellprefab);
                        cellobj.SetActive(false);
                        m_cellobjects.Enqueue(cellobj);
                    }
                }
            }

            /// <summary>
            /// Generates a grid and resets it if the grid is already made.
            /// </summary>
            public void GenerateGrid()
            {
                if (m_currentgridsize.x != 0 || m_currentgridsize.y != 0)
                {
                    ResetGeneration();
                }

                m_currentgridsize.x = m_wantedgridsizeX;
                m_currentgridsize.y = m_wantedgridsizeY;

                StartCoroutine(EGenerateGrid());
            }

            /// <summary>
            /// This Enumerater generates a grid with cells based on the scale set in the UI.
            /// </summary>
            //This depends if you are in Android or Windows. In android it will be and AR related grid that will be made at a point in AR space
            // and in Windows it will just start at zero. In android we also exclude any camera settings, cause we will use a different camera.
            IEnumerator EGenerateGrid()
            {
#if UNITY_ANDROID
                m_isgeneratinggrid = true;

#endif
                float cellsizex = m_cellprefab.transform.localScale.x;
                float cellsizey = m_cellprefab.transform.localScale.y;

                for (int x = 0; x < m_currentgridsize.x; x++)
                {
                    for (int y = 0; y < m_currentgridsize.y; y++)
                    {
                        GameObject cellobj = m_cellobjects.Dequeue();
#if UNITY_STANDALONE_WIN
                        cellobj.transform.position = new Vector3(x * cellsizex, 0, y * cellsizey);
#endif
#if UNITY_ANDROID
                        cellobj.transform.position = new Vector3(m_ARmanager.MazeStartPosition.x + x * cellsizex, m_ARmanager.MazeStartPosition.y, m_ARmanager.MazeStartPosition.z + y * cellsizey);
#endif
                        cellobj.name = "cell_" + x + "_" + y;
                        cellobj.SetActive(true);

                        Cell cell = cellobj.GetComponent<Cell>();
                        cell.XCoordinate = x;
                        cell.YCoordinate = y;
                        m_cellgrid[x, y] = cell;
                        cell.SetColor(m_basiccellcolor);
                        cell.CombineWallMeshes();

                        if (m_currentgridsize.x < 14 && m_currentgridsize.y < 14)
                        {
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                }
#if UNITY_STANDALONE_WIN
                m_frustrumcamera.SetBoundries(m_currentgridsize);
#endif
#if UNITY_ANDROID
                m_ARmanager.DisablePlaneDetection();
#endif
                m_mazegenerator.GenerateMaze();
                yield return null;
            }

            /// <summary>
            /// Will go through every cell resetting all aspects to make it reusable again for a new maze generation.
            /// </summary>
            void ResetGeneration()
            {
                StopAllCoroutines();

#if UNITY_STANDALONE_WIN
                m_frustrumcamera.SetBoundries(Vector2.zero);
#endif
                for (int x = 0; x < m_cellgrid.GetLength(0); x++)
                {
                    for (int y = 0; y < m_cellgrid.GetLength(1); y++)
                    {
                        if (m_cellgrid[x, y] != null)
                        {
                            m_cellgrid[x, y].ResetAllWalls();
                            m_cellgrid[x, y].RemoveCombinedMesh();
                            m_cellgrid[x, y].SetColor(m_basiccellcolor);
                            m_cellgrid[x, y].HasBeenVisited = false;
                            m_cellobjects.Enqueue(m_cellgrid[x, y].gameObject);
                            m_cellgrid[x, y].gameObject.SetActive(false);
                            m_cellgrid[x, y] = null;
                        }
                    }
                }
            }

            /// <summary>
            /// Changes the maze iteration speed according to a modifier.
            /// </summary>
            /// <param name="_modifier">The modifier for the speed of the iteration process</param>
            public void ChangeMazeIterationModifier(float _modifier)
            {
                m_iterationmodifier = _modifier;
            }

            /// <summary>
            /// Changes the generation algorithm used.
            /// </summary>
            /// <param name="_gen">The algorithm to use.</param>
            public void ChangeGenerationAlgorithm(MazeGenerator _gen)
            {
                m_mazegenerator = _gen;
            }

            /// <summary>
            /// Gets and sets the width of the maze. (This is filled in inside the UI)
            /// </summary>
            public int WantedMazeWidth { get { return m_wantedgridsizeX; } set { m_wantedgridsizeX = value; } }

            /// <summary>
            /// Gets and sets the height of the maze. (This is filled in inside the UI)
            /// </summary>
            public int WantedMazeHeight { get { return m_wantedgridsizeY; } set { m_wantedgridsizeY = value; } }

            /// <summary>
            /// Gets and sets the X coordinated where the generation will start from. (This is filled in inside the UI)
            /// </summary>
            public int WantedBeginPointX { get { return m_wantedbeginpointX; } set { m_wantedbeginpointX = value; } }

            /// <summary>
            /// Gets and sets the X coordinated where the generation will start from. (This is filled in inside the UI)
            /// </summary>
            public int WantedBeginPointY { get { return m_wantedbeginpointY; } set { m_wantedbeginpointY = value; } }

            /// <summary>
            /// Gets and returns the iteration modifier.
            /// </summary>
            public float IterationModifier { get { return m_iterationmodifier; } }

            /// <summary>
            /// Returns the iteration modifier (This is filled in inside the UI)
            /// </summary>
            public Color BasicCellColor { get { return m_basiccellcolor; } }

            /// <summary>
            /// Returns the entire grid of cells.
            /// </summary>
            public Cell[,] CellGrid { get { return m_cellgrid; } }

            /// <summary>
            /// Returns the boundries of the maze in grid coordinates
            /// </summary>
            public Vector2 GridBounds { get { return m_currentgridsize; } }

#if UNITY_ANDROID
            /// <summary>
            /// Returns if the grid is being generated. This is used to check if we can move the grid in AR
            /// </summary>
            public bool IsGeneratingGrid { get { return m_isgeneratinggrid; } }
#endif
        }
    }
}