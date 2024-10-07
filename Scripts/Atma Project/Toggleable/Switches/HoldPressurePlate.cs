using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Recording for a pressure plate that needs to be held down to be activated.
    /// </summary>
    [RequireComponent(typeof(SwitchPressurePlateHandler))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class HoldPressurePlate : TimedRecorder
    {
        public IReadOnlyList<HeldWindow> heldWindows => m_heldWindows;

        [SerializeField, Required] private SpriteRenderer m_bodySprRend = null;
        [SerializeField, Required] private SpriteRenderer m_offSymbolRend = null;
        [SerializeField, Required] private SpriteRenderer m_onSymbolRend = null;
        [SerializeField, Required] private Sprite m_onSprite = null;
        [SerializeField, Required, BoxGroup("Sound")] private SoundRecorder m_activateSoundRecorder = null;
        [SerializeField, Required, BoxGroup("Sound")] private SoundRecorder m_deactivateSoundRecorder = null;

        [SerializeField, ReadOnly, BoxGroup("Debugging")] private bool m_isBeingHeldDebug = false;

        private SwitchPressurePlateHandler m_pressurePlateHandler = null;
        private Collider2D m_ppTriggerCollider = null;
        private readonly List<HeldWindow> m_heldWindows = new List<HeldWindow>();
        private readonly List<Collider2D> m_overlapResults = new List<Collider2D>();
        private readonly List<HeldWindow> m_newWindows = new List<HeldWindow>();
        private readonly List<HeldWindow> m_windowsToEnd = new List<HeldWindow>();
        private TimedBool m_isBeingHeld = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_activateSoundRecorder, nameof(m_activateSoundRecorder), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_deactivateSoundRecorder, nameof(m_deactivateSoundRecorder), this);
            #endregion Asserts
            m_pressurePlateHandler = this.GetComponentSafe<SwitchPressurePlateHandler>();
            m_ppTriggerCollider = this.GetComponentSafe<Collider2D>();
        }
        private void Start()
        {
            UpdateHeldWindows();
            m_isBeingHeld = new TimedBool(HasOverlappingColliders());
            UpdateActivated(false);
            //if (m_isBeingHeld.curData)
            //{
            //    m_bodySprRend.sprite = m_onSprite;
            //    m_offSymbolRend.enabled = false;
            //    m_onSymbolRend.enabled = true;
            //}
        }
        private void FixedUpdate()
        {
            m_isBeingHeldDebug = m_isBeingHeld.curData;
            if (m_isBeingHeld.isRecording)
            {
                UpdateHeldWindows();
                m_isBeingHeld.curData = HasOverlappingColliders();
                UpdateActivated(true);
            }
            else
            {
                // Don't check if not recording.
                UpdateActivated(false);
            }
        }
        private void UpdateHeldWindows()
        {
            m_overlapResults.Clear();
            int t_resultAmount = m_pressurePlateHandler.CheckForOverlap(m_overlapResults);
            m_newWindows.Clear();
            m_windowsToEnd.Clear();
            m_windowsToEnd.AddRange(m_heldWindows);
            for (int i = t_resultAmount - 1; i >= 0; --i)
            {
                Collider2D t_overlapCol = m_overlapResults[i];
                // Tag is wrong, remove the result
                if (!m_pressurePlateHandler.IsValidTag(t_overlapCol.gameObject))
                {
                    m_overlapResults.RemoveAt(i);
                    continue;
                }

                HeldWindow t_associatedWindow = FindCurrentHeldWindowInvolvingObject(t_overlapCol.gameObject);
                if (t_associatedWindow == null)
                {
                    // Start a new window.
                    m_newWindows.Add(new HeldWindow(curTime, t_overlapCol.gameObject));
                }
                else
                {
                    // Remove the window from the ones to end
                    m_windowsToEnd.Remove(t_associatedWindow);
                }
            }
            // End the windows that now longer have an overlap
            for (int i = 0; i < m_windowsToEnd.Count; ++i)
            {
                m_windowsToEnd[i].SetEndTime(curTime);
            }

            // Add the new windows to the held windows
            m_heldWindows.AddRange(m_newWindows);
        }
        private HeldWindow FindCurrentHeldWindowInvolvingObject(GameObject holdingObject)
        {
            for (int i = 0; i < m_heldWindows.Count; ++i)
            {
                HeldWindow t_window = m_heldWindows[i];
                // Wrong object.
                if (t_window.holdingObject != holdingObject) { continue; }
                // Window has been closed already.
                if (t_window.hasEndTime) { continue; }

                return t_window;
            }
            return null;
        }
        private bool HasOverlappingColliders()
        {
            foreach (HeldWindow t_heldWindow in m_heldWindows)
            {
                if (t_heldWindow.timeFrame.ContainsTime(curTime, eTimeFrameContainsOption.EndExclusive))
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateActivated(bool shouldPlaySound)
        {
            if (IsHeldNow())
            {
                // Play sound
                if (shouldPlaySound && m_pressurePlateHandler.WouldActivateHaveEffect())
                {
                    m_activateSoundRecorder.Play();
                }

                m_pressurePlateHandler.Activate();
            }
            else
            {
                // Play sound
                if (shouldPlaySound && m_pressurePlateHandler.WouldDeactivateHaveEffect())
                {
                    m_deactivateSoundRecorder.Play();
                }

                m_pressurePlateHandler.Deactivate();
            }
        }
        private bool IsHeldNow() => m_isBeingHeld.curData;
        //// Normal-Time Logic End >

        /// <summary>
        /// Records the time frame an object holds down this pressure plate for.
        /// </summary>
        public sealed class HeldWindow
        {
            public float startTime => timeFrame.startTime;
            public float endTime => timeFrame.endTime;
            public TimeFrame timeFrame => m_timeFrame;
            public GameObject holdingObject => m_holdingObject;
            public bool hasEndTime => endTime != float.PositiveInfinity;

            private TimeFrame m_timeFrame;
            private readonly GameObject m_holdingObject = null;


            public HeldWindow(float startTime, GameObject holdingObj)
            {
                m_timeFrame = new TimeFrame(startTime, float.PositiveInfinity);
                m_holdingObject = holdingObj;
            }

            public void SetEndTime(float endTime)
            {
                m_timeFrame = new TimeFrame(m_timeFrame.startTime, endTime);
            }
            public void ResetEndTime()
            {
                m_timeFrame = new TimeFrame(m_timeFrame.startTime, float.PositiveInfinity);
            }
        }
    }
    /// <summary>
    /// Extensions for HeldWindow and HeldWindow lists.
    /// </summary>
    public static class HeldWindowExtensions
    {
        public static IReadOnlyList<TimeFrame> ConvertToTimeFrameList(this IReadOnlyList<HoldPressurePlate.HeldWindow> heldWindowList)
        {
            List<TimeFrame> t_timeFrameList = new List<TimeFrame>(heldWindowList.Count);
            foreach (HoldPressurePlate.HeldWindow t_window in heldWindowList)
            {
                t_timeFrameList.Add(t_window.timeFrame);
            }
            return t_timeFrameList;
        }
    }
}
