using System;
using System.Collections;
using UnityEngine;

//Original Authors - Jack Dekko, Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Tip popup
    /// </summary>
    public sealed class TipPopup : PopupDetails
    {
        [SerializeField] private float m_defaultPos = 0;
        [SerializeField] private float m_incrementAmt = 0;

        private RectTransform m_rectTransform = null;
        private bool m_isSlideDownCoroutActive = false;
        private Coroutine m_slideDownCorout = null;

        protected override void Awake()
        {
            base.Awake();
            m_rectTransform = GetComponent<RectTransform>();
            //CustomDebug.AssertComponentIsNotNull(m_rectTransform, this);
        }

        public override void OnDuplicateAdded(int popupCount, int index)
        {
            if (m_isSlideDownCoroutActive)
            {
                StopCoroutine(m_slideDownCorout);
            }
            if (shownState != eShownState.shown)
            {
                Vector2 t_curPosition = m_rectTransform.anchoredPosition;
                t_curPosition.y = CalculateEndPosition(index);
                m_rectTransform.anchoredPosition = t_curPosition;
            } else
            {
                m_slideDownCorout = StartCoroutine(SlideDownCoroutine(popupCount, index));
            }
        }

        private IEnumerator SlideDownCoroutine(int popupCount, int index)
        {
            float t_startValue = m_rectTransform.anchoredPosition.y;
            float t_endValue = CalculateEndPosition(index);
            float t = 0;
            while (t < 1)
            {
                Vector2 t_curPosition = m_rectTransform.anchoredPosition;
                t_curPosition.y = Mathf.Lerp(t_startValue, t_endValue, t);
                m_rectTransform.anchoredPosition = t_curPosition;
                yield return null;
                t += Time.deltaTime;
            }
        }

        private float CalculateEndPosition(int index)
        {
            return m_defaultPos + m_incrementAmt * (index + 1);
        }
    }
}
