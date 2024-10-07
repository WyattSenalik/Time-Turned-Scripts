using System;
using System.Collections;
using UnityEngine;

using UnityEngine.Experimental.Rendering.Universal;

using Helpers.Extensions;
using UnityEngine.UI;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(PixelPerfectCamera))]
    public sealed class CameraZoomer : MonoBehaviour
    {
        public bool isZoomCoroutActive => m_isZoomCoroutActive;
        public int roomDefaultPPU => m_roomDefaultPPU;
        public int curPPU => Mathf.FloorToInt(minPPU * curGameRenderImgSize);
        public float curGameRenderImgSize => m_gameRenderRawImg.rectTransform.localScale.x;
        public Vector3 curGameRenderImgLocalScale => m_gameRenderRawImg.rectTransform.localScale;
        public int minPPU => m_isMinPPUOverriden ? m_minPPUOverride : m_minPPU;

        [SerializeField] private int m_minPPU = 64;
        [SerializeField] private RawImage m_gameRenderRawImg = null;
        [SerializeField] private RenderTexture m_gameRenderTexture = null;

        private Camera m_camera = null;
        private CameraController m_camCont = null;
        private PixelPerfectCamera m_pixelPerfectCam = null;

        private bool m_isMinPPUOverriden = false;
        private int m_minPPUOverride = 64;
        private int m_roomDefaultPPU = 0;

        private bool m_isZoomCoroutActive = false;
        private Coroutine m_zoomCorout = null;
        private Action m_mostRecentOnFinCallback = null;


        private void Awake()
        {
            m_camera = this.GetComponentSafe<Camera>(this);
            m_camCont = this.GetComponentSafe<CameraController>(this);
            m_pixelPerfectCam = this.GetComponentSafe<PixelPerfectCamera>(this);
        }
        private void Start()
        {
            m_roomDefaultPPU = Mathf.FloorToInt(m_minPPU * curGameRenderImgSize);
        }


        public void TransitionToPPU(int startPPU, int endPPU, Func<float, float> tFunction, float timeToTake, Action onFinished = null)
        {
            if (m_isZoomCoroutActive)
            {
                StopCoroutine(m_zoomCorout);
                m_mostRecentOnFinCallback?.Invoke();
                m_isZoomCoroutActive = false;
            }
            m_mostRecentOnFinCallback = onFinished;
            m_zoomCorout = StartCoroutine(ZoomCoroutine(startPPU, endPPU, tFunction, timeToTake));
        }
        public void TransitionToPPU(int endPPU, Func<float, float> tFunction, float timeToTake, Action onFinished = null)
        {
            TransitionToPPU(curPPU, endPPU, tFunction, timeToTake, onFinished);
        }


        private IEnumerator ZoomCoroutine(int startPPU, int endPPU, Func<float, float> tFunction, float timeToTake)
        {
            m_isZoomCoroutActive = true;

            int t_realMinPPU = Mathf.Min(m_minPPU, startPPU, endPPU);
            if (t_realMinPPU < m_minPPU)
            {
                m_isMinPPUOverriden = true;
                m_minPPUOverride = t_realMinPPU;
            }
            else
            {
                m_isMinPPUOverriden = false;
            }
            
            m_camera.targetTexture = m_gameRenderTexture;
            m_pixelPerfectCam.assetsPPU = t_realMinPPU;
            m_gameRenderRawImg.enabled = true;
            m_gameRenderRawImg.texture = m_gameRenderTexture;

            float t_elapsedTime = 0.0f;
            float t_scale;
            while (t_elapsedTime < timeToTake)
            {
                float t = Mathf.Clamp01(tFunction.Invoke(t_elapsedTime));
                float t_desiredPPU = Mathf.Lerp(startPPU, endPPU, t);
                t_scale = t_desiredPPU / t_realMinPPU;
                m_gameRenderRawImg.rectTransform.localScale = new Vector3(t_scale, t_scale, t_scale);
                m_camCont.UpdateCameraToPixelPerfectPosition();

                yield return null;
                t_elapsedTime += Time.deltaTime;
            }
            t_scale = ((float)endPPU) / t_realMinPPU;
            m_gameRenderRawImg.rectTransform.localScale = new Vector3(t_scale, t_scale, t_scale);
            m_camCont.UpdateCameraToPixelPerfectPosition();

            m_camera.targetTexture = null;
            m_pixelPerfectCam.assetsPPU = endPPU;
            m_gameRenderRawImg.enabled = false;

            m_mostRecentOnFinCallback?.Invoke();

            m_isZoomCoroutActive = false;
        }
    }
}