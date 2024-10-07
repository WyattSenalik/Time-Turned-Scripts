using UnityEngine;
using UnityEngine.InputSystem;

using Atma.Settings;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class PickUpIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] m_sprRends = null;
        [SerializeField] private eInputAction m_correspondingAction = eInputAction.Interact;

        private GameSettings m_gameSettings = null;
        private PlayerSingleton m_playerSingleton = null;
        

        private void OnEnable()
        {
            UpdateSprites();
        }
        private void Start()
        {
            m_gameSettings = GameSettings.instance;
            m_playerSingleton = PlayerSingleton.GetInstanceSafe();
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_playerSingleton != null)
            {
                m_playerSingleton.onControlsChanged.ToggleSubscription(OnControlsChanged, cond);
            }
            if (m_gameSettings != null)
            {
                m_gameSettings.onSettingsChanged.ToggleSubscription(UpdateSprites, cond);
            }
        }
        private void OnControlsChanged(PlayerInput playerInp)
        {
            UpdateSprites();
        }
        private void UpdateSprites()
        {
            Sprite t_spr = m_correspondingAction.GetSpriteBasedOnCurrentBindings();
            foreach (SpriteRenderer t_rend in m_sprRends)
            {
                t_rend.sprite = t_spr;
            }
        }
    }
}