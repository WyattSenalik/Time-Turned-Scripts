using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Math;
using Timed;
using Helpers.Events;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeClone))]
    public sealed class TimeClonePickup : TimedRecorder, IPickUpHolder
    {
        private const bool IS_DEBUGGING = false;

        public bool isCarrying => curCarriedObj != null;
        public IPickUpObject curCarriedObj
        {
            get
            {
                PickupTimeFrame t_curFrame = FindCurPickupTimeFrame();
                if (t_curFrame != null)
                {
                    return t_curFrame.pickUpObj;
                }
                return null;
            }
        }
        public Vector3 holdPosCenterWorld => transform.position + m_holdOffset * pickupSize;
        public float pickupSize => transform.localScale.x;
        public IEventPrimer<IPickUpObject> onThrown => m_onThrown;

        private GlobalTimeManager timeMan
        {
            get
            {
                if (m_timeMan == null)
                {
                    m_timeMan = GlobalTimeManager.instance;
                }
                return m_timeMan;
            }
        }

        [SerializeField, Required] private Animator m_wearableSpringHatAnimator = null;
        [SerializeField, Required] private SpriteRenderer m_wearableSpringHatSprRend = null;
        [SerializeField, AnimatorParam(nameof(m_wearableSpringHatAnimator))]
        private string m_onLeptFromAnimParamTriggerName = "LeptFrom";
        [SerializeField, Min(0.0f)] private float m_pickupRadius = 0.6f;
        [SerializeField] private MixedEvent<IPickUpObject> m_onThrown = new MixedEvent<IPickUpObject>();

        [SerializeField] private int m_cloneDebugIndex = 0;
#if UNITY_EDITOR
        [SerializeField] private DebugPickupTimeSpanList m_debugHoldSpans = null;
#endif

        private GlobalTimeManager m_timeMan = null;
        private TimeClone m_clone = null;

        private readonly List<PickupTimeFrame> m_heldFrames = new List<PickupTimeFrame>();
        private TimedBool m_timedIsUsingAction = null;
        private TimedVector2 m_timedHoldDir = null;
        private Vector3 m_holdOffset = Vector3.zero;
        private bool m_shouldThrowFinalFrameObject = true;
        private bool m_isInitialized = false;

        private readonly List<ILeapObject> m_leapObjSubbedTo = new List<ILeapObject>();

        private PickupTimeFrame m_prevHeldFrame = null;


        protected override void Awake()
        {
            base.Awake();

            m_clone = this.GetComponentSafe<TimeClone>(this);
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_wearableSpringHatAnimator, nameof(m_wearableSpringHatAnimator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_wearableSpringHatSprRend, nameof(m_wearableSpringHatSprRend), this);
            #endregion Asserts
            m_clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (m_clone != null)
            {
                m_clone.onInitialized.ToggleSubscription(Initialize, false);
            }

            try
            {
                PickupTimeFrame t_curFrame = FindCurPickupTimeFrame();
                if (t_curFrame.pickUpObj.isHeld)
                {
                    if (t_curFrame.pickUpObj.holder is TimeClonePickup t_timeCloneHolder)
                    {
                        if (t_timeCloneHolder == this)
                        {
                            // Release
                            ReleaseObject(t_curFrame.pickUpObj);
                        }
                    }
                }
            }
            catch (System.Exception t_e)
            {
                // Its fine.
                //CustomDebug.LogWarningForComponent($"{t_e.StackTrace}", this);
            }
        }
        private void Initialize()
        {
            InitializeSpans();
            InitializeHoldOffset();

            m_isInitialized = true;
        }
        private void InitializeSpans()
        {
            TimeCloneInitData t_cloneData = m_clone.cloneData;
            // Get the PickupController to copy from
            GameObject t_origPlayerObj = t_cloneData.originalPlayerObj;
            PickupController t_pickupCont = t_origPlayerObj.GetComponentSafe<PickupController>(this);

            /// Copy the spans.
            IReadOnlyList<PickupTimeFrame> t_ogHeldFrames = t_pickupCont.heldFrames;
            foreach (PickupTimeFrame t_curOgFrame in t_ogHeldFrames)
            {
                m_heldFrames.Add(t_curOgFrame.Clone());
            }
            // If the final span end time isn't closed, end it at the furthest time.
            if (m_heldFrames.Count > 0 && m_heldFrames[^1].releaseTime > t_cloneData.farthestTime)
            {
                m_heldFrames[^1].SetReleaseTime(t_cloneData.farthestTime);
                // Also don't want to throw, just drop.
                m_shouldThrowFinalFrameObject = false;
            }
            /// Copy the using actions
            m_timedIsUsingAction = new TimedBool(t_pickupCont.timedIsUsingAction.scrapbook, true);
            /// Copy the hold directions
            m_timedHoldDir = new TimedVector2(t_pickupCont.timedHoldDir.scrapbook, true);

#if UNITY_EDITOR
            m_debugHoldSpans = new DebugPickupTimeSpanList(m_heldFrames);
#endif
        }
        private void InitializeHoldOffset()
        {
            TimeCloneInitData t_cloneData = m_clone.cloneData;
            // Get the PickupController to get the hold offset from.
            GameObject t_origPlayerObj = t_cloneData.originalPlayerObj;
            PickupController t_pickupCont = t_origPlayerObj.GetComponentSafe<PickupController>(this);

            m_holdOffset = t_pickupCont.holdPosCenterOffset;
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);
            // Wait till initialized
            if (!m_isInitialized) { return; }

            PickupTimeFrame t_curHeldFrame = FindCurPickupTimeFrame();
            // Normal time progression
            if (timeMan.shouldRecord)
            {
                if (WasObjectInPickupFrameStolen(t_curHeldFrame) || m_clone.HasEarlyDisappearanceBeforeOrAtTime(curTime))
                {
                    if (m_prevHeldFrame != null)
                    {
                        // If we were holding something, stop holding it.
                        ReleaseAndPotentiallyThrowObjectInPickupFrame(m_prevHeldFrame);
                        m_prevHeldFrame = null;
                    }
                    // Do nothing else, the item was stolen or the clone has an early disappearance.
                    return;
                }

                if (t_curHeldFrame != null)
                {
                    if (m_prevHeldFrame == null)
                    {
                        // Prev held frame was null, but this one wasn't, so we've picked something up.
                        PickupObject(t_curHeldFrame.pickUpObj);
                    }

                    // Held Direction and item position
                    t_curHeldFrame.pickUpObj.direction = m_timedHoldDir.curData;
                    t_curHeldFrame.pickUpObj.UpdateCarryObjPosition(holdPosCenterWorld, pickupSize);
                    // If its being fired.
                    t_curHeldFrame.pickUpObj.TakeAction(m_timedIsUsingAction.curData);
                }
                else if (m_prevHeldFrame != null)
                {
                    ReleaseAndPotentiallyThrowObjectInPickupFrame(m_prevHeldFrame);
                }
            }
            // Time is being manipulated, all we want to do is update whether or not the item is being held.
            else
            {
                if (m_prevHeldFrame == null)
                {
                    if (t_curHeldFrame != null)
                    {
                        PickupObject(t_curHeldFrame.pickUpObj);
                    }
                }
                else if (t_curHeldFrame == null)
                {
                    ReleaseObject(m_prevHeldFrame.pickUpObj);
                }
            }
            m_prevHeldFrame = t_curHeldFrame;
        }

        private void PickupObject(IPickUpObject pickupObj)
        {
            pickupObj.OnPickUp(this);
            UpdateWearableSpringHatVisual(pickupObj, true);
        }
        private void ReleaseAndPotentiallyThrowObjectInPickupFrame(PickupTimeFrame pickupFrame)
        {
            IPickUpObject t_pickupObj = pickupFrame.pickUpObj;
            // Prev held frame was released.
            if (pickupFrame.releaseTime == m_clone.cloneData.farthestTime && !m_shouldThrowFinalFrameObject)
            {
                // Just release, no throw
                ReleaseObject(t_pickupObj);
            }
            else
            {
                // Throw
                t_pickupObj.direction = m_timedHoldDir.curData;
                t_pickupObj.UpdateCarryObjPosition(holdPosCenterWorld, pickupSize);
                ReleaseAndThrowObject(t_pickupObj);
            }
        }
        private void ReleaseAndThrowObject(IPickUpObject pickupObj)
        {
            ReleaseObject(pickupObj);
            ThrowObject(pickupObj);
        }
        private void ReleaseObject(IPickUpObject pickupObj)
        {
            pickupObj.OnReleased();
            UpdateWearableSpringHatVisual(pickupObj, false);
        }
        private void ThrowObject(IPickUpObject pickupObj)
        {
            pickupObj.ThrowMe();
        }
        private PickupTimeFrame FindCurPickupTimeFrame()
        {
            for (int i = m_heldFrames.Count - 1; i >= 0; --i)
            {
                PickupTimeFrame t_curTimeFrame = m_heldFrames[i];
                if (t_curTimeFrame.pickupTime <= curTime && curTime < t_curTimeFrame.releaseTime)
                {
                    return t_curTimeFrame;
                }
            }
            return null;
        }
        private bool WasObjectInPickupFrameStolen(PickupTimeFrame pickupFrame)
        {
            // Can't have an object stolen if there is no frame
            if (pickupFrame == null) { return false; }

            IPickUpObject t_pickupObj = pickupFrame.pickUpObj;
            float t_dist = Vector2.Distance(holdPosCenterWorld, t_pickupObj.transform.position);
            if (t_dist > m_pickupRadius)
            {
                // Too far from where it needs to be to get picked up -> stolen.
                return true;
            }
            else if (t_pickupObj.isHeld)
            {
                if (t_pickupObj.holder is TimeClonePickup t_timeCloneHolder)
                {
                    // Was stolen if the time clone holder is not this.
                    return t_timeCloneHolder != this;
                }
                else
                {
                    // Is being held by the player -> stolen.
                    return true;
                }
            }
            else
            {
                // Not too far away and not being held -> not stolen.
                return false;
            }
        }


        private void UpdateWearableSpringHatVisual(IPickUpObject pickUpObj, bool pickedUpOrReleased)
        {
            // Hide hat if released
            if (!pickedUpOrReleased || !pickUpObj.TryGetComponent(out SpringHatBehaviour _))
            {
                m_wearableSpringHatSprRend.enabled = false;

                // Remove all previous subs
                while (m_leapObjSubbedTo.Count > 0)
                {
                    int t_lastIndex = m_leapObjSubbedTo.Count - 1;
                    ILeapObject t_leapObj = m_leapObjSubbedTo[t_lastIndex];
                    t_leapObj.onLeptFrom.ToggleSubscription(OnSpringHatWasLeptFrom, false);
                    m_leapObjSubbedTo.RemoveAt(t_lastIndex);
                }
            }
            // Actually picked up a hat
            else
            {
                m_wearableSpringHatSprRend.enabled = true;
                // Sub to listen for if the spring hat was lept from.
                ILeapObject t_springHatLeapObj = pickUpObj.GetComponentInChildren<ILeapObject>();
                #region Asserts
                //CustomDebug.AssertIComponentInChildrenOnOtherIsNotNull(t_springHatLeapObj, pickUpObj.gameObject, this);
                #endregion Asserts
                if (m_leapObjSubbedTo.Contains(t_springHatLeapObj))
                {
                    #region Logs
                    //CustomDebug.LogWarningForComponent($"Clone picked up same spring hat twice.", this);
                    #endregion Logs
                    return;
                }
                t_springHatLeapObj.onLeptFrom.ToggleSubscription(OnSpringHatWasLeptFrom, true);
                m_leapObjSubbedTo.Add(t_springHatLeapObj);
            }
        }
        private void OnSpringHatWasLeptFrom()
        {
            // Ignore if not recording.
            if (!timeMan.shouldRecord) { return; }

            m_wearableSpringHatAnimator.SetTrigger(m_onLeptFromAnimParamTriggerName);
        }
    }

#if UNITY_EDITOR
    [System.Serializable]
    public class DebugPickupTimeSpanList
    {
        [SerializeField] private List<DebugPickupTimeFrame> m_listOfSpans = new List<DebugPickupTimeFrame>();

        public DebugPickupTimeSpanList(List<PickupTimeFrame> listOfFramesOG)
        {
            foreach (PickupTimeFrame t_curFrame in listOfFramesOG)
            {
                m_listOfSpans.Add(new DebugPickupTimeFrame(t_curFrame));
            }
        }
    }
    [System.Serializable]
    public class DebugPickupTimeFrame
    {
        [SerializeField] private float m_pickupTime = 0.0f;
        [SerializeField] private float m_releaseTime = 0.0f;
        [SerializeField] private PickUpObject m_pickupObj = null;

        public DebugPickupTimeFrame(PickupTimeFrame timeFrame)
        {
            m_pickupTime = timeFrame.pickupTime;
            m_releaseTime = timeFrame.releaseTime;
            m_pickupObj = timeFrame.pickUpObj as PickUpObject;
        }
    }
    [System.Serializable]
    public class DebugVector2Snapshot
    {
        [SerializeField] private float m_time = 0.0f;
        [SerializeField] private Vector2 m_data = Vector2.zero;

        public DebugVector2Snapshot(Vector2Snapshot snap)
        {
            m_time = snap.time;
            m_data = snap.data;
        }
    }
#endif
}