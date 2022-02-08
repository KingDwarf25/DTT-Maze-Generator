using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace Optimization
    {
        [RequireComponent(typeof(MeshFilter))]
        [RequireComponent(typeof(MeshRenderer))]
        public class MeshCombiner : MonoBehaviour
        {
            MeshFilter m_meshfilter;

            void Awake()
            {
                m_meshfilter = GetComponent<MeshFilter>();
            }

            void Update()
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    CombineMesh();
                }
            }

            public void CombineMesh()
            {
                Vector3 position = transform.position;
                transform.position = Vector3.zero;


                MeshFilter[] meshfilters = GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshfilters.Length];

                for (int f = 0; f < meshfilters.Length; f++)
                {
                    if (meshfilters[f].mesh != null)
                    {
                        combine[f].mesh = meshfilters[f].sharedMesh;
                        combine[f].transform = meshfilters[f].transform.localToWorldMatrix;
                        meshfilters[f].gameObject.SetActive(false);
                    }
                }

                MeshFilter meshfilter = transform.GetComponent<MeshFilter>();
                meshfilter.mesh = new Mesh();
                meshfilter.mesh.CombineMeshes(combine, true, true);

                transform.gameObject.SetActive(true);

                //Reset Position
                transform.position = position;

                //Rotate 90x degrees so that the filter lies flat on the quad.
                transform.Rotate(90, 0, 0);
            }

            public void RemoveMesh()
            {
                m_meshfilter.mesh = new Mesh();
            }
        }
    }
}