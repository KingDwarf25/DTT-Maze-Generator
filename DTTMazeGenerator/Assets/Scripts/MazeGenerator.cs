using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.MazeGeneration.Cells;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        public class MazeGenerator : MonoBehaviour
        {
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

            protected Cell[,] m_cellgrid;

            //All variables for checking for neighbours.
            protected Cell m_currentcell;
            //protected List<Cell> m_cellswithoutneighbours;
            protected List<Cell> m_currentcellneighbours;

            protected float m_iterationspeed;
           
            protected bool m_mazecompleted;

            protected Vector2 m_currentgridsize;
            protected Transform m_frustrumcamerapos;
            protected float m_iterationmodifier;

            Color m_basiccellcolor;

            protected virtual void Awake()
            {
                //m_cellswithoutneighbours = new List<Cell>();
                m_currentcellneighbours = new List<Cell>();
            }

            public void ResetGeneration()
            {
                StopAllCoroutines();
                //m_cellswithoutneighbours.Clear();
                m_currentcellneighbours.Clear();
                m_currentcell = null;
                m_mazecompleted = false;
            }

            public void GenerateMaze()
            {
                SetValues();
                CalculateIterationSpeed();
                StartCoroutine(EGenerateMazeAlgorithm((int)MazeManager.Instance.WantedBeginPointX, (int)MazeManager.Instance.WantedBeginPointY));
            }

            public void CalculateIterationSpeed()
            {
                m_iterationspeed = 1f;
            }

            void SetValues()
            {
                m_currentgridsize = MazeManager.Instance.GridBounds;
                m_cellgrid = MazeManager.Instance.CellGrid;
                m_basiccellcolor = MazeManager.Instance.BasicCellColor;
                m_frustrumcamerapos = MazeManager.Instance.FrustCameraPos;
            }

            protected void CheckForNeighbors(ICellDirections _direction, int _x, int _y) 
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

            protected Cell ChooseNeighbor()
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

            protected virtual IEnumerator EGenerateMazeAlgorithm(int _startposx, int _startposy)
            {
                yield return null;
            }

            public Cell CurrentCell { get { return m_currentcell; } }
        }
    }
}