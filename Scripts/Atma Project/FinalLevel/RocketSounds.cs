using UnityEngine;

using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class RocketSounds : MonoBehaviour
    {
        public SoundRecorder stage1SoundRecorder => m_stage1SoundRecorder;
        public SoundRecorder stage2SoundRecorder => m_stage2SoundRecorder;
        public SoundRecorder stage3SoundRecorder => m_stage3SoundRecorder;

        [SerializeField] private UIntReference m_platformRaiseSoundID = null;
        [SerializeField] private UIntReference m_fireworkSoundID = null;

        [SerializeField] private SoundRecorder m_stage1SoundRecorder = null;
        [SerializeField] private SoundRecorder m_stage2SoundRecorder = null;
        [SerializeField] private SoundRecorder m_stage3SoundRecorder = null;


        public void PlayPlatformRaiseSound() => PostEventWithID(m_platformRaiseSoundID);
        public void PlayFireworkSound() => PostEventWithID(m_fireworkSoundID);

        public void PlayStage1Sound() => PlayPuffSound();
        public void PlayPuffSound() => m_stage1SoundRecorder.Play();
        public void PlayStage2Sound() => PlayRumbleSound();
        public void PlayRumbleSound() => m_stage2SoundRecorder.Play();
        public void PlayStage3Sound() => PlayLiftoffSound();
        public void PlayLiftoffSound() => m_stage3SoundRecorder.Play();

        public bool IsStage2SoundPlaying() => IsStage2SoundPlaying();
        public bool IsRumbleSoundPlaying() => m_stage2SoundRecorder.IsPlaying();
        public bool IsStage3SoundPlaying() => IsLiftoffSoundPlaying();
        public bool IsLiftoffSoundPlaying() => m_stage3SoundRecorder.IsPlaying();



        private void PostEventWithID(UIntReference soundIDRef)
        {
            if (soundIDRef != null)
            {
                AkSoundEngine.PostEvent(soundIDRef.value, gameObject);
            }
        }
    }
}