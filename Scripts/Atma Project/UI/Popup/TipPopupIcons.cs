using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using Atma.Settings;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class TipPopupIcons : MonoBehaviour
    {
        [SerializeField] private bool m_ignoreIfKeyboardMouse = false;
        [SerializeField] private Image[] m_imagesToDisableDueToOverride = new Image[0];
        [SerializeField] private ActionAndImage[] m_actionsAndImages = new ActionAndImage[0];

        [SerializeField, Min(0.0f)] private float m_maxWidth = 80.0f;
        [SerializeField, Min(0.0f)] private float m_maxHeight = 80.0f;

        private GameSettings m_gameSettings = null;
        private PlayerSingleton m_playerSingleton = null;
        private PlayerInput m_playerInput = null;


        private void OnEnable()
        {
            if (m_playerInput != null)
            {
                UpdateImages();
            }
        }
        private void Start()
        {
            m_gameSettings = GameSettings.instance;
            m_playerSingleton = PlayerSingleton.GetInstanceSafe();
            m_playerInput = m_playerSingleton.GetComponentSafe<PlayerInput>();
            UpdateImages();
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
                m_gameSettings.onSettingsChanged.ToggleSubscription(UpdateImages, cond);
            }
        }
        private void OnControlsChanged(PlayerInput playerInp)
        {
            UpdateImages();
        }
        private void UpdateImages()
        {
            if (!m_ignoreIfKeyboardMouse)
            {
                foreach (ActionAndImage t_aAndI in m_actionsAndImages)
                {
                    t_aAndI.UpdateImage(m_maxWidth, m_maxHeight);
                }
            }
            else if (m_ignoreIfKeyboardMouse)
            {
                bool t_isController = m_playerInput.IsCurrentControlSchemeController();
                if (t_isController)
                {
                    foreach (ActionAndImage t_aAndI in m_actionsAndImages)
                    {
                        t_aAndI.UpdateImage(m_maxWidth, m_maxHeight);
                    }
                    foreach (Image t_imgsToDisableOnOverride in m_imagesToDisableDueToOverride)
                    {
                        t_imgsToDisableOnOverride.enabled = false;
                    }
                }
                else
                {
                    foreach (ActionAndImage t_aAndI in m_actionsAndImages)
                    {
                        t_aAndI.HideImage();
                    }
                    foreach (Image t_imgsToDisableOnOverride in m_imagesToDisableDueToOverride)
                    {
                        t_imgsToDisableOnOverride.enabled = true;
                    }
                }
            }
        }
        private bool DoAnyActionsHaveOverride()
        {
            foreach (ActionAndImage t_aAndI in m_actionsAndImages)
            {
                if (t_aAndI.HasBindingOverride())
                {
                    return true;
                }
            }
            return false;
        }


        [Serializable]
        public sealed class ActionAndImage
        {
            public eInputAction action => m_action;
            public Image img => m_img;


            [SerializeField] private eInputAction m_action = eInputAction.Interact;
            [SerializeField] private Image m_img = null;


            public void UpdateImage(float maxWidth, float maxHeight)
            {
                Sprite t_spr = action.GetSpriteBasedOnCurrentBindings();
                img.sprite = t_spr;
                float t_imgWidth = 2 * t_spr.rect.width;
                float t_imgHeight = 2 * t_spr.rect.height;
                t_imgWidth = Mathf.Min(t_imgWidth, maxWidth);
                t_imgHeight = Mathf.Min(t_imgHeight, maxHeight);
                img.rectTransform.sizeDelta = new Vector2(t_imgWidth, t_imgHeight);
                img.enabled = true;
            }
            public void HideImage()
            {
                img.enabled = false;
            }
            public bool HasBindingOverride() => action.HasBindingOverride();
        }
    }
}