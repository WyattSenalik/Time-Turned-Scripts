using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using NaughtyAttributes;

using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class NoRemapInputImage : MonoBehaviour
    {
        [SerializeField, Required] private Image m_img = null;
        [SerializeField] private Sprite m_keyboardSpr = null;
        [SerializeField] private Sprite m_controllerSpr = null;

        private PlayerSingleton m_playerSingleton = null;
        private PlayerInput m_playerInp = null;


        private void Start()
        {
            m_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_playerInp = m_playerSingleton.GetComponentSafe<PlayerInput>();

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
        }
        private void OnControlsChanged(PlayerInput playerInp)
        {
            UpdateImage();
        }
        private void UpdateImage()
        {
            Sprite t_spr = DetermineCurrentSpriteToUse();
            m_img.sprite = t_spr;
            if (t_spr != null)
            {
                float t_imgWidth = 2 * t_spr.rect.width;
                float t_imgHeight = 2 * t_spr.rect.height;
                m_img.rectTransform.sizeDelta = new Vector2(t_imgWidth, t_imgHeight);
            }

            // Hide img if there is no sprite. Show it if there is.
            m_img.enabled = m_img.sprite != null;
        }
        private Sprite DetermineCurrentSpriteToUse()
        {
            if (m_playerInp.IsCurrentControlSchemeController())
            {
                return m_controllerSpr;
            }
            else
            {
                return m_keyboardSpr;
            }
        }
    }
}