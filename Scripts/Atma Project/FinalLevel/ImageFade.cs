using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Helpers.Animation.BetterCurve;
// Original Authors - Bryce Cernohous-Schrader
// Tweaked by Wyatt Senalik

namespace Atma
{
    public sealed class ImageFade : MonoBehaviour
    {
        [SerializeField] private Image m_imgToFadeIn = null;
        [SerializeField] private BetterCurve m_alphaCurve = new BetterCurve();


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_imgToFadeIn, nameof(m_imgToFadeIn), this);
            #endregion Asserts
        }


        public void StartFadeIn()
        {
            StartCoroutine(FadeImageCorout());
        }

        private IEnumerator FadeImageCorout()
        {
            m_imgToFadeIn.enabled = true;

            float t_elapsedTime = 0.0f;
            float t_endTime = m_alphaCurve.GetEndTime();
            Color t_col = m_imgToFadeIn.color;
            while (t_elapsedTime < t_endTime)
            {
                t_col.a = m_alphaCurve.Evaluate(t_elapsedTime);
                m_imgToFadeIn.color = t_col;

                yield return null;
                t_elapsedTime += Time.deltaTime;
            }

            t_col.a = m_alphaCurve.Evaluate(t_endTime);
            m_imgToFadeIn.color = t_col;
        }
    }
}