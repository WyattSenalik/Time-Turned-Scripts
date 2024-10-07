using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

using Helpers.Animation.BetterCurve;
using Helpers.Extensions;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class CameraMover : MonoBehaviour
    {
        public IEventPrimer onMoveFinished => m_onMoveFinished;
        public IEventPrimer onZoomFinished => m_onZoomFinished;

        [SerializeField] private Vector3 m_moveToPos = Vector3.zero;
        [SerializeField, Min(0.0f)] private float m_moveTimeToTake = 1.0f;
        [SerializeField] private int m_zoomToPPU = 100;
        [SerializeField] private BetterCurveSO m_zoomCurve = null;
        [SerializeField] private MixedEvent m_onMoveFinished = new MixedEvent();
        [SerializeField] private MixedEvent m_onZoomFinished = new MixedEvent();

        private CameraController m_camCont = null;
        private CameraZoomer m_camZoomer = null;


        private void Awake()
        {
            Camera t_mainCam = Camera.main;
            m_camCont = t_mainCam.GetComponentSafe<CameraController>(this);
            m_camZoomer = t_mainCam.GetComponentSafe<CameraZoomer>(this);
        }
        

        public void StartCameraMoveAndZoom()
        {
            StartCameraMove();
            StartCameraZoom();
        }
        public void StartCameraMove()
        {
            m_camCont.StartMoveCamera(m_moveToPos, m_moveTimeToTake);
            Invoke(nameof(InvokeCamMoveFinishedEvent), m_moveTimeToTake);
        }
        public void StartCameraZoom()
        {
            int t_startPPU = m_camZoomer.curPPU;
            Func<float, float> t_tFunc = m_zoomCurve == null ? (float t) => Mathf.Lerp(t_startPPU, m_zoomToPPU, t) : m_zoomCurve.Evaluate;
            m_camZoomer.TransitionToPPU(t_startPPU, m_zoomToPPU, t_tFunc, m_zoomCurve.GetEndTime(), m_onZoomFinished.Invoke);
        }
        public void OverrideCameraStartPositionToMoveToPos() => m_camCont.OverrideStartPosition(m_moveToPos);


        private void InvokeCamMoveFinishedEvent()
        {
            m_onMoveFinished.Invoke();
        }
    }
}