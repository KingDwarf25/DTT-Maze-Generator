using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTTMazeGenerator
{
    namespace Cameras
    {
        /// <summary>
        /// This class will zoom the camera in and out depending on the distance between 2 objects.
        /// </summary>
        [RequireComponent(typeof(Camera))]
        public class MultipleTargetCamera : MonoBehaviour
        {
            [Header("Targeting")]
            [SerializeField] Vector3 m_offset;
            List<Transform> m_targets;


            [Header("Smoothing")]
            [SerializeField] float m_smoothtime;

            [Header("Zooming")]
            [SerializeField] float m_minzoomfov;
            [SerializeField] float m_maxzoomfov;
            [SerializeField] float m_zoomlimiter;

            Vector3 m_velocity;
            Camera m_camera;

            void Awake()
            {
                m_camera = Camera.main;
                m_targets = new List<Transform>();
            }

            void LateUpdate()
            {
                if (m_targets.Count == 0)
                    return;

                MoveCamera();
                ZoomCamera();
            }

            /// <summary>
            /// Moves the camera to the new position it found using smoothing.
            /// </summary>
            void MoveCamera()
            {
                Vector3 centerpoint = GetCenterPoint();
                Vector3 newposition = centerpoint + m_offset;
                transform.position = Vector3.SmoothDamp(transform.position, newposition, ref m_velocity, m_smoothtime);
            }

            /// <summary>
            /// Zooms the camera to the calculated point.
            /// </summary>
            void ZoomCamera()
            {
                float newzoom = Mathf.Lerp(m_minzoomfov, m_maxzoomfov, GetGreatestDistance() / m_zoomlimiter);
                m_camera.orthographicSize = newzoom;
            }


            /// <summary>
            /// Calculates the view box boundry of multiple objects.
            /// </summary>
            /// <returns>Returns the box size</returns>
            float GetGreatestDistance()
            {
                Bounds bounds = new Bounds(m_targets[0].position, Vector3.zero);
                for (int t = 0; t < m_targets.Count; t++)
                {
                    bounds.Encapsulate(m_targets[t].position);
                }

                return bounds.size.x;
            }

            /// <summary>
            /// Gets the center point of the box boundry between multiple objects
            /// </summary>
            /// <returns>Returns the center of the box</returns>
            Vector3 GetCenterPoint()
            {
                if (m_targets.Count == 1) { return m_targets[0].position; }

                Bounds bounds = new Bounds(m_targets[0].position, Vector3.zero);
                for (int t = 0; t < m_targets.Count; t++)
                {
                    bounds.Encapsulate(m_targets[t].position);
                }

                return bounds.center;
            }

            public List<Transform> Targets { get { return m_targets; } }
        }
    }
}