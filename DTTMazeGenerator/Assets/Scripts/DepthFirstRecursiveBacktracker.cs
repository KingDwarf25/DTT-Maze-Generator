using DTTMazeGenerator.MazeGeneration.Cells;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        public class DepthFirstRecursiveBacktracker : MazeGenerator
        {
            [SerializeField] Color m_backtrackingcolor;
            
            Stack<Cell> m_backtracking;
            
            bool m_isbacktracking;

            protected override void Awake()
            {
                base.Awake();
                m_backtracking = new Stack<Cell>();
            }

            protected override IEnumerator EGenerateMazeAlgorithm(int _startposx, int _startposy)
            {
                m_currentcell = m_cellgrid[_startposx, _startposy];
                m_frustrumcamerapos.transform.position = new Vector3(m_currentcell.XCoordinate, m_frustrumcamerapos.transform.position.y, m_currentcell.YCoordinate);

                while (m_mazecompleted == false)
                {
                    m_currentcell.SetColor(m_currentcellcolor);
                    m_backtracking.Push(m_currentcell);

                    for (int d = 0; d < 4; d++)
                    {
                        CheckForNeighbors((ICellDirections)d, m_currentcell.XCoordinate, m_currentcell.YCoordinate);
                        yield return m_currentgridsize.x > 14 || m_currentgridsize.y > 14 ? new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 4) : new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 2);
                    }

                    Cell _checkingneighbor = ChooseNeighbor();
                    if (_checkingneighbor == null)
                    {
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
                                yield return new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 8);
                            }
                        }
                    }

                    if (m_currentgridsize.x < 14 && m_currentgridsize.y < 14) { new WaitForSeconds(m_iterationspeed * MazeManager.Instance.IterationModifier / 4); }
                }

                yield return null;
            }
        }
    }
}