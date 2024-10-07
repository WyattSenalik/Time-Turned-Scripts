using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace Helpers.Animation.BetterCurve
{
    [Serializable]
    public class BetterCurve : IBetterCurve
    {
        public enum eTimeChoice { Scale, Duration}

        // Constants
        private const bool IS_DEBUGGING = false;

        public float minValue => m_minValue;
        public float maxValue => m_maxValue;
        public eTimeChoice timing => m_timing;
        public float timeScale
        {
            get
            {
                switch (m_timing)
                {
                    case eTimeChoice.Scale:
                        return m_timeScale;
                    case eTimeChoice.Duration:
                        return 1 / m_timeDuration;
                    default:
                        CustomDebug.UnhandledEnum(m_timing, GetType().Name);
                        return -1.0f;
                }
            }
        }
        public float timeDuration
        {
            get
            {
                switch (m_timing)
                {
                    case eTimeChoice.Scale:
                        return 1 / m_timeScale;
                    case eTimeChoice.Duration:
                        return m_timeDuration;
                    default:
                        CustomDebug.UnhandledEnum(m_timing, GetType().Name);
                        return -1.0f;
                }
            }
        }

        [SerializeField, AllowNesting, CurveRange(0, 0, 1, 1)]
        private AnimationCurve m_rawCurve = null;
        [SerializeField] private float m_minValue = 0.0f;
        [SerializeField] private float m_maxValue = 1.0f;
        [SerializeField] private eTimeChoice m_timing = eTimeChoice.Scale;
        [SerializeField, AllowNesting, Min(0.0001f), ShowIf(nameof(TimeIsScale))]
        private float m_timeScale = 1.0f;
        [SerializeField, AllowNesting, Min(0.0001f), ShowIf(nameof(TimeIsDuration))]
        private float m_timeDuration = 1.0f;


        public BetterCurve()
        {
            m_rawCurve = new AnimationCurve();
            m_minValue = 0.0f;
            m_maxValue = 1.0f;
            m_timeScale = 1.0f;
        }
        public BetterCurve(float min, float max)
        {
            m_rawCurve = new AnimationCurve();
            m_minValue = min;
            m_maxValue = max;
            m_timeScale = 1.0f;
        }
        public BetterCurve(AnimationCurve animCurve, float min, float max, float timeScale)
        {
            m_rawCurve = animCurve;
            m_minValue = min;
            m_maxValue = max;
            m_timeScale = timeScale;
        }
        public float Evaluate(float time)
        {
            float t_scaledTime = time * timeScale;
            #region Logs
            //CustomDebug.LogForObject($"Given time {time} scaled to {t_scaledTime}", this, IS_DEBUGGING);
            #endregion Logs
            //#region Asserts
            //Assert.IsTrue(t_scaledTime >= 0 && t_scaledTime <= 1,
            //    $"{GetType().Name}'s {nameof(Evaluate)} expected given time " +
            //    $"({time}) to be in the range 0-1 once scaled with {timeScale}");
            //#endregion Asserts
            float t_rawTime = m_rawCurve.Evaluate(t_scaledTime);
            return t_rawTime * (m_maxValue - m_minValue) + m_minValue;
        }
        public float EvaluateClamped(float time)
        {
            float t_clampedTime = Mathf.Clamp(time, 0.0f, GetEndTime());
            return Evaluate(t_clampedTime);
        }
        public float GetEndTime()
        {
            if (m_rawCurve.length <= 0)
            {
                Debug.LogError($"{GetType().Name}'s curve has no points");
                return 0.0f;
            }

            float t_rawTime = m_rawCurve.keys[m_rawCurve.length - 1].time;
            float t_scaledTime = t_rawTime / timeScale;
            #region Logs
            //CustomDebug.LogForObject($"End time calcualted to be {t_scaledTime}.", this, IS_DEBUGGING);
            #endregion Logs
            return t_scaledTime;
        }


        #region Editor
        private bool TimeIsScale() => m_timing == eTimeChoice.Scale;
        private bool TimeIsDuration() => m_timing == eTimeChoice.Duration;
        #endregion Editor
    }
}
