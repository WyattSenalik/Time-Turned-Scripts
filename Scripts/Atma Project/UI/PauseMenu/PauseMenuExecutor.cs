using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;

using Helpers.SceneLoading;
using Timed;
using UnityEngine.InputSystem.Extension;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class PauseMenuExecutor : MonoBehaviour
    {
        [SerializeField, Required] private PauseMenuNavigator m_menuNavigator = null;
        [SerializeField, Required] private GameObject m_pauseMenuObj = null;
        [SerializeField, Required] private InputMapStack m_inputMapStack = null;
        [SerializeField, Required] private Animator m_controlsAnimator = null;
        [SerializeField, Required] private GameObject m_menuContent = null;
        [SerializeField] string m_pauseMenuInputMapName = "PauseMenu";
        [SerializeField] string m_restrictedInputMapName = "Restricted";
        [SerializeField, Scene] private string m_mainMenuScene = "MainMenu";
        [SerializeField, AnimatorParam(nameof(m_controlsAnimator))]
        private string m_showBoolAnimParamName = "Show";

        private SceneLoader m_sceneLoader = null;
        private GlobalTimeManager m_timeMan = null;

        private readonly GamePauser m_gamePauser = new GamePauser();


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_menuNavigator, nameof(m_menuNavigator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pauseMenuObj, nameof(m_pauseMenuObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_inputMapStack, nameof(m_inputMapStack), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_controlsAnimator, nameof(m_controlsAnimator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_menuContent, nameof(m_menuContent), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_sceneLoader = SceneLoader.GetInstanceSafe();
            m_timeMan = GlobalTimeManager.instance;
        }
        private void OnDestroy()
        {
            m_gamePauser.CancelRequestToPause();
        }


        public void ToggleMenuVisibility(bool cond)
        {
            m_menuContent.gameObject.SetActive(cond);
        }
        public void Pause()
        {
            m_inputMapStack.SwitchInputMap(m_pauseMenuInputMapName);
            m_gamePauser.RequestToPause();
            m_controlsAnimator.SetBool(m_showBoolAnimParamName, false);
            m_pauseMenuObj.SetActive(true);
        }
        public void Resume()
        {
            m_inputMapStack.PopInputMap(m_pauseMenuInputMapName);
            m_gamePauser.CancelRequestToPause();
            m_pauseMenuObj.SetActive(false);
        }
        public void Restart()
        {
            ResumeBeforeLoadScene();
            if (m_sceneLoader != null)
            {
                m_sceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"No {nameof(SceneLoader)} found. Please add one to the current scene ({SceneManager.GetActiveScene().name}).", this);
                #endregion Logs
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        public void Skip()
        {
            LevelsList t_lvlList = LevelsList.GetMasterList();
            int t_curLvlSaveIndex = t_lvlList.GetSaveIndexOfActiveScene();
            int t_nextSceneBuildIndex = t_lvlList.GetNextLevelsBuildIndexFromPreviousLevelsSaveIndex(t_curLvlSaveIndex);
            SafeSceneLoad(t_nextSceneBuildIndex);
        }
        public void MainMenu()
        {
            // TODO - Add an additional step for "are you sure?"
            SafeSceneLoad(m_mainMenuScene);
        }
        public void Controls()
        {
            bool t_curVal = m_controlsAnimator.GetBool(m_showBoolAnimParamName);
            m_controlsAnimator.SetBool(m_showBoolAnimParamName, !t_curVal);
        }


        private void SafeSceneLoad(int sceneIndex)
        {
            ResumeBeforeLoadScene();
            if (m_sceneLoader != null)
            {
                m_sceneLoader.LoadScene(sceneIndex);
            }
            else
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"No {nameof(SceneLoader)} found. Please add one to the current scene ({SceneManager.GetActiveScene().name}).", this);
                #endregion Logs
                SceneManager.LoadScene(sceneIndex);
            }
        }
        private void SafeSceneLoad(string sceneName)
        {
            ResumeBeforeLoadScene();
            if (m_sceneLoader != null)
            {
                m_sceneLoader.LoadScene(sceneName);
            }
            else
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"No {nameof(SceneLoader)} found. Please add one to the current scene ({SceneManager.GetActiveScene().name}).", this);
                #endregion Logs
                SceneManager.LoadScene(sceneName);
            }
        }
        private void ResumeBeforeLoadScene()
        {
            Resume();
            m_timeMan.timeScale = 0.0f;
            m_inputMapStack.SwitchInputMap(m_restrictedInputMapName);
        }
    }
}