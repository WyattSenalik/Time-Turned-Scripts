using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;
using Atma.Translation;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [CreateAssetMenu(fileName = "new Level List", menuName = "ScriptableObjects/LevelList")]
    public sealed class LevelsList : ScriptableObject
    {
        public IReadOnlyList<LevelListElement> levels => m_levels;

        [SerializeField] private LevelListElement[] m_levels = new LevelListElement[0];


        public LevelListElement GetLevelAtSaveIndex(int saveIndex)
        {
            int t_clampedIndex = Mathf.Clamp(saveIndex, 0, m_levels.Length - 1);
            if (t_clampedIndex != saveIndex)
            {
                #region Logs
                //CustomDebug.LogError($"Given index {saveIndex} was out of bounds and clamped to {t_clampedIndex}");
                #endregion Logs
            }
            return m_levels[t_clampedIndex];
        }
        public string GetFlavorNameForLevelAtSaveIndex(int saveIndex)
        {
            return GetLevelAtSaveIndex(saveIndex).flavorName;
        }
        public int GetSceneBuildIndexForLevelAtSaveIndex(int saveIndex)
        {
            return GetLevelAtSaveIndex(saveIndex).sceneBuildIndex;
        }
        public int GetNextLevelsBuildIndexFromPreviousLevelsSaveIndex(int prevLevelSaveIndex)
        {
            if (prevLevelSaveIndex + 1 < m_levels.Length)
            {
                return GetSceneBuildIndexForLevelAtSaveIndex(prevLevelSaveIndex + 1);
            }
            return GetSceneBuildIndexForLevelAtSaveIndex(prevLevelSaveIndex);
        }
        public int GetSaveIndexFromBuildIndex(int buildIndex)
        {
            for (int i = 0; i < m_levels.Length; ++i)
            {
                LevelListElement t_ele = m_levels[i];
                if (t_ele.sceneBuildIndex == buildIndex)
                {
                    return i;
                } 
            }
            return -1;
        }
        public int GetSaveIndexOfActiveScene()
        {
            int t_activeSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
            return GetSaveIndexFromBuildIndex(t_activeSceneBuildIndex);
        }


        public static LevelsList GetMasterList()
        {
            LevelsList t_loadedList = Resources.Load<LevelsList>("MasterLevelList");
            #region Asserts
            //CustomDebug.AssertIsTrue(t_loadedList != null, $"Failed to load {nameof(LevelsList)} from resources.", nameof(LevelsList));
            #endregion Asserts
            return t_loadedList;
        }



        [Serializable]
        public sealed class LevelListElement
        {
            public string flavorName
            {
                get
                {
                    TranslatorFileReader t_translator = TranslatorFileReader.instance;
                    // The key for level name is the english level name
                    return t_translator.GetTranslatedLevelName(m_flavorName);
                }
            }
            public int sceneBuildIndex => m_sceneBuildIndex;
            public bool shouldPlayCutsceneMusic => m_shouldPlayCutsceneMusic;
            public int musicIntensity => m_musicIntensity;

            [SerializeField] private string m_flavorName = "No Name";
            [SerializeField, Scene, AllowNesting] private int m_sceneBuildIndex = 0;
            [SerializeField] private bool m_shouldPlayCutsceneMusic = false;
            [SerializeField, HideIf(nameof(m_shouldPlayCutsceneMusic)), AllowNesting] private int m_musicIntensity = 0;
        }
    }
}