using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Physics.EnclosedTrigger;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Calls <see cref="IPitfallHandler.Fall"/> when a collider is completely
    /// enclosed by this.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EnclosedTriggerCollider))]
    public sealed class Pitfall : TimedRecorder
    {
        [SerializeField, Required] private GameObject m_visualsParentObj = null;

        [SerializeField, BoxGroup("Sound"), Min(0.0f)] private float m_delayBeforePlayFallSound = 0.0f;
        [SerializeField, Required, BoxGroup("Sound")] private SoundRecorder m_fallSoundRecorder = null;

        private EnclosedTriggerCollider m_enclosedTrigger = null;

        private readonly Dictionary<int, IPitfallHandler[]> m_alreadyEnclosedHandlers = new Dictionary<int, IPitfallHandler[]>();

        private readonly List<float> m_fallTimes = new List<float>();

        [SerializeField, ReadOnly] private string m_debugTimes = "";


        protected override void Awake()
        {
            base.Awake();

            m_enclosedTrigger = GetComponent<EnclosedTriggerCollider>();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_visualsParentObj, nameof(m_visualsParentObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_fallSoundRecorder, nameof(m_fallSoundRecorder), this);

            //CustomDebug.AssertComponentIsNotNull(m_enclosedTrigger, this);
            #endregion Asserts
        }
        private void OnEnable()
        {
            ToggleSubscriptions(true);
        }
        private void OnDisable()
        {
            ToggleSubscriptions(false);
        }
        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Playing the game normally (not manipulating time).
            if (isRecording)
            {
                m_debugTimes = "";

                float t_prevTime = curTime - deltaTime;
                // m_playFallSoundTime contains all the times we want to play the fall sound.
                foreach (float t_fallTime in m_fallTimes)
                {
                    float t_playTime = t_fallTime + m_delayBeforePlayFallSound;
                    // If we passed the time we wanted to play the sound at, then play the sound.
                    if (t_playTime >= t_prevTime && t_playTime < curTime)
                    {
                        m_fallSoundRecorder.Play();
                    }
                    m_debugTimes += $"fallTime{t_fallTime}; playTime:{t_playTime}\n";
                }
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            int t_index = m_fallTimes.Count - 1;
            while (t_index >= 0)
            {
                float t_fallTime = m_fallTimes[t_index];
                if (curTime <= t_fallTime)
                {
                    m_fallTimes.RemoveAt(t_index);
                    --t_index;
                }
                else
                {
                    // Found a time that is before the cur time, so all times before this time are also before the cur time (assumes we are storing in order).
                    break;
                }
            }
        }


        private void ToggleSubscriptions(bool cond)
        {
            m_enclosedTrigger?.onFullyEnclosedEnter.ToggleSubscription(OnFullyEnclosedEnter, cond);
            m_enclosedTrigger?.onFullyEnclosedStay.ToggleSubscription(OnFullyEnclosedStay, cond);
            m_enclosedTrigger?.onFullyEnclosedExit.ToggleSubscription(OnFullyEnclosedExit, cond);
        }


        private void OnFullyEnclosedEnter(Collider2D collision)
        {
            int t_instanceID = collision.gameObject.GetInstanceID();
            IPitfallHandler[] t_pitfallHandlers = collision.GetComponentsInParent<IPitfallHandler>();
            foreach (IPitfallHandler t_handler in t_pitfallHandlers)
            {
                t_handler.OnEnclosedInPitStart();
                t_handler.Fall(m_visualsParentObj);
            }
            #region Logs
            if (m_alreadyEnclosedHandlers.ContainsKey(t_instanceID))
            {
                //CustomDebug.LogErrorForComponent($"Expected collider ({collision}) to have not already been enclosed.", this);
            }
            #endregion Logs
            m_alreadyEnclosedHandlers.Add(t_instanceID, t_pitfallHandlers);
            m_fallTimes.Add(curTime);
        }
        private void OnFullyEnclosedStay(Collider2D collision)
        {
            int t_instanceID = collision.gameObject.GetInstanceID();
            IPitfallHandler[] t_pitfallHandlers;
            if (!m_alreadyEnclosedHandlers.TryGetValue(t_instanceID, out t_pitfallHandlers))
            {
                // Add component list if doesn't yet exist.
                t_pitfallHandlers = collision.GetComponentsInParent<IPitfallHandler>();
                m_alreadyEnclosedHandlers.Add(t_instanceID, t_pitfallHandlers);
                m_fallTimes.Add(curTime);
            }
            // Call fall stay.
            foreach (IPitfallHandler t_handler in t_pitfallHandlers)
            {
                t_handler.FallStay(m_visualsParentObj);
            }
        }
        private void OnFullyEnclosedExit(Collider2D collision)
        {
            IPitfallHandler[] t_pitfallHandlers = collision.GetComponentsInParent<IPitfallHandler>();
            foreach (IPitfallHandler t_handler in t_pitfallHandlers)
            {
                t_handler.OnEnclosedInPitEnd();
            }
            int t_instanceID = collision.gameObject.GetInstanceID();
            m_alreadyEnclosedHandlers.Remove(t_instanceID);
        }
    }
}
