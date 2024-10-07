using System.Collections;
using UnityEngine;

using NaughtyAttributes;

using Atma.Dialogue;
using Helpers;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class InWorldTVAnimator : SingletonMonoBehaviour<InWorldTVAnimator>
    {
        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))] private string m_loopStaticParamName = "LoopStatic";
        [SerializeField, AnimatorParam(nameof(m_animator))] private string m_turnOffParamName = "TurnOff";

        [SerializeField, BoxGroup("Sound")] private UIntReference m_turnOnSoundEvent = null;
        [SerializeField, BoxGroup("Sound")] private UIntReference m_turnOffSoundEvent = null;
        [SerializeField, BoxGroup("Sound")] private UIntReference m_playStaticLoopSoundEvent = null;
        [SerializeField, BoxGroup("Sound")] private UIntReference m_stopStaticLoopSoundEvent = null;

        [SerializeField, Min(0.0f), BoxGroup("Sound")] private float m_delayAfterTurnOnToPlayLoop = 0.85f;

        private ConversationSkipper m_convoSkipper = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_turnOnSoundEvent, nameof(m_turnOnSoundEvent), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_turnOffSoundEvent, nameof(m_turnOffSoundEvent), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_playStaticLoopSoundEvent, nameof(m_playStaticLoopSoundEvent), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_stopStaticLoopSoundEvent, nameof(m_stopStaticLoopSoundEvent), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
        }


        public void TurnOn()
        {
            m_animator.SetTrigger(m_loopStaticParamName);

            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                AkSoundEngine.PostEvent(m_turnOnSoundEvent.value, gameObject);
                StartCoroutine(PlayLoopAfterDelayCorout());
            }
        }
        public void TurnOff()
        {
            m_animator.SetTrigger(m_turnOffParamName);

            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                EndStaticLoop();
                AkSoundEngine.PostEvent(m_turnOffSoundEvent.value, gameObject);
            }
        }
        public void EndStaticLoop()
        {
            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                StopCoroutine(nameof(PlayLoopAfterDelayCorout));
            }
            AkSoundEngine.PostEvent(m_stopStaticLoopSoundEvent.value, gameObject);
        }
        public void StartStaticLoop()
        {
            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                AkSoundEngine.PostEvent(m_playStaticLoopSoundEvent.value, gameObject);
            }
        }


        private IEnumerator PlayLoopAfterDelayCorout()
        {
            yield return new WaitForSeconds(m_delayAfterTurnOnToPlayLoop);
            StartStaticLoop();
        }
    }
}