using System;
using UnityEngine;
using UnityEngine.Audio;

using NaughtyAttributes;
using TMPro;
using UnityEngine.UI;
using Atma.Settings;
using Helpers;
using UnityEngine.EventSystems;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class SoundMusicMenuOption : PauseMenuOption
    {
        private float curVolume
        {
            get => m_curVolume;
            set
            {
                if (m_curVolume == value) { return; }

                m_curVolume = Mathf.Clamp(value, m_minVolume, m_maxVolume);
                UpdateSliderToMatchVolume();
                ApplyCurrentVolume();
            }
        }

        [SerializeField, Required, ShowIf(nameof(ShouldShowCheckSFXWwiseEvent))] private UIntReference m_checkSFXWwiseEvent = null;
        [SerializeField, Required] private Slider m_slider = null;
        [SerializeField] private float m_minVolume = -80.0f;
        [SerializeField] private float m_maxVolume = 20.0f;
        [SerializeField, Min(0.0f)] private float m_horiInputSpeed = 1.0f;
        [SerializeField] private eSoundMusicOption m_isSoundOrMusic = eSoundMusicOption.Sound;

        private float m_curVolume = 0.0f;
        private float m_recentHoriInputVal = 0.0f;
        private bool m_isMouseInteractingWithSlider = false;
        private bool m_wasPrevHoriInputValZero = true;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_slider, nameof(m_slider), this);

            if (ShouldShowCheckSFXWwiseEvent())
            {
                //CustomDebug.AssertSerializeFieldIsNotNull(m_checkSFXWwiseEvent, nameof(m_checkSFXWwiseEvent), this);
            }
            #endregion Asserts
        }
        private void OnEnable()
        {
            // Setting m_curVolume instead of curVolume because curVolume updates GameSettings, which we don't need to do when pulling data from them.
            switch (m_isSoundOrMusic)
            {
                case eSoundMusicOption.Sound:
                    m_curVolume = GameSettings.instance.soundVolume;
                    UpdateSliderToMatchVolume();
                    break;
                case eSoundMusicOption.Music:
                    m_curVolume = GameSettings.instance.musicVolume;
                    UpdateSliderToMatchVolume();
                    break;
                case eSoundMusicOption.Master:
                    m_curVolume = GameSettings.instance.masterVolume;
                    UpdateSliderToMatchVolume();
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_isSoundOrMusic, this);
                    break;
            }
        }
        private void Start()
        {
            m_slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        private void OnDestroy()
        {
            m_slider?.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
        private void Update()
        {
            curVolume += m_recentHoriInputVal * m_horiInputSpeed * Time.unscaledDeltaTime;
            if (m_recentHoriInputVal == 0.0f && !m_wasPrevHoriInputValZero && m_isSoundOrMusic != eSoundMusicOption.Music)
            {
                AkSoundEngine.PostEvent(m_checkSFXWwiseEvent.value, gameObject);
            }

            m_wasPrevHoriInputValZero = m_recentHoriInputVal == 0.0f;
        }


        public override void OnHighlighted()
        {
            base.OnHighlighted();
        }
        public override void OnUnhighlighted()
        {
            base.OnUnhighlighted();

            m_recentHoriInputVal = 0.0f;
        }
        public override void OnChosen()
        {
            base.OnChosen();
        }
        public override void OnHorizontalInput(float inputValue)
        {
            base.OnHorizontalInput(inputValue);

            m_recentHoriInputVal = inputValue;
        }
        public override void OnHorizontalInputZeroed()
        {
            base.OnHorizontalInputZeroed();

            m_recentHoriInputVal = 0.0f;
        }

        public void OnSliderBeginDrag(BaseEventData eventData)
        {
            m_isMouseInteractingWithSlider = true;
        }
        public void OnSliderEndDrag(BaseEventData eventData)
        {
            m_isMouseInteractingWithSlider = false;
            if (m_isSoundOrMusic != eSoundMusicOption.Music && m_recentHoriInputVal == 0.0f)
            {
                AkSoundEngine.PostEvent(m_checkSFXWwiseEvent.value, gameObject);
            }
        }
        

        private void OnSliderValueChanged(float sliderValue)
        {
            // Don't use curVolume here because that calls ApplyCurrentVolume which sets the slider again. And this was called by the slider value being changed.
            m_curVolume = sliderValue * (m_maxVolume - m_minVolume) + m_minVolume;
            ApplyCurrentVolume();
        }


        private void UpdateSliderToMatchVolume()
        {
            m_slider.value = (curVolume - m_minVolume) / (m_maxVolume - m_minVolume);
        }
        private void ApplyCurrentVolume()
        {
            // Save to GameSettings
            switch (m_isSoundOrMusic)
            {
                case eSoundMusicOption.Sound:
                    GameSettings.instance.SetSoundVolume(m_curVolume);
                    if (!m_isMouseInteractingWithSlider && m_recentHoriInputVal == 0.0f)
                    {
                        AkSoundEngine.PostEvent(m_checkSFXWwiseEvent.value, gameObject);
                    }
                    break;
                case eSoundMusicOption.Music:
                    GameSettings.instance.SetMusicVolume(m_curVolume);
                    break;
                case eSoundMusicOption.Master:
                    GameSettings.instance.SetMasterVolume(m_curVolume);
                    if (!m_isMouseInteractingWithSlider && m_recentHoriInputVal == 0.0f)
                    {
                        AkSoundEngine.PostEvent(m_checkSFXWwiseEvent.value, gameObject);
                    }
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_isSoundOrMusic, this);
                    break;
            }
        }

        private bool ShouldShowCheckSFXWwiseEvent() => m_isSoundOrMusic != eSoundMusicOption.Music;


        public enum eSoundMusicOption { Sound, Music, Master }
    }
}