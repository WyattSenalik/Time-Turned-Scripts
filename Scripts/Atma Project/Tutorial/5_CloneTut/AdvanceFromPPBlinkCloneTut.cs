using UnityEngine;

using NaughtyAttributes;

using Timed;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Clone
{
    /// <summary>
    /// Tells the SrAgent MoveToOtherSide ProgrammedAction to advance when the pressure plate blinks.
    /// </summary>
    [RequireComponent(typeof(SwitchPressurePlateHandler))]
    public sealed class AdvanceFromPPBlinkCloneTut : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private SrAgentMoveToOtherSideSimple m_srAgentMove = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Min(0.0f)] private float m_pauseAfterBlink = 1.0f;

        private SwitchPressurePlateHandler m_ppHandler = null;

        private bool m_isPPOn = false;
        private bool m_hasOccurred = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMove, nameof(m_srAgentMove), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            #endregion Asserts
            m_ppHandler = GetComponent<SwitchPressurePlateHandler>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_ppHandler, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_ppHandler.onActivated.ToggleSubscription(OnPPActivate, true);
            m_ppHandler.onDeactivated.ToggleSubscription(OnPPDeactivate, true);
        }
        private void OnDestroy()
        {
            m_ppHandler?.onActivated.ToggleSubscription(OnPPActivate, false);
            m_ppHandler?.onDeactivated.ToggleSubscription(OnPPDeactivate, false);
        }


        private void OnPPActivate()
        {
            m_isPPOn = true;
        }
        private void OnPPDeactivate()
        {
            if (!m_isPPOn) { return; }
            if (m_hasOccurred) { return; }
            m_hasOccurred = true;

            Invoke(nameof(SetMoveFin), m_pauseAfterBlink);
        }
        private void SetMoveFin()
        {
            #region Logs
            //CustomDebug.LogForComponent($"SetMoveFin", this, IS_DEBUGGING);
            #endregion Logs
            StartCoroutine(SetMoveFinCorout());
        }
        private IEnumerator SetMoveFinCorout()
        {
            yield return new WaitForFixedUpdate();

            // Freeze time so that the time it takes to do dialogue doesn't get added to the total time.
            m_timeRewinder.StartRewind();

            m_srAgentMove.SetFinished();
        } 
    }
}