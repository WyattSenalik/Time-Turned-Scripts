using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

using NaughtyAttributes;
using TMPro;

using Atma.Dialogue;
using Helpers.Animation;
using Helpers.Animation.BetterCurve;
using Helpers.UI;
using TMPro.Extension;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Death
{
    [DisallowMultipleComponent]
    public sealed class ShowStopwatchPopupScriptedAnimation : ScriptedAnimationCoroutine
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField, Required] private GameObject m_stopwatchUIObj = null;
        [SerializeField] private ImageWithMaxAlpha[] m_imgsToFade = new ImageWithMaxAlpha[0];
        [SerializeField] private TextWithMaxAlpha[] m_textsToFade = new TextWithMaxAlpha[0];
        [SerializeField] private TextWithMaxAlpha m_advanceText = null;

        [SerializeField, Required] private BetterCurveSO m_growthCurve = null;
        [SerializeField, Required] private BetterCurveSO m_fadeCurve = null;
        [SerializeField, Required] private BetterCurveSO m_advanceFadeCurve = null;


        private void OnEnable()
        {
            if (ConversationSkipper.instance.ShouldSkipDialogue())
            {
                speed = 100.0f;
            }
        }


        protected override void StartAnimation()
        {
            m_stopwatchUIObj.SetActive(true);
        }
        protected override float GetEndTime()
        {
            return m_advanceFadeCurve.GetEndTime() + Mathf.Max(m_growthCurve.GetEndTime(), m_fadeCurve.GetEndTime());
        }
        protected override void UpdateAnimation(float timeElapsed)
        {
            UpdateAnimationFromLerpT(timeElapsed);
        }
        protected override void UpdateAnimationFinal()
        {
            UpdateAnimationFromLerpT(GetEndTime());
        }


        private void UpdateAnimationFromLerpT(float timeElapsed)
        {
            float t_growthVal = m_growthCurve.EvaluateClamped(timeElapsed);
            float t_fadeVal = m_fadeCurve.EvaluateClamped(timeElapsed);
            #region Logs
            //CustomDebug.LogForComponent($"TimeElapsed ({timeElapsed}). Growth ({t_growthVal}). Fade (val={t_fadeVal}).", this, IS_DEBUGGING);
            #endregion Logs

            m_stopwatchUIObj.transform.localScale = new Vector3(t_growthVal, t_growthVal, 1.0f);
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

            float t_reducedTimeElapsed = timeElapsed - Mathf.Max(m_growthCurve.GetEndTime(), m_fadeCurve.GetEndTime());
            if (t_reducedTimeElapsed >= 0)
            {
                m_advanceText.text.enabled = true;
                m_advanceText.text.text = eInputAction.AdvanceDialogue.GetSpriteAtlasTextBasedOnCurrentBindings();
                m_advanceText.text.alpha = m_advanceFadeCurve.EvaluateClamped(t_reducedTimeElapsed);
            }
            else
            {
                m_advanceText.text.enabled = false;
            }
        }
    }
}