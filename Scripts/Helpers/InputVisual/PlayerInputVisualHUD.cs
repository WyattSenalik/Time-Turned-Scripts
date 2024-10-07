using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Helpers.InputVisual
{
    /// <summary>
    /// Visualizes the input of a player for controller-based inputs using UI Images.
    /// </summary>
    public class PlayerInputVisualHUD : MonoBehaviour
    {
        // ScriptableObject that holds Sprites of the controls
        // for the controller that is being used.
        [SerializeField] [Required] private ControllerImageScheme m_controllerImgScheme = null;
        // UI Images
        [SerializeField] [Required] private Image m_buttonEastImg = null;
        [SerializeField] [Required] private Image m_buttonWestImg = null;
        [SerializeField] [Required] private Image m_buttonSouthImg = null;
        [SerializeField] [Required] private Image m_buttonNorthImg = null;
        [SerializeField] [Required] private Image m_dPadLeftImg = null;
        [SerializeField] [Required] private Image m_dPadRightImg = null;
        [SerializeField] [Required] private Image m_dPadDownImg = null;
        [SerializeField] [Required] private Image m_dPadUpImg = null;
        [SerializeField] [Required] private Image m_leftStickImg = null;
        [SerializeField] [Required] private Image m_rightStickImg = null;
        [SerializeField] [Required] private Image m_leftShoulderImg = null;
        [SerializeField] [Required] private Image m_rightShoulderImg = null;
        [SerializeField] [Required] private Image m_leftTriggerImg = null;
        [SerializeField] [Required] private Image m_rightTriggerImg = null;
        [SerializeField] [Required] private Image m_selectImg = null;
        [SerializeField] [Required] private Image m_startImg = null;


        // Called 1st
        // Foreign Initilization
        private void Start()
        {
            // Update the images to the control scheme if different than the default.
            ChangeImagesToReflectImageScheme();
            // Make sure all the images start off unhighlighted, since no input
            // has been made before the game starts.
            UnhighlightAllImages();
        }


        /// <summary>
        /// Updates the control scheme to be for the specified control scheme.
        /// </summary>
        public void SetControllerImageScheme(ControllerImageScheme imageScheme)
        {
            m_controllerImgScheme = imageScheme;
            ChangeImagesToReflectImageScheme();
        }


        /// <summary>
        /// Updates the sprites of the Images to display the Sprites
        /// located inside the controller image scheme.
        ///
        /// Pre Conditions - Assumes none of the Images are null.
        /// Post Conditions - All Images' Sprites will be updated to
        /// match the corresponding Sprite in the current ControllerImageScheme.
        /// </summary>
        private void ChangeImagesToReflectImageScheme()
        {
            m_buttonEastImg.sprite = m_controllerImgScheme.buttons.buttonEast;
            m_buttonWestImg.sprite = m_controllerImgScheme.buttons.buttonWest;
            m_buttonSouthImg.sprite = m_controllerImgScheme.buttons.buttonSouth;
            m_buttonNorthImg.sprite = m_controllerImgScheme.buttons.buttonNorth;

            m_dPadLeftImg.sprite = m_controllerImgScheme.dPad.left;
            m_dPadRightImg.sprite = m_controllerImgScheme.dPad.right;
            m_dPadDownImg.sprite = m_controllerImgScheme.dPad.down;
            m_dPadUpImg.sprite = m_controllerImgScheme.dPad.up;

            m_leftStickImg.sprite = m_controllerImgScheme.leftStick;
            m_rightStickImg.sprite = m_controllerImgScheme.rightStick;

            m_leftShoulderImg.sprite = m_controllerImgScheme.leftShoulder;
            m_rightShoulderImg.sprite = m_controllerImgScheme.rightShoulder;

            m_leftTriggerImg.sprite = m_controllerImgScheme.leftTrigger;
            m_rightTriggerImg.sprite = m_controllerImgScheme.rightTrigger;

            m_selectImg.sprite = m_controllerImgScheme.select;
            m_startImg.sprite = m_controllerImgScheme.start;
        }
        /// <summary>
        /// Unhighlights all the images.
        /// </summary>
        private void UnhighlightAllImages()
        {
            UnhighlightImage(m_buttonEastImg);
            UnhighlightImage(m_buttonWestImg);
            UnhighlightImage(m_buttonSouthImg);
            UnhighlightImage(m_buttonNorthImg);

            UnhighlightImage(m_dPadLeftImg);
            UnhighlightImage(m_dPadRightImg);
            UnhighlightImage(m_dPadDownImg);
            UnhighlightImage(m_dPadUpImg);

            UnhighlightImage(m_leftStickImg);
            UnhighlightImage(m_rightStickImg);

            UnhighlightImage(m_leftShoulderImg);
            UnhighlightImage(m_rightShoulderImg);

            UnhighlightImage(m_leftTriggerImg);
            UnhighlightImage(m_rightTriggerImg);

            UnhighlightImage(m_selectImg);
            UnhighlightImage(m_startImg);
        }
        /// <summary>
        /// Unhighlights the Image by darkening it.
        /// 
        /// Pre Conditions - Assumes the given Image is not null.
        /// Post Conditions - Sets the Image's brightness value (HSV's V) to be 0.5f.
        /// </summary>
        /// <param name="img">Image to unhighlight.</param>
        private void UnhighlightImage(Image img)
        {
            Color.RGBToHSV(img.color, out float temp_hue,
                out float temp_saturation, out _);
            img.color = Color.HSVToRGB(temp_hue, temp_saturation, 0.5f);
        }
        /// <summary>
        /// Highlights the Image by brightening it.
        /// 
        /// Pre Conditions - Assumes the given Image is not null.
        /// Post Conditions - Sets the Image's brightness value (HSV's V) to be 1.0f.
        /// </summary>
        /// <param name="img">Image to highlight.</param>
        private void HighlightImage(Image img)
        {
            Color.RGBToHSV(img.color, out float temp_hue,
                out float temp_saturation, out _);
            img.color = Color.HSVToRGB(temp_hue, temp_saturation, 1.0f);
        }


        #region PlayerInput Messages
        private void OnButtonEast(InputValue value) => HandleButtonInput(value, m_buttonEastImg);
        private void OnButtonWest(InputValue value) => HandleButtonInput(value, m_buttonWestImg);
        private void OnButtonSouth(InputValue value) => HandleButtonInput(value, m_buttonSouthImg);
        private void OnButtonNorth(InputValue value) => HandleButtonInput(value, m_buttonNorthImg);
        //private void OnDPad(InputValue value) { }
        private void OnDPadDown(InputValue value) => HandleButtonInput(value, m_dPadDownImg);
        private void OnDPadUp(InputValue value) => HandleButtonInput(value, m_dPadUpImg);
        private void OnDPadLeft(InputValue value) => HandleButtonInput(value, m_dPadLeftImg);
        private void OnDPadRight(InputValue value) => HandleButtonInput(value, m_dPadRightImg);
        //private void OnDPadX(InputValue value) { }
        //private void OnDPadY(InputValue value) { }
        private void OnLeftShoulder(InputValue value) => HandleButtonInput(value, m_leftShoulderImg);
        private void OnRightShoulder(InputValue value) => HandleButtonInput(value, m_rightShoulderImg);
        private void OnLeftTrigger(InputValue value) => HandleButtonInput(value, m_leftTriggerImg);
        private void OnRightTrigger(InputValue value) => HandleButtonInput(value, m_rightTriggerImg);
        private void OnLeftStick(InputValue value) { }
        private void OnLeftStickDown(InputValue value) { }
        private void OnLeftStickUp(InputValue value) { }
        private void OnLeftStickRight(InputValue value) { }
        private void OnLeftStickLeft(InputValue value) { }
        //private void OnLeftStickX(InputValue value) { }
        //private void OnLeftStickY(InputValue value) { }
        private void OnLeftStickPress(InputValue value) { }
        private void OnRightStick(InputValue value) { }
        private void OnRightStickDown(InputValue value) { }
        private void OnRightStickUp(InputValue value) { }
        private void OnRightStickRight(InputValue value) { }
        private void OnRightStickLeft(InputValue value) { }
        //private void OnRightStickX(InputValue value) { }
        //private void OnRightStickY(InputValue value) { }
        private void OnRightStickPress(InputValue value) { }
        private void OnSelect(InputValue value) => HandleButtonInput(value, m_selectImg);
        private void OnStart(InputValue value) => HandleButtonInput(value, m_startImg);
        //private void OnTriggerAxis(InputValue value) { }
        //private void OnShoulderAxis(InputValue value) { }
        //private void OnButtons(InputValue value) { }
        //private void OnButtonsX(InputValue value) { }
        //private void OnButtonsY(InputValue value) { }
        #endregion PlayerInput Messages


        /// <summary>
        /// Highlights or unhighlights the Image based on if
        /// the button/axis was used.
        ///
        /// Pre Conditions - Assumes img is not null. Assumes a float can
        /// be read out from the given input value.
        /// </summary>
        /// <param name="value">InputValue of the button.</param>
        /// <param name="img">Image to highlight or unhighlight.</param>
        private void HandleButtonInput(InputValue value, Image img)
        {
            float temp_pressedValue = value.Get<float>();

            if (temp_pressedValue > 0.1f)
            {
                HighlightImage(img);
            }
            else
            {
                UnhighlightImage(img);
            }
        }
    }
}
