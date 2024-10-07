using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace TMPro.Extension
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TMPEllipsisAnimation : MonoBehaviour
    {
        private const float INCREMENT_UNTIL_NEXT_PERIOD = 1.0f;

        [SerializeField] [ResizableTextArea] private string m_baseText = "";
        [SerializeField]
        [Min(0)]
        [OnValueChanged(nameof(OnMinPeriodAmountValueChanged))]
        private int m_minPeriodAmount = 1;
        [SerializeField]
        [Min(0)]
        [OnValueChanged(nameof(OnMaxPeriodAmountValueChanged))]
        private int m_maxPeriodAmount = 3;
        [SerializeField] [Min(0.0f)] private float m_speed = 1.0f;
        [SerializeField] private bool m_playOnStart = true;

        private TextMeshProUGUI m_textMesh = null;
        private bool m_shouldPlayAnim = false;
        private int m_curPeriodAmount = 0;
        private float m_timeWaited = 0.0f;


        // Domestic Initialization
        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
        }
        // Foreign Initialization
        private void Start()
        {
            m_shouldPlayAnim = m_playOnStart;
            m_curPeriodAmount = m_minPeriodAmount;
        }
        private void Update()
        {
            if (!m_shouldPlayAnim) { return; }

            m_textMesh.text = m_baseText + GetPeriodsString(m_curPeriodAmount);

            m_timeWaited += m_speed * Time.deltaTime;
            if (m_timeWaited >= INCREMENT_UNTIL_NEXT_PERIOD)
            {
                m_timeWaited = 0.0f;
                // Increment the amount periods and wrap around.
                if (++m_curPeriodAmount > m_maxPeriodAmount)
                {
                    m_curPeriodAmount = m_minPeriodAmount;
                }
            }
        }


        public void ToggleAnimation(bool cond)
        {
            m_shouldPlayAnim = cond;
        }
        public void StartAnimation() => ToggleAnimation(true);
        public void StopAnimation() => ToggleAnimation(false);


        private string GetPeriodsString(int desiredPeriods)
        {
            string temp_periods = "";
            for (int i = 0; i < m_curPeriodAmount; ++i)
            {
                temp_periods += ".";
            }
            return temp_periods;
        }


        #region Editor
        private void OnMinPeriodAmountValueChanged()
        {
            if (m_minPeriodAmount > m_maxPeriodAmount)
            {
                m_maxPeriodAmount = m_minPeriodAmount;
            }
        }
        private void OnMaxPeriodAmountValueChanged()
        {
            if (m_maxPeriodAmount < m_minPeriodAmount)
            {
                m_minPeriodAmount = m_maxPeriodAmount;
            }
        }
        #endregion Editor
    }
}
