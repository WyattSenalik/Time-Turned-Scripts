using UnityEngine;

using NaughtyAttributes;

using Atma.Saving;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class LeaveFinalLevel : MonoBehaviour
    {
        [SerializeField, Scene] private string m_levelToLoad = "7-2-1_Credits_SCENE";

        [SerializeField] private bool m_markLevelAsBeat = true;
        [SerializeField] private bool m_isRoomToRoom = true;

        private SceneLoader m_sceneLoader = null;
        private SaveManager m_saveManager = null;


        private void Start()
        {
            m_sceneLoader = SceneLoader.GetInstanceSafe(this);
            m_saveManager = SaveManager.instance;
        }


        public void Leave()
        {
            if (enabled)
            {
                if (m_markLevelAsBeat)
                {
                    MarkLevelAsBeat();
                }
                m_sceneLoader.LoadScene(m_levelToLoad, m_isRoomToRoom);
            }
        }


        private void MarkLevelAsBeat()
        {
            LevelsList t_levelsList = LevelsList.GetMasterList();
            int t_curSceneSaveIndex = t_levelsList.GetSaveIndexOfActiveScene();
            m_saveManager.SetLevelBeat(t_curSceneSaveIndex, 0.0f);
        }
    }
}