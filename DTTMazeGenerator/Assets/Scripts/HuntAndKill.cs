using DTTMazeGenerator.MazeGeneration.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        public class HuntAndKill : MazeGenerator
        {
            [SerializeField] Color m_huntcolor;
            [SerializeField] Color m_huntedcolor;

            bool m_ishunting;

            protected override void Awake()
            {
                base.Awake();
            }

            public override void ResetGeneration()
            {
                base.ResetGeneration();
                m_ishunting = false;
            }

            protected override IEnumerator EGenerateMazeAlgorithm(int _startposx, int _startposy)
            {
                m_currentcell = m_cellgrid[_startposx, _startposy];
                m_frustrumcamerapos.transform.position = new Vector3(m_currentcell.XCoordinate, m_frustrumcamerapos.transform.position.y, m_currentcell.YCoordinate);

                while (m_mazecompleted == false)
                {
                    m_currentcell.SetColor(m_currentcellcolor);

                    for (int d = 0; d < 4; d++)
                    {
                        CheckForNeighbors((ICellDirections)d, m_currentcell.XCoordinate, m_currentcell.YCoordinate);
                        yield return m_currentgridsize.x > 14 || m_currentgridsize.y > 14 ? new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 4) : new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 2);
                    }

                    Cell checkingneighbor = ChooseNeighbor();
                    if (checkingneighbor == null)
                    {
                        //m_cellswithoutneighbours.Add(m_currentcell);
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

                                if (m_currentcellneighbours.Count > 0)
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

            void HuntForNeighbor(ICellDirections _direction, int _x, int _y)
            {
                switch (_direction)
                {
                    case ICellDirections.E:
                        if (_x < m_currentgridsize.x - 1)
                        {
                            if (m_cellgrid[_x + 1, _y].HasBeenVisited == true)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x + 1, _y]);
                            }
                        }
                        break;
                    case ICellDirections.S:
                        if (_y > 0)
                        {
                            if (m_cellgrid[_x, _y - 1].HasBeenVisited == true)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x, _y - 1]);
                            }
                        }
                        break;
                    case ICellDirections.W:
                        if (_x > 0)
                        {
                            if (m_cellgrid[_x - 1, _y].HasBeenVisited == true)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x - 1, _y]);
                            }
                        }
                        break;
                    case ICellDirections.N:
                        if (_y < m_currentgridsize.y - 1)
                        {
                            if (m_cellgrid[_x, _y + 1].HasBeenVisited == true)
                            {
                                m_currentcellneighbours.Add(m_cellgrid[_x, _y + 1]);
                            }
                        }
                        break;
                }
            }
        }
    }
}