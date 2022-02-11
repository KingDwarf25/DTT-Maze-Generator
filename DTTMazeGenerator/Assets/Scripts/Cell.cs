using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.Optimization;
using System.Linq;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        namespace Cells
        {
            /// <summary>
            /// The whole maze is made up by cells. These cells hold information about themselfs like their walls, walls that already have been removed and
            /// their coordinates.
            /// </summary>
            public class Cell : MonoBehaviour
            {
                /// <summary>
                /// In this array we will store all the walls this cell will have at the beginning. 
                /// North is [0]
                /// West is [1]
                /// South is [2]
                /// and East is [3]
                /// </summary>
                [SerializeField] GameObject[] m_walls;
                [SerializeField] MeshCombiner m_meshcombiner;

                List<GameObject> m_algoritmwalls; //This is where we will store walls relative to the algoritm.

                Renderer m_renderer;
                bool m_visited;
                int m_xcoordinate, m_ycoordinate;

                void Awake()
                {
                    m_algoritmwalls = new List<GameObject>();
                    m_algoritmwalls = m_walls.ToList();
                    m_renderer = GetComponent<Renderer>();
                }


                /// <summary>
                /// Sets the color of this material for visualisation.
                /// </summary>
                /// <param name="_color">A color of choice.</param>
                public void SetColor(Color _color)
                {
                    m_renderer.material.color = _color;
                }

                /// <summary>
                /// Resets the current state of walls back to the beginning.
                /// </summary>
                public void ResetAllWalls()
                {
                    m_algoritmwalls.Clear();

                    for (int w = 0; w < m_walls.Length; w++)
                    {
                        if(m_walls[w].activeInHierarchy == false)
                        {
                            m_walls[w].SetActive(true);
                        }
                    }

                    m_algoritmwalls = m_walls.ToList();
                }

                /// <summary>
                /// Will set all remaining walls active for meshcombination
                /// </summary>
                public void ResetWallsForMeshCombination()
                {
                    for (int w = 0; w < m_algoritmwalls.Count; w++)
                    {
                        if (m_algoritmwalls[w].activeInHierarchy == false)
                        {
                            m_algoritmwalls[w].SetActive(true);
                        }
                    }
                }

                /// <summary>
                /// Calls the meshcombiner to comine all meshes
                /// </summary>
                /// <see href="https://github.com/KingDwarf25/DTT-Maze-Generator/blob/MazeGen_Bonus/DTTMazeGenerator/Assets/Scripts/MeshCombiner.cs">MeshCombiner</see>
                public void CombineWallMeshes()
                {
                    m_meshcombiner.CombineMesh(m_algoritmwalls);
                }

                /// <summary>
                /// Resets the algoritm kept walls and removes the combined mesh it previously made.
                /// </summary>
                public void ResetAndRemoveMeshes()
                {
                    ResetWallsForMeshCombination();
                    RemoveCombinedMesh();
                }

                /// <summary>
                /// Calls the meshcombiner to remove the current combined mesh.
                /// </summary>
                /// <see href="https://github.com/KingDwarf25/DTT-Maze-Generator/blob/MazeGen_Bonus/DTTMazeGenerator/Assets/Scripts/MeshCombiner.cs">MeshCombiner</see>
                public void RemoveCombinedMesh()
                {
                    m_meshcombiner.RemoveMesh();
                }

                /// <summary>
                /// Removes a wall from the algoritm list.
                /// </summary>
                /// <param name="_wall">The wall that will be removed</param>
                public void RemoveWall(GameObject _wall)
                {
                    _wall.SetActive(false);
                    m_algoritmwalls.Remove(_wall);
                }

                /// <summary>
                /// Returns the upper wall of this cell.
                /// </summary>
                public GameObject NWall { get { return m_walls[0]; } }

                /// <summary>
                /// Returns the left most wall of this cell.
                /// </summary>
                public GameObject WWall { get { return m_walls[1]; } }

                /// <summary>
                /// Returns the lower wall of this cell.
                /// </summary>
                public GameObject SWall { get { return m_walls[2]; } }

                /// <summary>
                /// Returns the right most wall of this cell.
                /// </summary>
                public GameObject EWall { get { return m_walls[3]; } }

                /// <summary>
                /// Gets or sets if the cell has been visited by the algorithm.
                /// </summary>
                public bool HasBeenVisited { get { return m_visited; } set { m_visited = value; } }

                /// <summary>
                /// Gets or sets the current X Coordinate of this cell.
                /// </summary>
                public int XCoordinate { get { return m_xcoordinate; } set { m_xcoordinate = value; } }

                /// <summary>
                /// Gets or sets the current Y Coordinate of this cell.
                /// </summary>
                public int YCoordinate { get { return m_ycoordinate; } set { m_ycoordinate = value; } }
            }
        }
    }
}
