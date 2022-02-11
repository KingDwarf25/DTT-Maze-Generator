using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace Optimization
    {
        /// <summary>
        /// This class combines different meshes from object into one mesh. Saving a lot of frames.
        /// </summary>
        [RequireComponent(typeof(MeshFilter))]
        [RequireComponent(typeof(MeshRenderer))]
        public class MeshCombiner : MonoBehaviour
        {
            MeshFilter m_meshfilter;

            void Awake()
            {
                m_meshfilter = GetComponent<MeshFilter>();
            }

            /// <summary>
            /// Combines all meshes inside a list to form one mesh. It then disables the objects used for combination.
            /// </summary>
            /// <param name="_tocombine">List of objects to combine into one mesh</param>
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

                //Reset the position and rotation to before matrix conversion. Else the object will be rotated.
                transform.SetPositionAndRotation(position, rotation);
            }

            /// <summary>
            /// Removes the current combined mesh.
            /// </summary>
            public void RemoveMesh()
            {
                m_meshfilter.mesh = new Mesh();
            }
        }
    }
}