using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;

using Dialogue;
using Helpers.Singletons;
using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class PlaySpeedParamSyncer : SingletonMonoBehaviourPersistant<PlaySpeedParamSyncer>
    {
        public GlobalTimeManager timeMan
        {
            get
            {
                if (m_timeMan == null)
                {
                    m_timeMan = GlobalTimeManager.instance;
                }
                return m_timeMan;
            }
        }
        public BranchPlayerController branchPlayerCont
        {
            get
            {
                if (m_branchPlayerCont == null)
                {
                    InitializeBranchPlayerController();
                }
                return m_branchPlayerCont;
            }
        }
        public float prevPitch { get; private set; } = 0.0f;


        [SerializeField, Required] private StringReference m_playSpeedRTPCName = null;
        [SerializeField, Required] private StringReference m_playSpeedSmoothRTPCName = null;

        private GlobalTimeManager m_timeMan = null;
        private BranchPlayerController m_branchPlayerCont = null;
        private bool m_wasRecording = false;

        private float m_recentManipStartTime = float.PositiveInfinity;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playSpeedRTPCName, nameof(m_playSpeedRTPCName), this);
            #endregion Asserts
        }
        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;

            InitializeBranchPlayerController();
        }
        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;

            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            if (!m_wasRecording && timeMan.shouldRecord)
            {
                // Resumed
                OnResumeFake();
            }

            if (!timeMan.shouldRecord)
            {
                // Time is being manipulated
                float t_pitch;
                if (Time.deltaTime == 0.0f)
                {
                    t_pitch = 0.0f;
                }
                else
                {
                    t_pitch = timeMan.deltaTime / Time.deltaTime;
                }

                float t_positivePitch = Mathf.Abs(t_pitch);
                AkSoundEngine.SetRTPCValue(m_playSpeedRTPCName.refString, t_positivePitch);

                // Calculate smooth pitch
                float t_smoothPitch = 0.0f;
                if (t_positivePitch > 0.0f)
                {
                    t_smoothPitch = Mathf.Abs(branchPlayerCont.timeManipNavVal);
                    if (Time.time > m_recentManipStartTime)
                    {
                        float t_timeSinceManipStart = Time.time - m_recentManipStartTime;
                        float t_increaseTime = branchPlayerCont.timeUntilIncreaseNavSpeed;
                        t_smoothPitch += t_timeSinceManipStart / t_increaseTime;
                    }
                }
                t_smoothPitch = Mathf.Min(t_smoothPitch, branchPlayerCont.maxRewindSpeed);
                AkSoundEngine.SetRTPCValue(m_playSpeedSmoothRTPCName.refString, t_smoothPitch);


                prevPitch = t_pitch;
            }

            m_wasRecording = timeMan.shouldRecord;
        }


        private void OnResumeFake()
        {
            AkSoundEngine.SetRTPCValue(m_playSpeedRTPCName.refString, 1.0f);
            prevPitch = 1.0f;
        }

        private void InitializeBranchPlayerController()
        {
            ToggleSubscriptions(false);
            PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
            if (t_playerSingleton != null)
            {
                m_branchPlayerCont = t_playerSingleton.GetComponent<BranchPlayerController>();
            }
            ToggleSubscriptions(true);
        }
        private void ToggleSubscriptions(bool toggle)
        {
            if (m_branchPlayerCont != null)
            {
                m_branchPlayerCont.onRewindNavVelocity.ToggleSubscription(OnRewindNavVelocity, toggle);
                m_branchPlayerCont.onFastForwardNavVelocity.ToggleSubscription(OnFastForwardNavVelocity, toggle);
            }
        }
        private void OnRewindNavVelocity()
        {
            m_recentManipStartTime = Time.time;
        }
        private void OnFastForwardNavVelocity()
        {
            m_recentManipStartTime = Time.time;
        }
        private void OnSceneChanged(Scene prevScene, Scene newScene)
        {
            InitializeBranchPlayerController();
        }
    }
}