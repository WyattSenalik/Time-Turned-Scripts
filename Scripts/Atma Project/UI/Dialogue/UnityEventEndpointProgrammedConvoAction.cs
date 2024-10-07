using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

using Atma.Settings;
using Dialogue;
using Dialogue.ConvoActions.Programmed;
// Original Authors - Wyatt Senalik

namespace Atma.Dialogue
{
    /// <summary>
    /// MonoBehaviour variant Endpoint for a ProgrammedConvoAction that just calls a unity event when told to begin.
    /// </summary>
    public sealed class UnityEventEndpointProgrammedConvoAction : MonoBehavEndpointProgrammedConvoAction
    {
        private const bool IS_DEBUGGING = false;

        private float waitTimeBeforeFinish => ConversationSkipper.instance.ShouldSkipDialogue() ? 0.1f : m_waitTimeBeforeFinish;

        [SerializeField] private bool m_autoFinishAfterSeconds = true;
        [SerializeField, ShowIf(nameof(m_autoFinishAfterSeconds)), Min(0.0f)]
        private float m_waitTimeBeforeFinish = 0.0f;
        [SerializeField] private bool m_areTimedEventsSkippable = false;
        [SerializeField] private bool m_waitFrameBeforeInvokingWhenSkipping = false;
        [SerializeField] private UnityEvent<ConvoData> m_onBegin = new UnityEvent<ConvoData>();
        [SerializeField] private UnityEvent m_onDone = new UnityEvent();

        [SerializeField] private UnityEventOnTimer[] m_eventsAfterTime = new UnityEventOnTimer[0];

        private Action m_onFinished = null;
        private bool m_isFinished = false;


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_isFinished = false;
            m_onBegin.Invoke(convoData);
            bool t_shouldSkip = ConversationSkipper.instance.ShouldSkipDialogue();
            if (m_autoFinishAfterSeconds)
            {
                if (t_shouldSkip)
                {
                    if (m_waitFrameBeforeInvokingWhenSkipping)
                    {
                        StartCoroutine(FinishAfterDelayCorout());
                    }
                    else
                    {
                        Finish();
                    }

                    IEnumerator FinishAfterDelayCorout()
                    {
                        yield return new WaitForFixedUpdate();
                        Finish();
                    }
                    void Finish()
                    {
                        m_isFinished = true;
                        m_onDone.Invoke();
                        onFinished?.Invoke();
                    }
                }
                else
                {
                    InvokeActionAfterSeconds(onFinished, waitTimeBeforeFinish);
                }
            }
            else
            {
                // For manual finish, save it for later.
                m_onFinished = onFinished;
            }

            foreach (UnityEventOnTimer t_eventOnTimer in m_eventsAfterTime)
            {
                if (!t_shouldSkip || !m_areTimedEventsSkippable)
                {
                    InvokeActionAfterSeconds(t_eventOnTimer.InvokeEvent, t_eventOnTimer.waitTime);
                }
                else if (t_shouldSkip && m_areTimedEventsSkippable)
                {
                    t_eventOnTimer.InvokeEvent();
                }
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            return m_isFinished;
        }
        public void Finish()
        {
            #region Logs
            //CustomDebug.LogForComponent($"Finishing", this, IS_DEBUGGING);
            #endregion Logs
            Action t_onFinished = m_onFinished;
            // Clear the onFinished variable before invoking on finished in the weird case that it might be set by the next action.
            m_onFinished = null;
            m_isFinished = true;
            m_onDone.Invoke();
            t_onFinished?.Invoke();
        }
        public void SetIsFinished() => m_isFinished = true;


        private void InvokeActionAfterSeconds(Action action, float seconds)
        {
            StartCoroutine(InvokeActionAfterSecondsCoroutine(action, seconds));
        }
        private IEnumerator InvokeActionAfterSecondsCoroutine(Action action, float seconds)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(seconds);
            yield return new WaitForFixedUpdate();
            m_isFinished = true;
            m_onDone.Invoke();
            action?.Invoke();
        }


        [Serializable]
        public sealed class UnityEventOnTimer
        {
            public UnityEvent onTimerEnds => m_onTimerEnds;
            public float waitTime => m_waitTime;

            [SerializeField] private UnityEvent m_onTimerEnds = new UnityEvent();
            [SerializeField, Min(0.0f)] private float m_waitTime = 0.0f;

            public void InvokeEvent() => m_onTimerEnds.Invoke();
        }
    }
}