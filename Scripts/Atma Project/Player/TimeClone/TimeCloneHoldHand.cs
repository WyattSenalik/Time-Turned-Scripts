using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Helpers.Extensions;
using Timed;

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeCloneDescendant))]
    public sealed class TimeCloneHoldHand : MonoBehaviour
    {
        public WindowData<ThrowWindowData> timeAwareMostRecentWindow
        {
            get
            {
                m_throwWindows.TryGetWindow(curTime, out WindowData<ThrowWindowData> t_window);
                return t_window;
            }
        }
        public float curTime => m_timeMan.curTime;

        private GlobalTimeManager m_timeMan = null;

        private TimeCloneDescendant m_cloneDescendant = null;
        private TimeClonePickup m_clonePickup = null;

        private readonly IWindowCollection<ThrowWindowData> m_throwWindows = new WindowCollection<ThrowWindowData>();


        private void Awake()
        {
            m_cloneDescendant = this.GetComponentSafe<TimeCloneDescendant>();
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;

            m_cloneDescendant.clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (m_cloneDescendant != null)
            {
                if (m_cloneDescendant.clone != null)
                {
                    m_cloneDescendant.clone.onInitialized.ToggleSubscription(Initialize, false);
                }
            }
        }
        private void Initialize()
        {
            TimeCloneInitData t_data = m_cloneDescendant.cloneData;
            m_clonePickup = m_cloneDescendant.clone.GetComponentSafe<TimeClonePickup>();

            // Copy the throw windows
            HoldHandPositionController t_holdHandPosCont = t_data.originalPlayerObj.GetComponentSafe<HoldHandPositionController>();
            foreach (WindowData<ThrowWindowData> t_ogWindowData in t_holdHandPosCont.windowCollection)
            {
                IPickUpObject t_thrownObj = t_ogWindowData.data.thrownObj;
                ThrowWindowData t_clonedData;
                if (t_thrownObj == null)
                {
                    t_clonedData = new ThrowWindowData();
                }
                else
                {
                    t_clonedData = new ThrowWindowData(t_thrownObj);
                }
                m_throwWindows.Add(new WindowData<ThrowWindowData>(t_ogWindowData.window, t_clonedData));
            }

            // Deparent itself
            name = $"Clone{t_data.occupyingCharge}'s HoldHand";
            transform.SetParent(null, true);
        }

        private void FixedUpdate()
        {
            // Since we unparent, we need to check if the clone is dead.
            if (m_cloneDescendant.clone == null)
            {
                Destroy(gameObject);
            }

            UpdatePosition();
            UpdateScale();
        }

        public bool HasCurrentWindow()
        {
            // We have a current window if there is a most recent window that has not been initialized.
            return m_throwWindows.mostRecentWindow != null && m_throwWindows.mostRecentWindow.window.endTime == float.PositiveInfinity;
        }

        private void UpdatePosition()
        {
            // Hold Hand
            if (m_clonePickup == null || m_clonePickup.isCarrying)
            {
                IPickUpObject t_curCarriedObj = m_clonePickup.curCarriedObj;
                if (t_curCarriedObj != null)
                {
                    transform.position = t_curCarriedObj.handPlacementPosition;
                }
            }
            // Otherwise, we might be throwing
            else
            {
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(HasCurrentWindow(), $"a current window to exist at time {curTime}.", this);
                #endregion Asserts
                WindowData<ThrowWindowData> t_recentWindow = timeAwareMostRecentWindow;
                ThrowWindowData t_recentData = t_recentWindow.data;
                // If throwing
                if (t_recentData.isThrowing)
                {
                    float t_animStartTime = t_recentWindow.window.startTime;
                    float t_animThrowEndTime = t_animStartTime + t_recentData.windUpAnimLength;
                    if (curTime < t_animThrowEndTime)
                    {
                        // Should play more of the throw anim
                        IPickUpObject t_thrownObj = t_recentData.thrownObj;
                        transform.position = t_thrownObj.handPlacementPosition;
                    }
                }
            }
        }
        private void UpdateScale()
        {
            transform.localScale = transform.localScale;
        }
    }
}