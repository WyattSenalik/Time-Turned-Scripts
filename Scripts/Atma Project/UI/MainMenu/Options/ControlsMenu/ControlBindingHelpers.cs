using System;
using UnityEngine;

using Helpers.InputVisual;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public enum eSupportedInputSchemes { KeyboardMouse, Controller }

    public static class ControlBindingHelpers
    {
        public static eSupportedInputSchemes DetermineCurrentInputScheme()
        {
            PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_playerSingleton, nameof(DetermineCurrentInputScheme));
            #endregion Asserts
            PlayerInput t_playerInp = t_playerSingleton.GetComponent<PlayerInput>();
            #region Asserts
            //CustomDebug.AssertComponentOnOtherIsNotNull(t_playerInp, t_playerSingleton.gameObject, nameof(DetermineCurrentInputScheme));
            #endregion Asserts

            string t_controlScheme = t_playerInp.currentControlScheme;
            if (t_controlScheme.Contains(PlayerInputExtensions.GAME_PAD_CTRL_SCHEME))
            {
                return eSupportedInputSchemes.Controller;
            }
            else if (t_controlScheme.Contains(PlayerInputExtensions.MOUSE_CTRL_SCHEME))
            {
                return eSupportedInputSchemes.KeyboardMouse;
            }
            else
            {
                //CustomDebug.LogError($"Unexpected control scheme name ({t_controlScheme}). Expected control scheme to contain either {PlayerInputExtensions.GAME_PAD_CTRL_SCHEME} or {PlayerInputExtensions.MOUSE_CTRL_SCHEME}.");
                return eSupportedInputSchemes.KeyboardMouse;
            }
        }

        public static bool TryGetControllerSpriteForBinding(string bindingDisplayName, out Sprite sprite, ControllerImageScheme controllerImgScheme)
        {
            if (IsNorthButton(bindingDisplayName))
            {
                sprite = controllerImgScheme.buttons.buttonNorth;
                return true;
            }
            else if (IsSouthButton(bindingDisplayName))
            {
                sprite = controllerImgScheme.buttons.buttonSouth;
                return true;
            }
            else if (IsWestButton(bindingDisplayName))
            {
                sprite = controllerImgScheme.buttons.buttonWest;
                return true;
            }
            else if (IsEastButton(bindingDisplayName))
            {
                sprite = controllerImgScheme.buttons.buttonEast;
                return true;
            }
            else if (IsDPadUp(bindingDisplayName))
            {
                sprite = controllerImgScheme.dPad.up;
                return true;
            }
            else if (IsDPadDown(bindingDisplayName))
            {
                sprite = controllerImgScheme.dPad.down;
                return true;
            }
            else if (IsDPadLeft(bindingDisplayName))
            {
                sprite = controllerImgScheme.dPad.left;
                return true;
            }
            else if (IsDPadRight(bindingDisplayName))
            {
                sprite = controllerImgScheme.dPad.right;
                return true;
            }
            else if (IsLeftJoystickUp(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftStickSeparate.up;
                return true;
            }
            else if (IsLeftJoystickDown(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftStickSeparate.down;
                return true;
            }
            else if (IsLeftJoystickRight(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftStickSeparate.right;
                return true;
            }
            else if (IsRightJoystickUp(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightStickSeparate.up;
                return true;
            }
            else if (IsRightJoystickDown(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightStickSeparate.down;
                return true;
            }
            else if (IsRightJoystickLeft(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightStickSeparate.left;
                return true;
            }
            else if (IsLeftJoystickIn(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftStick;
                return true;
            }
            else if (IsRightJoystickIn(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightStick;
                return true;
            }
            else if (IsSelect(bindingDisplayName))
            {
                sprite = controllerImgScheme.select;
                return true;
            }
            else if (IsStart(bindingDisplayName))
            {
                sprite = controllerImgScheme.start;
                return true;
            }
            else if (IsLeftTrigger(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftTrigger;
                return true;
            }
            else if (IsLeftBumper(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftShoulder;
                return true;
            }
            else if (IsRightTrigger(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightTrigger;
                return true;
            }
            else if (IsRightBumper(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightShoulder;
                return true;
            }
            // ORDER MATTERS: THATS WHY LEFT LEFT AND RIGHT RIGHT ARE AT THE BOTTOM
            else if (IsLeftJoystickLeft(bindingDisplayName))
            {
                sprite = controllerImgScheme.leftStickSeparate.left;
                return true;
            }
            else if (IsRightJoystickRight(bindingDisplayName))
            {
                sprite = controllerImgScheme.rightStickSeparate.right;
                return true;
            }
            else
            {
                sprite = null;
                return false;
            }
        }
        private static bool IsNorthButton(string bindingDisplayName)
        {
            return bindingDisplayName.Equals("Y") || bindingDisplayName.Contains("Triangle", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("North", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsSouthButton(string bindingDisplayName)
        {
            return bindingDisplayName.Equals("A") || bindingDisplayName.Contains("Cross", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("South", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsWestButton(string bindingDisplayName)
        {
            return bindingDisplayName.Equals("X") || bindingDisplayName.Contains("Square", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("West", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsEastButton(string bindingDisplayName)
        {
            return bindingDisplayName.Equals("B") || bindingDisplayName.Contains("Circle", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("East", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsDPadUp(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("Pad", StringComparison.OrdinalIgnoreCase) && bindingDisplayName.Contains("Up", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsDPadDown(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("Pad", StringComparison.OrdinalIgnoreCase) && bindingDisplayName.Contains("Down", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsDPadLeft(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("Pad", StringComparison.OrdinalIgnoreCase) && bindingDisplayName.Contains("Left", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsDPadRight(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("Pad", StringComparison.OrdinalIgnoreCase) && bindingDisplayName.Contains("Right", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsLeftJoystickUp(string bindingDisplayName)
        {
            return IsLeftStick(bindingDisplayName) && bindingDisplayName.Contains("Up", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsLeftJoystickDown(string bindingDisplayName)
        {
            return IsLeftStick(bindingDisplayName) && bindingDisplayName.Contains("Down", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsLeftJoystickLeft(string bindingDisplayName)
        {
            return IsLeftStick(bindingDisplayName) && bindingDisplayName.Contains("Left", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsLeftJoystickRight(string bindingDisplayName)
        {
            return IsLeftStick(bindingDisplayName) && bindingDisplayName.Contains("Right", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsRightJoystickUp(string bindingDisplayName)
        {
            return IsRightStick(bindingDisplayName) && bindingDisplayName.Contains("Up", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsRightJoystickDown(string bindingDisplayName)
        {
            return IsRightStick(bindingDisplayName) && bindingDisplayName.Contains("Down", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsRightJoystickLeft(string bindingDisplayName)
        {
            return IsRightStick(bindingDisplayName) && bindingDisplayName.Contains("Left", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsRightJoystickRight(string bindingDisplayName)
        {
            return IsRightStick(bindingDisplayName) && bindingDisplayName.Contains("Right", StringComparison.OrdinalIgnoreCase);
        }
        private static  bool IsLeftJoystickIn(string bindingDisplayName)
        {
            return (IsLeftStick(bindingDisplayName) && bindingDisplayName.Contains("Press")) || bindingDisplayName.Contains("L3");
        }
        private static bool IsRightJoystickIn(string bindingDisplayName)
        {
            return IsRightStick(bindingDisplayName) && bindingDisplayName.Contains("Press") || bindingDisplayName.Contains("R3");
        }
        private static bool IsSelect(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("Select", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("Share", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("Minus", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsStart(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("Start", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("Options", StringComparison.OrdinalIgnoreCase) || bindingDisplayName.Contains("Plus", StringComparison.OrdinalIgnoreCase);
        }
        private static bool IsLeftTrigger(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("LT") || bindingDisplayName.Contains("L2") || bindingDisplayName.Contains("ZL") || (bindingDisplayName.Contains("Left") && bindingDisplayName.Contains("Trigger"));
        }
        private static bool IsLeftBumper(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("LB") || bindingDisplayName.Contains("L1") || bindingDisplayName.Equals("L") || (bindingDisplayName.Contains("Left") && bindingDisplayName.Contains("Bumper"));
        }
        private static bool IsRightTrigger(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("RT") || bindingDisplayName.Contains("R2") || bindingDisplayName.Contains("ZR") || (bindingDisplayName.Contains("Right") && bindingDisplayName.Contains("Trigger"));
        }
        private static bool IsRightBumper(string bindingDisplayName)
        {
            return bindingDisplayName.Contains("RB") || bindingDisplayName.Contains("R1") || bindingDisplayName.Equals("R") || (bindingDisplayName.Contains("Right") && bindingDisplayName.Contains("Bumper"));
        }

        private static bool IsLeftStick(string bindingDisplayName)
        {
            return (bindingDisplayName.Contains("LS") || bindingDisplayName.Contains("Left Stick", StringComparison.OrdinalIgnoreCase));
        }
        private static bool IsRightStick(string bindingDisplayName)
        {
            return (bindingDisplayName.Contains("RS") || bindingDisplayName.Contains("Right Stick", StringComparison.OrdinalIgnoreCase));
        }
    }
}