using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Saving
{
    [Serializable]
    public sealed class SaveDataV3 : ISaveData
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


        public IReadOnlyList<SingleLevelSaveData> levelsBeaten => m_levelsBeaten;
        public string playerName { get => m_playerName; set => m_playerName = value; }
        public int saveSlot { get; private set; }

        private readonly SingleLevelSaveData[] m_levelsBeaten = new SingleLevelSaveData[59];
        private string m_playerName = "Bob";


        public SaveDataV3(int saveSlot)
        {
            this.saveSlot = saveSlot;

            for (int i = 0; i < m_levelsBeaten.Length; ++i)
            {
                m_levelsBeaten[i] = new SingleLevelSaveData();
            }
        }
        public SaveDataV3(SaveDataV1 v1SaveData) : this(new SaveDataV2(v1SaveData)) { }
        public SaveDataV3(SaveDataV2 v2SaveData)
        {
            playerName = v2SaveData.playerName;
            saveSlot = v2SaveData.saveSlot;

            // Mark beaten ones
            int t_furthestBeatenIndex = -1;
            for (int i = 0; i < m_levelsBeaten.Length; ++i)
            {
                if (v2SaveData.levelsBeaten[i])
                {
                    m_levelsBeaten[i] = new SingleLevelSaveData(eLevelClearState.Beaten);
                    t_furthestBeatenIndex = i;
                }
                else
                {
                    m_levelsBeaten[i] = new SingleLevelSaveData(eLevelClearState.Unreached);
                }
            }
            // Go through again and mark all unreached as reached if before furthest beaten
            for (int i = 0; i < t_furthestBeatenIndex; ++i)
            {
                if (m_levelsBeaten[i].clearState == eLevelClearState.Unreached)
                {
                    m_levelsBeaten[i].clearState = eLevelClearState.Reached;
                }
            }
        }


        public void SetLevelBeat(int saveIndex)
        {
            m_levelsBeaten[saveIndex].clearState = eLevelClearState.Beaten;
        }
        public int GetBuildIndexOfLevelToLoadForContinue()
        {
            int t_saveIndex = 0;
            for (int i = m_levelsBeaten.Length - 1; i >=0; --i)
            {
                if (m_levelsBeaten[i].clearState == eLevelClearState.Beaten)
                {
                    t_saveIndex = Mathf.Clamp(i + 1, 0, m_levelsBeaten.Length - 1);
                    break;
                }
                else if (m_levelsBeaten[i].clearState == eLevelClearState.Reached)
                {
                    t_saveIndex = i;
                    break;
                }
            }

            LevelsList t_masterKey = LevelsList.GetMasterList();
            return t_masterKey.GetSceneBuildIndexForLevelAtSaveIndex(t_saveIndex);
        }
        public int GetFurthestLevelBeatenSaveIndex()
        {
            for (int i = m_levelsBeaten.Length - 1; i >= 0; --i)
            {
                if (m_levelsBeaten[i].clearState == eLevelClearState.Beaten)
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
            return m_levelsBeaten[saveIndex].clearState == eLevelClearState.Beaten;
        }

        public void SetLevelReached(int saveIndex)
        {
            // Don't set to reached if already beaten, so only set it when unreached.
            if (m_levelsBeaten[saveIndex].clearState == eLevelClearState.Unreached)
            {
                m_levelsBeaten[saveIndex].clearState = eLevelClearState.Reached;
            }
        }
        public bool IsLevelReached(int saveIndex)
        {
            // Also reached when beaten.
            return m_levelsBeaten[saveIndex].clearState != eLevelClearState.Unreached;
        }
        public void UpdateBeatTimeForLevel(int saveIndex, float beatTime)
        {
            if (m_levelsBeaten[saveIndex].fastestBeatTime > beatTime)
            {
                m_levelsBeaten[saveIndex].fastestBeatTime = beatTime;
            }
        }
    }


    [Serializable]
    public sealed class SingleLevelSaveData
    {
        public eLevelClearState clearState { get => m_clearState; set => m_clearState = value; }
        public float fastestBeatTime { get => m_fastestBeatTime; set => m_fastestBeatTime = value; }

        private eLevelClearState m_clearState = eLevelClearState.Unreached;
        private float m_fastestBeatTime = float.PositiveInfinity;

        public SingleLevelSaveData() : this(eLevelClearState.Unreached) { }
        public SingleLevelSaveData(eLevelClearState clearState) : this(clearState, float.PositiveInfinity) { }
        public SingleLevelSaveData(eLevelClearState clearState, float fastestBeatTime)
        {
            m_clearState = clearState;
            m_fastestBeatTime = fastestBeatTime;
        }
    }
    public enum eLevelClearState { Unreached, Reached, Beaten }
}