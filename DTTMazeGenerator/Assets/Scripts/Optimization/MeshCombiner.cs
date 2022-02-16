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
                transform.localScale = Vector3.one;
                
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

                //Reset the position, rotation and scale to before matrix conversion. Else the object will be not be in the right state.
                transform.SetPositionAndRotation(position, rotation);
                m_meshfilter.transform.localScale = CalculateRescaling();
            }

            /// <summary>
            /// Removes the current combined mesh.
            /// </summary>
            public void RemoveMesh()
            {
                m_meshfilter.mesh = new Mesh();
            }

            /// <summary>
            /// Calculates a new scale relative to the parents scale.
            /// </summary>
            /// <returns>A Vector3 that represents a scale.</returns>
            Vector3 CalculateRescaling()
            {
               Vector3 parentscale = m_meshfilter.transform.parent.localScale;
               Vector3 newscale = new Vector3(1 / parentscale.x, 1 / parentscale.y, 1 / parentscale.z);
                Debug.Log(newscale);
                return newscale;
            }
        }
    }
}