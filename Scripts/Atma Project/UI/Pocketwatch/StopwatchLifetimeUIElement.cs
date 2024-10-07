using System;
using UnityEngine;
using UnityEngine.UI;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Single Lifetime UI Element to be spawned by the <see cref="LifetimeManager"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public sealed class StopwatchLifetimeUIElement : TimedRecorder, ILifetimeUIElement
    {
        private const bool IS_DEBUGGING = false;

        public TimeFrame frame { get; private set; }
        public int chargeIndex { get; private set; }
        public ITimeRewinder timeRewinder { get; private set; }

        [SerializeField]
        private ChargeSpecification[] m_chargeSpecs = new ChargeSpecification[] { 
            new ChargeSpecification(Color.red, new Color(0.5f, 0.0f, 0.0f, 1.0f), 75.0f),
            new ChargeSpecification(Color.green, new Color(0.0f, 0.5f, 0.0f, 1.0f), 125.0f),
            new ChargeSpecification(Color.blue, new Color(0.0f, 0.0f, 0.5f, 1.0f), 175.0f)
        };

        private LevelOptions m_levelOptions = null;

        private Image m_image = null;


        public void Initialize(TimeFrame frame, int chargeIndex, ITimeRewinder timeRewinder)
        {
            this.frame = frame;
            this.chargeIndex = chargeIndex;
            this.timeRewinder = timeRewinder;

            m_levelOptions = LevelOptions.instance;
            m_image = GetComponent<Image>();
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_levelOptions, this);
            //CustomDebug.AssertComponentIsNotNull(m_image, this);
            //CustomDebug.AssertIndexIsInRange(chargeIndex, m_chargeSpecs, this);
            #endregion Asserts
            ChargeSpecification t_chargeSpec = m_chargeSpecs[chargeIndex];
            m_image.color = t_chargeSpec.color;
            m_image.rectTransform.sizeDelta = t_chargeSpec.sizeV2;

            // Set the fill amount and where to start filling from (rotation)
            UpdateFillAmountAndRotationOffset();
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (!enabled) { return; }

            // If there is no time limit, gotta update the clone's fill
            if (m_levelOptions.noTimeLimit)
            {
                UpdateFillAmountAndRotationOffset();
            }

            // Update the color of the specification to be active or deactive when
            // the clone is active or deactive.
            UpdateColor();
        }


        private void UpdateColor()
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(chargeIndex, m_chargeSpecs, this);
            #endregion Asserts
            ChargeSpecification t_specs = m_chargeSpecs[chargeIndex];
            m_image.color = frame.ContainsTime(curTime) ? t_specs.color : t_specs.deactiveColor;
        }
        private void UpdateFillAmountAndRotationOffset()
        {
            CalculateFillAmountAndRotationalOffset(frame, out float t_fillAm, out Quaternion t_imgRot);
            m_image.fillAmount = t_fillAm;
            transform.rotation = t_imgRot;
        }
        private void CalculateFillAmountAndRotationalOffset(TimeFrame frame, out float fillAmount, out Quaternion imageRotation)
        {
            float t_levelTime;
            if (m_levelOptions.noTimeLimit)
            {
                t_levelTime = furthestTime;
            }
            else
            {
                t_levelTime = m_levelOptions.time;
            }
            // What should the rotation of the fill element be (where should it's fill begin).
            // Fill Origin is assumed to be at the top.
            // With testing, this is the table created for desired results:
            // startTime (as % of total time)	|	rotation offset (euler angles)
            // 0%                               |   0
            // 25%                              |   -90
            // 50%                              |   -180
            // 75%                              |   -270

            // Added complicatedness because of earliest time.
            float t_fixedLevelTime = t_levelTime - timeRewinder.earliestTime;
            float t_startTime = frame.startTime - timeRewinder.earliestTime;
            t_startTime = Mathf.Max(0.0f, t_startTime);
            float t_endTime = frame.endTime - timeRewinder.earliestTime;
            t_endTime = Mathf.Max(0.0f, t_endTime);

            // With earliest time being taken into account, the fixed level time can be 0. When that is true, just set the start percent to 0 and the end percent to 1.
            float t_startTimePercent;
            float t_endTimePercent;
            if (t_fixedLevelTime > 0.0)
            {
                t_startTimePercent = t_startTime / t_fixedLevelTime;
                t_endTimePercent = t_endTime / t_fixedLevelTime;
            }
            else
            {
                t_startTimePercent = 0.0f;
                t_endTimePercent = 1.0f;
            }

            float t_rotOffset = t_startTimePercent * -360.0f;
            float t_durationPercent = t_endTimePercent - t_startTimePercent;

            #region Logs
            //CustomDebug.LogForComponent($"CalculateFillAmountAndRotationalOffset. {nameof(furthestTime)}={furthestTime}. {nameof(m_levelOptions.time)}={m_levelOptions.time}. {nameof(timeRewinder.earliestTime)}={timeRewinder.earliestTime}. {nameof(t_fixedLevelTime)}={t_fixedLevelTime}. {nameof(frame.startTime)}={frame.startTime}. {nameof(frame.endTime)}={frame.endTime}. {nameof(t_startTime)}={t_startTime}. {nameof(t_endTime)}={t_endTime}. {nameof(t_startTimePercent)}={t_startTimePercent}. {nameof(t_endTimePercent)}={t_endTimePercent}. {nameof(t_rotOffset)}={t_rotOffset}. {nameof(t_durationPercent)}={t_durationPercent}.", this, IS_DEBUGGING);
            #endregion Logs

            fillAmount = t_durationPercent;
            imageRotation = Quaternion.Euler(0.0f, 0.0f, t_rotOffset);
        }


        /// <summary>
        /// Visual differences between each 
        /// </summary>
        [Serializable]
        public sealed class ChargeSpecification
        {
            public Color color => m_color;
            public Color deactiveColor => m_deactiveColor;
            public float size => m_size;
            public Vector2 sizeV2 => new Vector2(m_size, m_size);

            [SerializeField] private Color m_color = Color.magenta;
            [SerializeField] private Color m_deactiveColor = new Color(0.5f, 0.0f, 0.5f, 1.0f);
            [SerializeField] private float m_size = 75.0f;

            public ChargeSpecification(Color color, Color deactiveColor, float size)
            {
                m_color = color;
                m_deactiveColor = deactiveColor;
                m_size = size;
            }
        }
    }
}
