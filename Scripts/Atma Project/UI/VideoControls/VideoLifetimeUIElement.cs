using System;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Clone lifetime UI element to be used for the video.
    /// Utilizes sliders to make the things.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Slider))]
    public sealed class VideoLifetimeUIElement : MonoBehaviour, ILifetimeUIElement
    {
        private const bool IS_DEBUGGING = false;

        public TimeFrame frame { get; private set; }
        public int chargeIndex { get; private set; }
        public ITimeRewinder timeRewinder { get; private set; }

        private GlobalTimeManager timeMan
        {
            get
            {
                InitTimeManIfNotInit();
                return m_timeMan;
            }
        }
        #region TimeMan
        private void InitTimeManIfNotInit()
        {
            if (m_timeMan == null)
            {
                m_timeMan = GlobalTimeManager.instance;
                #region Asserts
                //CustomDebug.AssertSingletonMonoBehaviourIsNotNull(m_timeMan, this);
                #endregion Asserts
            }
        }
        private GlobalTimeManager m_timeMan = null;
        #endregion TimeMan

        [SerializeField, Required] private Image m_fillImage = null;
        [SerializeField]
        private ChargeSpecification[] m_chargeSpecs = new ChargeSpecification[] {
            new ChargeSpecification(Color.red, new Color(0.5f, 0.0f, 0.0f, 1.0f)),
            new ChargeSpecification(Color.green, new Color(0.0f, 0.5f, 0.0f, 1.0f)),
            new ChargeSpecification(Color.blue, new Color(0.0f, 0.0f, 0.5f, 1.0f))
        };

        private LevelOptions m_levelOptions = null;

        private RectTransform m_rectTrans = null;
        private Slider m_slider = null;


        private void Awake()
        {
            m_slider = GetComponent<Slider>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_slider, this);
            #endregion Asserts
        }
        private void Start()
        {
            InitTimeManIfNotInit();

            timeRewinder.onRewindBegin.ToggleSubscription(OnRewindBegin, true);
        }
        private void OnDestroy()
        {
            timeRewinder?.onRewindBegin.ToggleSubscription(OnRewindBegin, false);
        }
        private void Update()
        {
            // Shouldn't need to be called here as well but there are some cases where OnRewindBegin is not enough, so idk, this fixes it.
            UpdateAnchorsAndPosition();

            // Update the color of the specification to be active or deactive when
            // the clone is active or deactive.
            UpdateColor();
        }


        public void Initialize(TimeFrame frame, int chargeIndex, ITimeRewinder timeRewinder)
        {
            this.frame = frame;
            this.chargeIndex = chargeIndex;
            this.timeRewinder = timeRewinder;

            m_rectTrans = GetComponent<RectTransform>();
            m_levelOptions = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_rectTrans, this);
            //CustomDebug.AssertSingletonIsNotNull(m_levelOptions, this);
            #endregion Asserts

            UpdateAnchorsAndPosition();
            UpdateColor();
        }


        private void UpdateColor()
        {
            #region Asserts
            //CustomDebug.AssertIndexIsInRange(chargeIndex, m_chargeSpecs, this);
            #endregion Asserts
            ChargeSpecification t_specs = m_chargeSpecs[chargeIndex];
            float t_time = timeMan.curTime;
            m_fillImage.color = frame.ContainsTime(t_time) ? t_specs.color : t_specs.deactiveColor;
        }
        private void UpdateAnchorsAndPosition()
        {
            CalculateAnchors(out float t_xMin, out float t_xMax);
            // xMin
            Vector2 t_anchorMin = m_rectTrans.anchorMin;
            t_anchorMin.x = t_xMin;
            m_rectTrans.anchorMin = t_anchorMin;
            // xMax
            Vector2 t_anchorMax = m_rectTrans.anchorMax;
            t_anchorMax.x = t_xMax;
            m_rectTrans.anchorMax = t_anchorMax;
            // Zero out position
            m_rectTrans.anchoredPosition = Vector2.zero;
        }
        private void CalculateAnchors(out float xMin, out float xMax)
        {
            // If no time limit, just base it off the farthest time.
            float t_levelTime;
            if (m_levelOptions.noTimeLimit)
            {
                t_levelTime = timeRewinder.farthestTime;
            }
            else
            {
                t_levelTime = m_levelOptions.time;
            }

            // Complicatedness added w/ earliest time
            float t_fixedLevelTime = t_levelTime - timeRewinder.earliestTime;
            // If fixed time is 0, then just say the clone takes up everything.
            if (t_fixedLevelTime <= 0.0f)
            {
                xMin = 0.0f;
                xMax = 1.0f;
                #region Logs
                //CustomDebug.LogForComponent($"CalculateAnchors: FixedLevelTime <= 0.", this, IS_DEBUGGING);
                #endregion Logs
                return;
            }
            float t_startTime = frame.startTime - timeRewinder.earliestTime;
            t_startTime = Mathf.Max(0.0f, t_startTime);
            float t_endTime = frame.endTime - timeRewinder.earliestTime;
            t_endTime = Mathf.Max(0.0f, t_endTime);
            t_endTime = Mathf.Min(t_endTime, t_fixedLevelTime);

            float t_inverseLevelTime = 1.0f / t_fixedLevelTime;
            xMin = t_startTime * t_inverseLevelTime;
            xMax = t_endTime * t_inverseLevelTime;

            #region Logs
            //CustomDebug.LogForComponent($"CalculateAnchors: {nameof(t_levelTime)}={t_levelTime}. {nameof(timeRewinder.earliestTime)}={timeRewinder.earliestTime}. {nameof(t_fixedLevelTime)}={t_fixedLevelTime}. {nameof(frame.startTime)}={frame.startTime}. {nameof(frame.endTime)}={frame.endTime}. {nameof(t_startTime)}={t_startTime}. {nameof(t_endTime)}={t_endTime}. {nameof(t_inverseLevelTime)}={t_inverseLevelTime}. {nameof(xMin)}={xMin}. {nameof(xMax)}={xMax}.", this, IS_DEBUGGING);
            #endregion Logs
        }


        private void OnRewindBegin()
        {
            if (m_levelOptions.noTimeLimit)
            {
                UpdateAnchorsAndPosition();
            }
        }


        /// <summary>
        /// Visual differences between each 
        /// </summary>
        [Serializable]
        public sealed class ChargeSpecification
        {
            public Color color => m_color;
            public Color deactiveColor => m_deactiveColor;

            [SerializeField] private Color m_color = Color.magenta;
            [SerializeField] private Color m_deactiveColor = new Color(0.5f, 0.0f, 0.5f, 1.0f);

            public ChargeSpecification(Color color, Color deactiveColor)
            {
                m_color = color;
                m_deactiveColor = deactiveColor;
            }
        }
    }
}
