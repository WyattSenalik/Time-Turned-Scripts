using Helpers.Singletons;

namespace Dialogue.Sound
{
    public sealed class DialogueEventSoundManager : DynamicSingletonMonoBehaviourPersistant<DialogueEventSoundManager>
    {
        protected override void Awake()
        {
            base.Awake();
            if (!gameObject.TryGetComponent(out AkGameObj _))
            {
                gameObject.AddComponent<AkGameObj>();
            }
        }

        public void PlayDialogueSound(uint wwiseEventID)
        {
            AkSoundEngine.PostEvent(wwiseEventID, gameObject);
        }
    }
}