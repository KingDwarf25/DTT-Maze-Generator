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

            public void CombineMesh(List<GameObject> _tocombine)
            {
                Vector3 position = transform.position;
                Quaternion rotation = transform.rotation;
                
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;

                CombineInstance[] combine = new CombineInstance[_tocombine.Count];

                for (int f = 0; f < _tocombine.Count; f++)
                {
                    MeshFilter filter = _tocombine[f].GetComponent<MeshFilter>();
                    combine[f].mesh = filter.sharedMesh;
                    combine[f].transform = filter.transform.localToWorldMatrix;
                    _tocombine[f].gameObject.SetActive(false);
                }

                MeshFilter meshfilter = transform.GetComponent<MeshFilter>();
                meshfilter.mesh = new Mesh();
                meshfilter.mesh.CombineMeshes(combine, true, true);

                transform.gameObject.SetActive(true);

                //Reset Position
                //transform.position = position;

                //Rotate 90x degrees so that the filter lies flat on the quad.
                transform.SetPositionAndRotation(position, rotation);
            }

            public void RemoveMesh()
            {
                m_meshfilter.mesh = new Mesh();
            }
        }
    }
}