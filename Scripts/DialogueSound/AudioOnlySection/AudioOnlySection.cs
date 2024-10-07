using System;
using System.Collections;
using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Dialogue.Sound
{
    /// <summary>
    /// Conversation Action Object that just plays audio.
    /// </summary>
    [CreateAssetMenu(fileName = "new AudioOnlySection", menuName = "ScriptableObjects/Dialogue/ConvoActions/AudioOnlySection")]
    public sealed class AudioOnlySection : ConvoActionObject
    {
        public override bool autoAdvance => m_autoAdvance;

        [SerializeField] private bool m_autoAdvance = false;
        [SerializeField] private bool m_allowSkip = true;
        [SerializeField] private UIntReference m_wwiseSoundEventID = null;
        [SerializeField] private bool m_hideDialogueBox = true;
        [SerializeField] private float m_waitTime = 1.0f;

        private bool m_hasAudioFinishedPlaying = false;


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            m_hasAudioFinishedPlaying = false;
            if (m_hideDialogueBox)
            {
                convoData.dialougeBoxDisplay.Hide(() =>
                {
                    PlayAudio(onFinished);
                });
            }
            else
            {
                PlayAudio(onFinished);
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            if (m_allowSkip)
            {
                return true;
            }
            else
            {
                // Can only advance if audio has finished playing. Otherwise, not allowed.
                if (m_hasAudioFinishedPlaying)
                {
                    return true;
                }
                // Not allowed to skip.
                return false;
            }
        }


        private void PlayAudio(Action onFinished)
        {
            if (m_wwiseSoundEventID != null)
            {
                //CustomDebug.Log($"Posting event {m_wwiseSoundEventID.name}", true);
                DialogueEventSoundManager t_soundMan = DialogueEventSoundManager.instance;
                t_soundMan.PlayDialogueSound(m_wwiseSoundEventID.value);
            }
            else
            {
                //CustomDebug.LogWarning($"No event id specified for audio only section ({name})");
            }

            InvokeActionAfterSeconds(onFinished, m_waitTime);
        }
        private void InvokeActionAfterSeconds(Action action, float seconds)
        {
            CoroutineSingleton t_coroutSingleton = CoroutineSingleton.instance;
            t_coroutSingleton.StartCoroutine(InvokeActionAfterSecondsCoroutine(action, seconds));
        }
        private IEnumerator InvokeActionAfterSecondsCoroutine(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            m_hasAudioFinishedPlaying = true;
            action?.Invoke();
        }
    }
}
