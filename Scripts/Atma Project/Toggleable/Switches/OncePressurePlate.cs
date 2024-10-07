using System.Collections.Generic;
using UnityEngine;

using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Recording for a pressure plate that only gets activated once and then stays on forever.
    /// </summary>
    [RequireComponent(typeof(SwitchPressurePlateHandler))]
    [RequireComponent(typeof(SoundRecorder))]
    public sealed class OncePressurePlate : TimedRecorder
    {
        private SwitchPressurePlateHandler m_pressurePlateHandler = null;
        private SoundRecorder m_soundRecorder = null;
        private float m_activatedTime = float.PositiveInfinity;

        private readonly List<Collider2D> m_overlapResults = new List<Collider2D>();


        //// < Normal-Time Logic Start
        protected override void Awake()
        {
            base.Awake();
            
            m_pressurePlateHandler = GetComponent<SwitchPressurePlateHandler>();
            m_soundRecorder = GetComponent<SoundRecorder>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_pressurePlateHandler, this);
            //CustomDebug.AssertComponentIsNotNull(m_soundRecorder, this);
            #endregion Asserts
        }
        private void FixedUpdate()
        {
            // Ignore physics when not recording.
            if (!isRecording) { return; }
            int t_overlapCount = m_pressurePlateHandler.CheckForOverlap(m_overlapResults);
            if (t_overlapCount > 0)
            {
                for (int i = 0; i < t_overlapCount; ++i)
                {
                    OnTriggerEnterAndStay2D(m_overlapResults[i]);
                }
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            // If resumed before this had been activated. It has no longer been activated.
            if (curTime < m_activatedTime)
            {
                m_activatedTime = float.PositiveInfinity;
            }
        }

        private void OnTriggerEnterAndStay2D(Collider2D other)
        {
            // Pressure plate has already been activated.
            if (curTime >= m_activatedTime) { return; }
            // Tag is wrong.
            if (!m_pressurePlateHandler.IsValidTag(other.gameObject)) { return; }

            // Pressure plate has been activated.
            m_activatedTime = curTime;
            m_pressurePlateHandler.Activate();
            m_soundRecorder.Play();
        }
        //// Normal-Time Logic End >

        //// < Rewind/Fastforward Logic Start
        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // WE NOW ALWAYS CALL Deactive/Activate because there are cases where the player resumes just barely on top of the pp without it being on yet and then the pp would appear off until rewind started.
            // Do nothing if recording. Only care about when rewinding/fastforwarding.
            //if (isRecording) { return; }

            // Its okay if Deactivate and Activate are called multiple times because
            // PressurePlateHandler makes a check to not do things if they are already in those states.
            //
            // Deactivate if time is before pressure plate was activated.
            if (curTime < m_activatedTime)
            {
                m_pressurePlateHandler.Deactivate();
            }
            // Activate if time is at or after pressure plate was activated.
            else
            {
                m_pressurePlateHandler.Activate();
            }
        }
        //// Rewind/Fastforward Logic End >
    }
}
