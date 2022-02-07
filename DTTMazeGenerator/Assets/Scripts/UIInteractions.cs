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
        public class UIInteractions : MonoBehaviour
        {
            int m_mazewidth; //X
            int m_mazeheight; //Y
            int m_beginpointX;
            int m_beginpointY;

            [SerializeField] GameObject m_userinterface;
            [SerializeField] Slider m_widthslider;
            [SerializeField] Slider m_heightslider;
            [SerializeField] Slider m_beginXslider;
            [SerializeField] Slider m_beginYslider;
            [SerializeField] Slider m_iterationmodifierslider;
            [SerializeField] TextMeshProUGUI m_widthslidertext;
            [SerializeField] TextMeshProUGUI m_heightslidertext;
            [SerializeField] TextMeshProUGUI m_startpostext;

            bool m_showui = true;

            void Start()
            {
                MazeManager.Instance.ChangeMazeIterationSpeed(m_iterationmodifierslider.value);
            }

            public void ShowUI()
            {
                m_showui = !m_showui;
                m_userinterface.SetActive(m_showui);
            }
            public void ChangeHeightValue()
            {
                m_mazeheight = Mathf.RoundToInt(m_heightslider.value);
                m_heightslidertext.SetText("Height: " + m_mazeheight);
                m_beginYslider.maxValue = m_mazeheight;
                MazeManager.Instance.WantedMazeHeight = m_mazeheight;
            }
            public void ChangeWidthValue()
            {
                m_mazewidth = Mathf.RoundToInt(m_widthslider.value);
                m_widthslidertext.SetText("Width: " + m_mazewidth);
                m_beginXslider.maxValue = m_mazewidth;
                MazeManager.Instance.WantedMazeWidth = m_mazewidth;
            }

            public void ChangeBeginPointX()
            {
                m_beginpointX = Mathf.RoundToInt(m_beginXslider.value);
                SetStartPosText();
                MazeManager.Instance.WantedBeginPointX = m_beginpointX;
            }

            public void ChangeBeginPointY()
            {
                m_beginpointY = Mathf.RoundToInt(m_beginYslider.value);
                SetStartPosText();
                MazeManager.Instance.WantedBeginPointY = m_beginpointY;
            }

            void SetStartPosText()
            {
                m_startpostext.SetText("Start Position: (" + m_beginpointX + "," + m_beginpointY + ")");
            }

            public void ChangeIterationSpeed()
            {
                MazeManager.Instance.ChangeMazeIterationSpeed(m_iterationmodifierslider.value);
            }
        }
    }
}