using UnityEngine;

using Helpers;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class MusicIntensitySetter : SingletonMonoBehaviour<MusicIntensitySetter>
    {
        public enum eIntensityLevel { One, Two, Three, Four }

        [SerializeField, Min(0)] private int m_startIntensityIndex = 0;
        [SerializeField] private UIntReference m_setGameMusicEventID = null;
        [SerializeField] private UIntReference[] m_setIntensityEventIDs = null;
        [SerializeField] private UIntReference m_setFinalCutSceneEventID = null;
        [SerializeField] private UIntReference m_setCreditsEventID = null;

        [SerializeField] private bool m_shouldPlayCutsceneMusic = false;


        private void Start()
        {
            LevelsListManger t_levelsListMan = LevelsListManger.instance;
            LevelsList.LevelListElement t_curLevel = t_levelsListMan.GetLevelForCurScene();
            if (m_setGameMusicEventID != null)
            {
                AkSoundEngine.PostEvent(m_setGameMusicEventID.value, gameObject);
            }
            if (t_curLevel != null)
            {
                if (t_curLevel.shouldPlayCutsceneMusic)
                {
                    SetFinalCutsceneMusic();
                }
                else
                {
                    SetIntensity(t_curLevel.musicIntensity);
                }
            }
            else if (m_shouldPlayCutsceneMusic)
            {
                SetFinalCutsceneMusic();
            }
            else
            {
                SetIntensity(0);
            }
        }


        public void SetIntensity(eIntensityLevel intensityLevel)
        {
            //CustomDebug.LogError($"Old SetIntensity function being used to set intensity {intensityLevel}");
        }
        public void SetIntensity(int indexOfIntensityLevel)
        {
            UIntReference t_eventIDToPost = null;
            if (indexOfIntensityLevel < 0 || indexOfIntensityLevel >= m_setIntensityEventIDs.Length)
            {
                //CustomDebug.LogError($"No intensity level of {indexOfIntensityLevel}.");
            }
            else
            {
                t_eventIDToPost = m_setIntensityEventIDs[indexOfIntensityLevel];
            }

            SafePostEvent(t_eventIDToPost);
        }

        public void SetFinalLevelIntensity() => SetIntensity(m_setIntensityEventIDs.Length - 1);
        public void SetFinalCutsceneMusic() => SafePostEvent(m_setFinalCutSceneEventID);
        public void SetCreditsMusic() => SafePostEvent(m_setCreditsEventID);


        private void SafePostEvent(UIntReference eventToPost)
        {
            if (eventToPost != null)
            {
                AkSoundEngine.PostEvent(eventToPost.value, gameObject);
            }
        }
    }
}