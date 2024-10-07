using System;
using UnityEngine;

using NaughtyAttributes;
using System.Collections;
using Helpers.Events;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Dialogue
{
    /// <summary>
    /// Controller for showing and hiding the dialogue box.
    /// Assumes the speed of the show/hide animations are both 1 second.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class DialogueBoxDisplay : MonoBehaviour
    {
        public bool isShown => m_isShown;
        public IEventPrimer onShowAnimFin => m_onShowAnimFin;
        public IEventPrimer onHide => m_onHide;

        public float waitTime => m_waitTime;
        public bool shouldSkip
        {
            get => m_shouldSkip;
            set
            {
                if (m_shouldSkip == value) { return; }
                m_shouldSkip = value;
                m_animator.SetBool(m_shouldSkipBoolVarName, m_shouldSkip);
            }
        }

        [SerializeField, Required] private TypeWriter m_typeWriter = null;
        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(Animator))] private string m_showAnimBoolVarName = "Show";
        [SerializeField, AnimatorParam(nameof(Animator))] private string m_isAltAnimBoolVarName = "IsAlt";
        [SerializeField, AnimatorParam(nameof(Animator))] private string m_shouldSkipBoolVarName = "Skip";

        [SerializeField, Min(0.0f)] private float m_waitTime = 1.0f;
        [SerializeField] private MixedEvent m_onShowAnimFin = new MixedEvent();
        [SerializeField] private MixedEvent m_onHide = new MixedEvent();

        [SerializeField, BoxGroup("Sound"), Required] private UIntReference m_slideInWwiseEventID = null;
        [SerializeField, BoxGroup("Sound"), Min(0.0f)] private float m_delayBeforePlayingSlideInSound = 0.2f;
        [SerializeField, BoxGroup("Sound"), Required] private UIntReference m_slideOutWwiseEventID = null;
        [SerializeField, BoxGroup("Sound"), Min(0.0f)] private float m_delayBeforePlayingSlideOutSound = 0.2f;

        [SerializeField, BoxGroup("Sound"), Required] private UIntReference m_dialougeSkippedWwiseEventID = null;

        private bool m_isShown = false;
        private bool m_shouldSkip = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_typeWriter, nameof(m_typeWriter), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slideInWwiseEventID, nameof(m_slideInWwiseEventID), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slideOutWwiseEventID, nameof(m_slideOutWwiseEventID), this);
            #endregion Asserts
        }


        public void Show(Action onFinished=null)
        {
            if (m_isShown)
            {
                onFinished?.Invoke();
                return;
            }

            if (shouldSkip)
            {
                m_animator.SetBool(m_showAnimBoolVarName, true);
                // setting isShown before onFinished here because there was a skipping bug where it would soft lock because it expected to be shown by the time the next action was a thing.
                m_isShown = true;
                onFinished?.Invoke();
                m_onShowAnimFin.Invoke();

                AkSoundEngine.PostEvent(m_dialougeSkippedWwiseEventID.value, gameObject);
                return;
            }

            // Play Slide In Sound
            InvokeActionAfterSeconds(() =>
            {
                AkSoundEngine.PostEvent(m_slideInWwiseEventID.value, gameObject);
            }, m_delayBeforePlayingSlideInSound);
            

            m_animator.speed =  1.0f / waitTime;
            m_animator.SetBool(m_showAnimBoolVarName, true);
            InvokeActionAfterSeconds(() =>
            {
                onFinished?.Invoke();
                m_isShown = true;
                m_onShowAnimFin.Invoke();
            }, waitTime);
        }
        public void Hide(Action onFinished=null)
        {
            if (!m_isShown)
            {
                onFinished?.Invoke();
                return;
            }

            m_typeWriter.ClearText();
            m_onHide.Invoke();

            if (shouldSkip)
            {
                m_animator.SetBool(m_showAnimBoolVarName, false);
                // setting isShown before onFinished here because there was a skipping bug where it would soft lock because it expected to be shown by the time the next action was a thing.
                m_isShown = false;
                onFinished?.Invoke();

                AkSoundEngine.PostEvent(m_dialougeSkippedWwiseEventID.value, gameObject);
                return;
            }

            // Play Slide Out Sound
            InvokeActionAfterSeconds(() =>
            {
                AkSoundEngine.PostEvent(m_slideOutWwiseEventID.value, gameObject);
            }, m_delayBeforePlayingSlideOutSound);

            m_animator.speed = 1.0f / waitTime;
            m_animator.SetBool(m_showAnimBoolVarName, false);
            InvokeActionAfterSeconds(() =>
            {
                onFinished?.Invoke();
                m_isShown = false;
            }, waitTime);
        }
        public void SetIsAlt(bool isAlt)
        {
            m_animator.SetBool(m_isAltAnimBoolVarName, isAlt);
        }


        private void InvokeActionAfterSeconds(Action action, float seconds)
        {
            StartCoroutine(InvokeActionAfterSecondsCoroutine(action, seconds));
        }
        private IEnumerator InvokeActionAfterSecondsCoroutine(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}