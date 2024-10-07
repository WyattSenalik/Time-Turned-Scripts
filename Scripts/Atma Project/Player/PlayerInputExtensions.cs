using UnityEngine.InputSystem;

namespace Atma
{
    public static class PlayerInputExtensions
    {
        public const string GAME_PAD_CTRL_SCHEME = "GamePad";
        public const string MOUSE_CTRL_SCHEME = "MouseKeyboard";

        public static eControlScheme GetCurrentControlScheme(this PlayerInput playerInput)
        {
            switch (playerInput.currentControlScheme)
            {
                case GAME_PAD_CTRL_SCHEME: return eControlScheme.GamePad;
                case MOUSE_CTRL_SCHEME: return eControlScheme.MouseKeyboard;
                default:
                    CustomDebug.UnhandledValue(playerInput.currentControlScheme, nameof(GetCurrentControlScheme));
                    return eControlScheme.MouseKeyboard;
            }
        }
        public static bool IsCurrentControlSchemeController(this PlayerInput playerInput)
        {
            eControlScheme t_scheme = GetCurrentControlScheme(playerInput);
            return t_scheme == eControlScheme.GamePad;
        }
        public static bool IsCurrentControlSchemeKeyboard(this PlayerInput playerInput)
        {
            eControlScheme t_scheme = GetCurrentControlScheme(playerInput);
            return t_scheme == eControlScheme.MouseKeyboard;
        }
    }

    public enum eControlScheme { MouseKeyboard, GamePad }

    public static class ControlSchemeExtensions
    {
        public static string GetPlayerInputName(this eControlScheme scheme)
        {
            switch (scheme)
            {
                case eControlScheme.MouseKeyboard: return PlayerInputExtensions.MOUSE_CTRL_SCHEME;
                case eControlScheme.GamePad: return PlayerInputExtensions.GAME_PAD_CTRL_SCHEME;
                default:
                    CustomDebug.UnhandledEnum(scheme, nameof(GetPlayerInputName));
                    return PlayerInputExtensions.MOUSE_CTRL_SCHEME;
            }
        }
    }
}