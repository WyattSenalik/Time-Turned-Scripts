using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Saving
{
    [System.Serializable]
    public sealed class SaveDataV2 : ISaveData
    {
        // Index    |   Level Name                     |   Scene Name
        //----------|----------------------------------|------------------
        // 0        |   First Contact                  |   1_Movement
        // 1        |   The Stopwatch                  |   2_Death
        // 2        |   Under Pressure                 |   3&4_PP_once
        // 3        |   Weight of the Clone            |   6_TheWeightOfTheClone
        // 4        |   Seeing Double                  |   7_ItTakesTwo
        // 5        |   Mini Tortoise Stone            |   7-1(10)_MiniTurtleRock
        // 6        |   One by One                     |   7-2
        // 7        |   Old Friends and Explosions     |   7-2-1_SelfDestructBegins
        // 8        |   Defensive Line                 |   7-3
        // 9        |   Self Trapping                  |   8_SelfTrapping
        // 10       |   Too Few                        |   8-1
        // 11       |   Taking Turns                   |   8-2
        // 12       |   Everything in Its Place        |   8-3
        // 13       |   Box Protector                  |   9_BoxProtector
        // 14       |   Defensive Boxer                |   9-1
        // 15       |   Quick Spurts                   |   9-2
        // 16       |   Pass the Rock                  |   11_Rock_Intro
        // 17       |   Catch!                         |   11-1
        // 18       |   Double Trouble                 |   11-2_Double_Trouble
        // 19       |   Goose Chase                    |   12_GooseChase
        // 20       |   Our First Game of Tag          |   13_OurFirstGameOfTag
        // 21       |   Perilous Chase                 |   13-1_PerilousChase
        // 22       |   Distractions                   |   13-2
        // 23       |   Longing Gazes Across the Abyss |   14_LongingGazesAcrossTheAbyss
        // 24       |   Mirrored Image                 |   14-1_MirroredImage
        // 25       |   Fickle Admirers                |   14-2_FickleAdmirers
        // 26       |   Surrounded                     |   14-3_Surrounded
        // 27       |   Fast Pitch                     |   14-4
        // 28       |   Box Stranding                  |   15_BoxStranding
        // 29       |   Opposite Image                 |   15-1
        // 30       |   Precise Manipulation           |   15-2
        // 31       |   One Shot One Kill              |   16_OneShotOneKill
        // 32       |   Public Execution               |   16-1
        // 33       |   Friendly Fire                  |   16-2
        // 34       |   Prison Riot                    |   16-3_PrisonRiot
        // 35       |   Payback Time                   |   17_GunIntro
        // 36       |   Ring Around the Rosie          |   17-1
        // 37       |   Quick Draw                     |   17-2_QuickDraw
        // 38       |   Share                          |   18_Shoot_Target
        // 39       |   Shooting Range                 |   18-1_ShootingRange
        // 40       |   Runner and Gunner              |   18-2
        // 41       |   Bodyguards                     |   18-3_Bodyguards
        // 42       |   Protecting an Unlikely Ally    |   19_ProtectingAnUnlikelyAlly
        // 43       |   Save Him                       |   19-1
        // 44       |   Crossfire                      |   19-2
        // 45       |   Cull the Weak                  |   19-3
        // 46       |   A Deadly Game of Leapfrog      |   20_ADeadlyGameOfLeapFrog
        // 47       |   Eye of the Storm               |   21_SpringPlatformsOverBullets
        // 48       |   Get to Haulin'                 |   21-1_GetToHaulin
        // 49       |   Alcatraz                       |   21-2_Alcatraz
        // 50       |   Daedalus and Icarus            |   22_DaedalusAndIcarus
        // 51       |   Sliiiide to the Left           |   22-1_SslliideToTheLeft
        // 52       |   Reach the Island               |   22-2
        // 53       |   Three Paths Diverged           |   VR-1
        // 54       |   Formally Known as Tweeter      |   VR-2
        // 55       |   Open the Barrier               |   VR-3
        // 56       |   Pit Vaulting                   |   VR-4
        // 57       |   Backtracking                   |   VR-5 
        // 58       |   FinalLevel [TODO]


        public IReadOnlyList<bool> levelsBeaten => m_levelsBeaten;
        public string playerName { get => m_playerName; set => m_playerName = value; }
        public int saveSlot { get; private set; }

        private readonly bool[] m_levelsBeaten = new bool[59];
        private string m_playerName = "Bob";

        public SaveDataV2(int saveSlot)
        {
            this.saveSlot = saveSlot;
        }
        public SaveDataV2(SaveDataV1 v1SaveData)
        {
            playerName = v1SaveData.playerName;
            saveSlot = v1SaveData.saveSlot;

            // 0, 1, and 2 are the same
            for (int i = 0; i <= 2; ++i)
            {
                m_levelsBeaten[i] = v1SaveData.levelsBeaten[i];
            }
            // 5, 5-1, and 5-2 were removed so everything until 8-3 [index 12] (which was newly added) has a difference of 3.
            for (int i = 3; i <= 11; ++i)
            {
                m_levelsBeaten[i] = v1SaveData.levelsBeaten[i + 3];
            }
            // 8-3 is new, so not beaten
            m_levelsBeaten[12] = false;
            // Since 8-3 was added new, everything after has a difference of 2.
            for (int i = 13; i <= 58; ++i)
            {
                m_levelsBeaten[i] = v1SaveData.levelsBeaten[i + 2];
            }
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