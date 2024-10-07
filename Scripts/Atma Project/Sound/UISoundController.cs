using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class UISoundController : SingletonMonoBehaviourPersistant<UISoundController>
    {
        [SerializeField, BoxGroup("Play")] private UIntReference m_menuSelectSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_menuHighlightSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_typingSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_actionRestricted = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playTimeCloneLoopSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playTimeCloneInSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playTimeFreezeSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playTimeResumeSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playObtainStopwatchSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playShowChargeSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playTimerAppearSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playRegainChargesSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playLoseStopwatchSoundID = null;
        [SerializeField, BoxGroup("Play")] private UIntReference m_playStopwatchRewindSoundID = null;

        [SerializeField, BoxGroup("Stop")] private UIntReference m_stopTimeCloneLoopSoundID = null;
        [SerializeField, BoxGroup("Stop")] private UIntReference m_stopTimeCloneInSoundID = null;
        [SerializeField, BoxGroup("Stop")] private UIntReference m_stopTimeFreezeSoundID = null;
        [SerializeField, BoxGroup("Stop")] private UIntReference m_stopTimeResumeSoundID = null;
        [SerializeField, BoxGroup("Stop")] private UIntReference m_stopRegainChargesSoundID = null;
        [SerializeField, BoxGroup("Stop")] private UIntReference m_stopStopwatchRewindSoundID = null;

        [SerializeField, BoxGroup("Transitions")] private UIntReference m_levelToLevelTransitionSoundEventID = null;
        [SerializeField, BoxGroup("Transitions")] private UIntReference m_toMenuTransitionSoundEventID = null;

        private int m_frameSelectSoundWasPlayed = int.MinValue;



        public void PlayMenuSelectSound()
        {
            m_frameSelectSoundWasPlayed = Time.frameCount;
            PostEventWithID(m_menuSelectSoundID);
        }
        public void PlayMenuHighlightSound()
        {
            // Don't play highlight and select on same frame (happens when opening/closing sub menu)
            if (Time.frameCount != m_frameSelectSoundWasPlayed)
            {
                PostEventWithID(m_menuHighlightSoundID);
            }
        }
        public void PlayTypingSound() => PostEventWithID(m_typingSoundID);
        public void PlayActionRestrictedSound() => PostEventWithID(m_actionRestricted);
        public void PlayTimeCloneLoopSound() => PostEventWithID(m_playTimeCloneLoopSoundID);
        public void PlayTimeCloneInSound() => PostEventWithID(m_playTimeCloneInSoundID);
        public void PlayTimeFreezeSound() => PostEventWithID(m_playTimeFreezeSoundID);
        public void PlayTimeResumeSound() => PostEventWithID(m_playTimeResumeSoundID);
        public void PlayObtainStopwatchSound() => PostEventWithID(m_playObtainStopwatchSoundID);
        public void PlayShowChargeSound() => PostEventWithID(m_playShowChargeSoundID);
        public void PlayTimerAppearSound() => PostEventWithID(m_playTimerAppearSoundID);
        public void PlayRegainChargesSound() => PostEventWithID(m_playRegainChargesSoundID);
        public void PlayLoseStopwatchSound() => PostEventWithID(m_playLoseStopwatchSoundID);
        public void PlayRewindSound() => PostEventWithID(m_playStopwatchRewindSoundID);

        public void StopTimeCloneLoopSound() => PostEventWithID(m_stopTimeCloneLoopSoundID);
        public void StopTimeCloneInSound() => PostEventWithID(m_stopTimeCloneInSoundID);
        public void StopTimeFreezeSound() => PostEventWithID(m_stopTimeFreezeSoundID);
        public void StopTimeResumeSound() => PostEventWithID(m_stopTimeResumeSoundID);
        public void StopRegainChargesSound() => PostEventWithID(m_stopRegainChargesSoundID);
        public void StopRewindSound() => PostEventWithID(m_stopStopwatchRewindSoundID);

        public void PlayLevelToLevelTransition() => PostEventWithID(m_levelToLevelTransitionSoundEventID);
        public void PlayToMenuTransition() => PostEventWithID(m_toMenuTransitionSoundEventID);

        private void PostEventWithID(UIntReference soundIDRef)
        {
            if (soundIDRef != null)
            {
                AkSoundEngine.PostEvent(soundIDRef.value, gameObject);
            }
        }
    }
}