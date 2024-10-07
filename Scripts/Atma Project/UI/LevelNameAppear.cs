using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;
using TMPro;

using Atma.Settings;
using Atma.Translation;
using Helpers.UI;
using Helpers.Animation.BetterCurve;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class LevelNameAppear : MonoBehaviour
    {
        [SerializeField, Required] private TextMeshProUGUI m_textMesh = null;
        [SerializeField] private ImageWithMaxAlpha m_bgImg = null;
        [SerializeField, Required] private StopwatchTimelineAnimationController m_stopwatchAnimCont = null;

        [SerializeField] private bool m_shouldShowInScene = true;

        [SerializeField] private BetterCurve m_fadeInAlphaCurve = new BetterCurve();
        [SerializeField] private BetterCurve m_fadeOutAlphaCurve = new BetterCurve();
        [SerializeField] private BetterCurve m_quickFadeAlphaCurve = new BetterCurve();

        private LevelOptions m_lvlOpt = null;
        private GameSettings m_settings = null;

        private bool m_isFirstOccurrence = true;
        private float m_elapsedCoroutTime = 0.0f;

        private bool m_isFadeInCoroutActive = false;
        private Coroutine m_fadeInCorout = null;
        private bool m_isFadeOutCoroutActive = false;
        private Coroutine m_fadeOutCorout = null;
        private bool m_isQuickFadeCoroutActive = false;
        private Coroutine m_quickFadeCorout = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_textMesh, nameof(m_textMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_bgImg, nameof(m_bgImg), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopwatchAnimCont, nameof(m_stopwatchAnimCont), this);
            #endregion Asserts
        }
        private void Start()
        {
            if (m_shouldShowInScene)
            {
                m_lvlOpt = LevelOptions.GetInstanceSafe();
                m_settings = GameSettings.instance;
                ToggleSubscription(true);
            }
        }
        private void OnDestroy()
        {
            if (m_shouldShowInScene)
            {
                ToggleSubscription(false);
            }
        }


        private void ToggleSubscription(bool cond)
        {
            if (m_stopwatchAnimCont != null)
            {
                m_stopwatchAnimCont.onTransitionAnimFinished.ToggleSubscription(OnTransitionAnimFinished, cond);
                m_stopwatchAnimCont.onTransitionToStopwatchStarted.ToggleSubscription(OnTransitionToStopwatchStarted, cond);
            }
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateText, cond);
            }
        }
        private void OnTransitionAnimFinished()
        {
            if (!m_isFirstOccurrence) { return; }
            m_isFirstOccurrence = false;

            UpdateText();
            StartFadeInTextCoroutine();
        }
        private void OnTransitionToStopwatchStarted()
        {
            if (m_isFadeInCoroutActive)
            {
                StopCoroutine(m_fadeInCorout);
                m_elapsedCoroutTime = 0.0f;
                StartQuickFadeCoroutine();
            }
            else if (m_isFadeOutCoroutActive)
            {
                StopCoroutine(m_fadeOutCorout);
                StartQuickFadeCoroutine();
            }
        }


        private void StartFadeInTextCoroutine()
        {
            if (m_isFadeInCoroutActive) { return; }
            m_isFadeInCoroutActive = true;
            m_elapsedCoroutTime = 0.0f;
            m_textMesh.enabled = true;
            m_bgImg.img.enabled = true;
            m_fadeInCorout = StartCoroutine(GenericFadeCoroutine(m_fadeInAlphaCurve, () =>
            {
                m_isFadeInCoroutActive = false;
                StartFadeOutTextCoroutine();
            }));
        }
        private void StartFadeOutTextCoroutine()
        {
            if (m_isFadeOutCoroutActive) { return; }
            m_isFadeOutCoroutActive = true;
            m_elapsedCoroutTime = 0.0f;
            m_fadeOutCorout = StartCoroutine(GenericFadeCoroutine(m_fadeOutAlphaCurve, () =>
            {
                m_isFadeOutCoroutActive = false;
                m_textMesh.enabled = false;
            }));
        }
        private void StartQuickFadeCoroutine()
        {
            if (m_isQuickFadeCoroutActive) { return; }
            m_isQuickFadeCoroutActive = true;
            float t_curAlphaValue = m_textMesh.color.a;
            m_elapsedCoroutTime = 0.0f;
            while (t_curAlphaValue > m_quickFadeAlphaCurve.Evaluate(m_elapsedCoroutTime))
            {
                m_elapsedCoroutTime += 0.0625f;

                if (m_elapsedCoroutTime >= m_quickFadeAlphaCurve.GetEndTime())
                {
                    break;
                }
            }
            m_quickFadeCorout = StartCoroutine(GenericFadeCoroutine(m_quickFadeAlphaCurve, () =>
            {
                m_isQuickFadeCoroutActive = false;
                m_textMesh.enabled = false;
                m_bgImg.img.enabled = false;
            }));
        }


        private IEnumerator GenericFadeCoroutine(BetterCurve curve, Action onCoroutEnd)
        {
            float t_endTime = curve.GetEndTime();

            float t_alpha;
            Color t_txtCol;
            Color t_imgCol;
            while (m_elapsedCoroutTime < t_endTime)
            {
                t_alpha = curve.Evaluate(m_elapsedCoroutTime);

                t_txtCol = m_textMesh.color;
                t_txtCol.a = t_alpha;
                m_textMesh.color = t_txtCol;

                t_imgCol = m_bgImg.img.color;
                t_imgCol.a = t_alpha * m_bgImg.maxAlpha;
                m_bgImg.img.color = t_imgCol;

                yield return null;
                m_elapsedCoroutTime += Time.deltaTime;
            }
            t_alpha = curve.Evaluate(t_endTime);

            t_txtCol = m_textMesh.color;
            t_txtCol.a = t_alpha;
            m_textMesh.color = t_txtCol;

            t_imgCol = m_bgImg.img.color;
            t_imgCol.a = t_alpha * m_bgImg.maxAlpha;
            m_bgImg.img.color = t_imgCol;

            onCoroutEnd?.Invoke();
        }

        private void UpdateText()
        {
            LevelsList t_lvlList = LevelsList.GetMasterList();
            int t_activeSceneSaveIndex = t_lvlList.GetSaveIndexOfActiveScene();
            string t_lvlFlavorName = t_lvlList.GetFlavorNameForLevelAtSaveIndex(t_activeSceneSaveIndex);
            m_textMesh.text = t_lvlFlavorName;
            m_textMesh.ApplyFontToTextMeshForCurLanguage();
        }
    }
}