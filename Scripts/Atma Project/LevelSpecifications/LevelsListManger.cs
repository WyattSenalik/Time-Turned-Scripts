using System.Collections.Generic;
using UnityEngine;

using Helpers.Singletons;

using static Atma.LevelsList;
using UnityEngine.SceneManagement;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class LevelsListManger : DynamicSingletonMonoBehaviourPersistant<LevelsListManger>
    {
        public IReadOnlyList<LevelListElement> levels => levelsList.levels;
        public LevelsList levelsList
        {
            get
            {
                LoadLevelsListIfNeeded();
                return m_levelsList;
            }
        }

        private LevelsList m_levelsList = null;


        private void Start()
        {
            LoadLevelsListIfNeeded();
        }


        public LevelListElement GetLevelAtIndex(int listIndex) => levelsList.GetLevelAtSaveIndex(listIndex);
        public string GetFlavorNameForLevelAtIndex(int listIndex) => levelsList.GetFlavorNameForLevelAtSaveIndex(listIndex);
        public int GetSceneBuildIndexForLevelAtIndex(int listIndex) => levelsList.GetSceneBuildIndexForLevelAtSaveIndex(listIndex);
        public int GetLevelIndexFromBuildIndex(int buildIndex) => levelsList.GetSaveIndexFromBuildIndex(buildIndex);
        public LevelListElement GetLevelForCurScene()
        {
            int t_curBuildIndex = SceneManager.GetActiveScene().buildIndex;
            int t_correspondingLevelIndex = GetLevelIndexFromBuildIndex(t_curBuildIndex);
            if (t_correspondingLevelIndex < 0)
            {
                return null;
            }
            return GetLevelAtIndex(t_correspondingLevelIndex);
        }


        private void LoadLevelsListIfNeeded()
        {
            if (m_levelsList == null)
            {
                m_levelsList = GetMasterList();
                if (m_levelsList == null)
                {
                    //CustomDebug.LogErrorForComponent($"Failed to load levels list from resources", this);
                }
            }
        }
    }
}