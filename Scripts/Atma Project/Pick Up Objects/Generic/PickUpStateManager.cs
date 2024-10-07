using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Events;
using Timed;
using Timed.Animation.BetterCurve;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Keeps track of what state the <see cref="IPickUpObject"/> is in when recording.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IPickUpObject))]
    [RequireComponent(typeof(PickUpItemPitfallHandler))]
    public sealed class PickUpStateManager : TimedRecorder, IPickUpBehavior
    {
        public float secondsUntilLandAfterThrow => m_optionalThrowBehav != null ? m_optionalThrowBehav.windupAnimLength + m_optionalThrowBehav.physicsAnimLength : 0.0f;
        public ePickupState curState => (ePickupState)m_timedCurState.curData;
        public IPickUpObject pickUpObj { get; private set; } = null;
        public PickUpItemPitfallHandler pitfallHandler { get; private set; } = null;
        [ShowNativeProperty]
        public float mostRecentThrowTime
        {
            get
            {
                if (m_throwTimes.Count <= 0)
                {
                    return float.MaxValue;
                }
                return m_throwTimes[^1];
            }
        }
        public IEventPrimer<ePickupState> onStateChanged => m_onStateChanged;

        [SerializeField] private TimedBetterCurveAnimation m_throwAnim = null;
        [SerializeField, ReadOnly] private ePickupState m_curStateDebug = ePickupState.OnGround;
        [SerializeField] private MixedEvent<ePickupState> m_onStateChanged = new MixedEvent<ePickupState>();

        private ThrowBehavior m_optionalThrowBehav = null;

        private TimedInt m_timedCurState = null;
        private ePickupState m_internalCurState = ePickupState.OnGround;
        private readonly List<float> m_throwTimes = new List<float>();
        private int m_recentThrowDisallowPickupID = -1;


        protected override void Awake()
        {
            base.Awake();

            pickUpObj = GetComponent<IPickUpObject>();
            pitfallHandler = GetComponent<PickUpItemPitfallHandler>();
            m_optionalThrowBehav = GetComponent<ThrowBehavior>(); // Not required.
            #region Asserts
            //CustomDebug.AssertIComponentIsNotNull(pickUpObj, this);
            //CustomDebug.AssertComponentIsNotNull(pitfallHandler, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_internalCurState = ePickupState.OnGround;
            m_timedCurState = new TimedInt((int)m_internalCurState, eInterpolationOption.Step);
            m_onStateChanged.Invoke(ePickupState.OnGround);
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);
            // Wait till initialized
            if (m_timedCurState == null) { return; }

            // Only update the state if recording
            if (isRecording)
            {
                int t_prevState = m_timedCurState.curData;
                UpdateCurrentState();
                m_timedCurState.curData = (int)m_internalCurState;
                if (t_prevState != m_timedCurState.curData)
                {
                    // State changed so invoke the changed event.
                    m_onStateChanged.Invoke((ePickupState)m_timedCurState.curData);
                    // See if we should update allowing pickup
                    DisallowPickupUpdate();
                }
            }
            m_curStateDebug = (ePickupState)m_timedCurState.curData;
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            for (int i = 0; i < m_throwTimes.Count; ++i)
            {
                if (time < m_throwTimes[i])
                {
                    m_throwTimes.RemoveAt(i);
                    --i;
                }
            }

            ePickupState t_prevState = m_internalCurState;
            ePickupState t_curState = (ePickupState)m_timedCurState.curData;
            if (t_prevState != t_curState)
            {
                // State changed so invoke the changed event.
                m_onStateChanged.Invoke(t_curState);
                // See if we should update allowing pickup
                DisallowPickupUpdate();
            }
        }


        #region IPickUpBehaviour
        public void ThrowMe()
        {
            if (!isRecording) { return; }
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(curTime > mostRecentThrowTime || mostRecentThrowTime == float.MaxValue, $"the newest throw time ({curTime}) to be before the previous most recent throw time  ({mostRecentThrowTime}).", this);
            #endregion Asserts
            m_throwTimes.Add(curTime);
        }
        public void TakeAction(bool onPressedOrReleased) { }
        public void OnPickUp() { }
        public void OnReleased() { }
        #endregion IPickUpBehaviour


        private void UpdateCurrentState()
        {
            if (pickUpObj.isHeld)
            {
                m_internalCurState = ePickupState.Held;

                CancelThrowAnimation();
            }
            else if (mostRecentThrowTime <= curTime && curTime < mostRecentThrowTime + secondsUntilLandAfterThrow)
            {
                if (curTime <= mostRecentThrowTime + secondsUntilLandAfterThrow / 2)
                {
                    m_internalCurState = ePickupState.InAirRising;
                }
                else
                {
                    m_internalCurState = ePickupState.InAirFalling;
                }
            }
            else if (pitfallHandler.isInPit)
            {
                m_internalCurState = ePickupState.InVoid;

                CancelThrowAnimation();
            }
            else if (pitfallHandler.isOverPit)
            {
                m_internalCurState = ePickupState.FallingIntoVoid;

                CancelThrowAnimation();
            }
            else
            {
                m_internalCurState = ePickupState.OnGround;

                CancelThrowAnimation();
            }
        }

        private void DisallowPickupUpdate()
        {
            if (ShouldDisallowPickup())
            {
                if (m_recentThrowDisallowPickupID != -1) { return; }
                m_recentThrowDisallowPickupID = pickUpObj.RequestDisallowPickup();
            }
            else
            {
                if (m_recentThrowDisallowPickupID == -1) { return; }
                pickUpObj.CancelDisallowPickupRequest(m_recentThrowDisallowPickupID);
                m_recentThrowDisallowPickupID = -1;
            }
        }
        private bool ShouldDisallowPickup()
        {
            ePickupState t_curState = (ePickupState)m_timedCurState.curData;
            switch (t_curState)
            {
                case ePickupState.OnGround: return false;
                case ePickupState.Held: return true;
                case ePickupState.InAirRising: return true;
                case ePickupState.InAirFalling: return false;
                case ePickupState.FallingIntoVoid: return true;
                case ePickupState.InVoid: return true;
                default:
                {
                    CustomDebug.UnhandledEnum(t_curState, this);
                    return false;
                }
            }
        }

        private void CancelThrowAnimation()
        {
            if (m_throwAnim != null && m_throwAnim.enabled)
            {
                m_throwAnim.StopAndSkipToEnd();
            }
        }
    }

    public enum ePickupState { OnGround, Held, InAirRising, InAirFalling, FallingIntoVoid, InVoid }
}