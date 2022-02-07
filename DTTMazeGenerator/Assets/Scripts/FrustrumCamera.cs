using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTTMazeGenerator.MazeGeneration;
using TMPro;
using System;

namespace DTTMazeGenerator
{
    namespace Cameras
    {
        [RequireComponent(typeof(Camera))]
        public class FrustrumCamera : MonoBehaviour
        {
            [SerializeField] TMP_Dropdown m_cameramodesselection;
            [SerializeField] float m_cameramovespeed;
            [SerializeField] float m_zoomspeed;

            public enum ICameraMode
            {
                Following,
                TwoDimFreeform
            };

            Camera m_camera;
            ICameraMode m_cameramode;
            Vector2 m_currentboundries;

            void Start()
            {
                m_camera = Camera.main;
                m_cameramode = ICameraMode.TwoDimFreeform;
            }

            void LateUpdate()
            {
                switch (m_cameramode)
                {
                    case ICameraMode.Following:
                        if(MazeManager.Instance.CurrentCell != null)
                        m_camera.transform.position = new Vector3(MazeManager.Instance.CurrentCell.XCoordinate, 45, MazeManager.Instance.CurrentCell.YCoordinate);
                        break;

                    case ICameraMode.TwoDimFreeform:
                        Move2D();
                        break;
                }
            }

            public void SwitchCamera()
            {
                m_cameramode = (ICameraMode)m_cameramodesselection.value;
            }

            public void Move2D()
            {
                if (Input.GetAxis("Horizontal2D") != 0 || Input.GetAxis("Vertical2D") != 0 || Input.GetAxis("Zoom") != 0)
                {
                    float horizontalmovement = Input.GetAxis("Horizontal2D") * m_cameramovespeed;
                    float verticalmovement = Input.GetAxis("Vertical2D") * m_cameramovespeed;
                    float zoom = Input.GetAxis("Zoom") * m_zoomspeed;

                    transform.position = new Vector3(Mathf.Clamp(transform.position.x + horizontalmovement, 0, m_currentboundries.x), Mathf.Clamp(transform.position.y + zoom, 20, 60),
                                                     Mathf.Clamp(transform.position.z + verticalmovement, 0, m_currentboundries.y));
                }
            }

            public void SetBoundries(Vector2 _boundries)
            {
                m_currentboundries = _boundries;
            }
        }
    }
}