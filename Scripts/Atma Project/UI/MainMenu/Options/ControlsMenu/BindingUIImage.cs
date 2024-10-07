using UnityEngine;
using UnityEngine.UI;

using NaughtyAttributes;
using UnityEngine.InputSystem.Samples.RebindUI;

using Helpers.InputVisual;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RebindActionUI))]
    public sealed class BindingUIImage : MonoBehaviour
    {
        [SerializeField, Required] private Image m_img = null;
        [SerializeField, Required] private ControllerImageScheme m_controllerImgScheme = null;

        private RebindActionUI m_rebindActionUI = null;


        private void Awake()
        {
            m_rebindActionUI = GetComponent<RebindActionUI>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_rebindActionUI, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_img, nameof(m_img), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_controllerImgScheme, nameof(m_controllerImgScheme), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_rebindActionUI.updateBindingUIEvent.AddListener(UpdateBindingUI);
            m_rebindActionUI.startRebindEvent.AddListener(OnStartBinding);
            UpdateBindingUI();
        }
        private void OnDestroy()
        {
            if (m_rebindActionUI != null)
            {
                m_rebindActionUI.updateBindingUIEvent.RemoveListener(UpdateBindingUI);
                m_rebindActionUI.startRebindEvent.RemoveListener(OnStartBinding);
            }
        }


        private void OnStartBinding(RebindActionUI actionUI, InputActionRebindingExtensions.RebindingOperation rebindOp)
        {
            m_rebindActionUI.bindingText.enabled = false;

            m_img.enabled = true;
            m_img.sprite = m_controllerImgScheme.unknownButton;
        }
        private void UpdateBindingUI()
        {
            UpdateBindingUI(m_rebindActionUI, m_rebindActionUI.bindingText?.text, "", "");
        }
        private void UpdateBindingUI(RebindActionUI actionUI, string displayString, string deviceLayoutName, string controlPath)
        {
            if (ControlBindingHelpers.TryGetControllerSpriteForBinding(displayString, out Sprite t_bindingSprite, m_controllerImgScheme))
            {
                m_rebindActionUI.bindingText.enabled = false;

                m_img.enabled = true;
                m_img.sprite = t_bindingSprite;
            }
            else
            {
                m_rebindActionUI.bindingText.enabled = true;

                m_img.enabled = false;
            }
        }
        
    }
}