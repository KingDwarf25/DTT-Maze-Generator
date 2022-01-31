using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.MazeGeneration.Cells;

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
            [SerializeField] int m_maxgridsizeX, m_maxgridsizeY;
            [SerializeField] GameObject m_cellprefab;
            [SerializeField] Color m_checkedcolor;

            //All variables for checking for neighbours.
            Cell[,] m_cellgrid;
            Cell m_currentcell;
            Stack<Cell> m_backtracking;
            List<Cell> m_cellswithoutneighbours;
            List<Cell> m_currentcellneighbours;

            bool m_isbacktracking;
            bool m_mazecompleted;

            void Start()
            {
                m_cellgrid = new Cell[m_maxgridsizeX, m_maxgridsizeY];
                m_backtracking = new Stack<Cell>();
                m_cellswithoutneighbours = new List<Cell>();
                m_currentcellneighbours = new List<Cell>();
                GenerateGrid();
            }

            void GenerateGrid()
            {
                float cellsizex = m_cellprefab.transform.localScale.x;
                float cellsizey = m_cellprefab.transform.localScale.y;

                for (int x = 0; x < m_maxgridsizeX; x++)
                {
                    for (int y = 0; y < m_maxgridsizeY; y++)
                    {
                        GameObject cellobj = Instantiate(m_cellprefab);
                        cellobj.transform.position = new Vector3(x * cellsizex, 0, y * cellsizey);
                        cellobj.name = "cell_" + x + "_" + y;

                        Cell cell = cellobj.GetComponent<Cell>();
                        cell.XCoordinate = x;
                        cell.YCoordinate = y;
                        m_cellgrid[x, y] = cell;
                    }
                }
            }

            public void GenerateMaze()
            {
                StartCoroutine(EGenerateMaze(0, 0));
            }

            void CheckForNeighbors(ICellDirections _direction, int _x, int _y)
            {
                switch (_direction)
                {
                    case ICellDirections.E:
                        if(_x < m_maxgridsizeX - 1)
                        {
                            if(m_cellgrid[_x + 1, _y].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x + 1, _y]);
                            }
                        }
                        break;
                    case ICellDirections.W:
                        if (_y > 0)
                        {
                            if(m_cellgrid[_x, _y - 1].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x, _y - 1]);
                            }
                        }
                        break;
                    case ICellDirections.S:
                        if(_x > 0)
                        {
                            if (m_cellgrid[_x - 1, _y].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x - 1, _y]);
                            }
                        }
                        break;
                    case ICellDirections.N:
                        if (_y < m_maxgridsizeY - 1)
                        {
                            if (m_cellgrid[_x, _y + 1].HasBeenVisited == false)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x, _y + 1]);
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
                else if(_currentcell.XCoordinate > _checkingcell.XCoordinate) //W
                {
                    _currentcell.WWall.SetActive(false);
                    _checkingcell.EWall.SetActive(false);
                }
                else if(_currentcell.YCoordinate < _checkingcell.YCoordinate) //N
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
                    m_currentcellneighbours.Clear();
                }
                return neighbor;
            }

            IEnumerator EGenerateMaze(int _startposx, int _startposy)
            {
                m_currentcell = m_cellgrid[_startposx, _startposy];

                while (m_mazecompleted == false)
                {
                    m_backtracking.Push(m_currentcell);

                    for (int d = 0; d < 4; d++)
                    {
                        CheckForNeighbors((ICellDirections)d, m_currentcell.XCoordinate, m_currentcell.YCoordinate);
                    }

                    Cell _checkingneighbor = ChooseNeighbor();
                    if (_checkingneighbor == null) 
                    { 
                        Debug.Log("This one has no unchecked neighbors. Let's start backtracking!");
                        m_cellswithoutneighbours.Add(m_currentcell);
                        m_currentcell.HasBeenVisited = true;
                        m_isbacktracking = true;
                    }
                    else
                    {
                        RemoveWallsBetween(m_currentcell, _checkingneighbor);
                        m_currentcell.HasBeenVisited = true;
                        m_currentcell.SetMaterialColor(m_checkedcolor);
                        m_currentcell = _checkingneighbor;
                    }

                    while(m_isbacktracking == true)
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
                            if(m_currentcellneighbours.Count > 0) 
                            { 
                                m_isbacktracking = false; 
                                break;
                            }
                        }
                    }

                    yield return new WaitForSeconds(0.2f);
                }

                Debug.Log("Done");
                yield return null;
            }

            //void RearrangeWall(GameObject _cell, GameObject _wall, WallArrangement _arrangement, int _wallindex)
            //{
            //    _wall.transform.Rotate(0, _wallindex * 90, 0);

            //    switch (_arrangement)
            //    {
            //        case WallArrangement.Left:
            //            _wall.transform.localScale = new Vector3(_wall.transform.localScale.x, _wall.transform.localScale.y, _cell.transform.localScale.x);
            //            _wall.transform.position = new Vector3(_cell.transform.position.x - _cell.transform.localScale.x / 2, 0, _cell.transform.position.y - _cell.transform.localScale.y / 2);
            //            break;

            //        case WallArrangement.Up:
            //            _wall.transform.localScale = new Vector3(_cell.transform.localScale.x, _wall.transform.localScale.y, _wall.transform.localScale.z);
            //            _wall.transform.position = new Vector3(_cell.transform.position.x - _cell.transform.localScale.x / 2, 0, _cell.transform.position.y + _cell.transform.localScale.y / 2);
            //            break;

            //        case WallArrangement.Right:
            //            _wall.transform.localScale = new Vector3(_wall.transform.localScale.x, _wall.transform.localScale.y, _cell.transform.localScale.x);
            //            _wall.transform.position = new Vector3(_cell.transform.position.x + _cell.transform.localScale.x / 2, 0, _cell.transform.position.y + _cell.transform.localScale.y / 2);
            //            break;

            //        case WallArrangement.Down:
            //            _wall.transform.localScale = new Vector3(_cell.transform.localScale.x, _wall.transform.localScale.y, _wall.transform.localScale.z);
            //            _wall.transform.position = new Vector3(_cell.transform.position.x + _cell.transform.localScale.x / 2, 0, _cell.transform.position.y - _cell.transform.localScale.y / 2);
            //            break;
            //    }
            //}
        }
    }
}