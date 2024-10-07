using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Timed;
using Timed.TimedComponentImplementations;
using System.Linq;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Time Clone Copier Class.
    /// Plays the transform for the time clone.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeClone))]
    [RequireComponent(typeof(Int2DTransform))]
    [RequireComponent(typeof(CharacterMover))]
    public sealed class TimeCloneTransform : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public TimedInt internalPosX => m_timedPosX;
        public TimedInt internalPosY => m_timedPosY;
        public TimedInt internalAngle => m_timedAngle;
        public TimedInt internalSizeX => m_timedSizeX;
        public TimedInt internalSizeY => m_timedSizeY;
        public TimedInt internalReplayPosX => m_replayTimedPosX;
        public TimedInt internalReplayPosY => m_replayTimedPosY;

        [SerializeField, Required] private PushableBoxCollider m_boxCollider = null;
        [SerializeField] private int m_tooFarAwayIntDist = 16;

        /// <summary>For debugging only.</summary>
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private int m_snapCount = 0;
        [SerializeField, ReadOnly, BoxGroup("Debugging")] private bool m_isCurTimeDuringLeapDebug = false;

        private GlobalTimeManager m_timeMan = null;

        private TimeClone m_clone = null;
        private Int2DTransform m_intTransform = null;
        private CharacterMover m_charMover = null;

        private TimedInt m_timedPosX = null;
        private TimedInt m_timedPosY = null;

        private TimedInt m_timedAngle = null;

        private TimedInt m_timedSizeX = null;
        private TimedInt m_timedSizeY = null;

        private TimedInt m_replayTimedPosX = null;
        private TimedInt m_replayTimedPosY = null;

        private bool m_didRecordRotation = false;
        private bool m_didRecordSize = false;

        private float m_earlyDisappearanceTime = float.PositiveInfinity;
        private int m_earlyDisappearanceID = int.MinValue;

        private readonly List<LeapOccurrence> m_leapTimes = new List<LeapOccurrence>();
        private float m_leapDuration = float.NegativeInfinity;

        private bool m_hasRecordedReplay = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_boxCollider, nameof(m_boxCollider), this);
            #endregion Asserts
            m_clone = this.GetComponentSafe<TimeClone>();
            m_intTransform = this.GetComponentSafe<Int2DTransform>();
            m_charMover = this.GetComponentSafe<CharacterMover>();

            m_clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (m_clone != null)
            {
                m_clone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }
        private void Initialize()
        {
            m_timeMan = GlobalTimeManager.instance;

            GameObject t_origPlayerObj = m_clone.cloneData.originalPlayerObj;
            TimedIntTransform t_ogTimedTrans = t_origPlayerObj.GetComponentSafe<TimedIntTransform>();
            Leaper t_ogLeaper = t_origPlayerObj.GetComponentSafe<Leaper>();

            m_leapTimes.AddRange(t_ogLeaper.leapTimes);
            m_leapDuration = t_ogLeaper.duration;

            m_timedPosX = new TimedInt(t_ogTimedTrans.timedPosX, true);
            m_timedPosY = new TimedInt(t_ogTimedTrans.timedPosY, true);
            m_snapCount = m_timedPosX.scrapbook.Count + m_timedPosY.scrapbook.Count;
            if (t_ogTimedTrans.shouldRecordRotation)
            {
                m_timedAngle = new TimedInt(t_ogTimedTrans.timedAngle, true);
                m_didRecordRotation = true;
                m_snapCount += m_timedAngle.scrapbook.Count;
            }
            if (t_ogTimedTrans.shouldRecordSize)
            {
                m_timedSizeX = new TimedInt(t_ogTimedTrans.timedSizeX, true);
                m_timedSizeY = new TimedInt(t_ogTimedTrans.timedSizeY, true);
                m_didRecordSize = true;
                m_snapCount += m_timedSizeX.scrapbook.Count + m_timedSizeY.scrapbook.Count;
            }

            // We make the replays copy the whole thing because they will get trimmed if resuming and if not, they will be used for rewind (really only matters for rewinding on the final level).
            SnapshotScrapbook<IntSnapshot, int> t_replayPosXScrapbook = new SnapshotScrapbook<IntSnapshot, int>(m_timedPosX.scrapbook.GetAllSnapshotsAfter(0.0f, false));
            t_replayPosXScrapbook.RemoveSnapshotsAfter(m_timeMan.curTime);
            SnapshotScrapbook<IntSnapshot, int> t_replayPosYScrapbook = new SnapshotScrapbook<IntSnapshot, int>(m_timedPosX.scrapbook.GetAllSnapshotsAfter(0.0f, false));
            t_replayPosYScrapbook.RemoveSnapshotsAfter(m_timeMan.curTime);
            m_replayTimedPosX = new TimedInt(t_replayPosXScrapbook, eInterpolationOption.Linear);
            m_replayTimedPosY = new TimedInt(t_replayPosYScrapbook, eInterpolationOption.Linear);
            m_hasRecordedReplay = false;
        }

        private void FixedUpdate()
        {
            m_isCurTimeDuringLeapDebug = IsTimeDuringLeap(m_timeMan.curTime);
            float t_curTime = m_timedPosX.curTime;

            // If the time clone has an early disapperance before this time, do nothing
            if (m_clone.HasEarlyDisappearanceBeforeOrAtTime(t_curTime)) { return; }

            // Always update angle and size
            if (m_didRecordRotation)
            {
                m_intTransform.localAngle = m_timedAngle.curData;
            }
            if (m_didRecordSize)
            {
                m_intTransform.localSize = new Vector2Int(m_timedSizeX.curData, m_timedSizeY.curData);
            }

            Vector2Int t_desiredPosInt = new Vector2Int(m_timedPosX.curData, m_timedPosY.curData);
            Vector2 t_desiredPosFloat = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_desiredPosInt);
            Vector2 t_curPosFloat = m_intTransform.localPositionFloat;
            Vector2 t_physicsAwarePosFloat;
            if (m_replayTimedPosX.isRecording)
            {
                if (IsTimeDuringLeap(t_curTime, out LeapOccurrence t_leapOccurrence))
                {
                    // We are leaping, set position straight for the recording.
                    m_charMover.ResetEstimatedPosition();
                    m_intTransform.localPosition = t_desiredPosInt;
                    // Set the replay stuff
                    m_replayTimedPosX.curData = t_desiredPosInt.x;
                    m_replayTimedPosY.curData = t_desiredPosInt.y;
                    m_hasRecordedReplay = true;
                    // Check to reset early kill.
                    ResetEarlyDisappearanceIfShould(t_curTime);

                    t_physicsAwarePosFloat = t_desiredPosFloat;

                    float t_prevTime = t_curTime - m_replayTimedPosX.deltaTime;
                    if (!IsTimeDuringLeap(t_prevTime, out LeapOccurrence t_prevLeapOccurrence))
                    {
                        // If the previous time was not during a leap, tell the leap object we are leaping.
                        t_leapOccurrence.leapObj.OnLeptFrom();
                    }
                    else if (t_prevLeapOccurrence != t_leapOccurrence)
                    {
                        // If the previous time was a different leap, tell the leap object we are leaping .
                        t_leapOccurrence.leapObj.OnLeptFrom();
                    }
                }
                else
                {
                    // Figure out if we should push stuff.
                    Vector2 t_posDiff = t_desiredPosFloat - t_curPosFloat;
                    m_charMover.velocity = t_posDiff;
                    if (!m_charMover.DoPhysicsUpdate())
                    {
                        // Didn't hit anything, so just update the position to be the desired position.
                        m_intTransform.localPosition = t_desiredPosInt;
                    }
                    Vector2Int t_realPositionInt = m_intTransform.localPosition;
                    // Set the replay stuff
                    m_replayTimedPosX.curData = t_realPositionInt.x;
                    m_replayTimedPosY.curData = t_realPositionInt.y;
                    m_hasRecordedReplay = true;
                    // Check to reset early kill.
                    ResetEarlyDisappearanceIfShould(t_curTime);
                    // Check if we should kill clone early.
                    Vector2Int t_realVsDesiredDiff = t_realPositionInt - t_desiredPosInt;
                    if (t_realVsDesiredDiff.sqrMagnitude > m_tooFarAwayIntDist * m_tooFarAwayIntDist)
                    {
                        CauseEarlyDisapperance(t_curTime);
                    }

                    t_physicsAwarePosFloat = m_intTransform.localPositionFloat;
                }
            }
            else
            {
                m_charMover.ResetEstimatedPosition();
                Vector2Int t_replayPosInt;
                if (m_hasRecordedReplay && t_curTime > 0.0f)
                {
                    t_replayPosInt = new Vector2Int(m_replayTimedPosX.curData, m_replayTimedPosY.curData);
                }
                else
                {
                    t_replayPosInt = t_desiredPosInt;
                }
                m_intTransform.localPosition = t_replayPosInt;
                t_physicsAwarePosFloat = CustomPhysics2DInt.ConvertIntPositionToFloatPosition(t_replayPosInt);
            }

            //CustomDebug.DrawCrossHair(t_desiredPosFloat, 0.1f, Color.yellow, IS_DEBUGGING);
            //CustomDebug.DrawCircle(t_desiredPosFloat, 0.1f, 6, Color.yellow, IS_DEBUGGING);
            //CustomDebug.DrawCrossHair(t_physicsAwarePosFloat, 0.1f, Color.blue, IS_DEBUGGING);
            //CustomDebug.DrawCircle(t_physicsAwarePosFloat, 0.1f, 6, Color.blue, IS_DEBUGGING);
        }

        public Vector2 GetPositionForTime(float time)
        {
            return CustomPhysics2DInt.ConvertIntPositionToFloatPosition(new Vector2Int(m_timedPosX.scrapbook.GetSnapshot(time).data, m_timedPosY.scrapbook.GetSnapshot(time).data));
        }


        private bool IsTimeDuringLeap(float curTime) => IsTimeDuringLeap(curTime, out _);
        private bool IsTimeDuringLeap(float curTime, out LeapOccurrence activeOccurrence)
        {
            const float LENIENCY = 0.0f;

            for (int i = 0; i < m_leapTimes.Count; ++i)
            {
                activeOccurrence = m_leapTimes[i];
                float t_leapStartTime = activeOccurrence.time;
                float t_leapEndTime = t_leapStartTime + m_leapDuration;
                if (t_leapStartTime - LENIENCY <= curTime && curTime <= t_leapEndTime + LENIENCY)
                {
                    return true;
                }
            }
            activeOccurrence = null;
            return false;
        }
        private void CauseEarlyDisapperance(float curTime)
        {
            m_earlyDisappearanceTime = curTime;
            m_earlyDisappearanceID = m_clone.RegisterEarlyDisappearTime(m_earlyDisappearanceTime);
        }
        private void ResetEarlyDisappearanceIfShould(float curTime)
        {
            if (ShouldResetEarlyDisappearance(curTime))
            {
                ResetEarlyDisappearance();
            }
        }
        private void ResetEarlyDisappearance()
        {
            // If not already reset.
            if (m_earlyDisappearanceID != int.MinValue)
            {
                m_earlyDisappearanceTime = float.PositiveInfinity;
                m_clone.UnregisterEarlyDisappearTime(m_earlyDisappearanceID);
                m_earlyDisappearanceID = int.MinValue;
            }
        }
        private bool ShouldResetEarlyDisappearance(float curTime)
        {
            if (m_earlyDisappearanceID == int.MinValue)
            {
                return false;
            }
            return m_earlyDisappearanceTime > curTime;
        }
    }
}