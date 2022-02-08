using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.Optimization;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        namespace Cells
        {
            public class Cell : MonoBehaviour
            {
                /// <summary>
                /// In this array we will store all the walls. 
                /// North is [0]
                /// West is [1]
                /// South is [2]
                /// and East is [3]
                /// </summary>
                [SerializeField] GameObject[] m_walls;

                [SerializeField] Renderer m_renderer;
                [SerializeField] MeshCombiner m_meshcombiner;

                bool m_visited;
                int m_xcoordinate, m_ycoordinate;

                public void SetColor(Color _color)
                {
                    m_renderer.material.color = _color;
                }

                public void ResetAllWalls()
                {
                    for (int w = 0; w < m_walls.Length; w++)
                    {
                        if(m_walls[w].activeInHierarchy == false)
                        {
                            m_walls[w].SetActive(true);
                        }
                    }
                }

                public void CombineWallMeshes()
                {
                    m_meshcombiner.CombineMesh();
                }

                public void ResetAndRemoveMeshes()
                {
                    ResetAllWalls();
                    RemoveCombinedMesh();
                }

                public void RemoveCombinedMesh()
                {
                    m_meshcombiner.RemoveMesh();
                }

                public GameObject NWall { get { return m_walls[0]; } }
                public GameObject WWall { get { return m_walls[1]; } }
                public GameObject SWall { get { return m_walls[2]; } }
                public GameObject EWall { get { return m_walls[3]; } }

                public bool HasBeenVisited { get { return m_visited; } set { m_visited = value; } }
                public int XCoordinate { get { return m_xcoordinate; } set { m_xcoordinate = value; } }
                public int YCoordinate { get { return m_ycoordinate; } set { m_ycoordinate = value; } }
            }
        }
    }
}
