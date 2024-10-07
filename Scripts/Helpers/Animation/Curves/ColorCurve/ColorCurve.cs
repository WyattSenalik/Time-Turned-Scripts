using System;
using UnityEngine;
using UnityEngine.Assertions;

using NaughtyAttributes;

using static Helpers.Animation.BetterCurve.BetterCurve;
using Helpers.Math;
// Original Authors - Wyatt Senalik (Created referencing BetterCurve)

namespace Helpers.Animation.BetterCurve
{
    [Serializable]
    public sealed class ColorCurve : IColorCurve
    {
        // Constants
        private const bool IS_DEBUGGING = false;

        public Color minValue => m_minValue;
        public Color maxValue => m_maxValue;
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
        [SerializeField] private Color m_minValue = Color.white;
        [SerializeField] private Color m_maxValue = Color.black;
        [SerializeField] private bool m_hasIntermediateColors = false;
        [SerializeField, AllowNesting, ShowIf(nameof(m_hasIntermediateColors))]
        private IntermediateColor[] m_intermediateColors = new IntermediateColor[1];
        [SerializeField] private eTimeChoice m_timing = eTimeChoice.Scale;
        [SerializeField, AllowNesting, Min(0.0001f), ShowIf(nameof(TimeIsScale))]
        private float m_timeScale = 1.0f;
        [SerializeField, AllowNesting, Min(0.0001f), ShowIf(nameof(TimeIsDuration))]
        private float m_timeDuration = 1.0f;


        public ColorCurve()
        {
            m_rawCurve = new AnimationCurve();
            m_minValue = Color.white;
            m_maxValue = Color.black;
            m_timeScale = 1.0f;
        }
        public ColorCurve(Color min, Color max)
        {
            m_rawCurve = new AnimationCurve();
            m_minValue = min;
            m_maxValue = max;
            m_timeScale = 1.0f;
            m_hasIntermediateColors = false;
            m_intermediateColors = new IntermediateColor[0];
        }
        public ColorCurve(AnimationCurve animCurve, Color min, Color max, float timeScale)
        {
            m_rawCurve = animCurve;
            m_minValue = min;
            m_maxValue = max;
            m_timeScale = timeScale;
            m_hasIntermediateColors = false;
            m_intermediateColors = new IntermediateColor[0];
        }
        public ColorCurve(AnimationCurve animCurve, Color min, Color max, float timeScale, IntermediateColor[] intermediateColors)
        {
            m_rawCurve = animCurve;
            m_minValue = min;
            m_maxValue = max;
            m_timeScale = timeScale;
            m_hasIntermediateColors = true;
            m_intermediateColors = intermediateColors;
        }
        public Color Evaluate(float time)
        {
            float t_scaledTime = time * timeScale;
            #region Logs
            //CustomDebug.LogForObject($"Given time {time} scaled to {t_scaledTime}", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            Assert.IsTrue(t_scaledTime >= 0 && t_scaledTime <= 1,
                $"{GetType().Name}'s {nameof(Evaluate)} expected given time " +
                $"({time}) to be in the range 0-1 once scaled with {timeScale}. If nothing seems wrong with the time, it's possible that ({nameof(GetEndTime)}) is returning 0 (actually returning {GetEndTime()}) because either duration/scale = 0 or the curve has not been specified.");
            #endregion Asserts
            float t_rawTime = m_rawCurve.Evaluate(t_scaledTime);
            if (m_hasIntermediateColors)
            {
                IntermediateColor t_lowerColor = GetLowerBoundColor(t_rawTime);
                IntermediateColor t_upperColor = GetUpperBoundColor(t_rawTime);
                float t_timeDiff = t_upperColor.value - t_lowerColor.value;
                float t_smushFactor = 1.0f;
                if (t_timeDiff > 0.0f)
                {
                    t_smushFactor = 1.0f / t_timeDiff;
                }
                float t_correctedRawTime = (t_rawTime - t_lowerColor.value) * t_smushFactor;
                return Color.Lerp(t_lowerColor.color, t_upperColor.color, t_correctedRawTime);
            }
            return Color.Lerp(m_minValue, m_maxValue, t_rawTime);
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


        private IntermediateColor GetLowerBoundColor(float t)
        {
            if (m_intermediateColors.Length <= 0)
            {
                return new IntermediateColor(m_minValue, 0.0f);
            }
            IntermediateColor t_highestLowerBoundColor = new IntermediateColor(m_minValue, 0.0f);
            foreach (IntermediateColor t_intColor in m_intermediateColors)
            {
                if (t >= t_intColor.value &&
                    t_intColor.value > t_highestLowerBoundColor.value)
                {
                    t_highestLowerBoundColor = t_intColor;
                }
            }
            return t_highestLowerBoundColor;
        }
        private IntermediateColor GetUpperBoundColor(float t)
        {
            if (m_intermediateColors.Length <= 0)
            {
                return new IntermediateColor(m_maxValue, 1.0f);
            }
            IntermediateColor t_lowestUpperBoundColor = new IntermediateColor(m_maxValue, 1.0f);
            foreach (IntermediateColor t_intColor in m_intermediateColors)
            {
                if (t <= t_intColor.value && 
                    t_intColor.value < t_lowestUpperBoundColor.value)
                {
                    t_lowestUpperBoundColor = t_intColor;
                }
            }
            return t_lowestUpperBoundColor;
        }

        #region Editor
        private bool TimeIsScale() => m_timing == eTimeChoice.Scale;
        private bool TimeIsDuration() => m_timing == eTimeChoice.Duration;
        #endregion Editor
    }
}
