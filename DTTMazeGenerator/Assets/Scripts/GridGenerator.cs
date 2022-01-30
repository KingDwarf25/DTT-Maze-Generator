using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace GridGeneration
    {
        public class GridGenerator : MonoBehaviour
        {
            [SerializeField] int m_maxgridsizeX, m_maxgridsizeY;
            [SerializeField] GameObject m_cellprefab;

            void Start()
            {
                GenerateGrid();
            }

            void GenerateGrid()
            {
                float cellsizex = m_cellprefab.transform.localScale.x;
                float cellsizey = m_cellprefab.transform.localScale.y;

                for (int y = 0; y < m_maxgridsizeY; y++)
                {
                    for (int x = 0; x < m_maxgridsizeX; x++)
                    {
                        GameObject cell = Instantiate(m_cellprefab);
                        cell.transform.position = new Vector3(x * cellsizex, 0, y * cellsizey);
                    }
                }
            }
        }
    }
}