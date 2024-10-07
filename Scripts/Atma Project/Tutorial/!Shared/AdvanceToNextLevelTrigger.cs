using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Atma.Saving;
using Helpers;
using Helpers.Extensions;
using Timed;
using UnityEngine.InputSystem.Extension;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial
{
    /// <summary>
    /// Just the trigger for the player to load the next level.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class AdvanceToNextLevelTrigger : MonoBehaviour
    {
        [SerializeField, Scene, ShowIf(nameof(ShouldShowSceneToLoad))]
        private string m_sceneToLoad = "2_Death_SCENE";
        [SerializeField, Tag] private string m_playerTag = "Player";
        [SerializeField] private bool m_shouldUseTeleportTransition = true;

        [SerializeField, ShowIf(nameof(m_shouldUseTeleportTransition))] private Animator m_playerTeleportLeaveAnim = null;
        [SerializeField, ShowIf(nameof(m_shouldUseTeleportTransition))] private float m_teleportLeaveAnimLength = 1.0f;
        [SerializeField, ShowIf(nameof(m_shouldUseTeleportTransition))] private string m_restrictedInputMap = "Restricted";

        private SceneLoader m_sceneLoader = null;
        private SaveManager m_saveManager = null;
        private UISoundController m_uiSoundCont = null;
        private bool m_loadStarted = false;


        private void Awake()
        {
            #region Asserts
            if (m_shouldUseTeleportTransition)
            {
                //CustomDebug.AssertSerializeFieldIsNotNull(m_playerTeleportLeaveAnim, nameof(m_playerTeleportLeaveAnim), this);
            }
            #endregion Asserts
        }
        private void Start()
        {
            m_sceneLoader = SceneLoader.GetInstanceSafe(this);
            m_saveManager = SaveManager.instance;
            m_uiSoundCont = UISoundController.GetInstanceSafe(this);

            // Level has been reached. We do this here because we set the level as clear here as well, so it seemed appropriate.
            MarkLevelAsReached();
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (m_loadStarted) { return; }
            if (!other.gameObject.CompareTag(m_playerTag)) { return; }
            PlayerSingleton t_playerSingleton = other.gameObject.GetComponentInParent<PlayerSingleton>();
            if (t_playerSingleton == null) { return; }

            StartAdvance();
        }


        public void StartAdvance()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.GetInstanceSafe();

            MarkLevelAsBeat();

            if (m_shouldUseTeleportTransition)
            {
                // Suspend the player's recording.
                TimedObject t_playerTimedObj = t_playerSingleton.GetComponentSafe<TimedObject>();
                t_playerTimedObj.RequestSuspendRecording();

                m_playerTeleportLeaveAnim.gameObject.SetActive(true);
                m_playerTeleportLeaveAnim.transform.position = t_playerSingleton.transform.position;

                // No longer need to disallow player to have input because the whole object has been turned off by RequestSuspendRecording
                //InputMapStack t_inputMapStack = t_playerSingleton.GetComponentSafe<InputMapStack>();
                //t_inputMapStack.SwitchInputMap(m_restrictedInputMap);

                StartCoroutine(AdvanceToNextLevelAfterDelayCorout());

                // Play level to level transiiton sound.
                m_uiSoundCont.PlayLevelToLevelTransition();
            }
            else
            {
                StartLoadingNextScene();
            }
        }


        private void MarkLevelAsReached() => m_saveManager.SetLevelReached(GetCurSceneSaveIndex());
        private void MarkLevelAsBeat()
        {
            GlobalTimeManager t_timeMan = GlobalTimeManager.instance;
            float t_beatTime = t_timeMan.curTime;
            m_saveManager.SetLevelBeat(GetCurSceneSaveIndex(), t_beatTime);
        }
        private int GetCurSceneSaveIndex()
        {
            LevelsList t_levelsList = LevelsList.GetMasterList();
            return t_levelsList.GetSaveIndexOfActiveScene();
        }
        private void StartLoadingNextScene()
        {
            LevelsList t_levelsList = LevelsList.GetMasterList();
            int t_curSceneSaveIndex = t_levelsList.GetSaveIndexOfActiveScene();
            int t_nextSceneBuildIndex = t_levelsList.GetNextLevelsBuildIndexFromPreviousLevelsSaveIndex(t_curSceneSaveIndex);
            m_sceneLoader.LoadScene(t_nextSceneBuildIndex, m_shouldUseTeleportTransition);
            m_loadStarted = true;
        }
        private IEnumerator AdvanceToNextLevelAfterDelayCorout()
        {
            yield return null;

            GlobalTimeManager.instance.timeScale = 0.0f;

            Invoke(nameof(StartLoadingNextScene), m_teleportLeaveAnimLength);
        }


        private bool ShouldShowSceneToLoad()
        {
            // Only show if there is no level options in the scene.
            return FindObjectOfType<LevelOptions>() == null;
        }
    }
}
