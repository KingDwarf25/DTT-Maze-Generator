using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using DTTMazeGenerator.MazeGeneration;

namespace DTTMazeGenerator
{
    namespace AR
    {
        [RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
        public class ARManager : MonoBehaviour
        {
#if UNITY_ANDROID
            [SerializeField] GameObject m_targetobject;
            [SerializeField] Button m_generatebutton;

            GameObject m_spawnedtarget;
            Vector3 m_mazestartposition;
            ARRaycastManager m_ARraycastmanager;
            ARPlaneManager m_planemanager;

            Vector2 m_touchpositionscreen;

            List<ARRaycastHit> m_ARraycasthits;

            void Awake()
            {
                m_ARraycastmanager = GetComponent<ARRaycastManager>();
                m_planemanager = GetComponent<ARPlaneManager>();
                m_ARraycasthits = new List<ARRaycastHit>();
            }

            void GetTouchPosition()
            {
                m_touchpositionscreen = Input.GetTouch(0).position;
            }

            void Update()
            {
                if (Input.touchCount == 0 || MazeManager.Instance.IsGeneratingGrid == true) { return; }
                GetTouchPosition();

                if (m_ARraycastmanager.Raycast(m_touchpositionscreen, m_ARraycasthits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitpose = m_ARraycasthits[0].pose;

                    if (m_spawnedtarget == null)
                    {
                        m_spawnedtarget = Instantiate(m_targetobject, hitpose.position, hitpose.rotation);
                        m_generatebutton.interactable = true;
                    }
                    else
                    {
                        m_spawnedtarget.transform.position = hitpose.position;
                    }

                    m_mazestartposition = m_spawnedtarget.transform.position;
                }
            }

            public void DisablePlaneDetection()
            {
                m_planemanager.SetTrackablesActive(false);
                m_planemanager.requestedDetectionMode = PlaneDetectionMode.None;
                m_spawnedtarget.SetActive(false);
            }

            public Vector3 MazeStartPosition { get { return m_mazestartposition; } }
#endif
        }
    }
}
