using System;
using System.Collections;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Jack Dekko, Wyatt Senalik

namespace Atma.UI
{
    /// <summary>
    /// Specific details of the popup to show
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class PopupDetails : MonoBehaviour
    {
        private const string IS_SHOWN_BOOL_PARAM_NAME = "isShown";

        public eShownState shownState { get; private set; } = eShownState.showing;
        public string uniqueID => m_uniqueID;

        [SerializeField] private string m_uniqueID = Guid.NewGuid().ToString();
        [SerializeField, Min(0.0f)] private float m_showAnimDuration = 1.0f;
        [SerializeField, Min(0.0f)] private float m_hideAnimDuration = 1.0f;

        private Animator m_animator = null;

        private bool m_isFinishShowingCoroutActive = false;
        private Coroutine m_finishShowingCorout = null;
        private bool m_isFinishHidingCoroutActive = false;
        private Coroutine m_finishHidingCorout = null;


        protected virtual void Awake()
        {
            m_animator = GetComponent<Animator>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_animator, this);
            #endregion
        }


        // Called when another popup shows
        public abstract void OnDuplicateAdded(int popupCount, int index);

        public void Show()
        {
            if (shownState == eShownState.shown) { return; }

            m_animator.SetBool(IS_SHOWN_BOOL_PARAM_NAME, true);
            shownState = eShownState.showing;

            FinishShowingAfterAnimFinished();
        }
        public void Hide()
        {
            if (shownState == eShownState.hidden) { return; }

            m_animator.SetBool(IS_SHOWN_BOOL_PARAM_NAME, false);
            shownState = eShownState.hiding;

            FinishingHidingAfterAnimFinished();
        }


        private void FinishShowingAfterAnimFinished()
        {
            if (m_isFinishShowingCoroutActive)
            {
                StopCoroutine(m_finishShowingCorout);
            }
            m_finishShowingCorout = StartCoroutine(FinishShowingCoroutine());
        }
        private IEnumerator FinishShowingCoroutine()
        {
            m_isFinishShowingCoroutActive = true;

            yield return new WaitForSeconds(m_showAnimDuration);
            if (shownState == eShownState.showing)
            {
                shownState = eShownState.shown;
            }

            m_isFinishShowingCoroutActive = false;
        }
        private void FinishingHidingAfterAnimFinished()
        {
            if (m_isFinishHidingCoroutActive)
            {
                StopCoroutine(m_finishHidingCorout);
            }
            m_finishHidingCorout = StartCoroutine(FinishHidingCoroutine());
        }
        private IEnumerator FinishHidingCoroutine()
        {
            m_isFinishHidingCoroutActive = true;

            yield return new WaitForSeconds(m_hideAnimDuration);
            if (shownState == eShownState.hiding)
            {
                shownState = eShownState.hidden;
            }

            m_isFinishHidingCoroutActive = false;
        }


        public enum eShownState { showing, shown, hiding, hidden}


#if UNITY_EDITOR
        [Button] private void RandomizeUniqueID()
        {
            m_uniqueID = Guid.NewGuid().ToString();
        }
#endif
    }
}

