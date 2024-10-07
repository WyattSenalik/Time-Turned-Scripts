using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace TMPro.Extension
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPTimerText : MonoBehaviour
    {
        [SerializeField] private float m_timeScale = 1.0f;
        [SerializeField] [ResizableTextArea] private string m_baseText = "";
        [SerializeField] private bool m_autoStart = true;
        private TextMeshProUGUI m_textMesh = null;

        private float m_curTime = 0.0f;
        private bool m_isTimerActive = false;


        // Domestic Initialization
        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();

            m_isTimerActive = m_autoStart;

            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
        }
        private void Update()
        {
            if (!m_isTimerActive) { return; }

            m_curTime += m_timeScale * Time.deltaTime;
            UpdateText(m_curTime);
        }


        public void ResetTimer()
        {
            m_curTime = 0.0f;
        }
        public void StartTimer() => ToggleTimer(true);
        public void StopTimer() => ToggleTimer(false);
        public void ToggleTimer(bool cond)
        {
            m_isTimerActive = cond;
        }


        private void UpdateText(float time)
        {
            m_textMesh.text = m_baseText + FormatTime(time);
        }
        /// <summary>
        /// Formats the given time into MM:SS
        /// </summary>
        /// <param name="time">Time in seconds.</param>
        private string FormatTime(float time)
        {
            int temp_timeInt = Mathf.FloorToInt(time);
            int temp_mins = temp_timeInt / 60;
            int temp_secs = temp_timeInt % 60;

            string temp_minStr = "";
            if (temp_mins < 10)
            {
                temp_minStr = "0";
            }
            temp_minStr += temp_mins.ToString();

            string temp_secStr = "";
            if (temp_secs < 10)
            {
                temp_secStr = "0";
            }
            temp_secStr += temp_secs.ToString();

            return temp_minStr + ":" + temp_secStr;
        }
    }
}
