using UnityEngine;

using NaughtyAttributes;
using UnityEngine.InputSystem;

using Atma.Saving;
using Dialogue;
using Helpers.Events;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma.Settings
{
    [DisallowMultipleComponent]
    public sealed class GameSettings : DynamicSingletonMonoBehaviourPersistant<GameSettings>
    {
        private const string SETTINGS_FILE_PATH = "user.settings";
        private const string REBINDS_PLAYER_PREF_NAME = "Rebinds";
        private const string INPUT_ACTION_ASSET_PATH = "BranchCharge_INPUT_ACTION";

        public IEventPrimer onSettingsChanged => m_onSettingsChanged;
        public IEventPrimer onLanguageChanged => m_onLanguageChanged;

        public eLanguage language => m_settingsSaveData.language;
        public eFullScreenOption fullScreenOption => m_settingsSaveData.fullScreenOption;
        public Vector2Int resolution => m_settingsSaveData.resolution;
        public eTextSpeed textSpeed => m_settingsSaveData.textSpeed;
        public bool skipDialogue => m_settingsSaveData.skipDialogue;
        public float masterVolume => m_settingsSaveData.masterVolume;
        public float soundVolume => m_settingsSaveData.soundVolume;
        public float musicVolume => m_settingsSaveData.musicVolume;
        public bool runInBackground => m_settingsSaveData.runInBackground;

        [SerializeField, Required] private InputActionAsset m_inputActions = null;
        [SerializeField] private MixedEvent m_onSettingsChanged = new MixedEvent();
        [SerializeField] private MixedEvent m_onLanguageChanged = new MixedEvent();

        private SettingsSaveData m_settingsSaveData = null;


        protected override void Awake()
        {
            base.Awake();

            if (m_inputActions == null)
            {
                m_inputActions = Resources.Load<InputActionAsset>(INPUT_ACTION_ASSET_PATH);
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(m_inputActions != null, $"Failed to load input action asset", this);
                #endregion Asserts
            }
            LoadSettingsFromFile();

            ApplyLanguage();
            ApplyWindowSettings();
            ApplyTextSpeed();
            ApplyAudioVolumes();
            ApplyRunInBackground();
        }


        public void SetLanguage(eLanguage newLanguage)
        {
            if (language == newLanguage) { return; }
            m_settingsSaveData.language = newLanguage;
            
            ApplyLanguage();

            SaveSettingsToFile();
        }
        public void SetFullScreen(eFullScreenOption newFullScreenOption)
        {
            if (fullScreenOption == newFullScreenOption) { return; }
            m_settingsSaveData.fullScreenOption = newFullScreenOption;

            ApplyWindowSettings();

            SaveSettingsToFile();
        }
        public void SetResolution(Vector2Int newResolution)
        {
            if (resolution == newResolution) { return; }
            m_settingsSaveData.resolution = newResolution;

            ApplyWindowSettings();

            SaveSettingsToFile();
        }
        public void SetTextSpeed(eTextSpeed newTextSpeed)
        {
            if (textSpeed == newTextSpeed) { return; }
            m_settingsSaveData.textSpeed = newTextSpeed;

            ApplyTextSpeed();

            SaveSettingsToFile();
        }
        public void SetSkipDialogue(bool newSkipDialogue)
        {
            if (skipDialogue == newSkipDialogue) { return; }
            m_settingsSaveData.skipDialogue = newSkipDialogue;

            // No need to apply because things check this if should skip.

            SaveSettingsToFile();
        }
        public void SetMasterVolume(float newMasterVolume)
        {
            if (masterVolume == newMasterVolume) { return; }
            m_settingsSaveData.masterVolume = newMasterVolume;

            ApplyAudioVolumes();

            SaveSettingsToFile();
        }
        public void SetSoundVolume(float newSoundVolume)
        {
            if (soundVolume == newSoundVolume) { return; }
            m_settingsSaveData.soundVolume = newSoundVolume;

            ApplyAudioVolumes();

            SaveSettingsToFile();
        }
        public void SetMusicVolume(float newMusicVolume)
        {
            if (musicVolume == newMusicVolume) { return; }
            m_settingsSaveData.musicVolume = newMusicVolume;

            ApplyAudioVolumes();

            SaveSettingsToFile();
        }
        public void SetRunInBackground(bool newRunInBackground)
        {
            if (runInBackground == newRunInBackground) { return; }
            m_settingsSaveData.runInBackground = newRunInBackground;

            ApplyRunInBackground();

            SaveSettingsToFile();
        }
        public void SaveControlMappings()
        {
            string t_rebindsJSON = GetCurrentControlMappings();
            PlayerPrefs.SetString(REBINDS_PLAYER_PREF_NAME, t_rebindsJSON);
            m_onSettingsChanged.Invoke();
        }
        public string GetCurrentControlMappings()
        {
            return m_inputActions.SaveBindingOverridesAsJson();
        }


        private void LoadSettingsFromFile()
        {
            if (SaveSystem.CheckIfDataExists(SETTINGS_FILE_PATH))
            {
                byte[] t_loadedSaveData = SaveSystem.LoadData<byte[]>(SETTINGS_FILE_PATH);
                m_settingsSaveData = new SettingsSaveData(t_loadedSaveData);
            }
            else
            {
                m_settingsSaveData = SettingsSaveData.GetDefault();
            }

            string t_rebindsJSON = PlayerPrefs.GetString(REBINDS_PLAYER_PREF_NAME);
            if (!string.IsNullOrEmpty(t_rebindsJSON))
            {
                m_inputActions.LoadBindingOverridesFromJson(t_rebindsJSON);
            }
        }
        private void SaveSettingsToFile()
        {
            SaveSystem.SaveData(m_settingsSaveData.ConvertToSavableData(), SETTINGS_FILE_PATH);
            m_onSettingsChanged.Invoke();
        }

        private void ApplyWindowSettings()
        {
            Screen.SetResolution(resolution.x, resolution.y, fullScreenOption.GetCorrespondingMode());
        }
        private void ApplyLanguage()
        {
            // TODO?
            m_onLanguageChanged.Invoke();
        }
        private void ApplyTextSpeed()
        {
            DialogueSettings.charDelayMultiplier = textSpeed.ToCharacterDelayMultiplier();
        }
        private void ApplyAudioVolumes()
        {
            VolumeController.instance.UpdateVolumes();   
        }
        private void ApplyRunInBackground()
        {
            Application.runInBackground = runInBackground;

            AkWwiseInitializationSettings t_wwiseInitSettings = AkWwiseInitializationSettings.Instance;
            t_wwiseInitSettings.AdvancedSettings.m_SuspendAudioDuringFocusLoss = !runInBackground;
        }
    }
}