using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class SoundRecorder : TimedRecorder
    {
        public float clipLength => m_clipLength.value;

        [SerializeField] private bool m_isLooping = false;
        [SerializeField, Required] private FloatReference m_clipLength = null;
        [SerializeField, Required] private UIntReference m_playerPlayForwardEventId = null;
        [SerializeField, Required] private UIntReference m_playerPlayBackwardEventId = null;
        [SerializeField, Required] private UIntReference m_playerStopForwardEventId = null;
        [SerializeField, Required] private UIntReference m_playerStopBackwardEventId = null;

        [SerializeField] private SpriteRenderer m_rendererThatDeterminesIfSoundGetsPlayed = null;

        private readonly List<TimedSoundInstance> m_soundInstances = new List<TimedSoundInstance>();


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_clipLength, nameof(m_clipLength), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerPlayForwardEventId, nameof(m_playerPlayForwardEventId), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerPlayBackwardEventId, nameof(m_playerPlayBackwardEventId), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerStopForwardEventId, nameof(m_playerStopForwardEventId), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerStopBackwardEventId, nameof(m_playerStopBackwardEventId), this);
            #endregion Asserts
        }


        public void Play()
        {
            if (!isRecording)
            {
                //CustomDebug.LogWarningForComponent($"tried to play sound during manipulation.", this);
                return;
            }
            if (m_rendererThatDeterminesIfSoundGetsPlayed != null)
            {
                if (!m_rendererThatDeterminesIfSoundGetsPlayed.isVisible)
                {
                    // Don't play because not on screen.
                    return;
                }
            }

            bool t_successfullyPlayed = false;
            foreach (TimedSoundInstance t_activeInstance in m_soundInstances)
            {
                // Keep trying until something plays
                if (t_activeInstance.TryPlay(curTime))
                {
                    t_successfullyPlayed = true;
                    break;
                }
            }
            if (!t_successfullyPlayed)
            {
                m_soundInstances.Add(TimedSoundInstance.CreateInstance(curTime, m_isLooping, clipLength, m_playerPlayForwardEventId.value, m_playerPlayBackwardEventId.value, m_playerStopForwardEventId.value, m_playerStopBackwardEventId.value));
            }
        }
        public void Stop()
        {
            foreach (TimedSoundInstance t_singleInstance in m_soundInstances)
            {
                t_singleInstance.Stop(curTime);
            }
        }
        public bool IsPlaying()
        {
            foreach (TimedSoundInstance t_singleInstance in m_soundInstances)
            {
                // If not available, it means its playing currently.
                if (!t_singleInstance.IsAvailableForSound(curTime))
                {
                    return true;
                }
            }
            return false;
        }
    }
}