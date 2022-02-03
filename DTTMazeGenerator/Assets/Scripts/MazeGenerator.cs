using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.MazeGeneration.Cells;
using DTTMazeGenerator.Cameras;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        enum ICellDirections
        {
            N, //0
            W, //1
            S, //2
            E  //3
        };

        public class MazeGenerator : MonoBehaviour
        {
            public static MazeGenerator Instance;

            [SerializeField] FrustrumCamera m_frustrumcamera;
            [SerializeField] GameObject m_cellprefab;

            //Different colors for visualisation
            [SerializeField] Color m_basiccellcolor;
            [SerializeField] Color m_currentcellcolor;
            [SerializeField] Color m_backtrackingcolor;
            [SerializeField] Color m_neighborcolor;
            [SerializeField] Color m_noneighborcolor;

            int m_maxgridsize = 250; //Maximum size of maze. Do not change, this is what the user wants.
            int m_wantedgridsizeX = 10, m_wantedgridsizeY = 10; //Minimum size of maze. Do not change, this is what the user wants.
            Vector2 m_currentgridsize;

            //Pool
            Queue<GameObject> m_cellobjects;

            //All variables for checking for neighbours.
            Cell[,] m_cellgrid;
            Cell m_currentcell;
            Stack<Cell> m_backtracking;
            List<Cell> m_cellswithoutneighbours;
            List<Cell> m_currentcellneighbours;

            bool m_isbacktracking;
            bool m_mazecompleted;
            float m_iterationmodifier;
            float m_iterationspeed;

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
                m_backtracking = new Stack<Cell>();
                m_cellswithoutneighbours = new List<Cell>();
                m_currentcellneighbours = new List<Cell>();
                m_cellobjects = new Queue<GameObject>();
                m_currentgridsize = new Vector2();

                InstantiateObjectpooling();
            }

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

            public void GenerateGrid()
            {
                if (m_currentgridsize.x != 0 || m_currentgridsize.y != 0)
                {
                    ResetGeneration();
                }

                m_currentgridsize.x = m_wantedgridsizeX;
                m_currentgridsize.y = m_wantedgridsizeY;
                CalculateIterationSpeed();

                StartCoroutine(EGenerateGrid());
            }

            void ResetGeneration()
            {
                StopAllCoroutines();
                m_mazecompleted = false;
                m_cellswithoutneighbours.Clear();
                m_backtracking.Clear();
                m_currentcellneighbours.Clear();
                m_frustrumcamera.SetBoundries(Vector2.zero);

                for (int x = 0; x < m_cellgrid.GetLength(0); x++)
                {
                    for (int y = 0; y < m_cellgrid.GetLength(1); y++)
                    {
                        if (m_cellgrid[x, y] != null)
                        {
                            m_cellgrid[x, y].ResetWalls();
                            m_cellgrid[x, y].SetColor(m_basiccellcolor);
                            m_cellgrid[x, y].HasBeenVisited = false;
                            m_cellobjects.Enqueue(m_cellgrid[x, y].gameObject);
                            m_cellgrid[x, y].gameObject.SetActive(false);
                            m_cellgrid[x, y] = null;
                        }
                    }
                }
                m_currentcell = null;
            }

            void CalculateIterationSpeed()
            {
                float steps = m_currentgridsize.x * m_currentgridsize.y;
                m_iterationspeed = 1f;

                //m_iterationspeed = 100 / steps;
                //At 625000 (250x250) steps it needs to have an iteration of 0.00001f.
                //At 100 (10x10) steps it needs to have an iteration of 1.
            }

            IEnumerator EGenerateGrid()
            {
                float cellsizex = m_cellprefab.transform.localScale.x;
                float cellsizey = m_cellprefab.transform.localScale.y;

                for (int x = 0; x < m_currentgridsize.x; x++)
                {
                    for (int y = 0; y < m_currentgridsize.y; y++)
                    {
                        GameObject cellobj = m_cellobjects.Dequeue();
                        cellobj.transform.position = new Vector3(x * cellsizex, 0, y * cellsizey);
                        cellobj.name = "cell_" + x + "_" + y;
                        cellobj.SetActive(true);

                        Cell cell = cellobj.GetComponent<Cell>();
                        cell.XCoordinate = x;
                        cell.YCoordinate = y;
                        m_cellgrid[x, y] = cell;
                        cell.SetColor(m_basiccellcolor);

                        if (m_currentgridsize.x < 14 && m_currentgridsize.y < 14)
                        {
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                }

                m_frustrumcamera.SetBoundries(m_currentgridsize);
                GenerateMaze();
                yield return null;
            }

            public void GenerateMaze()
            {
                StartCoroutine(EGenerateMazeRecursiveBacktracking(0, 0));
            }

            void CheckForNeighbors(ICellDirections _direction, int _x, int _y)
            {
                switch (_direction)
                {
                    case ICellDirections.E:
                        if (_x < m_currentgridsize.x - 1)
                        {
                            if (m_cellgrid[_x + 1, _y].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x + 1, _y]);
                                m_cellgrid[_x + 1, _y].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                    case ICellDirections.S:
                        if (_y > 0)
                        {
                            if (m_cellgrid[_x, _y - 1].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x, _y - 1]);
                                m_cellgrid[_x, _y - 1].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                    case ICellDirections.W:
                        if (_x > 0)
                        {
                            if (m_cellgrid[_x - 1, _y].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x - 1, _y]);
                                m_cellgrid[_x - 1, _y].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                    case ICellDirections.N:
                        if (_y < m_currentgridsize.y - 1)
                        {
                            if (m_cellgrid[_x, _y + 1].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x, _y + 1]);
                                m_cellgrid[_x, _y + 1].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                }
            }


            void RemoveWallsBetween(Cell _currentcell, Cell _checkingcell)
            {
                if (_currentcell.XCoordinate < _checkingcell.XCoordinate) //E
                {
                    _currentcell.EWall.SetActive(false);
                    _checkingcell.WWall.SetActive(false);
                }
                else if (_currentcell.XCoordinate > _checkingcell.XCoordinate) //W
                {
                    _currentcell.WWall.SetActive(false);
                    _checkingcell.EWall.SetActive(false);
                }
                else if (_currentcell.YCoordinate < _checkingcell.YCoordinate) //N
                {
                    _currentcell.NWall.SetActive(false);
                    _checkingcell.SWall.SetActive(false);
                }
                else if (_currentcell.YCoordinate > _checkingcell.YCoordinate) //S
                {
                    _currentcell.SWall.SetActive(false);
                    _checkingcell.NWall.SetActive(false);
                }
            }

            Cell ChooseNeighbor()
            {
                Cell neighbor = null;
                if (m_currentcellneighbours.Count > 0)
                {
                    neighbor = m_currentcellneighbours[Random.Range(0, m_currentcellneighbours.Count)];

                    foreach (Cell c in m_currentcellneighbours)
                    {
                        c.SetColor(m_basiccellcolor);
                    }

                    m_currentcellneighbours.Clear();
                }
                return neighbor;
            }

            IEnumerator EGenerateMazeRecursiveBacktracking(int _startposx, int _startposy)
            {
                m_currentcell = m_cellgrid[_startposx, _startposy];
                m_frustrumcamera.transform.position = new Vector3(m_currentcell.XCoordinate, m_frustrumcamera.transform.position.y, m_currentcell.YCoordinate);

                while (m_mazecompleted == false)
                {
                    m_currentcell.SetColor(m_currentcellcolor);
                    m_backtracking.Push(m_currentcell);

                    for (int d = 0; d < 4; d++)
                    {
                        CheckForNeighbors((ICellDirections)d, m_currentcell.XCoordinate, m_currentcell.YCoordinate);
                        yield return m_currentgridsize.x > 14 || m_currentgridsize.y > 14 ? new WaitForSeconds(m_iterationspeed * m_iterationmodifier / 6) : new WaitForSeconds(m_iterationspeed * m_iterationmodifier / 4);
                    }

                    Cell _checkingneighbor = ChooseNeighbor();
                    if (_checkingneighbor == null)
                    {
                        m_cellswithoutneighbours.Add(m_currentcell);
                        m_currentcell.HasBeenVisited = true;
                        m_currentcell.SetColor(m_noneighborcolor);
                        m_isbacktracking = true;
                    }
                    else
                    {
                        RemoveWallsBetween(m_currentcell, _checkingneighbor);
                        m_currentcell.HasBeenVisited = true;
                        m_currentcell = _checkingneighbor;
                    }

                    while (m_isbacktracking == true)
                    {
                        if (m_backtracking.Count > 0)
                        {
                            //Still some backtracking to do.
                            m_currentcell = m_backtracking.Pop();
                        }
                        else
                        {
                            //Backtracking complete.
                            m_isbacktracking = false;
                            m_mazecompleted = true;
                        }

                        for (int d = 0; d < 4; d++)
                        {
                            CheckForNeighbors((ICellDirections)d, m_currentcell.XCoordinate, m_currentcell.YCoordinate);
                            if (m_currentcellneighbours.Count > 0)
                            {
                                m_isbacktracking = false;
                                break;
                            }
                            else
                            {
                                m_currentcell.SetColor(m_noneighborcolor);
                                yield return new WaitForSeconds(m_iterationspeed * m_iterationmodifier / 8);
                            }
                        }
                    }

                    yield return m_currentgridsize.x > 14 || m_currentgridsize.y > 14 ? new WaitForSeconds(m_iterationspeed * m_iterationmodifier / 4) : new WaitForSeconds(m_iterationspeed * m_iterationmodifier);
                }

                yield return null;
            }

            public int MazeWidth { set { m_wantedgridsizeX = value; } }
            public int MazeHeight { set { m_wantedgridsizeY = value; } }
            public float IterationModifier { set { m_iterationmodifier = value; } }
            public bool IsMazeCompleted { get { return m_mazecompleted; } }

            public Cell CurrentCell { get { return m_currentcell; } }
        }
    }
}