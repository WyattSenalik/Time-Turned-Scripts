using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation.BetterCurve;
using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// <see cref="IPitfallHandler"/> for pickup objects. Also records when they fall into the pit for the purpose of disallowing them to be picked up.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IPickUpObject))]
    [RequireComponent(typeof(PickUpStateManager))]
    public sealed class PickUpItemPitfallHandler : TimedRecorder, IPitfallHandler
    {
        private const bool IS_DEBUGGING = false;
        private const float MOVING_SQ_MAG_MIN_VALUE = 0.1f;
        private const int FIND_TIME_FOR_ANIM_INCREMENTS = 8;

        public IPickUpObject pickupObj { get; private set; } = null;
        public PickUpStateManager stateManager { get; private set; } = null;
        public float fallIntoPitTime { get; private set; } = float.PositiveInfinity;
        public int disallowPickupId { get; private set; } = -1;
        public bool isOverPit { get; private set; } = false;
        public bool isInPit => fallIntoPitTime != float.PositiveInfinity;

        [SerializeField, Required] private BetterCurveSO m_sizeWhenFallingCurve = null;

        private TimedBool m_timedIsPlayingFall = null;
        private float m_timeFallBegan = float.PositiveInfinity;
        private float m_fallAnimStartTimeOffset = 0.0f;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_sizeWhenFallingCurve, nameof(m_sizeWhenFallingCurve), this);
            #endregion Asserts
            pickupObj = this.GetIComponentSafe<IPickUpObject>(this);
            stateManager = this.GetComponentSafe<PickUpStateManager>(this);
        }
        private void Start()
        {
            m_timedIsPlayingFall = new TimedBool(false);
        }


        public override void TrimDataAfter(float time)
        {
            base.TrimDataAfter(time);

            // If the the new time is before the object fell into the pit, reset stuff.
            if (fallIntoPitTime > time)
            {
                ResetFallIntoPitTrackingValues();
            }
        }


        #region IPitfallHandler
        public void OnEnclosedInPitStart()
        {
            isOverPit = true;
        }
        public void OnEnclosedInPitEnd()
        {
            isOverPit = false;
        }
        public void Fall(GameObject pitVisualsParentObj)
        {
            isOverPit = true;
            HandleFall();
        }
        public void FallStay(GameObject pitVisualsParentObj)
        {
            isOverPit = true;
            HandleFall();
        }
        #endregion IPitfallHandler


        private void HandleFall()
        {
            // Currently being held, so no need to fall.
            if (pickupObj.isHeld) { return; }
            // Wait until InAir is over.
            if (stateManager.curState == ePickupState.FallingIntoVoid)
            {
                if (!m_timedIsPlayingFall.curData)
                {
                    // Fall should start
                    m_timedIsPlayingFall.curData = true;
                    m_timeFallBegan = curTime;
                    if (transform.localScale.x < 1.0f)
                    {
                        // Need to start falling later than usual.
                        m_fallAnimStartTimeOffset = FindTimeToBeginAnimForFallBasedOnSize(transform.localScale.x);
                    }
                    else
                    {
                        m_fallAnimStartTimeOffset = 0.0f;
                    }
                }

                float t_curAnimTime = (curTime - m_timeFallBegan) + m_fallAnimStartTimeOffset;
                float t_endAnimTime = m_sizeWhenFallingCurve.GetEndTime();
                // Check if the fall is over
                if (t_curAnimTime >= t_endAnimTime)
                {
                    OnFallAnimEnd();
                }
                // Fall not over, update the size
                else
                {
                    float t_size = m_sizeWhenFallingCurve.Evaluate(t_curAnimTime);
                    transform.localScale = new Vector3(t_size, t_size, t_size);
                }
            }
        }
        private void OnFallAnimEnd()
        {
            #region Error Checks
            if (disallowPickupId != -1)
            {
                //CustomDebug.LogErrorForComponent($"Expected {nameof(disallowPickupId)} to be unitialized, but its value was {disallowPickupId}", this);
                pickupObj.CancelDisallowPickupRequest(disallowPickupId);
                disallowPickupId = -1;
            }
            if (fallIntoPitTime != float.PositiveInfinity)
            {
                //CustomDebug.LogErrorForComponent($"Expected {nameof(fallIntoPitTime)} to be unitialized, but its value was {fallIntoPitTime}.", this);
            }
            #endregion Error Checks
            disallowPickupId = pickupObj.RequestDisallowPickup();
            fallIntoPitTime = curTime;
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(OnFallAnimEnd)}. {nameof(disallowPickupId)}={disallowPickupId}", this, IS_DEBUGGING);
            #endregion Logs
        }
        private void ResetFallIntoPitTrackingValues()
        {
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(ResetFallIntoPitTrackingValues)}. {nameof(disallowPickupId)}={disallowPickupId}", this, IS_DEBUGGING);
            #endregion Logs
            if (disallowPickupId != -1)
            {
                pickupObj.CancelDisallowPickupRequest(disallowPickupId);
                disallowPickupId = -1;
            }
            fallIntoPitTime = float.PositiveInfinity;
            isOverPit = false;
        }


        private float FindTimeToBeginAnimForFallBasedOnSize(float size)
        {
            float t_minTime = 0.0f;
            float t_maxTime = m_sizeWhenFallingCurve.GetEndTime();
            float t_pivotTime = float.NaN;
            for (int i = 0; i < FIND_TIME_FOR_ANIM_INCREMENTS; ++i)
            {
                t_pivotTime = (t_minTime + t_maxTime) * 0.5f;
                float t_curSize = m_sizeWhenFallingCurve.Evaluate(t_pivotTime);
                if (t_curSize < size)
                {
                    t_maxTime = t_pivotTime;
                }
                else if (t_curSize > size)
                {
                    t_minTime = t_pivotTime;
                }
                else
                {
                    return t_pivotTime;
                }
            }
            return t_pivotTime;
        }
    }
}