using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
using TMPro;
using Atma.UI;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class FailResumePopup : MonoBehaviour
    {
        [SerializeField, Required]
        private Image m_containerImg = null;
        [SerializeField, Required]
        private TextMeshProUGUI m_textMesh = null;

        [SerializeField, Min(0)] private float m_liveTime = 7;
        [SerializeField, Min(0)] private float m_fadeTime = 1;
        [SerializeField] private string m_defaultFailText = "You can't resume now!";
        [SerializeField, BoxGroup("Text Options")]
        private string m_deadText = "You'll die if you resume now!";
        [SerializeField, BoxGroup("Text Options")]
        private string m_cloneDeadText = "A clone will die if you resume now!";
        [SerializeField, BoxGroup("Text Options")]
        private string m_outOfTimeText = "You're out of time!";
        [SerializeField, BoxGroup("Text Options")]
        private string m_outOfCloneChargesText = "No clone charges are available!";
        [SerializeField, BoxGroup("Text Options")]
        private string m_ineptusDiedText = "You need to take Ineptus alive!";

        private BranchPlayerController m_playerCont = null;
        private ITimeManipController m_timeManipCont = null;

        private bool m_isFadeCoroutActive = false;
        private Coroutine m_fadeCorout = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_containerImg, nameof(m_containerImg), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_textMesh, nameof(m_textMesh), this);
            #endregion Asserts
        }
        private void Start()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_playerSingleton, this);
            #endregion Asserts
            m_playerCont = t_playerSingleton.GetComponent<BranchPlayerController>();
            m_timeManipCont = t_playerSingleton.GetComponent<ITimeManipController>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(m_playerCont, t_playerSingleton.gameObject, this);
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_timeManipCont, t_playerSingleton.gameObject, this);
            #endregion Asserts

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            m_playerCont?.onFailedToEndTimeManip.ToggleSubscription(ShowPopup, cond);
            m_playerCont?.onFailedToCreateClone.ToggleSubscription(ShowPopup, cond);
            m_playerCont?.onBeginRewinding.ToggleSubscription(BeginFadePopup, cond);
            m_playerCont?.onEndTimeManip.ToggleSubscription(InstantlyHidePopup, cond);

            m_timeManipCont?.onSkipToBegin.ToggleSubscription(BeginFadePopup, cond);
        }

        /// <summary>
        /// Shows the resume failed popup
        /// </summary>
        private void ShowPopup(FailResumeContext context)
        {
            // Set text
            switch (context.reason)
            {
                case FailResumeContext.eFailResumeReason.Dead:
                {
                    m_textMesh.text = m_deadText;
                    break;
                }
                case FailResumeContext.eFailResumeReason.CloneDead:
                {
                    m_textMesh.text = m_cloneDeadText;
                    break;
                }
                case FailResumeContext.eFailResumeReason.OutOfTime:
                {
                    m_textMesh.text = m_outOfTimeText;
                    break;
                }
                case FailResumeContext.eFailResumeReason.OutOfCloneCharges:
                {
                    m_textMesh.text = m_outOfCloneChargesText;
                    break;
                }
                case FailResumeContext.eFailResumeReason.IneptusDead:
                {
                    m_textMesh.text = m_ineptusDiedText;
                    break;
                }
                default:
                {
                    m_textMesh.text = m_defaultFailText;
                    CustomDebug.UnhandledEnum(context.reason, this);
                    break;
                }
            }
            // Show image and text
            m_containerImg.enabled = true;
            m_textMesh.enabled = true;

            // Should either start fading away after an amount of time or if player starts rewinding.
            CancelInvoke(nameof(BeginFadePopup));
            Invoke(nameof(BeginFadePopup), m_liveTime);
            // Should hide instantly if player resumes.
        }
        private void BeginFadePopup()
        {
            // Cancel any invoking to trigger this.
            CancelInvoke(nameof(BeginFadePopup));

            // If no even enabled, don't fade
            if (!m_containerImg.enabled) { return; }
            // If already fading, don't restart
            if (m_isFadeCoroutActive) { return; }

            m_fadeCorout = StartCoroutine(FadeCoroutine());
        }
        private void InstantlyHidePopup()
        {
            // Cancel any invoking to a fade
            CancelInvoke(nameof(BeginFadePopup));

            // If no even enabled, don't fade
            if (!m_containerImg.enabled) { return; }
            // If currently fading, stop it early.
            if (m_isFadeCoroutActive)
            {
                StopCoroutine(FadeCoroutine());
                m_fadeCorout = null;
                m_isFadeCoroutActive = false;
            }

            FinishFading();
        }

        private IEnumerator FadeCoroutine()
        {
            m_isFadeCoroutActive = true;

            float t_incrementAm = 1.0f / m_fadeTime;
            float t = 0.0f;
            while (t < m_fadeTime)
            {
                float t_newAlpha = Mathf.Lerp(1.0f, 0.0f, t);
                Color t_imgCol = m_containerImg.color;
                Color t_textCol = m_textMesh.color;
                t_imgCol.a = t_newAlpha;
                t_textCol.a = t_newAlpha;
                m_containerImg.color = t_imgCol;
                m_textMesh.color = t_textCol;
                yield return null;

                t += Time.deltaTime * t_incrementAm;
            }
            FinishFading();

            m_isFadeCoroutActive = false;
        }
        private void FinishFading()
        {
            m_containerImg.enabled = false;
            m_textMesh.enabled = false;

            Color t_imgCol = m_containerImg.color;
            Color t_textCol = m_textMesh.color;
            t_imgCol.a = 1.0f;
            t_textCol.a = 1.0f;
            m_containerImg.color = t_imgCol;
            m_textMesh.color = t_textCol;
        }
    }
}
