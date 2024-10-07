// Original Authors - Wyatt Senalik

namespace Atma.Saving
{
    /// <summary>
    /// Interface for save data in-case more levels are added later on.
    /// </summary>
    public interface ISaveData
    {
        string playerName { get; set; }
        int saveSlot { get; }

        void SetLevelBeat(int saveIndex);
        int GetBuildIndexOfLevelToLoadForContinue();
        int GetFurthestLevelBeatenSaveIndex();
        bool IsLevelBeat(int saveIndex);

        void SetLevelReached(int saveIndex);
        bool IsLevelReached(int saveIndex);
        void UpdateBeatTimeForLevel(int saveIndex, float beatTime);
    }
}