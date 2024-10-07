using UnityEngine;

using TMPro;

using Helpers.Events;
using Timed;
using NaughtyAttributes;
using Atma.UI;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// A timer that counts down from the specified start time using how much time has
    /// passed in the <see cref="GlobalTimeManager"/>.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class TimeDisplay : MonoBehaviour
    {
        public IEventPrimer onReachedZero => m_onReachedZero;
        public IEventPrimer onUndoReachedZero => m_onUndoReachedZero;

        [SerializeField, Min(0)] private int m_decimalPlaces = 0;
        [SerializeField] private bool m_isCountdown = true;
        [SerializeField] private bool m_showOutOfAmount = false;

        [SerializeField, ShowIf(nameof(m_isCountdown))]
        private MixedEvent m_onReachedZero = new MixedEvent();
        [SerializeField, ShowIf(nameof(m_isCountdown))] 
        private MixedEvent m_onUndoReachedZero = new MixedEvent();

        private GlobalTimeManager m_timeManager = null;
        private LevelOptions m_levelOptions = null;

        private ITimeManipController m_timeManipCont = null;

        private TextMeshProUGUI m_textMesh = null;
        private int m_shouldNotRecordCheckoutID = int.MinValue;
        private bool m_reachedZero = false;


        // Domestic Initialization
        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);
            #endregion Asserts
        }
        // Foreign Initialization
        private void Start()
        {
            m_timeManager = GlobalTimeManager.instance;
            m_levelOptions = LevelOptions.instance;
            PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeManager, this);
            //CustomDebug.AssertSingletonIsNotNull(m_levelOptions, this);
            //CustomDebug.AssertSingletonIsNotNull(t_playerSingleton, this);
            #endregion Asserts
            m_timeManipCont = t_playerSingleton.GetComponent<ITimeManipController>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeManipCont, t_playerSingleton.gameObject, this);
            #endregion Asserts
        }
        private void Update()
        {
            if (m_levelOptions.noTimeLimit)
            {
                m_textMesh.text = "";
                return;
            }
            if (m_isCountdown)
            {
                if (GetTimeValue() <= 0.0f)
                {
                    HandleReachedZero();
                }
                else
                {
                    HandleUndoReachedZero();
                }
            }
            m_textMesh.text = GetTextContent();
        }


        private string GetTextContent()
        {
            if (m_showOutOfAmount)
            {
                string t_displayTime = GetDisplayTime();
                string t_maxTime = GetDisplayMaxTime();
                return $"{t_displayTime} / {t_maxTime}";
            }
            else
            {
                return GetDisplayTime();
            } 
        }
        private string GetDisplayMaxTime()
        {
            return m_levelOptions.time.ToString($"n{m_decimalPlaces}");
        }
        private string GetDisplayTime()
        {
            string t_formatDecPlaces = "";
            for (int i = 0; i < m_decimalPlaces; ++i)
            {
                t_formatDecPlaces += "0";
            }
            return GetTimeValue().ToString($"00.{t_formatDecPlaces}");
        }
        private float GetTimeValue()
        {
            if (m_isCountdown)
            {
                return Mathf.Max(m_levelOptions.time - m_timeManager.curTime, 0.0f);
            }
            else
            {
                return Mathf.Min(m_timeManager.curTime, m_levelOptions.time);
            } 
        }
        private void HandleReachedZero()
        {
            // Don't reach zero multiple times in a row
            if (m_reachedZero) { return; }
            m_reachedZero = true;

            m_shouldNotRecordCheckoutID = m_timeManager.RequestShouldNotRecord();
            m_timeManager.SetTime(m_levelOptions.time);
            // Start rewinding for the player.
            m_timeManipCont.BeginTimeManipulation();

            m_onReachedZero.Invoke();
        }
        private void HandleUndoReachedZero()
        {
            // Don't undo reach zero multiple times in a row
            if (!m_reachedZero) { return; }
            m_reachedZero = false;

            m_timeManager.CancelShouldNotRecordRequest(m_shouldNotRecordCheckoutID);

            m_onUndoReachedZero.Invoke();
        }
    }
}
