using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using NaughtyAttributes;

using Helpers.Extensions;
using Timed;
using UnityEngine.InputSystem.Extension;
using System.Collections;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(InputMapStack))]
    [RequireComponent(typeof(BranchPlayerController))]
    [RequireComponent(typeof(CloneManager))]
    public sealed class RestartLevel : MonoBehaviour
    {
        [SerializeField, Required] private GameObject m_restartUIElement = null;
        [SerializeField, Required] private Slider m_restartSlider = null;
        [SerializeField, Min(0.0f)] private float m_holdTime = 1.5f;
        [SerializeField] private string m_restartingInputMapName = "Restricted";
        [SerializeField] private bool m_isFinalLevel = false;

        private GlobalTimeManager m_timeMan = null;
        private SceneLoader m_sceneLoader = null;
        private InputMapStack m_inputMapStack = null;
        private BranchPlayerController m_playerCont = null;
        private CloneManager m_cloneMan = null;
        private bool m_isHeld = false;
        private bool m_isFinalLevelReloadOccurring = false;


        private void Awake()
        {
            m_inputMapStack = this.GetComponentSafe<InputMapStack>();
            m_playerCont = this.GetComponentSafe<BranchPlayerController>();
            m_cloneMan = this.GetComponentSafe<CloneManager>();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_restartUIElement, nameof(m_restartUIElement), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_restartSlider, nameof(m_restartSlider), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_sceneLoader = SceneLoader.GetInstanceSafe(this);
            m_timeMan = GlobalTimeManager.instance;

            m_restartSlider.minValue = 0.0f;
            m_restartSlider.maxValue = m_holdTime;
        }
        private void Update()
        {
            // Don't restart if still restarting.
            if (m_sceneLoader.isLoading) { return; }
            if (m_isHeld)
            {
                float t_val = m_restartSlider.value + Time.deltaTime;
                m_restartSlider.value = Mathf.Min(t_val, m_restartSlider.maxValue);
                if (m_restartSlider.value >= m_restartSlider.maxValue)
                {
                    ReloadScene();
                    m_isHeld = false;
                    m_restartUIElement.SetActive(false);
                }
            }
        }

        private void ReloadScene()
        {
            if (m_isFinalLevel)
            {
                if (m_isFinalLevelReloadOccurring) { return; }
                m_isFinalLevelReloadOccurring = true;

                // When we restart in the final level, we actually just want to rewind to time 0 and delete all non-disconnected clones.
                m_playerCont.ForceBeginTimeManipulation();
                m_playerCont.PauseNavVelocity();
                // Continue after waiting for a few things.
                StartCoroutine(ReloadSceneFinalLevelCorout());

                IEnumerator ReloadSceneFinalLevelCorout()
                {
                    // Once the transition animation has finished, start rewinding.
                    yield return new WaitUntil(() => !m_playerCont.isTransitionToTimelinePlaying);
                    // Swap input maps here because don't want player to be able to have control of this rewind.
                    m_inputMapStack.SwitchInputMap(m_restartingInputMapName);
                    m_playerCont.RewindNavVelocity();

                    // Once we have rewound to the beginning, end the time manipulation.
                    yield return new WaitUntil(() => m_playerCont.timeRewinder.curTime == 0.0f);
                    // Pop off input map before swapping to the transitioning input map.
                    m_inputMapStack.PopInputMap(m_restartingInputMapName);
                    bool t_sanityCheck = m_playerCont.TryEndTimeManipulation();
                    #region Logs
                    if (!t_sanityCheck)
                    {
                        //CustomDebug.LogError($"Could resume at time 0????");
                    }
                    #endregion Logs
                    // Delete time clones
                    m_cloneMan.DeleteAllConnectedClones();

                    m_isFinalLevelReloadOccurring = false;
                }
            }
            else
            {
                m_inputMapStack.SwitchInputMap(m_restartingInputMapName);
                m_timeMan.timeScale = 0.0f;
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
        }

        #region InputMessages
        private void OnRestart(InputValue inpVal)
        {
            m_isHeld = inpVal.isPressed;
            m_restartUIElement.SetActive(m_isHeld);
            if (m_isHeld)
            {
                m_restartSlider.value = 0.0f;
            }
        }
        private void OnRestartInstant(InputValue inpVal)
        {
            // Don't restart if still restarting.
            if (m_sceneLoader.isLoading) { return; }
            ReloadScene();
        }
        #endregion InputMessages
    }
}
