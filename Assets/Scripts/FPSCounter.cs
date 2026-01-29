using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    //[RequireComponent(typeof(Text))]
    public class FPSCounter : MonoBehaviour
    {
        /*
        const float fpsMeasurePeriod = 0.2f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private float m_CurrentFps;
        const string display = "{0} FPS";*/
        public Text m_Text;


        int frameCount = 0;
        float dt = 0;
        float fps = 0;
        float updateRate = 4.0f;  // 4 updates per sec.

        void Update()
        {
            frameCount++;
            dt += Time.deltaTime;
            if (dt > 1.0 / updateRate)
            {
                fps = frameCount / dt;
                frameCount = 0;
                dt -= 1.0f / updateRate;

                m_Text.text = fps.ToString("F2");
            }
        }
    }
}