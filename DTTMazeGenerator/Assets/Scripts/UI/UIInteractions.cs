using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DTTMazeGenerator.MazeGeneration;

namespace DTTMazeGenerator
{
    namespace UI
    {
        /// <summary>
        /// Handles the user interaction with the UI.
        /// </summary>
        public class UIInteractions : MonoBehaviour
        {
            [SerializeField] GameObject m_userinterface;
            [SerializeField] Slider m_widthslider;
            [SerializeField] Slider m_heightslider;
            [SerializeField] Slider m_beginXslider;
            [SerializeField] Slider m_beginYslider;
            [SerializeField] Slider m_iterationmodifierslider;
            [SerializeField] TextMeshProUGUI m_widthslidertext;
            [SerializeField] TextMeshProUGUI m_heightslidertext;
            [SerializeField] TextMeshProUGUI m_startpostext;

            int m_mazewidth; //X
            int m_mazeheight; //Y
            int m_beginpointX;
            int m_beginpointY;

            bool m_showui = true;

            void Start()
            {
                MazeManager.Instance.ChangeMazeIterationModifier(m_iterationmodifierslider.value);
            }

            /// <summary>
            /// Shows UI when its not active and hides it when its showing.
            /// </summary>
            public void ShowUI()
            {
                m_showui = !m_showui;
                m_userinterface.SetActive(m_showui);
            }

            /// <summary>
            /// Changes the height of the maze.
            /// </summary>
            public void ChangeHeightValue()
            {
                m_mazeheight = Mathf.RoundToInt(m_heightslider.value);
                m_heightslidertext.SetText("Height: " + m_mazeheight);
                m_beginYslider.maxValue = m_mazeheight;
                MazeManager.Instance.WantedMazeHeight = m_mazeheight;
            }

            /// <summary>
            /// Changes the width of the maze.
            /// </summary>
            public void ChangeWidthValue()
            {
                m_mazewidth = Mathf.RoundToInt(m_widthslider.value);
                m_widthslidertext.SetText("Width: " + m_mazewidth);
                m_beginXslider.maxValue = m_mazewidth;
                MazeManager.Instance.WantedMazeWidth = m_mazewidth;
            }

            /// <summary>
            /// Changes the X coordinate from where the maze will start generating.
            /// </summary>
            public void ChangeBeginPointX()
            {
                m_beginpointX = Mathf.RoundToInt(m_beginXslider.value);
                SetStartPosText();
                MazeManager.Instance.WantedBeginPointX = m_beginpointX;
            }

            /// <summary>
            /// Changes the Y coordinate from where the maze will start generating.
            /// </summary>
            public void ChangeBeginPointY()
            {
                m_beginpointY = Mathf.RoundToInt(m_beginYslider.value);
                SetStartPosText();
                MazeManager.Instance.WantedBeginPointY = m_beginpointY;
            }

            /// <summary>
            /// Sets the textbox for startposition inside the UI to the X and Y selected.
            /// </summary>
            void SetStartPosText()
            {
                m_startpostext.SetText("Start Position: (" + m_beginpointX + "," + m_beginpointY + ")");
            }

            /// <summary>
            /// Changes the iteration modifier so the generation will go faster or slower.
            /// </summary>
            public void ChangeIterationModifier()
            {
                MazeManager.Instance.ChangeMazeIterationModifier(m_iterationmodifierslider.value);
            }
        }
    }
}