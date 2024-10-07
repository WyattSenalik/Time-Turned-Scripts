using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

using NaughtyAttributes;

using Helpers.Math;
using Helpers.Animation.BetterCurve;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Moves the camera when time manipulation begins.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PixelPerfectCamera))]
    public sealed class CameraController : MonoBehaviour
    {
        public new Camera camera => m_camera;
        public BranchPlayerController playerCont => m_playerCont;
        public float moveTime => m_moveTime;
        public Vector3 normalTimeMoveToPos => m_normalTimeMoveToPos;
        public eRoomSize roomSize => m_roomSize;
        public Vector3 smallRoomManipTimeMoveToPos => m_smallRoomManipTimeMoveToPos;
        public Vector3 largeRoomManipTimeMoveToPos => m_largeRoomManipTimeMoveToPos;
        public Vector3 timeManipOffset => m_timeManipOffset;
        public bool isMovingCamera => m_isMovingCamera;

        public float validMoveIncrement => 2.0f / (pixelPerfectCam.assetsPPU);

        public PixelPerfectCamera pixelPerfectCam { get; private set; } = null;

        [SerializeField, Required] private Camera m_camera = null;
        [SerializeField, Required] private BranchPlayerController m_playerCont = null;
        [SerializeField, Min(0.0f)] private float m_moveTime = 1.0f;
        [SerializeField] private bool m_useMoveToPositions = true;
        [SerializeField, ShowIf(nameof(m_useMoveToPositions))] private Vector3 m_normalTimeMoveToPos = new Vector3(0.0f, 3.5f, -10.0f);
        [SerializeField, ShowIf(nameof(m_useMoveToPositions))] private eRoomSize m_roomSize = eRoomSize.Small;
        [SerializeField, ShowIf(nameof(m_useMoveToPositions))] private Vector3 m_smallRoomManipTimeMoveToPos = new Vector3(0.0f, 2.65f, -10.0f);
        [SerializeField, ShowIf(nameof(m_useMoveToPositions))] private Vector3 m_largeRoomManipTimeMoveToPos = new Vector3(0.0f, 2.0f, -10.0f);
        [SerializeField, HideIf(nameof(m_useMoveToPositions))] private Vector3 m_timeManipOffset = new Vector3(0.0f, -1.5f, 0.0f);
        [SerializeField] private BetterCurve m_shakeCurveX = new BetterCurve();
        [SerializeField] private BetterCurve m_shakeCurveY = new BetterCurve();

        private Vector3 m_startCamPos = Vector3.zero;
        private Vector3 m_mostRecentCamRestingPos = Vector3.zero;

        private bool m_isMovingCamera = false;
        private float m_elapsedMoveCamTime = 0.0f;
        private float m_moveTimeToTake = 0.0f;
        private Vector3 m_camMoveStartPos = Vector3.zero;
        private Vector3 m_camMoveEndPos = Vector3.zero;
        private bool m_isShakingCamera = false;
        private float m_elapsedShakeCamTime = 0.0f;
        private Vector3 m_camShakeOffset = Vector3.zero;


        private void Awake()
        {
            pixelPerfectCam = this.GetComponentSafe<PixelPerfectCamera>(this);
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_camera, nameof(m_camera), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerCont, nameof(m_playerCont), this);
            #endregion Asserts

            // Doing this in Awake because its serialized and otherwise we get a race condition w/ PauseBeforeLevel
            ToggleSubscriptions(true);
        }
        private void Start()
        {
            m_startCamPos = m_camera.transform.position;
            m_mostRecentCamRestingPos = m_startCamPos;
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void Update()
        {
            if (m_isMovingCamera && m_isShakingCamera)
            {
                // Moving and shaking at same time.
                // Determine the shake offset first
                UpdateCameraShakeOffset();
                // Then move.
                UpdateCameraMove();
                UpdateCameraToPixelPerfectPosition();
            }
            else if (m_isMovingCamera)
            {
                // Just moving.
                UpdateCameraMove();
                UpdateCameraToPixelPerfectPosition();
            }
            else if (m_isShakingCamera)
            {
                // Just shaking.
                UpdateCameraShakeOffset();
                m_camera.transform.position = m_mostRecentCamRestingPos + m_camShakeOffset;
                UpdateCameraToPixelPerfectPosition();
            }
        }


        public void StartMoveCamera(Vector3 camEndPos, float? timeToTake = null)
        {
            m_isMovingCamera = true;
            m_elapsedMoveCamTime = 0.0f;
            m_moveTimeToTake = timeToTake.HasValue ? timeToTake.Value : m_moveTime;
            m_camMoveStartPos = m_camera.transform.position;
            m_camMoveEndPos = camEndPos;
        }
        public void ShakeCamera()
        {
            m_isShakingCamera = true;
            m_elapsedShakeCamTime = 0.0f;
        }
        public Vector3 DetermineTimeManipCamPositionFromRoomSize()
        {
            switch (m_roomSize)
            {
                case eRoomSize.Small:
                    return m_smallRoomManipTimeMoveToPos;
                case eRoomSize.Large:
                    return m_largeRoomManipTimeMoveToPos;
                default:
                {
                    CustomDebug.UnhandledEnum(m_roomSize, this);
                    return Vector3.zero;
                }
            }
        }
        public void UpdateCameraToPixelPerfectPosition()
        {
            Vector3 t_curPos = m_camera.transform.position;
            t_curPos.x = MathHelpers.RoundDownToNearestMultiple(t_curPos.x, validMoveIncrement);
            t_curPos.y = MathHelpers.RoundDownToNearestMultiple(t_curPos.y, validMoveIncrement);
            m_camera.transform.position = t_curPos;
        }
        public void OverrideStartPosition(Vector3 newPos)
        {
            m_startCamPos = newPos;
        }

        private void UpdateCameraShakeOffset()
        {
            float t_shakeTimeToTake = Mathf.Max(m_shakeCurveX.GetEndTime(), m_shakeCurveY.GetEndTime());
            if (m_elapsedShakeCamTime < t_shakeTimeToTake)
            {
                float t_xShake = m_shakeCurveX.EvaluateClamped(m_elapsedShakeCamTime);
                float t_yShake = m_shakeCurveY.EvaluateClamped(m_elapsedShakeCamTime);
                m_camShakeOffset = new Vector3(t_xShake, t_yShake);

                m_elapsedShakeCamTime += Time.deltaTime;
            }
            else
            {
                m_camShakeOffset = Vector3.zero;
                m_isShakingCamera = false;

                m_camera.transform.position = m_mostRecentCamRestingPos;
            }
        }
        private void UpdateCameraMove()
        {
            if (m_elapsedMoveCamTime < m_moveTimeToTake)
            {
                Vector3 t_newPos = Vector3.Lerp(m_camMoveStartPos, m_camMoveEndPos, m_elapsedMoveCamTime / m_moveTimeToTake);
                m_camera.transform.position = t_newPos + m_camShakeOffset;

                m_elapsedMoveCamTime += Time.deltaTime;
            }
            else
            {
                m_camera.transform.position = m_camMoveEndPos;
                m_mostRecentCamRestingPos = m_camMoveEndPos;
                m_isMovingCamera = false;
            }
        }

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
            if (m_playerCont.wasTransitionPlayedWhenManipulatingTime)
            {
                Vector3 t_timeManipMovePos = m_useMoveToPositions ? DetermineTimeManipCamPositionFromRoomSize() : m_startCamPos + m_timeManipOffset;
                StartMoveCamera(t_timeManipMovePos);
            }
        }
        private void OnEndTimeManip()
        {
            Vector3 t_resumeTimeMovePos = m_useMoveToPositions ? m_normalTimeMoveToPos : m_startCamPos;
            StartMoveCamera(t_resumeTimeMovePos);
        }


        public enum eRoomSize { Small, Large }
    }
}
