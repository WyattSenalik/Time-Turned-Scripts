using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using NaughtyAttributes;

using Helpers.Animation.BetterCurve;
using System.Collections;
using Atma.Saving;
using Dialogue.ConvoActions.Branching;
using Helpers.SceneLoading;
using TMPro;
using Helpers;
using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class MainMenuController : MonoBehaviour
    {
        public int curSaveSlotSelected => m_slotIndexForSlotConfirm;

        [SerializeField, Required] private GameObject m_awaitingInputMenuObj = null;
        [SerializeField, Required] private Image m_awaitFadeImage = null;
        [SerializeField, Required] private GameObject m_acceptingInputMenuObj = null;
        [SerializeField, Required] private Image m_acceptFadeImage = null;
        [SerializeField, Required] private PauseMenuNavigator m_acceptMenuNavigator = null;
        [SerializeField, Required] private MainMenuCoreAnimation m_mainMenuCoreAnim = null;
        [SerializeField, Required] private OptionsMenu m_optionsMenu = null;
        [SerializeField, Required] private PauseMenuNavigator m_optionsMenuNavigator = null;
        [SerializeField, Required] private PauseMenuNavigator m_slotSelectMenuNav = null;
        [SerializeField, Required] private PauseMenuNavigator m_slotConfirmMenuNav = null;
        [SerializeField, Required] private PauseMenuNavigator m_deleteSaveConfirmMenuNav = null;
        [SerializeField, Required] private LevelSelectOptionsMenu m_levelSelectMenu = null;
        [SerializeField, Required] private GameObject m_pulseEffectGameObject = null;
        [SerializeField, Required] private GameObject m_uiEventSystemObj = null;
        [SerializeField, Required] private TextMeshProUGUI m_slotConfirmTitleMesh = null;
        [SerializeField, Required] private TextMeshProUGUI m_slotDeleteTitleMesh = null;
        [SerializeField, Scene] private string m_newGameScene = "0_CallToAction_SCENE";

        [SerializeField] private eTranslationKey m_slotKey = eTranslationKey.MENU_SAVE_SLOT;

        [SerializeField] private BetterCurve m_fadeOutCurve = new BetterCurve();
        [SerializeField] private BetterCurve m_fadeInCurve = new BetterCurve();

        private SaveManager m_saveMan = null;
        private ConditionManager m_condMan = null;
        private SceneLoader m_sceneLoader = null;
        private TranslatorFileReader m_translator = null;

        private bool m_hasAnyBeenPressed = false;
        private bool m_isTransitionCoroutActive = false;
        private bool m_isSlotMenuOpen = false;
        private bool m_isSlotConfirmMenuOpen = false;
        private bool m_isDeleteConfirmMenuOpen = false;
        private bool m_isLevelSelectMenuOpen = false;

        private int[] m_optionFramesAcceptMenu = null;
        private int[] m_optionFramesSlotMenu = null;

        private int m_slotIndexForSlotConfirm = -1;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_awaitingInputMenuObj, nameof(m_awaitingInputMenuObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_awaitFadeImage, nameof(m_awaitFadeImage), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_acceptingInputMenuObj, nameof(m_acceptingInputMenuObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_acceptFadeImage, nameof(m_acceptFadeImage), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_acceptMenuNavigator, nameof(m_acceptMenuNavigator), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_mainMenuCoreAnim, nameof(m_mainMenuCoreAnim), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slotSelectMenuNav, nameof(m_slotSelectMenuNav), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slotConfirmMenuNav, nameof(m_slotConfirmMenuNav), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_deleteSaveConfirmMenuNav, nameof(m_deleteSaveConfirmMenuNav), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pulseEffectGameObject, nameof(m_pulseEffectGameObject), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_uiEventSystemObj, nameof(m_uiEventSystemObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slotConfirmTitleMesh, nameof(m_slotConfirmTitleMesh), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slotDeleteTitleMesh, nameof(m_slotDeleteTitleMesh), this);
            #endregion Asserts
            m_hasAnyBeenPressed = false;
        }
        private void Start()
        {
            m_saveMan = SaveManager.instance;
            m_condMan = ConditionManager.instance;
            m_sceneLoader = SceneLoader.instance;
            m_translator = TranslatorFileReader.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_saveMan, this);
            //CustomDebug.AssertSingletonIsNotNull(m_condMan, this);
            //CustomDebug.AssertSingletonIsNotNull(m_sceneLoader, this);
            #endregion Asserts

            m_awaitingInputMenuObj.SetActive(true);
            m_acceptingInputMenuObj.SetActive(false);

            m_optionFramesAcceptMenu = new int[m_acceptMenuNavigator.selectOptions.Count];
            m_optionFramesSlotMenu = new int[m_slotSelectMenuNav.selectOptions.Count];
        }
        private void OnDestroy()
        {
            if (m_acceptMenuNavigator != null)
            {
                m_acceptMenuNavigator.onSelectionUpdated.ToggleSubscription(UpdateCoreAnimationTargetFrame, false);
            }
        }


        public void SelectSaveSlot(int slotIndex)
        {
            m_condMan.UnregisterAllConditions();
            if (DoesSaveDataExist((byte)slotIndex))
            {
                // Open the slot confirm menu
                m_slotIndexForSlotConfirm = slotIndex;
                m_slotConfirmMenuNav.gameObject.SetActive(true);
                m_isSlotConfirmMenuOpen = true;
                ISaveData t_slotSaveData = m_saveMan.PeekSaveData(slotIndex);
                LevelsList t_levelsList = LevelsListManger.instance.levelsList;

                string t_farthestLevelName = t_levelsList.GetFlavorNameForLevelAtSaveIndex(t_levelsList.GetSaveIndexFromBuildIndex(t_slotSaveData.GetBuildIndexOfLevelToLoadForContinue()));
                string t_slotText = m_translator.GetTranslatedTextForKey(m_slotKey);
                m_slotConfirmTitleMesh.text = $"{t_slotText} {slotIndex + 1}: {t_slotSaveData.playerName}\n{t_farthestLevelName}";
                m_slotConfirmTitleMesh.ApplyFontToTextMeshForCurLanguage();

                m_saveMan.LoadSaveData(m_slotIndexForSlotConfirm);
            }
            else
            {
                // New game.
                m_saveMan.CreateNewSaveData((byte)slotIndex);
                m_sceneLoader.LoadScene(m_newGameScene);
                // Disallow mouse input while scene loader is loading scene.
                m_uiEventSystemObj.SetActive(false);
            }
        }
        public void CloseSlotConfirmMenu()
        {
            m_slotIndexForSlotConfirm = -1;
            m_slotConfirmMenuNav.gameObject.SetActive(false);
            m_isSlotConfirmMenuOpen = false;
        }
        public void OpenOptionsMenu()
        {
            m_optionsMenu.OpenOptionsMenu();
        }

        public void OpenSlotMenu()
        {
            m_slotSelectMenuNav.gameObject.SetActive(true);
            m_acceptingInputMenuObj.SetActive(false);
            m_isSlotMenuOpen = true;
        }
        public void CloseSlotMenu()
        {
            m_slotSelectMenuNav.gameObject.SetActive(false);
            m_acceptingInputMenuObj.SetActive(true);
            m_isSlotMenuOpen = false;
        }
        public void UpdateCoreAnimationTargetFrame()
        {
            if (m_isSlotMenuOpen)
            {
                UpdateCoreAnimationTargetFrame(m_optionFramesSlotMenu, m_slotSelectMenuNav);
            }
            else
            {
                UpdateCoreAnimationTargetFrame(m_optionFramesAcceptMenu, m_acceptMenuNavigator);
            }
        }
        public void ContinueSaveFileSlotConfirm()
        {
            int t_continueSceneIndex = m_saveMan.LoadSaveData(m_slotIndexForSlotConfirm);
            m_sceneLoader.LoadScene(t_continueSceneIndex);
            // Disallow mouse input while scene loader is loading scene.
            m_uiEventSystemObj.SetActive(false);
        }
        public void OpenDeleteSaveConfirm()
        {
            string t_playerName = m_saveMan.PeekSaveData(m_slotIndexForSlotConfirm).playerName;
            string t_delteConfirmQuestion = m_translator.GetTranslatedTextForKey(eTranslationKey.MENU_DELETE_CONFIRM_QUESTION);
            t_delteConfirmQuestion = t_delteConfirmQuestion.Replace("X", t_playerName);
            m_slotDeleteTitleMesh.text = t_delteConfirmQuestion;
            m_slotDeleteTitleMesh.ApplyFontToTextMeshForCurLanguage();

            m_deleteSaveConfirmMenuNav.gameObject.SetActive(true);
            m_isDeleteConfirmMenuOpen = true;

            m_slotConfirmMenuNav.gameObject.SetActive(false);
            m_isSlotConfirmMenuOpen = false;
        }
        public void CloseDeleteSaveConfirm()
        {
            m_deleteSaveConfirmMenuNav.gameObject.SetActive(false);
            m_isDeleteConfirmMenuOpen = false;

            m_slotConfirmMenuNav.gameObject.SetActive(true);
            m_isSlotConfirmMenuOpen = true;
        }
        public void ActuallyDeleteSave()
        {
            m_saveMan.DeleteSaveFile(m_slotIndexForSlotConfirm);
            CloseDeleteSaveConfirm();
            CloseSlotConfirmMenu();
        }
        public void OpenLevelSelectMenu()
        {
            m_slotConfirmMenuNav.gameObject.SetActive(false);

            m_levelSelectMenu.OpenMenu();
            m_isLevelSelectMenuOpen = true;
        }
        public void CloseLevelSelectMenu()
        {
            m_slotConfirmMenuNav.gameObject.SetActive(true);

            m_levelSelectMenu.CloseMenu();
            m_isLevelSelectMenuOpen = false;
        }


        private void OnAny(InputValue value)
        {
            if (ShouldIgnoreInput()) { return; }
            if (m_hasAnyBeenPressed) { return; }

            if (value.isPressed)
            {
                m_hasAnyBeenPressed = true;
                SwapToAcceptingInput();
            }
        }
        private void OnNavigate(InputValue value)
        {
            if (ShouldIgnoreInput()) { return; }
            if (!m_hasAnyBeenPressed) { return; }
            Vector2 t_navDir = value.Get<Vector2>();

            if (m_isLevelSelectMenuOpen)
            {
                m_levelSelectMenu.Navigate(t_navDir);
            }
            else if (m_optionsMenu.isOpen)
            {
                m_optionsMenu.Navigate(t_navDir);
            }
            else if (m_isDeleteConfirmMenuOpen)
            {
                m_deleteSaveConfirmMenuNav.Vector2InputForMovingAndHorizontalInput(t_navDir);
            }
            else if (m_isSlotConfirmMenuOpen)
            {
                m_slotConfirmMenuNav.Vector2InputForMovingAndHorizontalInput(t_navDir);
            }
            else if (m_isSlotMenuOpen)
            {
                m_slotSelectMenuNav.Vector2InputForMovingAndHorizontalInput(t_navDir);
            }
            else
            {
                NavigateAcceptMenuWithVector(t_navDir);
            }
        }
        private void OnSubmit(InputValue value)
        {
            if (ShouldIgnoreInput()) { return; }
            if (!m_hasAnyBeenPressed) { return; }

            if (value.isPressed)
            {
                if (m_isLevelSelectMenuOpen)
                {
                    m_levelSelectMenu.Submit();
                }
                else if (m_optionsMenu.isOpen)
                {
                    m_optionsMenu.Submit();
                }
                else if (m_isDeleteConfirmMenuOpen)
                {
                    m_deleteSaveConfirmMenuNav.ChooseCurSelection();
                }
                else if (m_isSlotConfirmMenuOpen)
                {
                    m_slotConfirmMenuNav.ChooseCurSelection();
                }
                else if (m_isSlotMenuOpen)
                {
                    m_slotSelectMenuNav.ChooseCurSelection();
                }
                else
                {
                    m_acceptMenuNavigator.ChooseCurSelection();
                }
            }
        }
        private void OnCancel(InputValue value)
        {
            if (ShouldIgnoreInput()) { return; }
            if (!m_hasAnyBeenPressed) { return; }

            if (value.isPressed)
            {
                if (m_isLevelSelectMenuOpen)
                {
                    CloseLevelSelectMenu();
                }
                else if (m_optionsMenu.isOpen)
                {
                    m_optionsMenu.Cancel();
                }
                else if (m_isDeleteConfirmMenuOpen)
                {
                    CloseDeleteSaveConfirm();
                }
                else if (m_isSlotConfirmMenuOpen)
                {
                    CloseSlotConfirmMenu();
                }
                else if (m_isSlotMenuOpen)
                {
                    CloseSlotMenu();
                }
            }
        }
        private void OnScrollWheel(InputValue value)
        {
            if (ShouldIgnoreInput()) { return; }
            if (!m_hasAnyBeenPressed) { return; }

            if (m_isLevelSelectMenuOpen)
            {
                m_levelSelectMenu.Scroll(value.Get<Vector2>().y);
            }
        }


        private void SwapToAcceptingInput()
        {
            StartCoroutine(TransitionFadeCoroutine());

            m_mainMenuCoreAnim.StopNormalAnimation();
            m_pulseEffectGameObject.SetActive(true);

            m_acceptMenuNavigator.SetSelected(0);
            // Initialize the optionFrames arrays
            InitializeFrameArray(ref m_optionFramesAcceptMenu);
            InitializeFrameArray(ref m_optionFramesSlotMenu);

            m_acceptMenuNavigator.onSelectionUpdated.ToggleSubscription(UpdateCoreAnimationTargetFrame, true);
        }
        private IEnumerator TransitionFadeCoroutine()
        {
            m_isTransitionCoroutActive = true;

            // Fade Out the await.
            float t_timeElapsed = 0.0f;
            float t_endTime = m_fadeOutCurve.GetEndTime();
            Color t_color = m_awaitFadeImage.color;
            while (t_timeElapsed < t_endTime)
            {
                float t_curAlpha = m_fadeOutCurve.Evaluate(t_timeElapsed);
                t_color.a = t_curAlpha;
                m_awaitFadeImage.color = t_color;
                yield return null;
                t_timeElapsed += Time.deltaTime;
            }
            t_color.a = m_fadeOutCurve.Evaluate(t_endTime);
            m_awaitFadeImage.color = t_color;

            m_awaitingInputMenuObj.SetActive(false);
            m_acceptingInputMenuObj.SetActive(true);

            // Fade In the accept.
            t_timeElapsed = 0.0f;
            t_endTime = m_fadeInCurve.GetEndTime();
            t_color = m_acceptFadeImage.color;
            m_acceptFadeImage.gameObject.SetActive(true);
            while (t_timeElapsed < t_endTime)
            {
                float t_curAlpha = m_fadeInCurve.Evaluate(t_timeElapsed);
                t_color.a = t_curAlpha;
                m_acceptFadeImage.color = t_color;
                yield return null;
                t_timeElapsed += Time.deltaTime;
            }
            t_color.a = m_fadeInCurve.Evaluate(t_endTime);
            m_acceptFadeImage.color = t_color;
            m_acceptFadeImage.gameObject.SetActive(false);

            m_isTransitionCoroutActive = false;
        }
        private void NavigateAcceptMenuWithVector(Vector2 navDir)
        {
            m_acceptMenuNavigator.NavigateVertical(navDir.y);
        }
        private bool DoesSaveDataExist(byte saveIndex)
        {
            return m_saveMan.DoesExistingSaveDataExist(saveIndex);
        }
        private void UpdateCoreAnimationTargetFrame(int[] optionFrames, PauseMenuNavigator navigator)
        {
            if (optionFrames[navigator.curSelIndex] >= 0)
            {
                int t_selOptCount = navigator.selectOptions.Count;

                int t_forwardDist = navigator.curSelIndex - navigator.prevSelIndex;
                if (t_forwardDist < 0)
                {
                    t_forwardDist += t_selOptCount;
                }
                int t_backwardDist = t_selOptCount - t_forwardDist;

                bool t_shouldAnimateForward = t_forwardDist < t_backwardDist;
                m_mainMenuCoreAnim.AnimateTowardsFrame(optionFrames[navigator.curSelIndex], t_shouldAnimateForward);
            }
        }
        private void InitializeFrameArray(ref int[] frameArrToInit)
        {
            frameArrToInit[0] = m_mainMenuCoreAnim.curFrame;
            float t_incFramesSlot = ((float)m_mainMenuCoreAnim.frameCount) / frameArrToInit.Length;
            int t_occasionalOffset = 1;
            for (int i = 1; i < frameArrToInit.Length; ++i)
            {
                float t_frameEstimation = m_mainMenuCoreAnim.curFrame + t_incFramesSlot * i;
                int t_frameForOption = Mathf.FloorToInt(t_frameEstimation) % m_mainMenuCoreAnim.frameCount;
                // We do this because coincidently if there are 4 options, it will be obvious that some frames are just copy pasted.
                if (frameArrToInit.Length % 4 == 0)
                {
                    t_frameForOption += t_occasionalOffset;
                    t_occasionalOffset += 1;
                }
                frameArrToInit[i] = t_frameForOption;
            }
        }
        private bool ShouldIgnoreInput()
        {
            if (m_sceneLoader.isLoading)
            {
                return true;
            }
            if (m_isTransitionCoroutActive)
            {
                return true;
            }
            return false;
        }
    }
}