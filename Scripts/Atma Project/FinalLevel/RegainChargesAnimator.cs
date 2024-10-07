using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;

using Atma.UI;
using Helpers.Events;
using Helpers.Singletons;
using Helpers.Animation.BetterCurve;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class RegainChargesAnimator : SingletonMonoBehaviour<RegainChargesAnimator>
    {
        public StopwatchChargeManager stopwatchChargeMan => m_stopwatchChargeMan;
        public IEventPrimer onRegainAnimEnd => m_onRegainAnimEnd;

        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))] private string m_playTriggerAnimParamName = "Play";
        [SerializeField, Required] private StopwatchChargeManager m_stopwatchChargeMan = null;
        [SerializeField] private MixedEvent m_onRegainAnimEnd = new MixedEvent();

        [SerializeField, Required, BoxGroup("BG Image")] private Image m_bgImg = null;
        [SerializeField, BoxGroup("BG Image")] private Color m_imgColor = Color.black;
        [SerializeField, BoxGroup("BG Image")] private BetterCurve m_fadeImgCurve = new BetterCurve();
        [SerializeField, BoxGroup("BG Image")] private BetterCurve m_fadeOutCurve = new BetterCurve();

        private UISoundController m_soundCont = null;

        private int m_hideChargesRequestID = -1;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopwatchChargeMan, nameof(m_stopwatchChargeMan), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_soundCont = UISoundController.GetInstanceSafe(this);
        }

        public void PlayRegainAnimation()
        {
            m_soundCont.PlayRegainChargesSound();
            m_animator.SetTrigger(m_playTriggerAnimParamName);
            m_hideChargesRequestID = m_stopwatchChargeMan.RequestActiveChargesBeHidden();

            StartCoroutine(ControlBGImageDuringAnimCorout());
        }


        // Called by an animation event
        private void OnRegainAnimationEnd()
        {
            m_stopwatchChargeMan.CancelRequestForActiveChargesToBeHidden(m_hideChargesRequestID);
            m_hideChargesRequestID = -1;
            m_onRegainAnimEnd.Invoke();
        }

        private IEnumerator ControlBGImageDuringAnimCorout()
        {
            m_bgImg.enabled = true;

            // Fade in
            float t_time = 0.0f;
            float t_endTime = m_fadeImgCurve.GetEndTime();
            while (t_time < t_endTime)
            {
                float t_alphaVal = m_fadeImgCurve.Evaluate(t_time);
                Color t_imgCol = m_imgColor;
                t_imgCol.a = t_alphaVal;
                m_bgImg.color = t_imgCol;

                yield return null;
                t_time += Time.deltaTime;
            }

            // While it's not done with the regain animation yet.
            yield return new WaitUntil(() => m_hideChargesRequestID == -1);

            // Fade out
            t_time = 0.0f;
            t_endTime = m_fadeOutCurve.GetEndTime();
            while (t_time < t_endTime)
            {
                float t_alphaVal = m_fadeOutCurve.Evaluate(t_time);
                Color t_imgCol = m_imgColor;
                t_imgCol.a = t_alphaVal;
                m_bgImg.color = t_imgCol;

                yield return null;
                t_time += Time.deltaTime;
            }

            m_bgImg.enabled = false;
        }
    }
}