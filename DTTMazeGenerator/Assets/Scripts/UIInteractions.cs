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

            [SerializeField] GameObject m_userinterface;
            [SerializeField] Slider m_widthslider;
            [SerializeField] Slider m_heightslider;
            [SerializeField] Slider m_iterationspeedslider;
            [SerializeField] TextMeshProUGUI m_widthslidertext;
            [SerializeField] TextMeshProUGUI m_heightslidertext;
            
            bool m_showui = true;

            void Start()
            {
                MazeGenerator.Instance.IterationSpeed = m_iterationspeedslider.value;
            }

            public void ShowUI()
            {
                m_showui = m_showui != true;
                m_userinterface.SetActive(m_showui);
            }
            public void ChangeHeightValue()
            {
                m_mazeheight = Mathf.RoundToInt(m_heightslider.value);
                m_heightslidertext.SetText("Height: " + m_mazeheight);
                MazeGenerator.Instance.MazeHeight = m_mazeheight;
            }
            public void ChangeWidthValue()
            {
                m_mazewidth = Mathf.RoundToInt(m_widthslider.value);
                m_widthslidertext.SetText("Width: " + m_mazewidth);
                MazeGenerator.Instance.MazeWidth = m_mazewidth;
            }

            public void ChangeIterationSpeed()
            {
                MazeGenerator.Instance.IterationSpeed = m_iterationspeedslider.value;
            }
        }
    }
}