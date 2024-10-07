using UnityEngine;

using TMPro;
using Timed;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Text displays the current navigation direction for the player's <see cref="ITimeRewinder"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class RewindSpeedText : MonoBehaviour
    {
        [SerializeField, Required] private TimeManipIcon m_timeManipIcon = null;
        [SerializeField, Min(0)] private int m_decimalPlaces = 0;
        [SerializeField] private string m_priorText = "x";
        [SerializeField] private bool m_hideIfZero = true;

        private TextMeshProUGUI m_textMesh = null;

        private ITimeRewinder m_timeRewinder = null;


        private void Awake()
        {
            m_textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_textMesh, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeManipIcon, nameof(m_timeManipIcon), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_player = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(t_player, this);
            #endregion Asserts
            m_timeRewinder = t_player.GetComponent<ITimeRewinder>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeRewinder, t_player.gameObject, this);
            #endregion Asserts

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_timeManipIcon != null)
            {
                m_timeManipIcon.onPausedState.ToggleSubscription(HideText, cond);
                m_timeManipIcon.onRewindState.ToggleSubscription(UpdateText, cond);
                m_timeManipIcon.onFastForwardState.ToggleSubscription(UpdateText, cond);
            }
        }
        private void HideText()
        {
            m_textMesh.enabled = false;
        }
        private void UpdateText()
        {
            m_textMesh.enabled = true;
            float t_navSpeed = Mathf.Abs(m_timeRewinder.navigationDir);
            m_textMesh.text = m_priorText + t_navSpeed.ToString($"n{m_decimalPlaces}");
        }
    }
}