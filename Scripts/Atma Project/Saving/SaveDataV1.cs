using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Saving
{
    [System.Serializable]
    public sealed class SaveDataV1 : ISaveData
    {
        // Index    |   Level Name
        //----------|--------------
        // 0        |   1_Movement
        // 1        |   2_Death
        // 2        |   3&4_PP_once
        // 3        |   5_The_Face
        // 4        |   5-1
        // 5        |   5-2
        // 6        |   6_TheWeightOfTheClone
        // 7        |   7_ItTakesTwo
        // 8        |   7-1(10)_MiniTurtleRock
        // 9        |   7-2
        // 10       |   7-2-1_SelfDestructBegins
        // 11       |   7-3
        // 12       |   8_SelfTrapping
        // 13       |   8-1
        // 14       |   8-2
        // 15       |   9_BoxProtector
        // 16       |   9-1
        // 17       |   9-2
        // 18       |   11_Rock_Intro
        // 19       |   11-1
        // 20       |   11-2_Double_Trouble
        // 21       |   12_GooseChase
        // 22       |   13_OurFirstGameOfTag
        // 23       |   13-1_PerilousChase
        // 24       |   13-2
        // 25       |   14_LongingGazesAcrossTheAbyss
        // 26       |   14-1_MirroredImage
        // 27       |   14-2_FickleAdmirers
        // 28       |   14-3_Surrounded
        // 29       |   14-4
        // 30       |   15_BoxStranding
        // 31       |   15-1
        // 32       |   15-2
        // 33       |   16_OneShotOneKill
        // 34       |   16-1
        // 35       |   16-2
        // 36       |   16-3_PrisonRiot
        // 37       |   17_GunIntro
        // 38       |   17-1
        // 39       |   17-2_QuickDraw
        // 40       |   18_Shoot_Target
        // 41       |   18-1_ShootingRange
        // 42       |   18-2
        // 43       |   18-3_Bodyguards
        // 44       |   19_ProtectingAnUnlikelyAlly
        // 45       |   19-1
        // 46       |   19-2
        // 47       |   19-3
        // 48       |   20_ADeadlyGameOfLeapFrog
        // 49       |   21_SpringPlatformsOverBullets
        // 50       |   21-1_GetToHaulin
        // 51       |   21-2_Alcatraz
        // 52       |   22_DaedalusAndIcarus
        // 53       |   22-1_SslliideToTheLeft
        // 54       |   22-2
        // 55       |   VR-1
        // 56       |   VR-2
        // 57       |   VR-3
        // 58       |   VR-4
        // 59       |   VR-5
        // 60       |   FinalLevel [TODO]
        public IReadOnlyList<bool> levelsBeaten => m_levelsBeaten;
        public string playerName { get => m_playerName; set => m_playerName = value; }
        public int saveSlot { get; private set; }

        private readonly bool[] m_levelsBeaten = new bool[61];
        private string m_playerName = "Bob";

        public SaveDataV1(int saveSlot)
        {
            this.saveSlot = saveSlot;
        } 


        public void SetLevelBeat(int saveIndex)
        {
            m_levelsBeaten[saveIndex] = true;
        }
        public int GetBuildIndexOfLevelToLoadForContinue()
        {
            int t_furthestLevelBeaten = GetFurthestLevelBeatenSaveIndex();
            //CustomDebug.Log($"FurthestLevelBeaten {t_furthestLevelBeaten}", true);
            LevelsList t_masterKey = LevelsList.GetMasterList();
            return t_masterKey.GetNextLevelsBuildIndexFromPreviousLevelsSaveIndex(t_furthestLevelBeaten);
        }
        public int GetFurthestLevelBeatenSaveIndex()
        {
            for (int i = m_levelsBeaten.Length - 1; i >= 0; --i)
            {
                if (m_levelsBeaten[i])
                {
                    return i;
                }
            }
            return -1;
        }
        public bool IsLevelBeat(int saveIndex)
        {
            int t_clampedIndex = Mathf.Clamp(saveIndex, 0, m_levelsBeaten.Length - 1);
            if (saveIndex != t_clampedIndex)
            {
                #region Logs
                //CustomDebug.LogError($"Invalid saveIndex ({saveIndex}) pass to {nameof(IsLevelBeat)}. Allowed range is [0, {m_levelsBeaten.Length - 1}]");
                #endregion Logs
                return false;
            }
            return m_levelsBeaten[saveIndex];
        }


        // V1 and V2 do not support levels being "reached"
        public void SetLevelReached(int saveIndex) { }
        public bool IsLevelReached(int saveIndex) => false;
        // V1 and V2 do not support beat times.
        public void UpdateBeatTimeForLevel(int saveIndex, float beatTime) { }
    }
}