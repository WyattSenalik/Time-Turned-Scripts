using UnityEngine;

using Helpers.Singletons;
using Helpers.InputVisual;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class ImageSchemeManager : DynamicSingletonMonoBehaviourPersistant<ImageSchemeManager>
    {
        public const string KEYBOARD_IMG_SCHEME_RESOURCES_PATH = "KeyboardSprites";
        public const string CONTROLLER_IMG_SCHEME_RESOURCES_PATH = "ControllerSprites";
        public const string MOUSE_IMG_SCHEME_RESOURCES_PATH = "MouseSprites";

        public KeyboardImageScheme keyboardImgScheme
        { 
            get
            {
                if (m_keyboardImgScheme == null)
                {
                    m_keyboardImgScheme = Resources.Load<KeyboardImageScheme>(KEYBOARD_IMG_SCHEME_RESOURCES_PATH);
                    if (m_keyboardImgScheme == null)
                    {
                        //CustomDebug.LogError($"Failed to load KeyboardImageScheme with path {KEYBOARD_IMG_SCHEME_RESOURCES_PATH}");
                    }
                }
                return m_keyboardImgScheme;
            }
        }
        public ControllerImageScheme controllerImgScheme
        {
            get
            {
                if (m_controllerImgScheme == null)
                {
                    m_controllerImgScheme = Resources.Load<ControllerImageScheme>(CONTROLLER_IMG_SCHEME_RESOURCES_PATH);
                    if (m_controllerImgScheme == null)
                    {
                        //CustomDebug.LogError($"Failed to load ControllerImageScheme with path {CONTROLLER_IMG_SCHEME_RESOURCES_PATH}");
                    }
                }
                return m_controllerImgScheme;
            }
        }
        public MouseImageScheme mouseImgScheme
        {
            get
            {
                if (m_mouseImgScheme == null)
                {
                    m_mouseImgScheme = Resources.Load<MouseImageScheme>(MOUSE_IMG_SCHEME_RESOURCES_PATH);
                    if (mouseImgScheme == null)
                    {
                        //CustomDebug.LogError($"Failed to load MouseImageScheme with path {MOUSE_IMG_SCHEME_RESOURCES_PATH}");
                    }
                }
                return m_mouseImgScheme;
            }
        }

        private KeyboardImageScheme m_keyboardImgScheme = null;
        private ControllerImageScheme m_controllerImgScheme = null;
        private MouseImageScheme m_mouseImgScheme = null;
    }
}