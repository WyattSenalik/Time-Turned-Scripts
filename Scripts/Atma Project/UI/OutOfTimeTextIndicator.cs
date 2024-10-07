using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Timed;
using Helpers.Animation.BetterCurve;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Starts flashing the text colors when only x amount of time is left.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class OutOfTimeTextIndicator : MonoBehaviour
    {
        public GlobalTimeManager timeMan { get; private set; } = null;
        public LevelOptions levelOpt { get; private set; } = null;
        public TextMeshProUGUI textMesh { get; private set; } = null;
        public Color baseColor { get; private set; } = Color.black;
        public float baseSize { get; private set; } = float.NaN;
        public float timeToBeginFlash => m_timeToBeginFlash;
        public Color flashColor => m_flashColor;
        public float minFlashSpeed => m_flashSpeedRange.x;
        public float maxFlashSpeed => m_flashSpeedRange.y;
        public BetterCurve colorLerpCurve => m_colorLerpCurve;
        public float timeToBeginSizeChange => m_timeToBeginSizeChange;
        public float endSize => m_endSize;
        public float minSizeChangeSpeed => m_sizeChangeSpeed.x;
        public float maxSizeChangeSpeed => m_sizeChangeSpeed.y;
        public BetterCurve sizeLerpCurve => m_sizeLerpCurve;

        [SerializeField, Min(0.0f), BoxGroup("Flash")] 
        private float m_timeToBeginFlash = 3.0f; 
        [SerializeField, BoxGroup("Flash")] 
        private Color m_flashColor = new Color(171.0f / 255.0f, 34.0f / 255.0f, 34 / 255.0f);
        [SerializeField, MinMaxSlider(0.0f, 50.0f), BoxGroup("Flash")]
        private Vector2 m_flashSpeedRange = new Vector2(1.0f, 20.0f);
        [SerializeField, BoxGroup("Flash")] 
        private BetterCurve m_colorLerpCurve = new BetterCurve();

        [SerializeField, Min(0.0f), BoxGroup("SizeChange")]
        private float m_timeToBeginSizeChange = 2.0f;
        [SerializeField, Min(0.0f), BoxGroup("SizeChange")]
        private float m_endSize = 24;
        [SerializeField, MinMaxSlider(0.0f, 50.0f), BoxGroup("SizeChange")]
        private Vector2 m_sizeChangeSpeed = new Vector2(1.0f, 2.0f);
        [SerializeField, BoxGroup("SizeChange")]
        private BetterCurve m_sizeLerpCurve = new BetterCurve();


        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(textMesh, this);
            #endregion Asserts
        }
        private void Start()
        {
            timeMan = GlobalTimeManager.instance;
            levelOpt = LevelOptions.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(timeMan, this);
            //CustomDebug.AssertSingletonIsNotNull(levelOpt, this);
            #endregion Asserts
            baseColor = textMesh.color;
            baseSize = textMesh.fontSize;
        }
        private void Update()
        {
            float t_countdownTime = GetCountdownTime();
            // Flash
            if (t_countdownTime <= timeToBeginFlash)
            {
                float t_elapsedTime = timeToBeginFlash - t_countdownTime;

                float t_flashSpeed = Mathf.Lerp(minFlashSpeed, maxFlashSpeed, t_elapsedTime /  timeToBeginFlash);

                float t_spedTime = t_elapsedTime * t_flashSpeed;
                float t_wrappedTime = t_spedTime % colorLerpCurve.GetEndTime();

                float t_lerpVal = colorLerpCurve.Evaluate(t_wrappedTime);
                Color t_curTextColor = Color.Lerp(baseColor, flashColor, t_lerpVal);

                textMesh.color = t_curTextColor;
            }
            else
            {
                textMesh.color = baseColor;
            }

            // Size Change
            if (t_countdownTime <= timeToBeginSizeChange)
            {
                float t_elapsedTime = timeToBeginSizeChange - t_countdownTime;

                float t_sizeChangeSpeed = Mathf.Lerp(minSizeChangeSpeed, maxSizeChangeSpeed, t_elapsedTime / timeToBeginSizeChange);

                float t_spedTime = t_elapsedTime * t_sizeChangeSpeed;
                float t_wrappedTime = t_spedTime % sizeLerpCurve.GetEndTime();

                float t_lerpVal = sizeLerpCurve.Evaluate(t_wrappedTime);
                float t_curTextSize = Mathf.Lerp(baseSize, endSize, t_lerpVal);

                textMesh.fontSize = t_curTextSize;
            }
            else
            {
                textMesh.fontSize = baseSize;
            }
        }


        private float GetCountdownTime()
        {
            return Mathf.Max(levelOpt.time - timeMan.curTime, 0.0f);
        }
    }
}