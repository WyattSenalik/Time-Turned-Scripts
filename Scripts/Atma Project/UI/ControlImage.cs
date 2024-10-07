using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using NaughtyAttributes;
using Atma.Settings;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class ControlImage : MonoBehaviour
    {
        [SerializeField, Required] private Image m_img = null;
        [SerializeField] private eInputAction m_action = eInputAction.AdvanceDialogue;
        [SerializeField, Min(0.0f)] private float m_sizeFactor = 2.0f;

        private GameSettings m_settings = null;
        private PlayerSingleton m_player = null;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_img, nameof(m_img), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_settings = GameSettings.instance;
            m_player = PlayerSingleton.GetInstanceSafe(this);

            UpdateImage();

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void ToggleSubscriptions(bool toggle)
        {
            if (m_settings != null)
            {
                m_settings.onSettingsChanged.ToggleSubscription(UpdateImage, toggle);
            }
            if (m_player != null)
            {
                m_player.onControlsChanged.ToggleSubscription(UpdateImage, toggle);
            }
        }

        private void UpdateImage(PlayerInput playerInp) => UpdateImage();
        private void UpdateImage()
        {
            m_img.sprite = m_action.GetSpriteBasedOnCurrentBindings();
            m_img.SetNativeSize();
            m_img.rectTransform.sizeDelta *= m_sizeFactor;
        }
    }
}