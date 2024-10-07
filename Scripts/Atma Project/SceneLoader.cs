using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;

using Helpers;
using Helpers.Events;
using Helpers.Singletons;
using UnityEngine.InputSystem;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Provides scene loading with some animated screen.
    /// </summary>
    public sealed class SceneLoader : SingletonMonoBehaviourPersistant<SceneLoader>
    {
        public const int ROOM_TO_ROOM_ENDING_INDEX = 1;
        public const int TO_MENU_ENDING_INDEX = 0;
        private const bool IS_DEBUGGING = false;

        public float transitionTime => m_transitionTime;
        public float endAnimTime => m_endAnimTime;
        public float noGroundTeleEndAnimTime => m_noGroundTeleEndAnimTime;
        public bool isLoading => m_hasLoadScreenStarted;
        public IEventPrimer onStartLoading => m_onStartLoading;
        public IEventPrimer onEndLoading => m_onEndLoading;

        public event Action<float> onProgressChanged;

        private PlayerInput playerInp
        {
            get
            {
                if (m_playerInp == null)
                {
                    PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
                    if (t_playerSingleton == null)
                    {
                        //CustomDebug.LogWarningForComponent($"No PlayerSingleton in the scene", this);
                        return null;
                    }
                    else
                    {
                        m_playerInp = t_playerSingleton.GetComponent<PlayerInput>();
                        if (m_playerInp == null)
                        {
                            //CustomDebug.LogWarningForComponent($"PlayerSingleton has no PlayerInput.", this);
                        }
                    }
                }
                return m_playerInp;
            }
        }

        [SerializeField, Min(0.0f)] private float m_transitionTime = 1.0f;
        [SerializeField, Min(0.0f)] private float m_endAnimTime = 1.0f;
        [SerializeField, Min(0.0f)] private float m_noGroundTeleEndAnimTime = 1.0f;
        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_startTriggerParameter = "Start";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_endTriggerParameter = "End";
        [SerializeField, AnimatorParam(nameof(m_animator))]
        private string m_endingIndexIntParameter = "EndingIndex";
        [SerializeField] private bool m_shouldSetTimeScaleToZeroUntilAnimEnd = true;
        [SerializeField, Scene] private string[] m_dontEndScenes = new string[0];
        [SerializeField, Scene] private string[] m_noGroundTeleScenes = new string[0];
        [SerializeField] private MixedEvent m_onStartLoading = new MixedEvent();
        [SerializeField] private MixedEvent m_onEndLoading = new MixedEvent();

        private UISoundController m_uiSoundCont = null;
        private readonly GamePauser m_gamePauser = new GamePauser();
        private PlayerInput m_playerInp = null;

        private bool m_hasLoadScreenStarted = false;
        private bool m_isCoroutActive = false;
        private int m_endingIndex = -1;


        protected override void Awake()
        {
            base.Awake();

            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        private void Start()
        {
            m_playerInp = PlayerSingleton.GetInstanceSafe(this).GetComponentSafe<PlayerInput>(this);
            m_uiSoundCont = UISoundController.GetInstanceSafe(this);
        }
        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }


        public void LoadScene(string sceneName, bool isRoomToRoom = false)
        {
            //CustomDebug.Log("LoadSceneByName", IS_DEBUGGING);
            StartCoroutine(LoadSceneByNameCoroutine(sceneName, isRoomToRoom));
        }
        public void LoadScene(int sceneIndex, bool isRoomToRoom = false)
        {
            //CustomDebug.Log("LoadSceneByIndex", IS_DEBUGGING);
            StartCoroutine(LoadSceneByIndexCoroutine(sceneIndex, isRoomToRoom));
        }
        public void ShowLoadingScreen(AsyncOperation asyncLoadingOp)
        {
            StartCoroutine(LoadingCoroutine(asyncLoadingOp));
        }
        public void StartLoadingScreen()
        {
            // Only advance to start, if it hasn't been started yet
            if (!m_hasLoadScreenStarted)
            {
                m_onStartLoading.Invoke();

                playerInp.DeactivateInput();

                m_animator.SetTrigger(m_startTriggerParameter);
                m_hasLoadScreenStarted = true;

                if (m_shouldSetTimeScaleToZeroUntilAnimEnd)
                {
                    m_gamePauser.RequestToPause();
                }

                if (m_endingIndex == ROOM_TO_ROOM_ENDING_INDEX)
                {
                    // SOUND NOW PLAYED IN AdvanceToNextLevelTrigger
                }
                else if (m_endingIndex == TO_MENU_ENDING_INDEX)
                {
                    m_uiSoundCont.PlayToMenuTransition();
                }
            }
        }
        public void EndLoadingScreen()
        {
            // Only end if we've started
            if (m_hasLoadScreenStarted)
            {
                string t_activeSceneName = SceneManager.GetActiveScene().name;
                if (!m_dontEndScenes.Contains(t_activeSceneName))
                {
                    m_onEndLoading.Invoke();

                    m_animator.SetTrigger(m_endTriggerParameter);
                    m_hasLoadScreenStarted = false;

                    float t_endAnimTime = m_noGroundTeleScenes.Contains(t_activeSceneName) ? m_noGroundTeleEndAnimTime : m_endAnimTime;
                    InvokeUnscaled(OnEndLoadingTransitionFinished, t_endAnimTime);
                }
            }
        }


        private IEnumerator LoadSceneByNameCoroutine(string sceneName, bool isRoomToRoom)
        {
            UpdateEndingIndex(isRoomToRoom);
            StartLoadingScreen();
            yield return new WaitForSecondsRealtime(m_transitionTime);
            AsyncOperation t_asyncOp = SceneManager.LoadSceneAsync(sceneName);
            StartCoroutine(LoadingCoroutine(t_asyncOp));
        }
        private IEnumerator LoadSceneByIndexCoroutine(int sceneIndex, bool isRoomToRoom)
        {
            UpdateEndingIndex(isRoomToRoom);
            StartLoadingScreen();
            yield return new WaitForSecondsRealtime(m_transitionTime);
            AsyncOperation t_asyncOp = SceneManager.LoadSceneAsync(sceneIndex);
            StartCoroutine(LoadingCoroutine(t_asyncOp));
        }
        private IEnumerator LoadingCoroutine(AsyncOperation asyncLoadingOp)
        {
            m_isCoroutActive = true;

            #region Logs
            //CustomDebug.LogForComponent($"Starting loading screen", this, IS_DEBUGGING);
            #endregion Logs
            StartLoadingScreen();

            const float t_nineTenthsInverted = 1.0f / 0.9f;
            while (asyncLoadingOp != null && !asyncLoadingOp.isDone)
            {
                #region Logs
                //CustomDebug.LogForComponent($"Loading progress: {asyncLoadingOp.progress}", this, IS_DEBUGGING);
                #endregion Logs
                // Divide by 0.9f because from 0-0.9 is the async loading.
                // The 0.9-1 is the deleting of the last scene.
                onProgressChanged?.Invoke(asyncLoadingOp.progress * t_nineTenthsInverted);
                yield return null;
            }

            EndLoadingScreen();
            #region Logs
            //CustomDebug.LogForComponent($"Ending loading screen", this, IS_DEBUGGING);
            #endregion Logs

            m_isCoroutActive = false;
        }

        private void OnEndLoadingTransitionFinished()
        {
            playerInp.ActivateInput();
            if (m_shouldSetTimeScaleToZeroUntilAnimEnd)
            {
                m_gamePauser.CancelRequestToPause();
            }
        }

        private void UpdateEndingIndex(bool isRoomToRoom)
        {
            m_endingIndex = isRoomToRoom ? ROOM_TO_ROOM_ENDING_INDEX : TO_MENU_ENDING_INDEX;
            m_animator.SetInteger(m_endingIndexIntParameter, m_endingIndex);
        }
        private void OnSceneChanged(Scene prevScene, Scene newScene)
        {
            // If we change scenes and the loading coroutine isn't active, yet
            // the loading screen has started, end the loading screen.
            if (!m_isCoroutActive && m_hasLoadScreenStarted)
            {
                // Only auto end loading screen if scene is not a don't end scene.
                if (!m_dontEndScenes.Contains(newScene.name))
                {
                    EndLoadingScreen();
                }
            }

            // Loading a new scene resets the time scale to 1, set it to 0 until we are ready
            if (m_hasLoadScreenStarted && m_shouldSetTimeScaleToZeroUntilAnimEnd)
            {
                m_gamePauser.RequestToPause();
            }

            if (m_hasLoadScreenStarted)
            {
                playerInp.DeactivateInput();
            }
        }

        private void InvokeUnscaled(Action action, float seconds)
        {
            StartCoroutine(InvokeUnscaledCorout(action, seconds));
        }
        private IEnumerator InvokeUnscaledCorout(Action action, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action?.Invoke();
        }
    }
}