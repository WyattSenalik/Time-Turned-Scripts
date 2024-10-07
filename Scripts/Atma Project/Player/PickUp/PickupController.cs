using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Helpers.Events;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Player's recording data for pickups.
    /// </summary>
    [RequireComponent(typeof(PlayerStateManager))]
    public sealed class PickupController : TimedRecorder, IPickUpHolder
    {
        public IEventPrimer<IPickUpObject> onPickUp => m_onPickUp;
        public IEventPrimer<IPickUpObject> onThrow => m_onThrow;
        public IEventPrimer<IPickUpObject> onRelease => m_onRelease;

        /// <summary>
        /// PickUpObject that is currently being held by the player.
        /// </summary>
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
        public bool isCarrying => curCarriedObj != null;
        /// <summary>Offset from the player the object is being held around (local position).</summary>
        public Vector3 holdPosCenterOffset => m_holdPosCenter != null ? m_holdPosCenter.localPosition : Vector3.zero;
        /// <summary>Hold position in world space for the held object.</summary>
        public Vector3 holdPosCenterWorld => m_holdPosCenter != null ? m_holdPosCenter.position : transform.position;
        public float pickupSize => transform.localScale.x;

        public IReadOnlyList<PickupTimeFrame> heldFrames => m_heldFrames;
        public TimedBool timedIsUsingAction => m_timedIsUsingAction;
        public TimedVector2 timedHoldDir => m_timedHoldDir;


        [SerializeField] private Transform m_holdPosCenter = null;
        [SerializeField, Required] private SoundRecorder m_throwSoundRecorder = null;

        // Events for pickup/throw when those happen during a recording.
        [SerializeField] private MixedEvent<IPickUpObject> m_onPickUp = new MixedEvent<IPickUpObject>();
        [SerializeField] private MixedEvent<IPickUpObject> m_onThrow = new MixedEvent<IPickUpObject>();
        [SerializeField] private MixedEvent<IPickUpObject> m_onRelease = new MixedEvent<IPickUpObject>();


        private GamePauseController m_pauseController = null;

        private bool m_isHoldingActionButton = false;

        private readonly List<PickupTimeFrame> m_heldFrames = new List<PickupTimeFrame>();
        private TimedBool m_timedIsUsingAction = null;
        private TimedVector2 m_timedHoldDir = null;

        private IPickUpObject m_internalHeldPickupObj = null;
        private bool m_internalShouldThrow = false;
        private Vector2 m_internalCarryDir = Vector2.zero;

        private PickupTimeFrame m_prevHeldFrame = null;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_throwSoundRecorder, nameof(m_throwSoundRecorder), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_pauseController = GamePauseController.instance;

            m_timedHoldDir = new TimedVector2(Vector2.zero);
            m_timedIsUsingAction = new TimedBool(false);

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Normal time progression
            if (isRecording)
            {
                if (m_internalHeldPickupObj != null)
                {
                    PickupTimeFrame t_newPickupFrame = new PickupTimeFrame(m_internalHeldPickupObj, curTime);
                    m_heldFrames.Add(t_newPickupFrame);
                    // We need to actually pick up the object as well.
                    PickupObject(t_newPickupFrame.pickUpObj);

                    m_internalHeldPickupObj = null;
                }
                if (m_internalShouldThrow)
                {
                    if (m_heldFrames.Count > 0)
                    {
                        PickupTimeFrame t_recentHeldFrame = m_heldFrames[^1];
                        t_recentHeldFrame.SetReleaseTime(curTime);
                        // Actually throw the object.
                        ReleaseAndThrowObject(t_recentHeldFrame.pickUpObj);
                    }

                    m_internalShouldThrow = false;
                }

                m_timedHoldDir.curData = m_internalCarryDir;
                m_timedIsUsingAction.curData = m_isHoldingActionButton;
                PickupTimeFrame t_curHeldFrame = FindCurPickupTimeFrame();
                if (t_curHeldFrame != null)
                {
                    // Held Direction and item position
                    t_curHeldFrame.pickUpObj.direction = m_timedHoldDir.curData;
                    t_curHeldFrame.pickUpObj.UpdateCarryObjPosition(holdPosCenterWorld, pickupSize);
                    // If its being fired.
                    t_curHeldFrame.pickUpObj.TakeAction(m_timedIsUsingAction.curData);
                }

                m_prevHeldFrame = t_curHeldFrame;
            }
            // Time is being manipulated, all we want to do is update whether or not the item is being held.
            else
            {
                PickupTimeFrame t_curHeldFrame = FindCurPickupTimeFrame();

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

                m_prevHeldFrame = t_curHeldFrame;
            }
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            // We do this because when the map switches back, it recalls the input message for OnActivateItem if the player is holding down.
            StopTakingCurAction();
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            // Delete all frames that have a pickup in the future.
            while (m_heldFrames.Count > 0 && curTime < m_heldFrames[^1].pickupTime)
            {
                m_heldFrames.RemoveAt(m_heldFrames.Count - 1);
            }
            // If the most recent frame ends after right now, make it so that there is no set release time.
            if (m_heldFrames.Count > 0 && curTime < m_heldFrames[^1].releaseTime)
            {
                m_heldFrames[^1].SetReleaseTime(float.PositiveInfinity);
            }
        }
        
        private void PickupObject(IPickUpObject pickupObj)
        {
            pickupObj.OnPickUp(this);
            m_onPickUp.Invoke(pickupObj);
        }
        private void ReleaseAndThrowObject(IPickUpObject pickupObj)
        {
            ReleaseObject(pickupObj);
            ThrowObject(pickupObj);
        }
        private void ReleaseObject(IPickUpObject pickupObj)
        {
            pickupObj.OnReleased();
            m_onRelease.Invoke(pickupObj);
        }
        private void ThrowObject(IPickUpObject pickupObj)
        {
            pickupObj.ThrowMe();
            m_onThrow.Invoke(pickupObj);
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


        // Functions to be called from PickupInput. These are used to create/close spans.
        public void SetPickup(IPickUpObject obj)
        {
            m_internalHeldPickupObj = obj;
        }
        public void ThrowPickup()
        {
            m_internalShouldThrow = true;
        }
        public void TakeAction(bool pressedOrReleased)
        {
            m_isHoldingActionButton = pressedOrReleased;
        }
        public void SetCarryDirection(Vector2 carryDir)
        {
            m_internalCarryDir = carryDir;
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_pauseController != null)
            {
                m_pauseController.onPause.ToggleSubscription(OnGamePaused, cond);
            }
        }
        private void OnGamePaused()
        {
            // We do this because when the map switches back, it recalls the input message for OnActivateItem if the player is holding down.
            if (isRecording)
            {
                StopTakingCurAction();
            }
            else
            {
                TakeAction(false);
            }
        }
        private void StopTakingCurAction()
        {
            TakeAction(false);
            PickupTimeFrame t_curHeldFrame = FindCurPickupTimeFrame();
            if (t_curHeldFrame != null)
            {
                t_curHeldFrame.pickUpObj.TakeAction(false);
            }
        }
    }

    public sealed class PickupTimeFrame
    {
        public float pickupTime => timeFrame.startTime;
        public float releaseTime => timeFrame.endTime;
        public TimeFrame timeFrame { get; private set; } = TimeFrame.NaN;
        public IPickUpObject pickUpObj { get; private set; } = null;


        public PickupTimeFrame(IPickUpObject pickUpObj, float pickupTime)
        {
            timeFrame = new TimeFrame(pickupTime, float.PositiveInfinity);
            this.pickUpObj = pickUpObj;
        }
        public PickupTimeFrame(TimeFrame timeFrame, IPickUpObject pickUpObj)
        {
            this.timeFrame = timeFrame;
            this.pickUpObj = pickUpObj;
        }

        public PickupTimeFrame Clone() => new PickupTimeFrame(timeFrame, pickUpObj);
        public void SetReleaseTime(float time)
        {
            timeFrame = new TimeFrame(timeFrame.startTime, time);
        }
    }
}
