using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    public enum eTimeFrameContainsOption { Inclusive, Exclusive, StartExclusive, EndExclusive }

    /// <summary>
    /// Window of time between two times.
    /// </summary>
    [Serializable]
    public struct TimeFrame
    {
        public static TimeFrame NaN => new TimeFrame(float.NaN, float.NaN);

        /// <summary>
        /// Start of the time frame.
        /// </summary>
        public float startTime => m_startTime;
        /// <summary>
        /// End of the time frame.
        /// </summary>
        public float endTime => m_endTime;
        /// <summary>
        /// How long the time frame is. 
        /// <see cref="endTime"/>-<see cref="startTime"/>.
        /// </summary>
        public float length => endTime - startTime;

        [SerializeField] private float m_startTime;
        [SerializeField] private float m_endTime;


        public TimeFrame(float start, float end)
        {
            m_startTime = start;
            m_endTime = end;
        }


        public bool ContainsTime(float time, eTimeFrameContainsOption containsOpt = eTimeFrameContainsOption.Inclusive)
        {
            switch (containsOpt)
            {
                case eTimeFrameContainsOption.Inclusive:
                    return startTime <= time && time <= endTime;
                case eTimeFrameContainsOption.Exclusive:
                    return startTime < time && time < endTime;
                case eTimeFrameContainsOption.StartExclusive:
                    return startTime < time && time <= endTime;
                case eTimeFrameContainsOption.EndExclusive:
                    return startTime <= time && time < endTime;
                default:
                    CustomDebug.UnhandledEnum(containsOpt, GetType().Name);
                    return ContainsTime(time);
            }
        }
        public bool HasOverlap(TimeFrame other)
        {
            return startTime <= other.endTime && other.startTime <= endTime;
        }


        public override string ToString()
        {
            return $"({startTime}, {endTime})";
        }

        public static bool IsNaN(TimeFrame frame)
        {
            return float.IsNaN(frame.m_startTime) && float.IsNaN(frame.m_endTime);
        }
    }
}