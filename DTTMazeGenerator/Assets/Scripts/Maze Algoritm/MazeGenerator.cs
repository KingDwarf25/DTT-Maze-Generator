using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.MazeGeneration.Cells;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        /// <summary>
        /// This is the base class for all current mazegenerators. This class processes the generation of the maze.
        /// </summary>
        public class MazeGenerator : MonoBehaviour
        {
            /// <summary>
            /// The different directions the algorithm can search.
            /// </summary>
            protected enum ICellDirections
            {
                N, //0
                W, //1
                S, //2
                E  //3
            };

            //Different colors for visualisation
            [SerializeField] protected Color m_currentcellcolor;
            [SerializeField] protected Color m_neighborcolor;
            [SerializeField] protected Color m_noneighborcolor;

            [SerializeField] protected Transform m_frustrumcamerapos;

            protected Cell[,] m_cellgrid;

            //All variables for checking for neighbours.
            protected Cell m_currentcell;
            protected List<Cell> m_currentcellneighbors;

            protected float m_iterationspeed;
           
            protected bool m_mazecompleted;

            protected Vector2 m_currentgridsize;
            
            protected float m_iterationmodifier;

            Color m_basiccellcolor;

            protected virtual void Awake()
            {
                m_currentcellneighbors = new List<Cell>();
            }

            /// <summary>
            /// A virtual function that resets the generation.
            /// </summary>
            public virtual void ResetGeneration()
            {
                StopAllCoroutines();
                m_currentcellneighbors.Clear();
                m_currentcell = null;
                m_mazecompleted = false;
            }

            /// <summary>
            /// This will call the enumerator of the mazegenerator to start.
            /// </summary>
            public void GenerateMaze()
            {
                SetValues();
                CalculateIterationSpeed();
                StartCoroutine(EGenerateMazeAlgorithm((int)MazeManager.Instance.WantedBeginPointX, (int)MazeManager.Instance.WantedBeginPointY));
            }

            /// <summary>
            /// Calculates the iteration speed of based on how big the maze is.
            /// For the time being. This will be 1.
            /// </summary>
            public void CalculateIterationSpeed()
            {
                m_iterationspeed = 1f;
            }

            /// <summary>
            /// Sets a couple of values necessary for the maze generator.
            /// </summary>
            void SetValues()
            {
                m_currentgridsize = MazeManager.Instance.GridBounds;
                m_cellgrid = MazeManager.Instance.CellGrid;
                m_basiccellcolor = MazeManager.Instance.BasicCellColor;
            }

            /// <summary>
            /// Checks around a cell and only sees a cell as a neighbor when it has not been visited yet.
            /// </summary>
            /// <param name="_direction">The direction to compare</param>
            /// <param name="_x">The X coordinate of the cell</param>
            /// <param name="_y">The Y coordinate of the cell</param>
            protected void CheckForNeighbors(ICellDirections _direction, int _x, int _y) 
            {
                switch (_direction)
                {
                    case ICellDirections.E:
                        if (_x < m_currentgridsize.x - 1)
                        {
                            if (m_cellgrid[_x + 1, _y].HasBeenVisited == false)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x + 1, _y]);
                                m_cellgrid[_x + 1, _y].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                    case ICellDirections.S:
                        if (_y > 0)
                        {
                            if (m_cellgrid[_x, _y - 1].HasBeenVisited == false)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x, _y - 1]);
                                m_cellgrid[_x, _y - 1].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                    case ICellDirections.W:
                        if (_x > 0)
                        {
                            if (m_cellgrid[_x - 1, _y].HasBeenVisited == false)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x - 1, _y]);
                                m_cellgrid[_x - 1, _y].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                    case ICellDirections.N:
                        if (_y < m_currentgridsize.y - 1)
                        {
                            if (m_cellgrid[_x, _y + 1].HasBeenVisited == false)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x, _y + 1]);
                                m_cellgrid[_x, _y + 1].SetColor(m_neighborcolor);
                            }
                        }
                        break;
                }
            }
            
            /// <summary>
            /// Removes the combined mesh of all walls and removes walls between two cells. Then it will combine them into a single mesh again.
            /// </summary>
            /// <param name="_currentcell">The current cell your on.</param>
            /// <param name="_checkingcell">The cell the algoritm has found.</param>
            protected void RemoveWallsBetween(Cell _currentcell, Cell _checkingcell)
            {
                _currentcell.ResetAndRemoveMeshes();
                _checkingcell.ResetAndRemoveMeshes();

                if (_currentcell.XCoordinate < _checkingcell.XCoordinate) //E
                {
                    _currentcell.RemoveWall(_currentcell.EWall);
                    _checkingcell.RemoveWall(_checkingcell.WWall);
                }
                else if (_currentcell.XCoordinate > _checkingcell.XCoordinate) //W
                {
                    _currentcell.RemoveWall(_currentcell.WWall);
                    _checkingcell.RemoveWall(_checkingcell.EWall);
                }
                else if (_currentcell.YCoordinate < _checkingcell.YCoordinate) //N
                {
                    _currentcell.RemoveWall(_currentcell.NWall);
                    _checkingcell.RemoveWall(_checkingcell.SWall);
                }
                else if (_currentcell.YCoordinate > _checkingcell.YCoordinate) //S
                {
                    _currentcell.RemoveWall(_currentcell.SWall);
                    _checkingcell.RemoveWall(_checkingcell.NWall);
                }

                _currentcell.CombineWallMeshes();
                _checkingcell.CombineWallMeshes();
            }

            /// <summary>
            /// Chooses a random neighbor from a list of found neighbors.
            /// </summary>
            /// <returns>Returns a cell</returns>
            protected Cell ChooseNeighbor()
            {
                Cell neighbor = null;
                if (m_currentcellneighbors.Count > 0)
                {
                    neighbor = m_currentcellneighbors[Random.Range(0, m_currentcellneighbors.Count)];

                    foreach (Cell c in m_currentcellneighbors)
                    {
                        c.SetColor(m_basiccellcolor);
                    }

                    m_currentcellneighbors.Clear();
                }
                return neighbor;
            }

            /// <summary>
            /// A virutal enumerator that will generate the maze using its algoritm.
            /// </summary>
            /// <param name="_startposx">The X coordinate to start the algoritm from</param>
            /// <param name="_startposy">The Y coordinate to start the algoritm from</param>
            protected virtual IEnumerator EGenerateMazeAlgorithm(int _startposx, int _startposy)
            {
                yield return null;
            }

            public Cell CurrentCell { get { return m_currentcell; } }
        }
    }
}