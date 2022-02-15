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
        /// <summary>
        /// This class handles the standard frustrum camera from Unity in a more fancier way.
        /// </summary>
        [RequireComponent(typeof(Camera))]
        public class FrustrumCamera : MonoBehaviour
        {
            enum ICameraMode //The different camera modes you can choose from.
            {
                Following, //This will follow the current cell of the iteration.
                TwoDimFreeform //This will let the user move the camera.
            };

            [SerializeField] TMP_Dropdown m_cameramodesselection;
            [SerializeField] float m_cameramovespeed;
            [SerializeField] float m_zoomspeed;

            MazeGenerator m_mazegenerator;
            Camera m_camera;
            ICameraMode m_cameramode;
            Vector2 m_currentboundries;

            void Start()
            {
                m_camera = Camera.main;
                m_cameramode = ICameraMode.TwoDimFreeform;
                m_mazegenerator = FindObjectOfType<MazeGenerator>();
            }

            void LateUpdate()
            {
                switch (m_cameramode)
                {
                    case ICameraMode.Following:
                        if(m_mazegenerator.CurrentCell != null)
                        m_camera.transform.position = new Vector3(m_mazegenerator.CurrentCell.XCoordinate, 45, m_mazegenerator.CurrentCell.YCoordinate);
                        break;

                    case ICameraMode.TwoDimFreeform:
                        Move2D();
                        break;
                }
            }
                
            /// <summary>
            /// Switches the modus of the camera depending on the choice inside a dropdown menu.
            /// </summary>
            public void SwitchCamera()
            {
                m_cameramode = (ICameraMode)m_cameramodesselection.value;
            }

            /// <summary>
            /// Moves the camera using the Horizontal and Vertical axis inside a 2D space. Zooms the camera using the Zoom axis.
            /// </summary>
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

            /// <summary>
            /// Sets the boundries of the movement from the camera.
            /// </summary>
            /// <param name="_boundries"></param>
            public void SetBoundries(Vector2 _boundries)
            {
                m_currentboundries = _boundries;
            }

            /// <summary>
            /// Changes the generation algorithm used.
            /// </summary>
            /// <param name="_gen">The algorithm to use.</param>
            public void ChangeGenerationAlgorithm(MazeGenerator _gen)
            {
                m_mazegenerator = _gen;
            }
        }
    }
}