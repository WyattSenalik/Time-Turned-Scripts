using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

using NaughtyAttributes;
using Helpers.Math;
using Helpers.Animation.BetterCurve;
using Atma.Dialogue;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class TVController : MonoBehaviour
    {
        private float camMoveTime => ConversationSkipper.instance.ShouldSkipDialogue() ? 0.0f : m_camMoveTime;
        private float moveFadeTransitionWaitTime => ConversationSkipper.instance.ShouldSkipDialogue() ? 0.0f : m_moveFadeTransitionWaitTime;
        private float fadeInTime => ConversationSkipper.instance.ShouldSkipDialogue() ? 0.0f : m_fadeInTime;
        private float fadeIdleTransitionWaitTime => ConversationSkipper.instance.ShouldSkipDialogue() ? 0.0f : m_fadeIdleTransitionWaitTime;

        [SerializeField, Required] private CameraController m_camCont = null;
        [SerializeField, Required] private CameraZoomer m_camZoomer = null;
        [SerializeField, Required] private GameObject m_uiTvElementParentObj = null;
        [SerializeField, Required] private Animator m_uiTvAnimator = null;
        [SerializeField] private GameObject[] m_uiElementsToTurnOff = new GameObject[0];
        [SerializeField] private Image[] m_imgsToFadeIn = new Image[0];
        [SerializeField, AnimatorParam(nameof(m_uiTvAnimator))] private string m_smirkTriggerParamName = "Smirk";
        [SerializeField, AnimatorParam(nameof(m_uiTvAnimator))] private string m_laughTriggerParamName = "Laugh";
        [SerializeField, AnimatorParam(nameof(m_uiTvAnimator))] private string m_frownTriggerParamName = "Frown";
        [SerializeField, AnimatorParam(nameof(m_uiTvAnimator))] private string m_staticTriggerParamName = "Static";
        [SerializeField, Min(0.0f)] private float m_camMoveTime = 1.0f;
        [SerializeField, Min(0.0f)] private float m_moveFadeTransitionWaitTime = 0.5f;
        [SerializeField, Min(0.0f)] private float m_fadeInTime = 1.0f;
        [SerializeField, Min(0.0f)] private float m_fadeIdleTransitionWaitTime = 0.5f;
        [SerializeField, Min(1)] private int m_ppUStartSize = 100;
        [SerializeField, Min(100)] private int m_ppUEndSize = 600;

        [SerializeField] private BetterCurve m_ppuCurve = new BetterCurve();

        private InWorldTVAnimator m_inWorldTVAnim = null;

        private Vector3 m_camStartingPosition = Vector3.zero;
        private bool m_isUiTvShowing = false;
        private bool m_isUiTvTransitionCoroutActive = false;
        private Coroutine m_uiTvTransitionCorout = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_camCont, nameof(m_camCont), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_camZoomer, nameof(m_camZoomer), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_uiTvElementParentObj, nameof(m_uiTvElementParentObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_uiTvAnimator, nameof(m_uiTvAnimator), this);
            #endregion Asserts
        }
        private void Start()
        {
            // Okay if we don't find it if there is no tv stuff in this scene..
            m_inWorldTVAnim = InWorldTVAnimator.instance;
        }


        public void BeginTransitionToUITV()
        {
            if (ConversationSkipper.instance.ShouldSkipDialogue())
            {
                return;
            }
            if (m_isUiTvShowing)
            {
                // Already showing the tv.
                return;
            }

            ToggleUnwantedUIElements(false);

            // Store camera's starting position before moving it so we can get it back.
            m_camStartingPosition = m_camCont.transform.position;

            Vector3 t_endPos = m_inWorldTVAnim.transform.position;
            t_endPos.z = m_camCont.transform.position.z;
            m_camCont.StartMoveCamera(t_endPos, camMoveTime);
            m_camZoomer.TransitionToPPU(m_ppUEndSize, m_ppuCurve.Evaluate, camMoveTime, BeginTransitionToTVCoroutine);
        }
        public void EndUITV()
        {
            if (m_isUiTvTransitionCoroutActive)
            {
                StopCoroutine(m_uiTvTransitionCorout);
            }

            if (!m_isUiTvShowing && ConversationSkipper.GetInstanceSafe(this).ShouldSkipDialogue())
            {
                // No need to play the transition if the TV isn't even showing
                return;
            }

            m_uiTvTransitionCorout = StartCoroutine(TransitionFromUITVCoroutine());
        }
        public void PlaySmirk()
        {
            m_uiTvAnimator.SetTrigger(m_smirkTriggerParamName);
            m_inWorldTVAnim.EndStaticLoop();
        }
        public void PlayFrown()
        {
            m_uiTvAnimator.SetTrigger(m_frownTriggerParamName);
            m_inWorldTVAnim.EndStaticLoop();
        }
        public void PlayLaugh()
        {
            m_uiTvAnimator.SetTrigger(m_laughTriggerParamName);
            m_inWorldTVAnim.EndStaticLoop();
        }
        public void PlayStatic()
        {
            m_uiTvAnimator.SetTrigger(m_staticTriggerParamName);
            m_inWorldTVAnim.StartStaticLoop();
        }


        private void BeginTransitionToTVCoroutine()
        {
            if (m_isUiTvTransitionCoroutActive)
            {
                StopCoroutine(m_uiTvTransitionCorout);
            }
            m_uiTvTransitionCorout = StartCoroutine(TransitionToUITVCoroutine());
        }
        private IEnumerator TransitionToUITVCoroutine()
        {
            m_isUiTvTransitionCoroutActive = true;

            ConversationSkipper t_convoSkipper = ConversationSkipper.GetInstanceSafe(this);

            // Wait a little bit before fading in the images.
            if (!t_convoSkipper.ShouldSkipDialogue())
            {
                yield return new WaitForSeconds(moveFadeTransitionWaitTime);
            }
            m_uiTvElementParentObj.SetActive(true);

            // Fade in the images.
            if (!t_convoSkipper.ShouldSkipDialogue())
            {
                float t_elapsedTime = 0.0f;
                while (t_elapsedTime < fadeInTime)
                {
                    SetAlphaOfFadeUIElements(t_elapsedTime / fadeInTime);
                    yield return null;
                    t_elapsedTime += Time.deltaTime;
                }
            }
            SetAlphaOfFadeUIElements(1.0f);

            // Wait a little bit before swapping to the idle animation.
            if (!t_convoSkipper.ShouldSkipDialogue())
            {
                yield return new WaitForSeconds(fadeIdleTransitionWaitTime);
            }

            PlaySmirk();

            SteamAchievementManager.instance.GrantAchievement( AchievementId.MEET_INEPTUS );

            m_isUiTvShowing = true;

            m_isUiTvTransitionCoroutActive = false;
        }
        private IEnumerator TransitionFromUITVCoroutine()
        {
            m_isUiTvTransitionCoroutActive = true;

            ConversationSkipper t_convoSkipper = ConversationSkipper.GetInstanceSafe(this);

            if (!t_convoSkipper.ShouldSkipDialogue())
            {
                // Play static and wait a little bit before fading out.
                PlayStatic();
                yield return new WaitForSeconds(fadeIdleTransitionWaitTime);

                // Fade out the images
                float t_elapsedTime = 0.0f;
                while (t_elapsedTime < fadeInTime)
                {
                    SetAlphaOfFadeUIElements(1.0f - t_elapsedTime / fadeInTime);
                    yield return null;
                    t_elapsedTime += Time.deltaTime;
                }
            }
            SetAlphaOfFadeUIElements(0.0f);
            m_uiTvElementParentObj.SetActive(false);

            if (!t_convoSkipper.ShouldSkipDialogue())
            {
                // Wait a little bit before moving after fading out the images.
                yield return new WaitForSeconds(moveFadeTransitionWaitTime);
            }

            // Start moving the camera back to its starting position
            m_camCont.StartMoveCamera(m_camStartingPosition, camMoveTime);
            // As the camera is moving zoom out the camera until we are back at the starting zoom amount.
            
            PixelPerfectCamera t_ppCam = m_camCont.pixelPerfectCam;
            int t_endPPU = m_camZoomer.roomDefaultPPU;
            m_camZoomer.TransitionToPPU(t_endPPU, m_ppuCurve.Evaluate, camMoveTime);
            if (!t_convoSkipper.ShouldSkipDialogue())
            {
                yield return new WaitForSeconds(camMoveTime);
            }
            m_camCont.UpdateCameraToPixelPerfectPosition();

            // Retoggle on the ui elements we toggled off
            ToggleUnwantedUIElements(true);

            m_isUiTvShowing = false;

            m_isUiTvTransitionCoroutActive = false;
        }
        private void ToggleUnwantedUIElements(bool cond)
        {
            foreach (GameObject t_uiEleGO in m_uiElementsToTurnOff)
            {
                t_uiEleGO.SetActive(cond);
            }
        }
        private void SetAlphaOfFadeUIElements(float alpha)
        {
            foreach (Image t_img in m_imgsToFadeIn)
            {
                Color t_col = t_img.color;
                t_col.a = alpha;
                t_img.color = t_col;
            }
        }
    }
}