using System;
using System.Collections;
using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Pause
{
    /// <summary>
    /// ConvoAction that just waits for a specified amount of time.
    /// </summary>
    [CreateAssetMenu(fileName = "new Pause", menuName = "ScriptableObjects/Dialogue/ConvoActions/Pause")]
    public sealed class PauseConvoAction : ConvoActionObject
    {
        public override bool autoAdvance => m_autoAdvance;

        [SerializeField] private bool m_autoAdvance = true;
        [SerializeField] private bool m_hideDialogue = true;
        [SerializeField, Min(0.0f)] private float m_secondsToPause = 1.0f;

        private bool m_finishedPausing = false;


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_finishedPausing = false;

            bool t_shouldSkip = convoData.convoDriver.shouldSkip;
            if (m_hideDialogue)
            {
                if (!t_shouldSkip)
                {
                    convoData.dialougeBoxDisplay.Hide(() => BeginPause(convoData, onFinished));
                }
                else
                {
                    convoData.dialougeBoxDisplay.Hide();
                    BeginPause(convoData, onFinished);
                }
            }
            else
            {
                BeginPause(convoData,onFinished);
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            return m_finishedPausing;
        }


        private void BeginPause(ConvoData convoData, Action onFinished)
        {
            if (m_secondsToPause <= 0.0f || convoData.convoDriver.shouldSkip)
            {
                m_finishedPausing = true;
                onFinished?.Invoke();
            }
            else
            {
                InvokeActionAfterSeconds(onFinished, m_secondsToPause);
            }
        }
        private void InvokeActionAfterSeconds(Action action, float seconds)
        {
            CoroutineSingleton t_coroutSingleton = CoroutineSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_coroutSingleton, this);
            #endregion Asserts
            t_coroutSingleton.StartCoroutine(InvokeActionAfterSecondsCoroutine(action, seconds));
        }
        private IEnumerator InvokeActionAfterSecondsCoroutine(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            m_finishedPausing = true;
            action?.Invoke();
        }
    }
}