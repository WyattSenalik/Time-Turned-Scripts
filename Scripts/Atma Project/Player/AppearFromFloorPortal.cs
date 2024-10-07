using UnityEngine;

using NaughtyAttributes;
using Timed.TimedComponentImplementations;
using Timed;
using Helpers.Events;
using Helpers.Events.CatchupEvents;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Turns on the player object at appropriate time during floor teleporter anim.
    /// </summary>
    public sealed class AppearFromFloorPortal : MonoBehaviour
    {
        public IEventPrimer onSpawnPlayer => m_onSpawnPlayer;
        public IEventPrimer onSkipToEnd => m_onSkipToEnd;
        public bool isLoadingIntoLevel { get; private set; } = false;

        [SerializeField, Required] private Animator m_portalAnimator = null;
        [SerializeField, Required] private Animator m_poofAnimator = null;
        [SerializeField, AnimatorParam(nameof(m_portalAnimator))] private string m_portalSkipToEndTriggerName = "SkipToEnd";
        [SerializeField, AnimatorParam(nameof(m_poofAnimator))] private string m_poofSkipToEndTriggerName = "SkipToEnd";

        [SerializeField, Required] private TimedObject m_playerTimedObject = null;

        [SerializeField] private MixedEvent m_onSpawnPlayer = new MixedEvent();
        private readonly CatchupEvent m_onSkipToEnd = new CatchupEvent();

        private SceneLoader m_sceneLoader = null;

        private int m_suspendPlayerRecordingRequestID = -1;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerTimedObject, nameof(m_playerTimedObject), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_portalAnimator, nameof(m_portalAnimator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_poofAnimator, nameof(m_poofAnimator), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_sceneLoader = SceneLoader.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_sceneLoader, this);
            #endregion Asserts
            // We are loading, so turn of the player's visuals
            if (m_sceneLoader.isLoading)
            {
                isLoadingIntoLevel = true;
                // There is a race condition where if the player's TimedObject's start is called after this start, and this turns off the player, that start will never be called, leading to the player's spawn and farthest times to be uninitialized and the player will never turn itself on then. To fix this we wait 1 frame.
                StartCoroutine(WaitAFrameToTurnOffPlayerCorout());
                IEnumerator WaitAFrameToTurnOffPlayerCorout()
                {
                    yield return null;
                    SetPlayerActive(false);
                }
            }
            // Not loading (run scene from editor) so skip to the animation ends right away [won't happen in builds].
            else
            {
                m_portalAnimator.SetTrigger(m_portalSkipToEndTriggerName);
                m_poofAnimator.SetTrigger(m_poofSkipToEndTriggerName);
                isLoadingIntoLevel = false;

                m_onSkipToEnd.Invoke();
            }
        }


        /// <summary>
        /// Should be called from animation event.
        /// </summary>
        public void SpawnPlayer()
        {
            SetPlayerActive(true);
            isLoadingIntoLevel = false;

            m_onSpawnPlayer.Invoke();
        }


        private void SetPlayerActive(bool active)
        {
            if (active)
            {
                if (m_suspendPlayerRecordingRequestID != -1)
                {
                    m_playerTimedObject.CancelSuspendRecordingRequest(m_suspendPlayerRecordingRequestID);
                    m_suspendPlayerRecordingRequestID = -1;
                }
            }
            else
            {
                if (m_suspendPlayerRecordingRequestID == -1)
                {
                    m_suspendPlayerRecordingRequestID = m_playerTimedObject.RequestSuspendRecording();
                }
            } 
        }
    }
}