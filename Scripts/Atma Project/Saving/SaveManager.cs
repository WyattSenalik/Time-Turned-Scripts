using Dialogue;
using Helpers.Singletons;

using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Saving
{
    [DisallowMultipleComponent]
    public sealed class SaveManager : DynamicSingletonMonoBehaviourPersistant<SaveManager>
    {
        private const string PLAYER_NAME_STRING_REF_FILE_NAME = "PlayerName_DYNAMIC_STRREF";

        private const string SAVE_FOLDER_NAME = "saves";
        private const string SAVE_DATA_FILE_NAME = "saveFile";
        private const string SAVE_DATA_FILE_EXTENSION = "savedata";

        public ISaveData curSaveData
        {
            get
            {
                if (m_curSaveData == null)
                {
                    //CustomDebug.LogWarningForComponent($"New save data made because none existed.", this);
                    CreateNewSaveData(0);
                }
                return m_curSaveData;
            }
        }

        private ISaveData m_curSaveData = null;


        protected override void OnSingletonCreated()
        {
            base.OnSingletonCreated();

            SaveSystem.CreateMainSaveFolder(SAVE_FOLDER_NAME);
            SaveSystem.TransferDataFromOldSaveLocation(new string[3]
            {
                GetSaveDataPath(0),
                GetSaveDataPath(1),
                GetSaveDataPath(2)
            });
        }


        /// <summary>
        /// Loads save data from file and returns the index of the scene to load to continue the game.
        /// </summary>
        public int LoadSaveData(int saveSlot)
        {
            m_curSaveData = ReadSaveDataFromFile(saveSlot);
            // Load the player name reference and set it.
            LoadPlayerNameRef().SetDynamicStringValue(m_curSaveData.playerName);

            return m_curSaveData.GetBuildIndexOfLevelToLoadForContinue();
        }
        public void CreateNewSaveData(byte saveIndex)
        {
            m_curSaveData = new SaveDataV3(saveIndex);
        }
        /// <summary>
        /// <see cref="SaveDataV3"/> for what the save level index should be for each level.
        /// </summary>
        public void SetLevelBeat(int saveLevelIndex, float beatTime)
        {
            curSaveData.SetLevelBeat(saveLevelIndex);
            curSaveData.UpdateBeatTimeForLevel(saveLevelIndex, beatTime);
            WriteCurrentSaveDataToFile(curSaveData.saveSlot);

            switch (saveLevelIndex)
            {
                case 7:
                    SteamAchievementManager.instance.GrantAchievement(AchievementId.LEAVE_DIRIGO);
                    break;
                case 19:
                    SteamAchievementManager.instance.GrantAchievement(AchievementId.GOOSE_CHASE);
                    break;
                case 35:
                    SteamAchievementManager.instance.GrantAchievement(AchievementId.FIRST_SHOT);
                    break;
                case 52:
                    SteamAchievementManager.instance.GrantAchievement(AchievementId.REACH_ISLAND);
                    break;
                case 58:
                    SteamAchievementManager.instance.GrantAchievement(AchievementId.DEFEAT_EVIL);
                    break;
            }
            
            if (curSaveData.GetFurthestLevelBeatenSaveIndex() == 58)
            {
                bool t_allBeaten = true;
                for ( int i = 0; i < 58; ++i )
                {
                    if (!curSaveData.IsLevelBeat(i))
                    {
                        t_allBeaten = false;
                        break;
                    }
                }
                if (t_allBeaten)
                {
                    SteamAchievementManager.instance.GrantAchievement(AchievementId.TIME_LORD);
                }
            }
        }
        public void SetLevelReached(int saveLevelIndex)
        {
            curSaveData.SetLevelReached(saveLevelIndex);
            WriteCurrentSaveDataToFile(curSaveData.saveSlot);
        }
        public bool DoesExistingSaveDataExist(int saveSlotIndex)
        {
            return SaveSystem.CheckIfDataExists(GetSaveDataPath(saveSlotIndex));
        }
        public ISaveData PeekSaveData(int saveSlot)
        {
            return ReadSaveDataFromFile(saveSlot);
        }
        public void DeleteSaveFile(int saveSlot)
        {
            SaveSystem.DeleteData(GetSaveDataPath(saveSlot));
        }
        public bool DoesHaveActiveSaveData()
        {
            return m_curSaveData != null;
        }

        private ISaveData ReadSaveDataFromFile(int saveSlot)
        {
            object t_loadedData = SaveSystem.LoadData<object>(GetSaveDataPath(saveSlot));
            if (t_loadedData == null)
            {
                #region Logs
                //CustomDebug.LogWarningForComponent("Failed to read data from file. Creating empty data in its place.", this);
                #endregion Logs
                return new SaveDataV3(saveSlot);
            }
            else if (t_loadedData is SaveDataV3 t_v3Data)
            {
                return t_v3Data;
            }
            else if (t_loadedData is SaveDataV2 t_v2Data)
            {
                return new SaveDataV3(t_v2Data);
            }
            else if (t_loadedData is SaveDataV1 t_v1Data)
            {
                return new SaveDataV3(t_v1Data);
            }
            else
            {
                #region Logs
                //CustomDebug.LogErrorForComponent("Failed to cast loaded save data. Creating empty data in its place.", this);
                #endregion Logs
                return new SaveDataV3(saveSlot);
            }
        }
        private void WriteCurrentSaveDataToFile(int saveSlot)
        {
            curSaveData.playerName = LoadPlayerNameRef().GetCurrentDynamicStringValue();
            SaveSystem.CreateMainSaveFolder(SAVE_FOLDER_NAME);
            SaveSystem.SaveData(curSaveData, GetSaveDataPath(saveSlot));
        }
        private string GetSaveDataPath(int saveSlot)
        {
            return $"{SAVE_FOLDER_NAME}/{SAVE_DATA_FILE_NAME}_{saveSlot}.{SAVE_DATA_FILE_EXTENSION}";
        }
        private DynamicStringReference LoadPlayerNameRef()
        {
            return Resources.Load<DynamicStringReference>(PLAYER_NAME_STRING_REF_FILE_NAME);
        }
    }
}