using System;
using UnityEngine;

using Helpers.Events;
using Helpers.UnityEnums;

using Helpers.Physics.Custom2DInt;
using Timed;
using Timed.Animation.BetterCurve;
using Helpers.Extensions;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IMovementSuspender))]
    [RequireComponent(typeof(Int2DTransform))]
    public sealed class Leaper : TimedBetterCurveAnimation, ILeaper
    {
        public float leapDistance => m_leapDistance;
        public IEventPrimer onLeapBegin => m_onLeapBegin;
        public IEventPrimer onLeapEnd => m_onLeapEnd;
        public IReadOnlyList<LeapOccurrence> leapTimes => m_leapTimes;

        [SerializeField] private float m_leapDistance = 2.5f;
        [SerializeField] private PushableBoxCollider m_pusherCollider = null;
        [SerializeField] private PlayerPitfallHandler m_pitfallHandler = null;
        [SerializeField] private MixedEvent m_onLeapBegin = new MixedEvent();
        [SerializeField] private MixedEvent m_onLeapEnd = new MixedEvent();

        private BoxPhysicsManager m_boxPhysicsMan = null;

        private Int2DTransform m_intTransform = null;
        private IMovementSuspender m_movementSuspender = null;
        private TimedVector2 m_leapStartPositions = null;
        private TimedVector2 m_leapTargetPositions = null;
        private readonly List<LeapOccurrence> m_leapTimes = new List<LeapOccurrence>();
        private TimedInt m_leapDir = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pusherCollider, nameof(m_pusherCollider), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pitfallHandler, nameof(m_pitfallHandler), this);
            #endregion Asserts

            m_intTransform = this.GetComponentSafe<Int2DTransform>();
            m_movementSuspender = this.GetIComponentSafe<IMovementSuspender>();
        }
        protected override void Start()
        {
            base.Start();

            m_boxPhysicsMan = BoxPhysicsManager.instance;

            m_leapStartPositions = new TimedVector2(Vector2.negativeInfinity, eInterpolationOption.Step);
            m_leapTargetPositions = new TimedVector2(Vector2.negativeInfinity, eInterpolationOption.Step);

            m_leapDir = new TimedInt(-1, eInterpolationOption.Step);
        }
        private void OnEnable()
        {
            onEnd += m_onLeapEnd.Invoke;
        }
        private void OnDisable()
        {
            onEnd -= m_onLeapEnd.Invoke;
        }


        public void Leap(eEightDirection leapDir, ILeapObject objLeaptFrom)
        {
            m_leapTimes.Add(new LeapOccurrence(curTime, objLeaptFrom));
            m_leapDir.curData = (int)leapDir;

            // Disallow movement while leaping.
            m_movementSuspender.SuspendForTime(curve.timeDuration, false);

            m_leapStartPositions.curData = transform.position;
            m_leapTargetPositions.curData = GetLeapEndPosition(leapDir, objLeaptFrom, m_leapDistance);
            // Play the leap animation.
            Play(true);
            // Invoke begin event.
            m_onLeapBegin.Invoke();
        }
        protected override void OnEnd()
        {
            base.OnEnd();

            if (!m_pitfallHandler.isOverPit)
            {
                // Correct position
                float t_singleIncrDist = 4 * CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT;
                eEightDirection t_recentEightDir = (eEightDirection)m_leapDir.curData;
                Vector2 t_recentDir = t_recentEightDir.ToOffset();
                Vector2 t_beginPosFloat = m_intTransform.positionFloat;
                int t_iterations = 1;
                // See which is closer, going forward, or going backward
                while (true)
                {
                    Vector2 t_diff = (t_iterations * t_singleIncrDist) * t_recentDir;
                    // Backtrack first.
                    m_intTransform.positionFloat = t_beginPosFloat - t_diff;
                    RectangleInt t_checkRect = m_pusherCollider.rectangleCollider.rectangle;
                    if (m_boxPhysicsMan.IsRectangleInValidStandingOrFallingPosition(t_checkRect))
                    {
                        break;
                    }
                    // Go further second.
                    m_intTransform.positionFloat = t_beginPosFloat + t_diff;
                    t_checkRect = m_pusherCollider.rectangleCollider.rectangle;
                    if (m_boxPhysicsMan.IsRectangleInValidStandingOrFallingPosition(t_checkRect))
                    {
                        break;
                    }

                    ++t_iterations;
                    if (t_iterations >= 1000)
                    {
                        //CustomDebug.LogError($"INFINITY GUARD OCCURRED WHEN LEAPING. No valid standing spots after leaping???");
                        m_intTransform.positionFloat = t_beginPosFloat;
                        break;
                    }
                }
            }
            m_leapDir.curData = -1;
        }

        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);
            // Trim times.
            for (int i = m_leapTimes.Count - 1; i >= 0; --i)
            {
                if (m_leapTimes[i].time >= time)
                {
                    m_leapTimes.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }
        }

        public bool IsLeapingAtTime(float time)
        {
            for (int i = 0; i < m_leapTimes.Count; ++i)
            {
                float t_leapStartTime = m_leapTimes[i].time;
                float t_leapEndTime = t_leapStartTime + duration;
                if (t_leapStartTime <= time && time <= t_leapEndTime)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TryGetLeapTimeFrame(float time, out TimeFrame leapTimeFrame)
        {
            for (int i = 0; i < m_leapTimes.Count; ++i)
            {
                float t_leapStartTime = m_leapTimes[i].time;
                float t_leapEndTime = t_leapStartTime + duration;
                if (t_leapStartTime <= time && time <= t_leapEndTime)
                {
                    leapTimeFrame = new TimeFrame(t_leapStartTime, t_leapEndTime);
                    return true;
                }
            }
            leapTimeFrame = TimeFrame.NaN;
            return false;
        }



        public static Vector2 GetLeapEndPosition(eEightDirection leapDir, ILeapObject objLeaptFrom, float leapDistance)
        {
            return objLeaptFrom.leapObjectPos + leapDistance * leapDir.ToOffset();
        }


        protected override void TakeCurveAction(float curveValue)
        {
            // Now we changed the curve to be the t value of lerping between the start and end position.
            Vector2 t_curPosFloat = m_intTransform.positionFloat;
            Vector2 t_desiredPosFloat = Vector2.Lerp(m_leapStartPositions.curData, m_leapTargetPositions.curData, curveValue);
            Vector2 t_diffFloat = t_desiredPosFloat - t_curPosFloat;
            float t_dist = t_diffFloat.magnitude;
            Vector2 t_dir = t_diffFloat * (1 / t_dist);
            // Check to see if we hit anything
            RectangleInt t_curRect = m_pusherCollider.rectangleCollider.rectangle;
            float t_singleIncrDist = 4 * CustomPhysics2DInt.INVERTED_INTS_PER_UNIT_AS_FLOAT;
            Vector2 t_singleIncrVel = t_dir * t_singleIncrDist;
            int t_amValidIncrements = 0;
            bool t_wasStoppedEarly = false;
            for (int i = 1; i * t_singleIncrDist < t_dist; ++i)
            {
                Vector2 t_curIncrBotLeftFloat = t_curRect.GetBotLeftPointAsFloatPosition() + i * t_singleIncrVel;
                Vector2Int t_curIncrBotLeft = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(t_curIncrBotLeftFloat);
                RectangleInt t_curIncrRect = new RectangleInt(t_curIncrBotLeft, t_curRect.size);
                if (m_boxPhysicsMan.DoesRectangleOverlapWallWithWallTag(t_curIncrRect))
                {
                    t_wasStoppedEarly = true;
                    break;
                }
                t_amValidIncrements = i;
            }
            Vector2 t_validPosFloat = t_desiredPosFloat;
            if (t_wasStoppedEarly)
            {
                t_validPosFloat = t_curPosFloat + t_singleIncrVel * t_amValidIncrements;
            }
            m_intTransform.positionFloat = t_validPosFloat;
        }


        [Serializable]
        public sealed class Collider2DContactFilterPair
        {
            public Collider2D collider => m_collider;
            public ContactFilter2D contactFilter => m_contactFilter;

            [SerializeField] private Collider2D m_collider = null; 
            [SerializeField] private ContactFilter2D m_contactFilter = default;
        }
    }

    public sealed class LeapOccurrence
    {
        public float time { get; private set; } = 0.0f;
        public ILeapObject leapObj { get; private set; } = null;


        public LeapOccurrence(float time, ILeapObject leapObj)
        {
            this.time = time;
            this.leapObj = leapObj;
        }
    }
}
