using UnityEngine;
using UnityEngine.SceneManagement;

using Atma.Settings;
using Helpers;
using Helpers.Singletons;
using Dialogue;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class VolumeController : DynamicSingletonMonoBehaviourPersistant<VolumeController>
    {
        private const string MASTER_VOLUME_RTPC_NAME = "Master_Volume";
        private const string MUSIC_VOLUME_RTPC_NAME = "Music_Volume";
        private const string SOUND_VOLUME_RTPC_NAME = "SFX_Volume";
        private const string DIALOGUE_VOLUME_RTPC_NAME = "Dialogue_Volume";
        private const string UI_VOLUME_RTPC_NAME = "UI_Volume";

        private GamePauseController pauseCont
        {
            get
            {
                if (m_pauseCont == null)
                {
                    m_pauseCont = GamePauseController.instance;
                }
                return m_pauseCont;
            }
        }

        private GamePauseController m_pauseCont = null;
        private bool m_dialogueTextIsBeingWritten = false;


        private void Start()
        {
            if (m_pauseCont == null)
            {
                m_pauseCont = GamePauseController.instance;
            }
            ToggleSubscriptions(true);
            TryToSubscribeToConversation();
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public void UpdateVolumes()
        {
            GameSettings t_settings = GameSettings.instance;

            float t_sfxVolume = t_settings.soundVolume;
            if (pauseCont.isPaused)
            {
                t_sfxVolume = 0.0f;
            }

            // Sound
            float t_dialogueVolume = t_sfxVolume;
            float t_soundVolume = t_sfxVolume;
            float t_uiVolume = t_settings.soundVolume;
            AkSoundEngine.SetRTPCValue(DIALOGUE_VOLUME_RTPC_NAME, t_dialogueVolume);
            AkSoundEngine.SetRTPCValue(SOUND_VOLUME_RTPC_NAME, t_soundVolume);
            AkSoundEngine.SetRTPCValue(UI_VOLUME_RTPC_NAME, t_uiVolume);
            // Music
            float t_musicVolume = t_settings.musicVolume;
            AkSoundEngine.SetRTPCValue(MUSIC_VOLUME_RTPC_NAME, t_musicVolume);
            // Master
            float t_masterVolume = t_settings.masterVolume;
            AkSoundEngine.SetRTPCValue(MASTER_VOLUME_RTPC_NAME, t_masterVolume);
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_pauseCont != null)
            {
                m_pauseCont.onResume.ToggleSubscription(UpdateVolumes, cond);
                m_pauseCont.onPause.ToggleSubscription(UpdateVolumes, cond);
            }
            if (cond)
            {
                SceneManager.activeSceneChanged += OnSceneChanged;
            }
            else
            {
                SceneManager.activeSceneChanged -= OnSceneChanged;
            }
        }
        private void OnSceneChanged(Scene current, Scene next)
        {
            TryToSubscribeToConversation();
        }
        private void TryToSubscribeToConversation()
        {
            ConversationDriver t_convoDriver = ConversationDriver.instance;
            if (t_convoDriver != null)
            {
                DialogueBoxDisplay t_dialogueBoxDisplay = t_convoDriver.dialogueBoxDisplay;
                t_dialogueBoxDisplay.onShowAnimFin.ToggleSubscription(OnDialogueBoxShown, true);
                t_dialogueBoxDisplay.onHide.ToggleSubscription(OnDialogueBoxHide, true);
            }
        }
        private void OnDialogueBoxShown()
        {
            m_dialogueTextIsBeingWritten = true;
            UpdateVolumes();
        }
        private void OnDialogueBoxHide()
        {
            m_dialogueTextIsBeingWritten = false;
            UpdateVolumes();
        }
    }
}