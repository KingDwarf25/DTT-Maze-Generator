using DTTMazeGenerator.MazeGeneration.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        /// <summary>
        /// This class will represent the hunt and kill search algorithm.
        /// </summary>
        //This algorithm will look for neighbors until it hasn't any unvisited anymore. It will then start from the top right and hunt all unvisited cells.
        //If one of these cells has a visited neighbor it will stop there and continue the maze generation.
        public class HuntAndKill : MazeGenerator
        {
            [SerializeField] Color m_huntcolor;
            [SerializeField] Color m_huntedcolor;

            bool m_ishunting;

            protected override void Awake()
            {
                base.Awake();
            }

            /// <summary>
            /// Overrides the reset to include its own hunting reset. 
            /// </summary>
            public override void ResetGeneration()
            {
                base.ResetGeneration();
                m_ishunting = false;
            }

            /// <summary>
            /// Enumerates generation of the maze using its own algoritm.
            /// </summary>
            /// <param name="_startposx">The X coordinate to start generation from</param>
            /// <param name="_startposy">The Y coordinate to start generation from</param>
            protected override IEnumerator EGenerateMazeAlgorithm(int _startposx, int _startposy)
            {
                m_currentcell = m_cellgrid[_startposx, _startposy];
#if UNITY_STANDALONE_WIN
                m_frustrumcamerapos.transform.position = new Vector3(m_currentcell.XCoordinate, m_frustrumcamerapos.transform.position.y, m_currentcell.YCoordinate);
#endif

                while (m_mazecompleted == false)
                {
                    m_currentcell.SetColor(m_currentcellcolor);

                    //Looks around the cell for any unvisited neighbors.
                    for (int d = 0; d < 4; d++)
                    {
                        CheckForNeighbors((ICellDirections)d, m_currentcell.XCoordinate, m_currentcell.YCoordinate);
                        yield return m_currentgridsize.x > 14 || m_currentgridsize.y > 14 ? new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 4) : new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 2);
                    }

                    //Gets a random neighbor
                    Cell checkingneighbor = ChooseNeighbor();

                    //If it has no neighbor it will start hunting, else it will just remove the walls between the two cells.
                    if (checkingneighbor == null)
                    {
                        m_currentcell.HasBeenVisited = true;
                        m_currentcell.SetColor(m_noneighborcolor);
                        m_ishunting = true;
                    }
                    else
                    {
                        RemoveWallsBetween(m_currentcell, checkingneighbor);
                        m_currentcell.HasBeenVisited = true;
                        m_currentcell = checkingneighbor;
                    }

                    //When we found a dead end we will start hunting for an unvisited cell that has a visited neigbor.
                    while (m_ishunting == true)
                    {
                        Cell huntingcell = null;

                        for (int y = (int)m_currentgridsize.y - 1; y >= 0; y--)
                        {
                            if (m_ishunting == false) { break; }

                            for (int x = 0; x < m_currentgridsize.x; x++)
                            {
                                if (huntingcell != null) { huntingcell.SetColor(m_huntedcolor); }
                                huntingcell = m_cellgrid[x, y];
                                huntingcell.SetColor(m_huntcolor);

                                //If we found a unvisited neighbor we are done and can break the cycle.
                                if (m_currentcellneighbors.Count > 0)
                                {
                                    Cell _foundcell = ChooseNeighbor();
                                    RemoveWallsBetween(m_currentcell, _foundcell);
                                    m_currentcell = _foundcell;
                                    m_ishunting = false;
                                    break;
                                }

                                if (huntingcell.HasBeenVisited == false)
                                {
                                    for (int d = 0; d < 4; d++)
                                    {
                                        HuntForNeighbor((ICellDirections)d, x, y);
                                    }
                                }


                                //If we are at the downright most cell and it has been visited than that means our maze is complete.
                                if(huntingcell.XCoordinate == m_currentgridsize.x - 1 && huntingcell.YCoordinate == 0 && huntingcell.HasBeenVisited == true)
                                {
                                    m_ishunting = false;
                                    m_mazecompleted = true;
                                    break;
                                }

                                if (m_currentgridsize.x < 14 && m_currentgridsize.y < 14) { new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 4); }
                            }
                        }
                    }

                    yield return null;
                }
            }

            /// <summary>
            /// Hunts for any neighbors that have already been visited.
            /// </summary>
            /// <param name="_direction">The direction to compare</param>
            /// <param name="_x">The X coordinate of the cell</param>
            /// <param name="_y">The Y coordinate of the cell</param>
            //Instead of looking for any unvisited cells the hunt and kill algorithm will look for visited cells next to an unvisited cell.
            void HuntForNeighbor(ICellDirections _direction, int _x, int _y)
            {
                switch (_direction)
                {
                    case ICellDirections.E:
                        if (_x < m_currentgridsize.x - 1)
                        {
                            if (m_cellgrid[_x + 1, _y].HasBeenVisited == true)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x + 1, _y]);
                            }
                        }
                        break;
                    case ICellDirections.S:
                        if (_y > 0)
                        {
                            if (m_cellgrid[_x, _y - 1].HasBeenVisited == true)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x, _y - 1]);
                            }
                        }
                        break;
                    case ICellDirections.W:
                        if (_x > 0)
                        {
                            if (m_cellgrid[_x - 1, _y].HasBeenVisited == true)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x - 1, _y]);
                            }
                        }
                        break;
                    case ICellDirections.N:
                        if (_y < m_currentgridsize.y - 1)
                        {
                            if (m_cellgrid[_x, _y + 1].HasBeenVisited == true)
                            {
                                m_currentcellneighbors.Add(m_cellgrid[_x, _y + 1]);
                            }
                        }
                        break;
                }
            }
        }
    }
}