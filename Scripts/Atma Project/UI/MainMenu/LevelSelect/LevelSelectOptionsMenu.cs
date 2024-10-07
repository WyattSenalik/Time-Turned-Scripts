using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Atma.Saving;
using Helpers.UI;

using static Atma.LevelsList;
using Atma.Settings;

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class LevelSelectOptionsMenu : MonoBehaviour
    {
        public bool isOpen => gameObject.activeInHierarchy;
        public LevelsListManger lvlListMan
        {
            get
            {
                if (m_lvlListMan == null)
                {
                    m_lvlListMan = LevelsListManger.instance;
                }
                return m_lvlListMan;
            }
        }
        public bool isParentMoving { get; private set; } = false;

        private UISoundController soundCont
        {
            get
            {
                if (m_soundCont == null)
                {
                    m_soundCont = UISoundController.GetInstanceSafe();
                }
                return m_soundCont;
            }
        }
        private GameSettings settings
        {
            get
            {
                if (m_settings == null)
                {
                    m_settings = GameSettings.instance;
                }
                return m_settings;
            }
        }

        [SerializeField] private MainMenuController m_mainMenuCont = null;
        [SerializeField, Required] private RectTransform m_levelOptionParent = null;
        [SerializeField, Required] private GameObject m_levelOptionPrefab = null;
        [SerializeField] private Vector2 m_firstOptionPos = new Vector2(-768, 384);
        [SerializeField] private float m_horizontalSpacing = 384;
        [SerializeField] private float m_verticalSpacing = -256;
        [SerializeField] private int m_amountBeforeNewLine = 5;
        [SerializeField, Min(0)] private float m_vertDiffBeforeParentMoves = 413;
        [SerializeField, Min(0.01f)] private float m_parentLerpAmount = 0.5f;
        [SerializeField, Min(0.01f)] private float m_scrollStrength = 1f;

        [SerializeField] private MenuMover m_menuMover = new MenuMover();

        private UISoundController m_soundCont = null;
        private GameSettings m_settings = null;
        private LevelsListManger m_lvlListMan = null;
        private ISaveData m_curSlotSaveData = null;

        private LevelSelectOption[] m_spawnedOptions = null;
        private bool m_areOptionsSpawned = false;
        private int m_curSelectedIndex = 0;

        private float m_parentCurDesiredY = 0.0f;
        private bool m_isScrolling = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_levelOptionParent, nameof(m_levelOptionParent), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_levelOptionPrefab, nameof(m_levelOptionPrefab), this);
            #endregion Asserts
        }
        private void Start()
        {
            if (m_soundCont == null)
            {
                m_soundCont = UISoundController.GetInstanceSafe();
            }

            InstantiateLevelOptionsIfNeeded();
            m_menuMover.onMove.ToggleSubscription(OnMenuMove, true);
            settings.onLanguageChanged.ToggleSubscription(UpdateAllLevelSelectOptions, true);
        }
        private void OnDestroy()
        {
            m_menuMover.onMove.ToggleSubscription(OnMenuMove, false);
            if (m_settings != null)
            {
                m_settings.onLanguageChanged.ToggleSubscription(UpdateAllLevelSelectOptions, false);
            }
        }
        private void Update()
        {
            m_menuMover.Update();

            if (!m_isScrolling)
            {
                float t_diff = m_levelOptionParent.anchoredPosition.y - m_parentCurDesiredY;
                if (Mathf.Abs(t_diff) != 0.0f)
                {
                    float t_newY = Mathf.Lerp(m_levelOptionParent.anchoredPosition.y, m_parentCurDesiredY, m_parentLerpAmount * Time.unscaledDeltaTime);
                    t_newY = Mathf.Round(t_newY);
                    if (t_newY == m_levelOptionParent.anchoredPosition.y)
                    {
                        t_newY = m_parentCurDesiredY;
                    }
                    m_levelOptionParent.anchoredPosition = new Vector2(0, t_newY);
                    isParentMoving = true;
                }
                else
                {
                    m_levelOptionParent.anchoredPosition = new Vector2(0, m_parentCurDesiredY);
                    isParentMoving = false;
                }
            }
            else
            {
                isParentMoving = false;
            }
        }


        public void OpenMenu()
        {
            gameObject.SetActive(true);

            // Assumes there is save data
            SaveManager t_saveMan = SaveManager.instance;
            if (t_saveMan.DoesHaveActiveSaveData())
            {
                m_curSlotSaveData = t_saveMan.curSaveData;
            }
            else if (m_mainMenuCont != null)
            {
                m_curSlotSaveData = t_saveMan.PeekSaveData(m_mainMenuCont.curSaveSlotSelected);
            }
            else
            {
                if (Application.isEditor)
                {
                    m_curSlotSaveData = t_saveMan.curSaveData;
                }
                else
                {
                    //CustomDebug.LogErrorForComponent($"No active save data and main menu not specified", this);
                    return;
                }
            }
            
            if (!InstantiateLevelOptionsIfNeeded())
            {
                UpdateAllLevelSelectOptions();
            }

            m_isScrolling = false;

            m_curSelectedIndex = -1;
            SelectFurthestLevel();
            UpdateOptionsParentPosition(true);
        }
        public void CloseMenu()
        {
            gameObject.SetActive(false);
            UnhighlightAllLevels();
        }
        public void Navigate(Vector2 navDir)
        {
            m_menuMover.curNavDir = navDir;
            m_isScrolling = false;
        }
        public void Submit()
        {
            soundCont.PlayMenuSelectSound();

            SceneLoader t_sceneLoader = SceneLoader.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_sceneLoader, this);
            #endregion Asserts
            int t_buildIndex = m_lvlListMan.GetSceneBuildIndexForLevelAtIndex(m_curSelectedIndex);
            t_sceneLoader.LoadScene(t_buildIndex);
        }
        public void Scroll(float scrollInp)
        {
            m_isScrolling = true;

            float t_newY = m_levelOptionParent.anchoredPosition.y - (scrollInp * m_scrollStrength);
            t_newY = Mathf.Clamp(t_newY, 0.0f, GetDesiredMinimumParentPos());
            m_levelOptionParent.anchoredPosition = new Vector2(0.0f, t_newY);
        }
        public void SetSelected(LevelSelectOption optToSel)
        {
            int t_index = Array.IndexOf(m_spawnedOptions, optToSel);
            SelectOptionAtIndex(t_index);
        }


        /// <summary>
        /// </summary>
        /// <returns>True when it was needed, false if it wasn't.</returns>
        private bool InstantiateLevelOptionsIfNeeded()
        {
            if (m_areOptionsSpawned) { return false; }
            m_areOptionsSpawned = true;

            IReadOnlyList<LevelListElement> t_levelList = lvlListMan.levels;
            int t_amLevels = t_levelList.Count;
            m_spawnedOptions = new LevelSelectOption[t_amLevels];

            int t_horiCounter = 0;
            int t_vertCounter = 0;
            for (int i = 0; i < t_amLevels; ++i)
            {
                LevelListElement t_curLevelEle = t_levelList[i];

                // Spawn UI element
                Vector2 t_spawnPos = m_firstOptionPos + new Vector2(t_horiCounter * m_horizontalSpacing, t_vertCounter * m_verticalSpacing);
                GameObject t_spawnedObj = Instantiate(m_levelOptionPrefab, m_levelOptionParent);
                RectTransform t_rectTrans = t_spawnedObj.GetComponent<RectTransform>();
                LevelSelectOption t_levelSelectOption = t_spawnedObj.GetComponent<LevelSelectOption>();
                #region Asserts
                //CustomDebug.AssertComponentOnOtherIsNotNull(t_rectTrans, t_spawnedObj, this);
                //CustomDebug.AssertComponentOnOtherIsNotNull(t_levelSelectOption, t_spawnedObj, this);
                #endregion Asserts
                t_rectTrans.localPosition = t_spawnPos;
                m_spawnedOptions[i] = t_levelSelectOption;

                UpdateSingleLevelSelectOption(t_levelSelectOption, i, t_curLevelEle);

                if (++t_horiCounter >= m_amountBeforeNewLine)
                {
                    ++t_vertCounter;
                    t_horiCounter = 0;
                }
            }
            return true;
        }
        private void UpdateAllLevelSelectOptions()
        {
            for (int i = 0; i < m_spawnedOptions.Length; ++i)
            {
                LevelSelectOption t_curOption = m_spawnedOptions[i];
                LevelListElement t_curLevelEle = lvlListMan.GetLevelAtIndex(i);
                UpdateSingleLevelSelectOption(t_curOption, i, t_curLevelEle);
            }
        }
        private void UpdateSingleLevelSelectOption(LevelSelectOption levelSelectOption, int curIndex, LevelListElement correspondingLevelEle)
        {
            levelSelectOption.levelSelOptMenu = this;
            if (DeveloperConstants.ALLOW_LOAD_EVERY_LEVEL)
            {
                levelSelectOption.SetCleared(correspondingLevelEle.flavorName);
                return;
            }

            if (m_curSlotSaveData.IsLevelBeat(curIndex))
            {
                levelSelectOption.SetCleared(correspondingLevelEle.flavorName);
            }
            else if (m_curSlotSaveData.IsLevelReached(curIndex))
            {
                levelSelectOption.SetReachedButNotCleared(correspondingLevelEle.flavorName);
            }
            else if (m_curSlotSaveData.IsLevelBeat(curIndex - 1))
            {
                levelSelectOption.SetReachedButNotCleared(correspondingLevelEle.flavorName);
            }
            else
            {
                levelSelectOption.SetUnreached();
            }
        }
        private void SelectFurthestLevel()
        {
            int t_levelIndex = 0;
            int t_amLevels = lvlListMan.levels.Count;
            for (int i = t_amLevels - 1; i >= 0; --i)
            {
                if (m_curSlotSaveData.IsLevelBeat(i))
                {
                    t_levelIndex = Mathf.Clamp(i + 1, 0, t_amLevels - 1);
                    break;
                }
                else if (m_curSlotSaveData.IsLevelReached(i))
                {
                    t_levelIndex = i;
                    break;
                }
                else if (m_curSlotSaveData.IsLevelBeat(i - 1))
                {
                    t_levelIndex = i;
                    break;
                }
            }

            SelectOptionAtIndex(t_levelIndex);
        }
        private void UnhighlightAllLevels()
        {
            foreach (LevelSelectOption t_levelOpt in m_spawnedOptions)
            {
                t_levelOpt.Unhighlight();
            }
        }
        private void SelectOptionAtIndex(int newIndex)
        {
            // Don't select if its already the current selection.
            if (newIndex == m_curSelectedIndex) { return; }

            LevelSelectOption t_newOptToSelect = m_spawnedOptions[newIndex];
            // Don't select if new option is not selectable.
            if (!t_newOptToSelect.isSelectable) { return; }

            if (m_curSelectedIndex >= 0)
            {
                m_spawnedOptions[m_curSelectedIndex].Unhighlight();
            }
            m_curSelectedIndex = newIndex;
            t_newOptToSelect.Highlight();

            soundCont.PlayMenuHighlightSound();
        }
        private void UpdateOptionsParentPosition(bool shouldSnap = false)
        {
            m_parentCurDesiredY = GetDesiredParentPosition();
            if (shouldSnap)
            {
                m_levelOptionParent.anchoredPosition = new Vector2(0, m_parentCurDesiredY);
            }
        }
        private float GetDesiredParentPosition()
        {
            LevelSelectOption t_curSelOpt = m_spawnedOptions[m_curSelectedIndex];
            float t_curSelOptY = t_curSelOpt.rectTrans.anchoredPosition.y;
            float t_sum = t_curSelOptY + m_parentCurDesiredY;
            float t_absSum = Mathf.Abs(t_sum);

            if (t_absSum > m_vertDiffBeforeParentMoves)
            {
                float t_neededChangeAmount = t_absSum - m_vertDiffBeforeParentMoves;
                int t_changeIncrements = Mathf.FloorToInt(t_neededChangeAmount / m_verticalSpacing);
                if (t_sum < 0)
                {
                    // Cur opt is lower, so move everything up
                    return m_parentCurDesiredY +  m_verticalSpacing * t_changeIncrements;
                }
                else
                {
                    // Cur opt is higher, so move everything down
                    return m_parentCurDesiredY - m_verticalSpacing * t_changeIncrements;
                }
            }
            return m_parentCurDesiredY;
        }
        private float GetDesiredMinimumParentPos()
        {
            LevelSelectOption t_lastOpt = m_spawnedOptions[^1];
            float t_curSelOptY = t_lastOpt.rectTrans.anchoredPosition.y;
            float t_absPos = Mathf.Abs(t_curSelOptY);

            if (t_absPos > m_vertDiffBeforeParentMoves)
            {
                float t_neededChangeAmount = t_absPos - m_vertDiffBeforeParentMoves;
                int t_changeIncrements = Mathf.FloorToInt(t_neededChangeAmount / m_verticalSpacing);
                // Cur opt is lower, so move everything up
                return m_verticalSpacing * t_changeIncrements;
            }
            return 0.0f;
        }


        private void OnMenuMove(Vector2 navDir)
        {
            int t_newIndex;
            if (Mathf.Abs(navDir.x) > Mathf.Abs(navDir.y))
            {
                // Move horizontally
                if (navDir.x > 0)
                {
                    t_newIndex = m_curSelectedIndex + 1;
                }
                else
                {
                    t_newIndex = m_curSelectedIndex - 1;
                }
            }
            else
            {
                // Move vertically
                if (navDir.y > 0)
                {
                    t_newIndex = m_curSelectedIndex - m_amountBeforeNewLine;
                }
                else
                {
                    t_newIndex = m_curSelectedIndex + m_amountBeforeNewLine;
                }
            }
            t_newIndex = Mathf.Clamp(t_newIndex, 0, m_spawnedOptions.Length - 1);

            SelectOptionAtIndex(t_newIndex);
            UpdateOptionsParentPosition();
        }
    }
}