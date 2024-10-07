using System;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
using TMPro;

using Helpers.Animation;
using Helpers.Animation.BetterCurve;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Death
{
    public sealed class HideStopwatchPopupScriptedAnimation : ScriptedAnimationCoroutine
    {
        public GameObject stopwatchTimeManipUIObj => m_stopwatchTimeManipUIObj;

        [SerializeField, Required] private GameObject m_stopwatchGetUIObj = null;
        [SerializeField, Required] private GameObject m_stopwatchTimeManipUIObj = null;
        [SerializeField] private ImageWithMaxAlpha[] m_imgsToFade = new ImageWithMaxAlpha[0];
        [SerializeField] private TextWithMaxAlpha[] m_textsToFade = new TextWithMaxAlpha[0];
        [SerializeField] private TextWithMaxAlpha m_advanceText = null;
        [SerializeField] private BetterCurve m_fadeCurve = new BetterCurve(0, 1);


        private void OnEnable()
        {
            if (ConversationSkipper.instance.ShouldSkipDialogue())
            {
                speed = 100.0f;
            }
        }


        protected override void StartAnimation() { }
        protected override float GetEndTime()
        {
            return m_fadeCurve.GetEndTime();
        }
        protected override void UpdateAnimation(float timeElapsed)
        {
            UpdateAnimationFromLerpT(timeElapsed);
        }
        protected override void UpdateAnimationFinal()
        {
            UpdateAnimationFromLerpT(GetEndTime());

            m_stopwatchGetUIObj.SetActive(false);
            m_stopwatchTimeManipUIObj.SetActive(true);
        }


        private void UpdateAnimationFromLerpT(float timeElapsed)
        {
            float t_fadeVal = m_fadeCurve.Evaluate(timeElapsed);

            foreach (ImageWithMaxAlpha t_imgWAlpha in m_imgsToFade)
            {
                Image t_img = t_imgWAlpha.img;
                Color t_col = t_img.color;
                t_col.a = t_fadeVal * t_imgWAlpha.maxAlpha;
                t_img.color = t_col;
            }
            foreach (TextWithMaxAlpha t_textWAlpha in m_textsToFade)
            {
                TextMeshProUGUI t_text = t_textWAlpha.text;
                Color t_col = t_text.color;
                t_col.a = t_fadeVal * t_textWAlpha.maxAlpha;
                t_text.color = t_col;
            }

            if (t_fadeVal <= 0.0f)
            {
                m_advanceText.text.enabled = false;
            }
            else
            {
                m_advanceText.text.enabled = true;
                m_advanceText.text.text = eInputAction.AdvanceDialogue.GetSpriteAtlasTextBasedOnCurrentBindings();
                m_advanceText.text.alpha = t_fadeVal;
            }
        }


        [Serializable]
        public sealed class ImageWithMaxAlpha
        {
            public Image img => m_img;
            public float maxAlpha => m_maxAlpha;

            [SerializeField] private Image m_img = null;
            [SerializeField, Range(0.0f, 1.0f)] private float m_maxAlpha = 1.0f;
        }
        [Serializable]
        public sealed class TextWithMaxAlpha
        {
            public TextMeshProUGUI text => m_text;
            public float maxAlpha => m_maxAlpha;

            [SerializeField] private TextMeshProUGUI m_text = null;
            [SerializeField, Range(0.0f, 1.0f)] private float m_maxAlpha = 1.0f;
        }
    }
}
