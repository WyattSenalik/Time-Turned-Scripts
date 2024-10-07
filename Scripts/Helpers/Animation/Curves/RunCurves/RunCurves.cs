using System;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Common curves for run speed.
    /// </summary>
    [Serializable]
    public sealed class RunCurves : IRunCurves
    {
        public float accelTime => m_accelTime;
        public float decelTime => m_decelTime;
        public float topSpeed => m_topSpeed;
        public AnimationCurve accelCurve => m_accelCurve;
        public AnimationCurve decelCurve => m_decelCurve;

        [SerializeField, Min(0.0f)] private float m_accelTime = 0.1f; 
        [SerializeField, Min(0.0f)] private float m_decelTime = 0.05f;
        [SerializeField, Min(0.0f)] private float m_topSpeed = 4.0f;

        [SerializeField, AllowNesting, CurveRange(0, 0, 1, 1)]
        private AnimationCurve m_accelCurve = new AnimationCurve(new Keyframe[2]
        { 
            new Keyframe(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f / 3), 
            new Keyframe(1.0f, 1.0f, 2.0f, 0.0f, 1.0f / 3, 0.0f) 
        });
        [SerializeField, AllowNesting, CurveRange(0, 0, 1, 1)]
        private AnimationCurve m_decelCurve = new AnimationCurve(new Keyframe[2]
        {
            new Keyframe(0.0f, 1.0f, 0.0f, -2.0f, 0.0f, 1.0f / 3),
            new Keyframe(1.0f, 0.0f, 0.0f, 0.0f, 1.0f / 3, 0.0f)
        });


        public float EvaluateAccelMove(float beginRunTime, float curTime)
        {
            float t_timeSinceBegin = curTime - beginRunTime;
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(t_timeSinceBegin >= 0, $"expected curTime ({curTime}) to be after beginRunTime ({beginRunTime})", this);
            #endregion Asserts
            if (t_timeSinceBegin < m_accelTime)
            {
                return m_accelCurve.Evaluate(t_timeSinceBegin / m_accelTime) * m_topSpeed;
            }
            else
            {
                return m_topSpeed;
            }
        }
        public float EvaluateDecel(float beginDecel, float curTime)
        {
            float t_timeSinceBegin = curTime - beginDecel;
            #region Asserts
            //CustomDebug.AssertIsTrueForObj(t_timeSinceBegin >= 0, $"expected curTime ({curTime}) to be after beginDecel ({beginDecel})", this);
            #endregion Asserts
            if (t_timeSinceBegin < m_decelTime)
            {
                return m_decelCurve.Evaluate(t_timeSinceBegin / m_decelTime) * m_topSpeed;
            }
            else
            {
                return 0.0f;
            }
        }
    }
}