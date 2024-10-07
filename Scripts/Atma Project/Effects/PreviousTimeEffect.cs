using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Experimental.Rendering.Universal;

using NaughtyAttributes;

using Helpers;
using Helpers.Extensions;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class PreviousTimeEffect : SingletonMonoBehaviour<PreviousTimeEffect>
    {
        public bool isEffectActive { get; private set; } = false;
        public int amUpdatesToDisplay => m_amUpdatesToDisplay;
        public float secondsBeforeTextureUpdate => m_secondsBeforeTextureUpdate;

        [SerializeField, Required] private Camera m_mainCam = null;
        [SerializeField, Required] private Camera m_cam = null;
        [SerializeField, Tooltip("Must have length equal to the amUpdatesToDisplay")] private RawImage[] m_displayImgs = null;
        [SerializeField, Min(0)] private int m_amUpdatesToDisplay = 2;
        [SerializeField] private float m_secondsBeforeTextureUpdate = 0.25f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_startingAlpha = 0.25f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_alphaReductionMultiplier = 0.5f;

        private CameraController m_mainCamCont = null;
        private CameraZoomer m_mainCamZoomer = null;
        private PixelPerfectCamera m_mainCamPPC = null;
        private BranchPlayerController m_playerCont = null;
        private PixelPerfectCamera m_myCamPPC = null;

        private RenderTexture m_curTexture = null;
        private RenderTexture[] m_prevTextures = null;
        private float m_mostRecentTextureUpdateTime = 0.0f;

        private readonly IDLibrary m_hideRequestsLibrary = new IDLibrary();


        protected override void Awake()
        {
            base.Awake();

            m_curTexture = new RenderTexture(1920, 1080, 0);
            m_prevTextures = new RenderTexture[m_amUpdatesToDisplay];
            for (int i = 0; i < m_amUpdatesToDisplay; ++i)
            {
                m_prevTextures[i] = new RenderTexture(1920, 1080, 0);
            }
            m_cam.targetTexture = m_curTexture;

            m_cam.transform.parent = m_mainCam.transform;
            m_cam.transform.localPosition = Vector3.zero;

            m_mainCamCont = m_mainCam.GetComponentSafe<CameraController>(this);
            m_mainCamZoomer = m_mainCam.GetComponentSafe<CameraZoomer>(this);
            m_mainCamPPC = m_mainCam.GetComponentSafe<PixelPerfectCamera>(this);

            m_myCamPPC = m_cam.GetComponentSafe<PixelPerfectCamera>(this);
            m_myCamPPC.assetsPPU = m_mainCamPPC.assetsPPU;

            InitializeDisplayImages();
            ToggleDisplayImages(false);
        }
        private void Start()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_playerCont = t_playerSingleton.GetComponentSafe<BranchPlayerController>();
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            if (!isEffectActive) { return; }
            m_myCamPPC.assetsPPU = m_mainCamPPC.assetsPPU;

            if (Time.time - m_mostRecentTextureUpdateTime >= m_secondsBeforeTextureUpdate)
            {
                m_mostRecentTextureUpdateTime = Time.time;

                // Update the textures.
                RenderTexture t_oldestTexture = m_prevTextures[^1];
                for (int i = 0; i < m_amUpdatesToDisplay - 1; ++i)
                {
                    m_prevTextures[i + 1] = m_prevTextures[i];
                }
                m_prevTextures[0] = m_curTexture;
                m_curTexture = t_oldestTexture;
                m_cam.targetTexture = m_curTexture;

                // Apply the textures.
                for (int i = 0; i < m_amUpdatesToDisplay; ++i)
                {
                    m_displayImgs[i].texture = m_prevTextures[i];
                    // Also update the size
                    if (m_mainCamZoomer.isZoomCoroutActive)
                    {
                        m_displayImgs[i].rectTransform.localScale = m_mainCamZoomer.curGameRenderImgLocalScale;
                    }
                    else
                    {
                        m_displayImgs[i].rectTransform.localScale = Vector3.one;
                    }
                } 
            }

            ToggleDisplayImages(m_hideRequestsLibrary.AreAllIDsReturned());
        }


        public int RequestHideEffect() => m_hideRequestsLibrary.CheckoutID();
        public void CancelHideEffectRequest(int requestID) => m_hideRequestsLibrary.ReturnID(requestID);


        private void ToggleSubscriptions(bool cond)
        {
            if (m_playerCont != null)
            {
                m_playerCont.onBeginTimeManip.ToggleSubscription(OnBeginTimeManip, cond);
                m_playerCont.onEndTimeManip.ToggleSubscription(OnEndTimeManip, cond);
            }
        }
        private void OnBeginTimeManip()
        {
            // Set most recent texture update time here so tha we don't start recording previous stuff until camera has finished moving.
            m_mostRecentTextureUpdateTime = Time.time + m_mainCamCont.moveTime;

            m_cam.enabled = true;

            isEffectActive = true;
            ToggleDisplayImages(true);
        }
        private void OnEndTimeManip()
        {
            isEffectActive = false;
            ToggleDisplayImages(false);

            m_cam.enabled = false;
            // Release the textures.
            m_curTexture.Release();
            for (int i = 0; i < m_amUpdatesToDisplay; ++i)
            {
                m_prevTextures[i].Release();
            }
        }

        private void InitializeDisplayImages()
        {
            float t_alpha = m_startingAlpha;
            for (int i = 0; i < m_amUpdatesToDisplay; ++i)
            {
                m_displayImgs[i].color = new Color(1.0f, 1.0f, 1.0f, t_alpha);
                t_alpha *= m_alphaReductionMultiplier;
                m_displayImgs[i].texture = m_prevTextures[i];
            }
        }
        private void ToggleDisplayImages(bool toggle)
        {
            for (int i = 0; i < m_amUpdatesToDisplay; ++i)
            {
                m_displayImgs[i].enabled = toggle;
            }
        }
    }
}