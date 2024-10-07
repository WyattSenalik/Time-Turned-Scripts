using System;
using System.Collections.Generic;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma.Settings
{
    public sealed class SettingsSaveData
    {
        public eLanguage language { get; set; } = eLanguage.EN;
        public eFullScreenOption fullScreenOption { get; set; } = eFullScreenOption.Borderless;
        public Vector2Int resolution { get; set; } = new Vector2Int(1920, 1080);
        public eTextSpeed textSpeed { get; set; } = eTextSpeed.Normal;
        public bool skipDialogue { get; set; } = false;
        public float masterVolume { get; set; } = 80.0f;
        public float soundVolume { get; set; } = 80.0f;
        public float musicVolume { get; set; } = 80.0f;
        public bool runInBackground { get; set; } = false;

        public SettingsSaveData(eLanguage language, eFullScreenOption fullScreenOption, Vector2Int resolution, eTextSpeed textSpeed, bool skipDialogue, float masterVolume, float soundVolume, float musicVolume, bool runInBackground)
        {
            this.language = language;
            this.fullScreenOption = fullScreenOption;
            this.resolution = resolution;
            this.textSpeed = textSpeed;
            this.skipDialogue = skipDialogue;
            this.masterVolume = masterVolume;
            this.soundVolume = soundVolume;
            this.musicVolume = musicVolume;
            this.runInBackground = runInBackground;
        }

        public SettingsSaveData(byte[] loadedSaveData)
        {
            if (loadedSaveData == null) { return; }

            // Language
            if (loadedSaveData.Length <= 0) { return; }
            language = (eLanguage)loadedSaveData[0];

            // IsFullScreen
            if (loadedSaveData.Length <= 1) { return; }
            fullScreenOption = (eFullScreenOption)loadedSaveData[1];

            // TextSpeed
            if (loadedSaveData.Length <= 2) { return; }
            textSpeed = (eTextSpeed)loadedSaveData[2];

            // SkipDialogue
            if (loadedSaveData.Length <= 3) { return; }
            skipDialogue = loadedSaveData[3] > 0;

            // Resolution
            if (loadedSaveData.Length <= 7) { return; }
            byte[] t_resolutionXBytes = new byte[4] { loadedSaveData[4], loadedSaveData[5], loadedSaveData[6], loadedSaveData[7] };
            int t_resX = BitConverter.ToInt32(t_resolutionXBytes);

            if (loadedSaveData.Length <= 11) { return; }
            byte[] t_resolutionYBytes = new byte[4] { loadedSaveData[8], loadedSaveData[9], loadedSaveData[10], loadedSaveData[11] };
            int t_resY = BitConverter.ToInt32(t_resolutionYBytes);

            resolution = new Vector2Int(t_resX, t_resY);

            // Master Volume
            if (loadedSaveData.Length <= 15) { return; }
            byte[] t_masterVolBytes = new byte[4] { loadedSaveData[12], loadedSaveData[13], loadedSaveData[14], loadedSaveData[15] };
            masterVolume = BitConverter.ToSingle(t_masterVolBytes);

            // Sound Volume
            if (loadedSaveData.Length <= 19) { return; }
            byte[] t_soundVolBytes = new byte[4] { loadedSaveData[16], loadedSaveData[17], loadedSaveData[18], loadedSaveData[19] };
            soundVolume = BitConverter.ToSingle(t_soundVolBytes);

            // Music Volume
            if (loadedSaveData.Length <= 23) { return; }
            byte[] t_musicVolBytes = new byte[4] { loadedSaveData[20], loadedSaveData[21], loadedSaveData[22], loadedSaveData[23] };
            musicVolume = BitConverter.ToSingle(t_musicVolBytes);

            // Run In Background
            if (loadedSaveData.Length <= 24) { return; }
            runInBackground = loadedSaveData[24] > 0;
        }

        public byte[] ConvertToSavableData()
        {
            List<byte> t_data = new List<byte>()
            {
                (byte)language,                 // 0
                (byte)fullScreenOption,         // 1
                (byte)textSpeed,                // 2
                (byte)(skipDialogue ? 1 : 0),   // 3
            };
            t_data.AddRange(BitConverter.GetBytes(resolution.x));   // 4-7
            t_data.AddRange(BitConverter.GetBytes(resolution.y));   // 8-11
            t_data.AddRange(BitConverter.GetBytes(masterVolume));   // 12-15
            t_data.AddRange(BitConverter.GetBytes(soundVolume));    // 16-19
            t_data.AddRange(BitConverter.GetBytes(musicVolume));    // 20-23
            t_data.Add((byte)(runInBackground ? 1 : 0));

            return t_data.ToArray();
        }


        public static SettingsSaveData GetDefault() => new SettingsSaveData(eLanguage.EN, eFullScreenOption.Borderless, new Vector2Int(1920, 1080), eTextSpeed.Normal, false, 80.0f, 80.0f, 80.0f, false);
    }
}