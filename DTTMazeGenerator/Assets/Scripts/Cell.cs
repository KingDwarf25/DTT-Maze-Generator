using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace MazeGeneration
    {
        namespace Cells
        {
            public class Cell : MonoBehaviour
            {
                [SerializeField] GameObject m_northwall;
                [SerializeField] GameObject m_westwall;
                [SerializeField] GameObject m_southwall;
                [SerializeField] GameObject m_eastwall;

                bool m_visited;
                int m_xcoordinate, m_ycoordinate;

                public GameObject NWall { get { return m_northwall; } }
                public GameObject WWall { get { return m_westwall; } }
                public GameObject SWall { get { return m_southwall; } }
                public GameObject EWall { get { return m_eastwall; } }

                public bool HasBeenVisited { get { return m_visited; } set { m_visited = value; } }
                public int XCoordinate { get { return m_xcoordinate; } set { m_xcoordinate = value; } }
                public int YCoordinate { get { return m_ycoordinate; } set { m_ycoordinate = value; } }

                public void SetMaterialColor(Color _color)
                {
                    GetComponent<Renderer>().material.SetColor("Color", _color);
                }
            }
        }
    }
}
