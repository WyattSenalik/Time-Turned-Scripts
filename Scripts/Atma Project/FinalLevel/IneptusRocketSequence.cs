using UnityEngine;

using NaughtyAttributes;
using Timed;
using System;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class IneptusRocketSequence : TimedRecorder
    {
        [SerializeField, Required] private TimedNPCMover m_npcMover = null;
        [SerializeField, Required] private RocketLiftoff m_rocketLiftoff = null;
        [SerializeField] private TransformAndTime[] m_moveToPositionsAndWhen = new TransformAndTime[0];

        private TimedObjectReference<TransformAndTime> m_timedCurMoveToPosAndWhen = null;
        private TimedInt m_timedPathIndex = null;

        private int m_cancelRecordingRequestID = -1;


        private void Start()
        {
            m_timedCurMoveToPosAndWhen = new TimedObjectReference<TransformAndTime>(null as TransformAndTime);
            m_timedPathIndex = new TimedInt(0, eInterpolationOption.Step);
        }

        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            // Turn off ineptus when rocket is taking off.
            if (curTime < m_rocketLiftoff.stage3BeginTime)
            {
                if (m_cancelRecordingRequestID != -1)
                {
                    m_npcMover.timedObject.CancelSuspendRecordingRequest(m_cancelRecordingRequestID);
                    m_cancelRecordingRequestID = -1;
                }
            }
            else
            {
                if (m_cancelRecordingRequestID == -1)
                {
                    m_cancelRecordingRequestID = m_npcMover.timedObject.RequestSuspendRecording();
                }
            }

            if (isRecording)
            {
                TransformAndTime t_mostRecentMoveToPositionAndTime = GetMoveRecentMovePosition();
                if (t_mostRecentMoveToPositionAndTime != null)
                {
                    int t_curIndex = m_timedPathIndex.curData;
                    int t_newIndex = t_curIndex;
                    if (m_timedCurMoveToPosAndWhen.curData.objRef != t_mostRecentMoveToPositionAndTime)
                    {
                        t_newIndex = 0;
                    }

                    m_timedCurMoveToPosAndWhen.curData = new ObjectReferenceData<TransformAndTime>(t_mostRecentMoveToPositionAndTime);
                    m_npcMover.SetTargetPosition(t_mostRecentMoveToPositionAndTime.transPath[t_newIndex].position);
                    if (!m_npcMover.isMoving)
                    {
                        if (t_newIndex < t_mostRecentMoveToPositionAndTime.transPath.Length - 1)
                        {
                            ++t_newIndex;
                        }

                        switch (t_mostRecentMoveToPositionAndTime.faceDirAfter)
                        {
                            case eFaceDir.None: break;
                            case eFaceDir.Up:
                            {
                                m_npcMover.PlayBackwardIdle();
                                break;
                            }
                            case eFaceDir.Down:
                            {
                                m_npcMover.PlayForwardIdle();
                                break;
                            }
                            default:
                            {
                                CustomDebug.UnhandledEnum(t_mostRecentMoveToPositionAndTime.faceDirAfter, this);
                                break;
                            }
                        }
                    }
                    m_timedPathIndex.curData = t_newIndex;
                }
            }
        }


        private TransformAndTime GetMoveRecentMovePosition()
        {
            TransformAndTime t_mostRecent = null;
            for (int i = 0; i < m_moveToPositionsAndWhen.Length; ++i)
            {
                TransformAndTime t_moveToPosAndTime = m_moveToPositionsAndWhen[i];
                if (t_moveToPosAndTime.time <= curTime)
                {
                    if (t_mostRecent == null || t_moveToPosAndTime.time > t_mostRecent.time)
                    {
                        t_mostRecent = t_moveToPosAndTime;
                    }
                }
            }
            return t_mostRecent;
        }


        [Serializable]
        public sealed class TransformAndTime
        {
            public Transform[] transPath => m_transPath;
            public float time => m_time;
            public eFaceDir faceDirAfter => m_faceDirAfter;

            [SerializeField] private Transform[] m_transPath = new Transform[0];
            [SerializeField] private float m_time = 0.0f;
            [SerializeField] private eFaceDir m_faceDirAfter = eFaceDir.None;

        }
        public enum eFaceDir { None, Up, Down }
    }
}