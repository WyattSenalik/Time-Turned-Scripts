using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

using NaughtyAttributes;

using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Timed;
using Timed.TimedComponentImplementations;
using UnityEngine.InputSystem.Extension;
using Helpers.Physics.Custom2D;
using Helpers.Events;
using Helpers.Animation.BetterCurve;
using Dialogue;
using UnityEngine.Tilemaps;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class EnterNextQuadrantDetector : TimedRecorder
    {
        public static EnterNextQuadrantDetector instance { get; private set; } = null;

        public Rectangle centerAreaRect => new Rectangle(m_centerOfLevel, m_insideSquare);
        public IEventPrimer<QuadrantChangedEventData> onBegunQuadrantTransition => m_onBegunQuadrantTransition;
        public IEventPrimer<eQuadrant> onQuadrantTransitionEnded => m_onQuadrantTransitionEnded;

        [SerializeField, Required] private FakePlayerVisuals m_fakePlayerVisuals = null;
        [SerializeField, Required] private GameObject m_stopwatchTimelineParentObj = null;
        [SerializeField, Required] private Conversation m_endCutsceneConvo = null;

        [SerializeField] private Vector3 m_cameraQuadrantOnePosition = new Vector3(-7.0f, 3.5f, -10.0f);
        [SerializeField] private Vector2 m_centerOfLevel = new Vector2(-7.0f, 10.0f);
        [SerializeField] private Vector2 m_levelSize = new Vector2(27.0f, 27.0f);
        [SerializeField] private SerializedRectangleInt m_quadrant1Area = new SerializedRectangleInt();
        [SerializeField] private SerializedRectangleInt m_quadrant2Area = new SerializedRectangleInt();
        [SerializeField] private SerializedRectangleInt m_quadrant3Area = new SerializedRectangleInt();
        [SerializeField] private SerializedRectangleInt m_quadrant4Area = new SerializedRectangleInt();
        [SerializeField] private Tilemap m_quadrant2FogTilemap = null;
        [SerializeField] private Tilemap m_quadrant3FogTilemap = null;
        [SerializeField] private Tilemap m_quadrant4FogTilemap = null;
        [SerializeField] private Vector2 m_insideSquare = new Vector2(5.0f, 5.0f);
        [SerializeField, Min(0.0f)] private float m_cameraTransitionDuration = 1.0f;
        [SerializeField, Min(1)] private int m_fullyZoomedOutPPU = 32;
        [SerializeField, Min(1)] private int m_fullyZoomedInPPU = 100;
        [SerializeField, Required] private BetterCurveSO m_ppuCurve = null;
        [SerializeField] private BetterCurve m_fogFadeAwayCurve = null;
        [SerializeField] private string m_restrictedInputMap = "Restricted";
        [SerializeField] private string m_dialogueInputMap = "Dialogue";
        [SerializeField] private eQuadrant m_beginQuadrant = eQuadrant.One;

        [SerializeField] private MixedEvent<QuadrantChangedEventData> m_onBegunQuadrantTransition = new MixedEvent<QuadrantChangedEventData>();
        [SerializeField] private MixedEvent<eQuadrant> m_onQuadrantTransitionEnded = new MixedEvent<eQuadrant>();

        private ConversationDriver m_convoDriver = null;
        private PlayerSingleton m_player = null;
        private TimedObject m_playerTimedObj = null;
        private InputMapStack m_inputMapStack = null;
        private BranchPlayerController m_playerCont = null;
        private CloneManager m_cloneManager = null;
        private TimeRewinder m_timeRewinder = null;
        private Int2DTransform m_playerIntTransform = null;
        private TimedIntTransform m_playerTimedTrans = null;
        private PixelPerfectCamera m_pixelPerfectCamera = null;
        private CameraController m_camCont = null;
        private CameraZoomer m_cameraZoomer = null;
        private RegainChargesAnimator m_regainChargesAnimator = null;
        private PreviousTimeEffect m_prevTimeEffect = null;
        private eQuadrant m_furthestQuadReached = eQuadrant.One;
        private eQuadrant m_prevQuad = eQuadrant.One;
        private Vector2Int m_playerPosAtCutsceneBegin = Vector2Int.zero;

        private bool m_isPlayingNextQuadReachedCutscene = false;
        private bool m_regainChargeAnimFin = false;
        private int m_hideChargeRequestID = -1;


        protected override void Awake()
        {
            base.Awake();

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Another {GetType().Name} is already in the scene. Destroy this one ({gameObject.GetFullName()})", this);
                #endregion Logs
                Destroy(gameObject);
            }

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_fakePlayerVisuals, nameof(m_fakePlayerVisuals), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopwatchTimelineParentObj, nameof(m_stopwatchTimelineParentObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ppuCurve, nameof(m_ppuCurve), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_convoDriver = ConversationDriver.GetInstanceSafe(this);
            m_player = PlayerSingleton.GetInstanceSafe(this);
            m_playerTimedObj = m_player.GetComponentSafe<TimedObject>(this);
            m_playerCont = m_player.GetComponentSafe<BranchPlayerController>(this);
            m_cloneManager = m_player.GetComponentSafe<CloneManager>(this);
            m_inputMapStack = m_player.GetComponentSafe<InputMapStack>(this);
            m_timeRewinder = m_player.GetComponentSafe<TimeRewinder>(this);
            m_playerIntTransform = m_player.GetComponentSafe<Int2DTransform>(this);
            m_playerTimedTrans = m_player.GetComponentSafe<TimedIntTransform>(this);
            Camera t_mainCam = Camera.main;
            m_pixelPerfectCamera = t_mainCam.GetComponentSafe<PixelPerfectCamera>(this);
            m_camCont = t_mainCam.GetComponentSafe<CameraController>(this);
            m_cameraZoomer = t_mainCam.GetComponentSafe<CameraZoomer>(this);
            m_regainChargesAnimator = RegainChargesAnimator.GetInstanceSafe(this);
            m_prevTimeEffect = PreviousTimeEffect.GetInstanceSafe(this);

            m_furthestQuadReached = m_beginQuadrant;
            // That was just for testing. Doesn't work now with conversation.
            //if (m_beginQuadrant != eQuadrant.One)
            //{
            //    Vector3 t_camPos = DetermineCameraPositionForQuadrant(m_beginQuadrant);
            //    m_camCont.transform.position = t_camPos;
            //}

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            // Wait until cut scene has finished.
            if (m_convoDriver.isConversationActive) { return; }
            if (m_isPlayingNextQuadReachedCutscene) { return; }

            eQuadrant t_curQuad = DetermineQuadrantPlayerIsIn();

            //if (Input.GetKey(KeyCode.P))
            //{
            //    if (m_furthestQuadReached < eQuadrant.Four)
            //    {
            //        m_furthestQuadReached = eQuadrant.Four;
            //    }
            //    t_curQuad = eQuadrant.End;
            //}

            if (IsNextQuad(m_furthestQuadReached, t_curQuad))
            {
                m_isPlayingNextQuadReachedCutscene = true;
                m_furthestQuadReached = t_curQuad;

                // Restrict player input
                RestrictPlayerInput();
                // Create a time clone and cut off the other clones from being tied to the charges on the stopwatch.
                m_cloneManager.SpawnCloneDisconnected(0.0f, curTime);
                m_cloneManager.DisconnectExistingClones();
                // Request charges be hidden since we just disconnected clones, which will cause them to be shown otherwise.
                m_hideChargeRequestID = m_regainChargesAnimator.stopwatchChargeMan.RequestActiveChargesBeHidden();
                // Fake player. Show fake, hide real.
                m_playerPosAtCutsceneBegin = m_playerIntTransform.localPosition;
                m_fakePlayerVisuals.Activate();
                // Start rewinding.
                m_playerCont.ForceBeginTimeManipulationWithoutTransitioningToTimeline();
                if (m_furthestQuadReached != eQuadrant.End)
                {
                    // If final, wait to rewind until the camera has moved.
                    m_playerCont.RewindNavVelocity();
                }

                m_onBegunQuadrantTransition.Invoke(new QuadrantChangedEventData(m_prevQuad, t_curQuad));
                // Wait until both the animation and the time rewind are done before moving the camera.
                StartCoroutine(MoveCameraAfterDelayForNextQuadReachedCoroutine(t_curQuad));
            }
            else if (t_curQuad == eQuadrant.Transition)
            {
                // Don't update previous quad if in transition
                return;
            }
            else if (m_prevQuad != t_curQuad)
            {
                m_onBegunQuadrantTransition.Invoke(new QuadrantChangedEventData(m_prevQuad, t_curQuad));
                StartMovingCamera(t_curQuad);
            }

            m_prevQuad = t_curQuad;
        }

        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            if (m_isPlayingNextQuadReachedCutscene)
            {
                m_fakePlayerVisuals.Deactivate();
                // Teleport the real player over.
                m_playerIntTransform.localPosition = m_playerPosAtCutsceneBegin;
                m_playerTimedTrans.ClearAllRecordedData();

                m_isPlayingNextQuadReachedCutscene = false;
            }
        }

        private IEnumerator MoveCameraAfterDelayForNextQuadReachedCoroutine(eQuadrant curQuad)
        {
            int t_requestHidePrevEffectID = -1;
            if (m_furthestQuadReached != eQuadrant.End)
            {
                yield return new WaitUntil(() => m_timeRewinder.curTime == 0.0f);
            }
            else
            {
                yield return new WaitForSeconds(1.5f);
                t_requestHidePrevEffectID = m_prevTimeEffect.RequestHideEffect();
                m_cameraZoomer.TransitionToPPU(m_fullyZoomedOutPPU, m_ppuCurve.Evaluate, m_cameraTransitionDuration, null);
            }
            StartMovingCamera(curQuad);

            // Fade away the fog for the section
            FadeAwayFogForQuad(m_furthestQuadReached);

            if (m_furthestQuadReached == eQuadrant.End)
            {
                yield return new WaitForSeconds(1.5f + m_prevTimeEffect.secondsBeforeTextureUpdate * m_prevTimeEffect.amUpdatesToDisplay);
                m_prevTimeEffect.CancelHideEffectRequest(t_requestHidePrevEffectID);
            }
        }
        private void ToggleSubscriptions(bool cond)
        {
            if (m_regainChargesAnimator != null)
            {
                m_regainChargesAnimator.onRegainAnimEnd.ToggleSubscription(OnRegainChargesAnimEnd, cond);
            }
        }
        private void OnRegainChargesAnimEnd()
        {
            m_regainChargeAnimFin = true;
            if (m_isPlayingNextQuadReachedCutscene)
            {
                ReallowPlayerControl();
            }
        }
        private void StartMovingCamera(eQuadrant curQuad)
        {
            Vector3 t_newCamPos = DetermineCameraPositionForQuadrant(curQuad);
            Vector3 t_timeAwareMoveToPos;
            float t_timeAwareCameraTransitionDuration;
            if (isRecording)
            {
                t_timeAwareMoveToPos = t_newCamPos;
                t_timeAwareCameraTransitionDuration = m_cameraTransitionDuration;
                // Restrict player
                RestrictPlayerInput();
                // Pause time while camera moves.
                m_timeRewinder.StartRewind();

                Invoke(nameof(OnEndCameraMove), t_timeAwareCameraTransitionDuration);
            }
            else if (curQuad == eQuadrant.End)
            {
                t_timeAwareMoveToPos = t_newCamPos;
                t_timeAwareCameraTransitionDuration = m_cameraTransitionDuration;

                Invoke(nameof(OnEndCameraMove), t_timeAwareCameraTransitionDuration);
            }
            else if (m_isPlayingNextQuadReachedCutscene)
            {
                t_timeAwareMoveToPos = t_newCamPos;
                t_timeAwareCameraTransitionDuration = m_cameraTransitionDuration;

                // Stop manipualting the way we were to end the animations that go along with pausing time through the player controller. Then start pausing again but through start rewind.
                bool t_sanityCheck = m_playerCont.TryEndTimeManipulationWithoutTransitioningToStopwatch();
                #region Logs
                if (!t_sanityCheck)
                {
                    //CustomDebug.LogError($"Failed to resume after next quad reached cutscene");
                }
                #endregion Logs
                m_timeRewinder.StartRewind();

                Invoke(nameof(OnEndCameraMove), t_timeAwareCameraTransitionDuration);
            }
            else
            {
                t_timeAwareMoveToPos = t_newCamPos + m_camCont.timeManipOffset;
                t_timeAwareCameraTransitionDuration = m_cameraTransitionDuration * 0.1f;
            }
            Invoke(nameof(InvokeOnQuadrantTransitionEndedEvent), t_timeAwareCameraTransitionDuration);
            // We changed quads, move the camera.
            m_camCont.OverrideStartPosition(t_newCamPos);
            m_camCont.StartMoveCamera(t_timeAwareMoveToPos, t_timeAwareCameraTransitionDuration);
        }
        private void RestrictPlayerInput()
        {
            m_inputMapStack.SwitchInputMap(m_restrictedInputMap);
        }
        private void OnEndCameraMove()
        {
            if (m_isPlayingNextQuadReachedCutscene)
            {
                if (m_furthestQuadReached != eQuadrant.End)
                {
                    // Stop requesting to hide the charges, cause the animation will do that.
                    m_regainChargesAnimator.stopwatchChargeMan.CancelRequestForActiveChargesToBeHidden(m_hideChargeRequestID);
                    m_hideChargeRequestID = -1;
                    // Play animation for regaining charges
                    m_regainChargesAnimator.PlayRegainAnimation();
                }
                else
                {
                    // It is the end quadrant, start rewinding
                    m_playerCont.RewindNavVelocity();
                    StartCoroutine(FinalSectionCorout());
                    IEnumerator FinalSectionCorout()
                    {
                        yield return new WaitUntil(() => m_timeRewinder.curTime == 0.0f);
                        // Play clone disappear animations
                        m_cloneManager.HaveAllClonesPlayDisappearAnimation();
                        yield return new WaitForSeconds(2.0f);
                        // Delete the clones to improve performance (we don't need them anymore anyway)
                        m_cloneManager.DeleteAllClones();

                        m_stopwatchTimelineParentObj.SetActive(false);
                        m_playerCont.TryEndTimeManipulationWithoutTransitioningToStopwatch();
                        m_timeRewinder.StartRewind();
                        m_cameraZoomer.TransitionToPPU(m_fullyZoomedInPPU, m_ppuCurve.Evaluate, m_cameraTransitionDuration, () =>
                        {
                            m_inputMapStack.PopInputMap(m_restrictedInputMap);
                            m_inputMapStack.SwitchInputMap(m_dialogueInputMap);
                            m_convoDriver.StartConversation(m_endCutsceneConvo);
                        });
                    }
                }
            }
            else
            {
                ReallowPlayerControl();
            }
        }
        private void ReallowPlayerControl()
        {
            m_inputMapStack.PopInputMap(m_restrictedInputMap);
            m_timeRewinder.CancelRewind();
        }
        private void InvokeOnQuadrantTransitionEndedEvent()
        {
            m_onQuadrantTransitionEnded.Invoke(DetermineQuadrantPointIsIn(m_fakePlayerVisuals.fakePlayerBodySprRend.transform.position));
        }

        private void FadeAwayFogForQuad(eQuadrant quadrantToFadeFogFor)
        {
            switch (quadrantToFadeFogFor)
            {
                case eQuadrant.One: break; // No fog for quad 1.
                case eQuadrant.Two:
                {
                    BeginFadeFogAway(m_quadrant2FogTilemap);
                    break;
                }
                case eQuadrant.Three:
                {
                    BeginFadeFogAway(m_quadrant3FogTilemap);
                    break;
                }
                case eQuadrant.Four:
                {
                    BeginFadeFogAway(m_quadrant4FogTilemap);
                    break;
                }
                case eQuadrant.Transition: break;
                case eQuadrant.End: break;
                default:
                {
                    CustomDebug.UnhandledEnum(quadrantToFadeFogFor, this);
                    break;
                }
            }
        }
        private void BeginFadeFogAway(Tilemap fogTilemap)
        {
            StartCoroutine(FadeFogAwayCorout(fogTilemap));
        }
        private IEnumerator FadeFogAwayCorout(Tilemap fogTilemap)
        {
            Color t_color = fogTilemap.color;
            float t_elapsedTime = 0.0f;
            float t_endTime = m_fogFadeAwayCurve.GetEndTime();
            while (t_elapsedTime < t_endTime)
            {
                t_color.a = m_fogFadeAwayCurve.Evaluate(t_elapsedTime);
                fogTilemap.color = t_color;
                yield return null;
                t_elapsedTime += Time.deltaTime;
            }
            fogTilemap.enabled = false;
        }


        private eQuadrant DetermineQuadrantPointIsIn(Vector2 point)
        {
            if (CustomPhysics2D.IsPointInRectangle(centerAreaRect, point))
            {
                return eQuadrant.End;
            }
            else
            {
                Vector2Int t_pointInt = CustomPhysics2DInt.ConvertFloatPositionToIntPosition(point);
                if (CustomPhysics2DInt.IsPointInRectangle(m_quadrant4Area.rectangle, t_pointInt))
                {
                    return eQuadrant.Four;
                }
                else if (CustomPhysics2DInt.IsPointInRectangle(m_quadrant3Area.rectangle, t_pointInt))
                {
                    return eQuadrant.Three;
                }
                else if (CustomPhysics2DInt.IsPointInRectangle(m_quadrant2Area.rectangle, t_pointInt))
                {
                    return eQuadrant.Two;
                }
                else if (CustomPhysics2DInt.IsPointInRectangle(m_quadrant1Area.rectangle, t_pointInt))
                {
                    return eQuadrant.One;
                }
                else
                {
                    return eQuadrant.Transition;
                }
            }
        }
        private Vector3 DetermineCameraPositionForQuadrant(eQuadrant quadrant)
        {
            switch (quadrant)
            {
                case eQuadrant.One: return m_cameraQuadrantOnePosition;
                case eQuadrant.Two: return m_cameraQuadrantOnePosition + new Vector3(0.0f, m_levelSize.y * 0.5f);
                case eQuadrant.Three: return m_cameraQuadrantOnePosition + new Vector3(m_levelSize.x * 0.5f, m_levelSize.y * 0.5f);
                case eQuadrant.Four: return m_cameraQuadrantOnePosition + new Vector3(m_levelSize.x * 0.5f, 0.0f);
                case eQuadrant.End: return new Vector3(m_centerOfLevel.x, m_centerOfLevel.y, m_cameraQuadrantOnePosition.z);
                default:
                {
                    CustomDebug.UnhandledEnum(quadrant, this);
                    return m_cameraQuadrantOnePosition;
                }
            }
        }

        private static bool IsNextQuad(eQuadrant firstQuad, eQuadrant potentialNexQuad)
        {
            switch (firstQuad)
            {
                case eQuadrant.One: return potentialNexQuad == eQuadrant.Two;
                case eQuadrant.Two: return potentialNexQuad == eQuadrant.Three;
                case eQuadrant.Three: return potentialNexQuad == eQuadrant.Four;
                case eQuadrant.Four: return potentialNexQuad == eQuadrant.End;
                case eQuadrant.End: return false;
                case eQuadrant.Transition: return false;
                default:
                {
                    CustomDebug.UnhandledEnum(firstQuad, nameof(EnterNextQuadrantDetector.IsNextQuad));
                    return false;
                }
            }
        }


        public eQuadrant DetermineQuadrantPlayerIsIn() => DetermineQuadrantPointIsIn(m_player.transform.position);


        private void OnDrawGizmosSelected()
        {
            // Draw horizontal lines
            m_quadrant1Area.rectangle.DrawOutlineGizmos(Color.red);
            m_quadrant2Area.rectangle.DrawOutlineGizmos(Color.green);
            m_quadrant3Area.rectangle.DrawOutlineGizmos(Color.blue);
            m_quadrant4Area.rectangle.DrawOutlineGizmos(Color.cyan);

            // Inside bit
            centerAreaRect.DrawOutlineGizmos(Color.magenta);
        }


        public enum eQuadrant { One, Two, Three, Four, End, Transition }

        public sealed class QuadrantChangedEventData
        {
            public eQuadrant oldQuadrant { get; private set; }
            public eQuadrant newQuadrant { get; private set; }

            public QuadrantChangedEventData(eQuadrant oldQuadrant, eQuadrant newQuadrant)
            {
                this.oldQuadrant = oldQuadrant;
                this.newQuadrant = newQuadrant;
            }
        }
    }
}