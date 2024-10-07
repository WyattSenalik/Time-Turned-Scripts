using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ToggleActiveIfOnScreen : MonoBehaviour
    {
        [SerializeField, Required] private GameObject m_gameObjectToToggle = null;

        private Camera m_mainCam = null;
        private CameraController m_camCont = null;
        private CameraZoomer m_camZoomer = null;

        private Vector2 m_prevCamPos = Vector2.zero;
        private float m_prevCamZoom = 0.0f;


        private void Start()
        {
            m_mainCam = Camera.main;
            m_camCont = m_mainCam.GetComponentSafe<CameraController>(this);
            m_camZoomer = m_mainCam.GetComponentSafe<CameraZoomer>(this);
            UpdateIfActive();

            m_prevCamPos = m_mainCam.transform.position;
            m_prevCamZoom = m_mainCam.orthographicSize;
        }
        private void Update()
        {
            Vector2 t_camPos = m_mainCam.transform.position;
            float t_camZoom = m_mainCam.orthographicSize;

            if (m_camCont.isMovingCamera || m_camZoomer.isZoomCoroutActive || t_camPos != m_prevCamPos || t_camZoom != m_prevCamZoom)
            {
                UpdateIfActive();
            }

            m_prevCamPos = t_camPos;
            m_prevCamZoom = t_camZoom;
        }


        private void UpdateIfActive()
        {
            Vector3 t_toggleObjPos = m_gameObjectToToggle.transform.position;
            Vector3 t_viewPoint = m_mainCam.WorldToViewportPoint(t_toggleObjPos);
            bool t_isOnScreen = t_viewPoint.x > 0 && t_viewPoint.x < 1 && t_viewPoint.y > 0 && t_viewPoint.y < 1;
            m_gameObjectToToggle.SetActive(t_isOnScreen);
        }
    }
}