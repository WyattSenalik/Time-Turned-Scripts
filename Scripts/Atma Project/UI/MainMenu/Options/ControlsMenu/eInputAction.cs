using Newtonsoft.Json.Linq;
using UnityEngine.InputSystem;

using Atma.Settings;
using Helpers.Extensions;
using UnityEngine;
using Helpers.InputVisual;

namespace Atma
{
    public enum eInputAction { Move_Up, Move_Down, Move_Left, Move_Right, FreezeTime, Interact, Leap, Throw, Shoot, Restart, Pause, Rewind, FastForward, Resume, SkipToBegin, SkipToEnd, CreateClone, AdvanceDialogue, Menu_Up, Menu_Down, Menu_Left, Menu_Right, MenuSubmit, Aim_Up, Aim_Down, Aim_Left, Aim_Right }


    public static class InputActionExtensions
    {
        private const bool IS_DEBUGGING = false;

        public const string MOVE_UP_KEYBOARD_ID = "5ee1529d-9052-4655-8763-8db1e2370020";
        public const string MOVE_DOWN_KEYBOARD_ID = "6c0f51b7-623a-45aa-966d-5cebb1f5623b";
        public const string MOVE_LEFT_KEYBOARD_ID = "d233cc07-9f72-44a6-9f7d-8e6e62295337";
        public const string MOVE_RIGHT_KEYBOARD_ID = "c9e4420f-c75a-461c-9e29-3ed0c002dde7";
        public const string FREEZE_TIME_KEYBOARD_ID = "babf39f5-76a0-49ec-afe4-cf855c6afeb2";
        public const string INTERACT_KEYBOARD_ID = "b5e70d8f-45d3-435b-887d-9a9ed0a5792e";
        public const string LEAP_KEYBOARD_ID = "6430236b-3318-454e-bedd-dd705faaa1aa";
        public const string THROW_KEYBOARD_ID = "08695837-c482-45b3-a69a-c03231f3f7be";
        public const string SHOOT_KEYBOARD_ID = "f068ee01-f58d-403c-863f-69f3caf40d93";
        public const string RESTART_KEYBOARD_ID = "226eb49d-cc75-44bc-810a-b88e8c4e31a4";
        public const string PAUSE_KEYBOARD_ID = "89f92996-123d-4199-9d7e-ebfeadb2e5f5";
        public const string REWIND_KEYBOARD_ID = "a5ded004-6968-442e-8974-5f8ca329e7cb";
        public const string FAST_FORWARD_KEYBOARD_ID = "c46f76e1-0a8b-45ae-84e7-ca13b0a4026f";
        public const string RESUME_KEYBOARD_ID = "928d4fcc-a246-44f2-9785-e685707718c4";
        public const string SKIP_TO_BEGIN_KEYBOARD_ID = "ba52b241-496f-4d12-9dbc-e37db8885bbe";
        public const string SKIP_TO_END_KEYBOARD_ID = "46f39c81-831e-4239-9972-43518a6335aa";
        public const string CREATE_CLONE_KEYBOARD_ID = "894fbce2-86d8-45ce-a25f-cac2fac13c36";
        public const string ADVANCE_DIALOGUE_KEYBOARD_ID = "95dccfe5-44ea-45ae-b947-97f3e2145c6a";
        public const string MENU_UP_KEYBOARD_ID = "b5c37dfa-2083-4e0d-b185-7fd5c09f9ab0";
        public const string MENU_DOWN_KEYBOARD_ID = "3cafd1e3-2c53-426c-a30c-2ac8f826cbf2";
        public const string MENU_LEFT_KEYBOARD_ID = "81c09d57-deb3-40be-a078-200f7800b5ad";
        public const string MENU_RIGHT_KEYBOARD_ID = "8520e61b-fe74-42e6-86af-c886ba161be2";
        public const string MENU_SUBMIT_KEYBOARD_ID = "d949d726-eb43-402e-a1bf-d236b4b499b1";

        public const string MOVE_UP_CONTROLLER_ID = "cab6df4c-6181-4886-83e1-1ae2993d5de5";
        public const string MOVE_DOWN_CONTROLLER_ID = "aa1c14b0-f416-436f-8f49-46af56e3de36";
        public const string MOVE_LEFT_CONTROLLER_ID = "f12db1ea-e834-4ca5-a6d1-658226d733d1";
        public const string MOVE_RIGHT_CONTROLLER_ID = "a2922a78-ce82-455a-91ad-e7d2cb2465d9";
        public const string FREEZE_TIME_CONTROLLER_ID = "eeeedcdd-f644-4c45-bb11-29c78bf058f7";
        public const string INTERACT_CONTROLLER_ID = "62c9becb-dd85-4ae6-a4a1-d419e0d78b84";
        public const string LEAP_CONTROLLER_ID = "b32f4219-8183-4f69-9a99-b856cf54b928";
        public const string THROW_CONTROLLER_ID = "ad03589c-6a15-4be4-9263-0108006e1dfc";
        public const string SHOOT_CONTROLLER_ID = "de317e0c-d675-470e-8689-e132ebf9e32d";
        public const string RESTART_CONTROLLER_ID = "737c2470-5e55-403e-8a3b-b55e2ab55c2a";
        public const string PAUSE_CONTROLLER_ID = "02a87447-ceca-4e50-bef8-391a35224a48";
        public const string REWIND_CONTROLLER_ID = "099854f6-7c99-42eb-9771-38dcb5d4adb9";
        public const string FAST_FORWARD_CONTROLLER_ID = "366cf047-b8af-41a6-ab25-09ca0c92cebf";
        public const string RESUME_CONTROLLER_ID = "7ce49ae6-70e2-4536-ad55-f72b38b85a0d";
        public const string SKIP_TO_BEGIN_CONTROLLER_ID = "aeeb8a0a-df67-4bb3-9e00-bdc1bfecad03";
        public const string SKIP_TO_END_CONTROLLER_ID = "fe4a0683-6b9b-41c2-88a0-46fe2815f42d";
        public const string CREATE_CLONE_CONTROLLER_ID = "6a1d3a39-17af-4f3d-805e-9b33a4160306";
        public const string ADVANCE_DIALOGUE_CONTROLLER_ID = "e4865ec2-77d0-4e8a-bdbd-f95aa8c19b7f";
        public const string MENU_UP_CONTROLLER_ID = "6df570fb-7513-4051-85ab-e944b4e6e0d7";
        public const string MENU_DOWN_CONTROLLER_ID = "58f68132-7e78-49d4-a894-f657ec83578b";
        public const string MENU_LEFT_CONTROLLER_ID = "1826446c-5ace-442d-b914-5144202d1f99";
        public const string MENU_RIGHT_CONTROLLER_ID = "a4a5f7bd-0650-45c4-b240-b72b2833d806";
        public const string MENU_SUBMIT_CONTROLLER_ID = "fd3a3386-bd3f-4409-b473-c392d887a033";
        public const string AIM_UP_CONTROLLER_ID = "34c3a83a-e442-435b-a9ad-d2256c779a89";
        public const string AIM_DOWN_CONTROLLER_ID = "c47dc3eb-d2a8-4e64-8099-1ca52b4d9b7c";
        public const string AIM_LEFT_CONTROLLER_ID = "3b020627-aa3b-4d14-ba0f-72aa4e3e3c82";
        public const string AIM_RIGHT_CONTROLLER_ID = "80fda060-1563-47d8-9b2b-6933d7a8fad1";


        public static bool HasBindingOverride(this eInputAction action)
        {
            GameSettings t_settings = GameSettings.instance;

            string t_overrideJSONStr = t_settings.GetCurrentControlMappings();
            if (t_overrideJSONStr == string.Empty)
            {
                return false;
            }
            JObject t_overrideJSON = JObject.Parse(t_overrideJSONStr);
            JToken t_bindings = t_overrideJSON["bindings"];
            if (t_bindings == null)
            {
                return false;
            }

            return true;
        }
        public static string ExtractBindingPathFromCurrentMappings(this eInputAction action)
        {
            GameSettings t_settings = GameSettings.instance;

            string t_overrideJSONStr = t_settings.GetCurrentControlMappings();
            if (t_overrideJSONStr == string.Empty)
            {
                return GetDefaultBindingPath(action);
            }
            JObject t_overrideJSON = JObject.Parse(t_overrideJSONStr);
            //UnityEngine.Debug.Log(t_overrideJSON);
            JToken t_bindings = t_overrideJSON["bindings"];
            if (t_bindings == null)
            {
                return GetDefaultBindingPath(action);
            }

            JToken t_foundBinding = null;
            string t_desiredID = action.GetCorrespondingID();
            if (t_desiredID == "")
            {
                // The corresponding ID has no id attached to it.
                return GetDefaultBindingPath(action);
            }
            // Find the desired binding
            foreach (JToken t_singleBinding in t_bindings)
            {
                if (t_singleBinding == null)
                {
                    //CustomDebug.LogWarning($"Null binding contained in JSON bindings ({t_bindings}).");
                    continue;
                }
                JToken t_id = t_singleBinding["id"];
                if (t_id == null)
                {
                    //CustomDebug.LogWarning($"Binding ({t_singleBinding}) had no id in JSON file");
                    continue;
                }
                
                if (t_id.ToString().Equals(t_desiredID))
                {
                    t_foundBinding = t_singleBinding;
                    break;
                }
            }
            // If no finding was found.
            if (t_foundBinding == null)
            {
                return GetDefaultBindingPath(action);
            }

            JToken t_path = t_foundBinding["path"];
            if (t_path == null)
            {
                return GetDefaultBindingPath(action);
            }
            return t_path.ToString();
        }

        public static string GetCorrespondingID(this eInputAction action)
        {
            PlayerSingleton t_player = PlayerSingleton.GetInstanceSafe();
            PlayerInput t_inp = t_player.gameObject.GetComponentSafe<PlayerInput>();
            eControlScheme t_controlScheme = t_inp.GetCurrentControlScheme();
            switch (t_controlScheme)
            {
                case eControlScheme.GamePad: return GetCorrespondingIDController(action);
                case eControlScheme.MouseKeyboard: return GetCorrespondingIDKeyboard(action);
                default:
                {
                    CustomDebug.UnhandledEnum(t_controlScheme, nameof(GetCorrespondingID));
                    return GetCorrespondingIDKeyboard(action);
                } 
            }
        }
        public static string GetCorrespondingIDController(this eInputAction action)
        {
            switch (action)
            {
                case eInputAction.Move_Up: return MOVE_UP_CONTROLLER_ID;
                case eInputAction.Move_Down: return MOVE_DOWN_CONTROLLER_ID;
                case eInputAction.Move_Left: return MOVE_LEFT_CONTROLLER_ID;
                case eInputAction.Move_Right: return MOVE_RIGHT_CONTROLLER_ID;
                case eInputAction.FreezeTime: return FREEZE_TIME_CONTROLLER_ID;
                case eInputAction.Interact: return INTERACT_CONTROLLER_ID;
                case eInputAction.Leap: return LEAP_CONTROLLER_ID;
                case eInputAction.Throw: return THROW_CONTROLLER_ID;
                case eInputAction.Shoot: return SHOOT_CONTROLLER_ID;
                case eInputAction.Restart: return RESTART_CONTROLLER_ID;
                case eInputAction.Pause: return PAUSE_CONTROLLER_ID;
                case eInputAction.Rewind: return REWIND_CONTROLLER_ID;
                case eInputAction.FastForward: return FAST_FORWARD_CONTROLLER_ID;
                case eInputAction.Resume: return RESUME_CONTROLLER_ID;
                case eInputAction.SkipToBegin: return SKIP_TO_BEGIN_CONTROLLER_ID;
                case eInputAction.SkipToEnd: return SKIP_TO_END_CONTROLLER_ID;
                case eInputAction.CreateClone: return CREATE_CLONE_CONTROLLER_ID;
                case eInputAction.AdvanceDialogue: return ADVANCE_DIALOGUE_CONTROLLER_ID;
                case eInputAction.Menu_Up: return MENU_UP_CONTROLLER_ID;
                case eInputAction.Menu_Down: return MENU_DOWN_CONTROLLER_ID;
                case eInputAction.Menu_Left: return MENU_LEFT_CONTROLLER_ID;
                case eInputAction.Menu_Right: return MENU_RIGHT_CONTROLLER_ID;
                case eInputAction.MenuSubmit: return MENU_SUBMIT_CONTROLLER_ID;
                case eInputAction.Aim_Up: return AIM_UP_CONTROLLER_ID;
                case eInputAction.Aim_Down: return AIM_DOWN_CONTROLLER_ID;
                case eInputAction.Aim_Left: return AIM_LEFT_CONTROLLER_ID;
                case eInputAction.Aim_Right: return AIM_RIGHT_CONTROLLER_ID;
                default:
                {
                    CustomDebug.UnhandledEnum(action, nameof(GetCorrespondingIDController));
                    return "";
                }
            }
        }
        public static string GetCorrespondingIDKeyboard(this eInputAction action)
        {
            switch (action)
            {
                case eInputAction.Move_Up: return MOVE_UP_KEYBOARD_ID;
                case eInputAction.Move_Down: return MOVE_DOWN_KEYBOARD_ID;
                case eInputAction.Move_Left: return MOVE_LEFT_KEYBOARD_ID;
                case eInputAction.Move_Right: return MOVE_RIGHT_KEYBOARD_ID;
                case eInputAction.FreezeTime: return FREEZE_TIME_KEYBOARD_ID;
                case eInputAction.Interact: return INTERACT_KEYBOARD_ID;
                case eInputAction.Leap: return LEAP_KEYBOARD_ID;
                case eInputAction.Throw: return THROW_KEYBOARD_ID;
                case eInputAction.Shoot: return SHOOT_KEYBOARD_ID;
                case eInputAction.Restart: return RESTART_KEYBOARD_ID;
                case eInputAction.Pause: return PAUSE_KEYBOARD_ID;
                case eInputAction.Rewind: return REWIND_KEYBOARD_ID;
                case eInputAction.FastForward: return FAST_FORWARD_KEYBOARD_ID;
                case eInputAction.Resume: return RESUME_KEYBOARD_ID;
                case eInputAction.SkipToBegin: return SKIP_TO_BEGIN_KEYBOARD_ID;
                case eInputAction.SkipToEnd: return SKIP_TO_END_KEYBOARD_ID;
                case eInputAction.CreateClone: return CREATE_CLONE_KEYBOARD_ID;
                case eInputAction.AdvanceDialogue: return ADVANCE_DIALOGUE_KEYBOARD_ID;
                case eInputAction.Menu_Up: return MENU_UP_KEYBOARD_ID;
                case eInputAction.Menu_Down: return MENU_DOWN_KEYBOARD_ID;
                case eInputAction.Menu_Left: return MENU_LEFT_KEYBOARD_ID;
                case eInputAction.Menu_Right: return MENU_RIGHT_KEYBOARD_ID;
                case eInputAction.MenuSubmit: return MENU_SUBMIT_KEYBOARD_ID;
                case eInputAction.Aim_Up: return "";
                case eInputAction.Aim_Down: return "";
                case eInputAction.Aim_Left: return "";
                case eInputAction.Aim_Right: return "";
                default:
                {
                    CustomDebug.UnhandledEnum(action, nameof(GetCorrespondingIDKeyboard));
                    return "";
                }
            }
        }
        public static string GetDefaultBindingPath(this eInputAction action)
        {
            PlayerSingleton t_player = PlayerSingleton.GetInstanceSafe();
            PlayerInput t_inp = t_player.gameObject.GetComponentSafe<PlayerInput>();
            eControlScheme t_controlScheme = t_inp.GetCurrentControlScheme();
            switch (t_controlScheme)
            {
                case eControlScheme.GamePad: return GetDefaultBindingPathController(action);
                case eControlScheme.MouseKeyboard: return GetDefaultBindingPathKeyboard(action);
                default:
                {
                    CustomDebug.UnhandledEnum(t_controlScheme, nameof(GetDefaultBindingPath));
                    return GetDefaultBindingPathKeyboard(action);
                }
            }
        }
        public static string GetDefaultBindingPathKeyboard(this eInputAction action)
        {
            switch (action)
            {
                case eInputAction.Move_Up: return "<Keyboard>/w";
                case eInputAction.Move_Down: return "<Keyboard>/s";
                case eInputAction.Move_Left: return "<Keyboard>/a";
                case eInputAction.Move_Right: return "<Keyboard>/d";
                case eInputAction.FreezeTime: return "<Keyboard>/space";
                case eInputAction.Interact: return "<Keyboard>/e";
                case eInputAction.Leap: return "<Keyboard>/e";
                case eInputAction.Throw: return "<Mouse>/rightButton";
                case eInputAction.Shoot: return "<Mouse>/leftButton";
                case eInputAction.Restart: return "<Keyboard>/r";
                case eInputAction.Pause: return "<Keyboard>/escape";
                case eInputAction.Rewind: return "<Keyboard>/a";
                case eInputAction.FastForward: return "<Keyboard>/d";
                case eInputAction.Resume: return "<Keyboard>/space";
                case eInputAction.SkipToBegin: return "<Keyboard>/e";
                case eInputAction.SkipToEnd: return "<Keyboard>/q";
                case eInputAction.CreateClone: return "<Keyboard>/f";
                case eInputAction.AdvanceDialogue: return "<Keyboard>/space";
                case eInputAction.Menu_Up: return "<Keyboard>/w";
                case eInputAction.Menu_Down: return "<Keyboard>/s";
                case eInputAction.Menu_Left: return "<Keyboard>/a";
                case eInputAction.Menu_Right: return "<Keyboard>/d";
                case eInputAction.MenuSubmit: return "<Keyboard>/enter";
                case eInputAction.Aim_Up: return "<Mouse>/delta";
                case eInputAction.Aim_Down: return "<Mouse>/delta";
                case eInputAction.Aim_Left: return "<Mouse>/delta";
                case eInputAction.Aim_Right: return "<Mouse>/delta";
                default:
                {
                    CustomDebug.UnhandledEnum(action, nameof(GetDefaultBindingPathKeyboard));
                    return "";
                }
            }
            
        }
        public static string GetDefaultBindingPathController(this eInputAction action)
        {
            switch (action)
            {
                case eInputAction.Move_Up: return "<Gamepad>/dpad/up";
                case eInputAction.Move_Down: return "<Gamepad>/dpad/down";
                case eInputAction.Move_Left: return "<Gamepad>/dpad/left";
                case eInputAction.Move_Right: return "<Gamepad>/dpad/right";
                case eInputAction.FreezeTime: return "<Gamepad>/buttonSouth";
                case eInputAction.Interact: return "<Gamepad>/buttonWest";
                case eInputAction.Leap: return "<Gamepad>/buttonWest";
                case eInputAction.Throw: return "<Gamepad>/leftTrigger";
                case eInputAction.Shoot: return "<Gamepad>/rightTrigger";
                case eInputAction.Restart: return "<Gamepad>/select";
                case eInputAction.Pause: return "<Gamepad>/start";
                case eInputAction.Rewind: return "<Gamepad>/leftTrigger";
                case eInputAction.FastForward: return "<Gamepad>/rightTrigger";
                case eInputAction.Resume: return "<Gamepad>/buttonSouth";
                case eInputAction.SkipToBegin: return "<Gamepad>/leftShoulder";
                case eInputAction.SkipToEnd: return "<Gamepad>/rightShoulder";
                case eInputAction.CreateClone: return "<Gamepad>/buttonWest";
                case eInputAction.AdvanceDialogue: return "<Gamepad>/buttonSouth";
                case eInputAction.Menu_Up: return "<Gamepad>/dpad/up";
                case eInputAction.Menu_Down: return "<Gamepad>/dpad/down";
                case eInputAction.Menu_Left: return "<Gamepad>/dpad/left";
                case eInputAction.Menu_Right: return "<Gamepad>/dpad/right";
                case eInputAction.MenuSubmit: return "<Gamepad>/buttonSouth";
                case eInputAction.Aim_Up: return "<Gamepad>/rightStick/up";
                case eInputAction.Aim_Down: return "<Gamepad>/rightStick/down";
                case eInputAction.Aim_Left: return "<Gamepad>/rightStick/left";
                case eInputAction.Aim_Right: return "<Gamepad>/rightStick/right";
                default:
                {
                    CustomDebug.UnhandledEnum(action, nameof(GetDefaultBindingPathController));
                    return "";
                }
            }
        }
        public static string GetSpriteAtlasTextBasedOnCurrentBindings(this eInputAction action)
        {
            string t_path = ExtractBindingPathFromCurrentMappings(action);
            //CustomDebug.Log($"Getting atlas text from path ({t_path}) for action ({action})", IS_DEBUGGING);
            if (t_path.Contains("Keyboard"))
            {
                // Substring it because <Keyboard>/ is 11 characters long and we want to trim that from the front.
                t_path = t_path.Substring(11);
                //CustomDebug.Log($"New path ({t_path}) for action ({action})", IS_DEBUGGING);
                return GetSpriteAtlasTextKeyboard(t_path);
            }
            else if (t_path.Contains("Mouse"))
            {
                // TODO
                t_path = t_path.Replace("Mouse", "");
                return GetSpriteAtlasTextMouse(t_path);
            }
            // TODO: IDK if its actually gamepad
            else if (t_path.Contains("Gamepad"))
            {
                // Substring it because <Gamepad>/ is 10 characters long and we want to trim that from the front.
                t_path = t_path.Substring(10);
                //CustomDebug.Log($"New path ({t_path}) for action ({action})", IS_DEBUGGING);
                string t_atlasText = GetSpriteAtlasTextGamepad(t_path);
                //CustomDebug.Log($"AtlasText: ({t_atlasText}) for action ({action})", IS_DEBUGGING);
                return t_atlasText;
            }
            else
            {
                return GetKeysAtlasText("Unknown");
            }
        }
        public static string GetSpriteAtlasTextKeyboard(string path)
        {
            if (path.Length == 1)
            {
                char t_pathChar = path[0];
                if (char.IsLetter(t_pathChar))
                {
                    // Letters simply need to be uppercase.
                    return GetKeysAtlasText(path.ToUpper());
                }
                else if (char.IsDigit(t_pathChar))
                {
                    // Numbers are fine.
                    return GetKeysAtlasText(path);
                }
            }
            // Not 1 long
            else
            {
                if (path.Contains("numpad"))
                {
                    // Trim off numpad
                    path = path.Substring(6);
                    // Try again (hopefully, its a single digit now or one of the things in the switch below)
                    return GetSpriteAtlasTextKeyboard(path);
                }

                switch (path.ToLower())
                {
                    case "backquote": return GetKeysAtlasText("BackTick");
                    case "minus": return GetKeysAtlasText("Minus");
                    case "equals": return GetKeysAtlasText("Equals");
                    case "backspace": return GetKeysAtlasText("Back");
                    case "tab": return GetKeysAtlasText("Tab");
                    case "leftbracket": return GetKeysAtlasText("LBracket");
                    case "rightbracket": return GetKeysAtlasText("RBracket");
                    case "backslash": return GetKeysAtlasText("Backslash");
                    case "capslock": return GetKeysAtlasText("Caps");
                    case "semicolon": return GetKeysAtlasText("SemiColon");
                    case "quote": return GetKeysAtlasText("Apostrophy");
                    case "enter": return GetKeysAtlasText("Enter");
                    case "leftshift":
                    case "rightshift":
                    case "shift": return GetKeysAtlasText("Shift");
                    case "comma": return GetKeysAtlasText("Comma");
                    case "period": return GetKeysAtlasText("Period");
                    case "divide":
                    case "slash": return GetKeysAtlasText("Forwardslash");
                    case "leftctrl":
                    case "rightctrl":
                    case "ctrl": return GetKeysAtlasText("Control");
                    case "leftalt":
                    case "rightalt":
                    case "alt": return GetKeysAtlasText("Alt");
                    case "space": return GetKeysAtlasText("Space");
                    case "uparrow": return GetKeysAtlasText("Up");
                    case "downarrow": return GetKeysAtlasText("Down");
                    case "leftarrow": return GetKeysAtlasText("Left");
                    case "rightarrow": return GetKeysAtlasText("Right");
                }
            }

            return GetKeysAtlasText("Unknown");
        }
        public static string GetSpriteAtlasTextMouse(string path)
        {
            // TODO
            return GetKeysAtlasText("Unknown");
        }
        public static string GetSpriteAtlasTextGamepad(string path)
        {
            path = path.ToLower();
            // D-Pad
            if (path.Contains("dpad"))
            {
                // Substring it because dpad/ is 5 characters long and we want to trim that from the front
                path = path.Substring(5);
                switch (path)
                {
                    case "up": return GetControllerAtlasText("D-Pad/Up");
                    case "down": return GetControllerAtlasText("D-Pad/Down");
                    case "left": return GetControllerAtlasText("D-Pad/Left");
                    case "right": return GetControllerAtlasText("D-Pad/Right");
                }
            }
            // Sticks (not pressing in on them)
            else if (path.Contains("stick") && !path.Contains("press"))
            {
                string t_stickAtlasName;
                if (path.Contains("rightstick"))
                {
                    // Substring it because rightstick/ is 11 characters long and we want to trim that from the front
                    path = path.Substring(11);
                    t_stickAtlasName = "RS";
                }
                else if (path.Contains("leftstick"))
                {
                    // Substring it because leftstick/ is 10 characters long and we want to trim that from the front
                    path = path.Substring(10);
                    t_stickAtlasName = "LS";
                }
                else
                {
                    // Might happen if just "stick" (idk if this can happen)
                    // Substring it because stick/ is 6 characters long and we want to trim that from the front
                    path = path.Substring(6);
                    t_stickAtlasName = "LS";
                }

                switch (path)
                {
                    case "up": return GetControllerAtlasText($"{t_stickAtlasName}/Up");
                    case "down": return GetControllerAtlasText($"{t_stickAtlasName}/Down");
                    case "left": return GetControllerAtlasText($"{t_stickAtlasName}/Left");
                    case "right": return GetControllerAtlasText($"{t_stickAtlasName}/Right");
                }
            }
            else
            {
                switch (path)
                {
                    case "buttonnorth": return GetControllerAtlasText("Y");
                    case "buttonwest": return GetControllerAtlasText("X");
                    case "buttoneast": return GetControllerAtlasText("B");
                    case "buttonsouth": return GetControllerAtlasText("A");
                    case "start": return GetControllerAtlasText("Start");
                    case "select": return GetControllerAtlasText("Select");
                    case "leftstickpress": return GetControllerAtlasText("Left Stick Press");
                    case "rightstickpress": return GetControllerAtlasText("Right Stick Press");
                    case "leftshoulder": return GetControllerAtlasText("LB");
                    case "rightshoulder": return GetControllerAtlasText("RB");
                    case "lefttrigger": return GetControllerAtlasText("LT");
                    case "righttrigger": return GetControllerAtlasText("RT");
                }
            }

            return GetControllerAtlasText("Unknown");
        }

        private static string GetKeysAtlasText(string elementName)
        {
            return GetAtlasText("Keys", elementName);
        }
        private static string GetMouseAtlasText(string elementName)
        {
            // TODO MAKE THIS SPRITE ATLAS
            return GetAtlasText("Mouse", elementName);
        }
        private static string GetControllerAtlasText(string elementName)
        {
            return GetAtlasText("ControllerButtons", elementName);
        }
        private static string GetAtlasText(string spriteName, string elementName)
        {
            return $"<sprite=\"{spriteName}\" name=\"{elementName}\">";
        }

        public static Sprite GetSpriteBasedOnCurrentBindings(this eInputAction action)
        {
            string t_path = ExtractBindingPathFromCurrentMappings(action);
            //CustomDebug.Log($"Getting sprite from path ({t_path}) for action ({action})", IS_DEBUGGING);
            if (t_path.Contains("Keyboard"))
            {
                // Substring it because <Keyboard>/ is 11 characters long and we want to trim that from the front.
                t_path = t_path.Substring(11);
                //CustomDebug.Log($"New path ({t_path}) for action ({action})", IS_DEBUGGING);
                return GetSpriteKeyboard(t_path);
            }
            else if (t_path.Contains("Mouse"))
            {
                // Substring it because <Mouse>/ is 8 characters long and we want to trim that from the front.
                t_path = t_path.Substring(8);
                //CustomDebug.Log($"New path ({t_path}) for action ({action})", IS_DEBUGGING);
                return GetSpriteMouse(t_path);
            }
            // TODO: IDK if its actually gamepad
            else if (t_path.Contains("Gamepad"))
            {
                // Substring it because <Gamepad>/ is 10 characters long and we want to trim that from the front.
                t_path = t_path.Substring(10);
                //CustomDebug.Log($"New path ({t_path}) for action ({action})", IS_DEBUGGING);
                Sprite t_sprite = GetSpriteGamepad(t_path);
                //CustomDebug.Log($"AtlasText: ({t_sprite}) for action ({action})", IS_DEBUGGING);
                return t_sprite;
            }
            else
            {
                return null;
            }
        }

        public static Sprite GetSpriteKeyboard(string path)
        {
            KeyboardImageScheme t_imgScheme = ImageSchemeManager.instance.keyboardImgScheme;

            if (path.Length == 1)
            {
                char t_pathChar = path[0];
                if (char.IsLetter(t_pathChar))
                {
                    // Letters simply need to be uppercase.
                    return t_imgScheme.GetSpriteForLetterChar(t_pathChar);
                }
                else if (char.IsDigit(t_pathChar))
                {
                    // Numbers are fine.
                    return t_imgScheme.GetSpriteForNumberChar(t_pathChar);
                }
            }
            // Not 1 long
            else
            {
                if (path.Contains("numpad"))
                {
                    // Trim off numpad
                    path = path.Substring(6);
                    // Try again (hopefully, its a single digit now or one of the things in the switch below)
                    return GetSpriteKeyboard(path);
                }

                switch (path.ToLower())
                {
                    case "backquote": return t_imgScheme.backTick;
                    case "minus": return t_imgScheme.minus;
                    case "equals": return t_imgScheme.equals;
                    case "backspace": return t_imgScheme.backspace;
                    case "tab": return t_imgScheme.tab;
                    case "leftbracket": return t_imgScheme.leftBracket;
                    case "rightbracket": return t_imgScheme.rightBracket;
                    case "backslash": return t_imgScheme.backSlash;
                    case "capslock": return t_imgScheme.caps;
                    case "semicolon": return t_imgScheme.semicolon;
                    case "quote": return t_imgScheme.singleQuote;
                    case "enter": return t_imgScheme.enter;
                    case "leftshift":
                    case "rightshift":
                    case "shift": return t_imgScheme.shift;
                    case "comma": return t_imgScheme.comma;
                    case "period": return t_imgScheme.period;
                    case "divide":
                    case "slash": return t_imgScheme.forwardSlash;
                    case "leftctrl":
                    case "rightctrl":
                    case "ctrl": return t_imgScheme.ctrl;
                    case "leftalt":
                    case "rightalt":
                    case "alt": return t_imgScheme.alt;
                    case "space": return t_imgScheme.space;
                    case "uparrow": return t_imgScheme.upArrow;
                    case "downarrow": return t_imgScheme.downArrow;
                    case "leftarrow": return t_imgScheme.leftArrow;
                    case "rightarrow": return t_imgScheme.rightArrow;
                }
            }

            return t_imgScheme.unknown;
        }
        public static Sprite GetSpriteMouse(string path)
        {
            MouseImageScheme t_imgScheme = ImageSchemeManager.instance.mouseImgScheme;

            switch (path.ToLower())
            {
                case "scroll": return t_imgScheme.scroll;
                case "leftbutton": return t_imgScheme.leftClick;
                case "rightbutton": return t_imgScheme.rightClick;
                case "middlebutton": return t_imgScheme.middleClick;
            }

            return t_imgScheme.fullMouse;
        }
        public static Sprite GetSpriteGamepad(string path)
        {
            ControllerImageScheme t_imgScheme = ImageSchemeManager.instance.controllerImgScheme;

            path = path.ToLower();
            // D-Pad
            if (path.Contains("dpad"))
            {
                // Substring it because dpad/ is 5 characters long and we want to trim that from the front
                path = path.Substring(5);
                switch (path)
                {
                    case "up": return t_imgScheme.dPad.up;
                    case "down": return t_imgScheme.dPad.down;
                    case "left": return t_imgScheme.dPad.left;
                    case "right": return t_imgScheme.dPad.right;
                }
            }
            // Sticks (not pressing in on them)
            else if (path.Contains("stick") && !path.Contains("press"))
            {
                bool t_isRightStickOrLeftStick;
                if (path.Contains("rightstick"))
                {
                    // Substring it because rightstick/ is 11 characters long and we want to trim that from the front
                    path = path.Substring(11);
                    t_isRightStickOrLeftStick = true;
                }
                else if (path.Contains("leftstick"))
                {
                    // Substring it because leftstick/ is 10 characters long and we want to trim that from the front
                    path = path.Substring(10);
                    t_isRightStickOrLeftStick = false;
                }
                else
                {
                    // Might happen if just "stick" (idk if this can happen)
                    // Substring it because stick/ is 6 characters long and we want to trim that from the front
                    path = path.Substring(6);
                    t_isRightStickOrLeftStick = false;
                }

                if (t_isRightStickOrLeftStick)
                {
                    switch (path)
                    {
                        case "up": return t_imgScheme.rightStickSeparate.up;
                        case "down": return t_imgScheme.rightStickSeparate.down;
                        case "left": return t_imgScheme.rightStickSeparate.left;
                        case "right": return t_imgScheme.rightStickSeparate.right;
                    }
                }
                else
                {
                    switch (path)
                    {
                        case "up": return t_imgScheme.leftStickSeparate.up;
                        case "down": return t_imgScheme.leftStickSeparate.down;
                        case "left": return t_imgScheme.leftStickSeparate.left;
                        case "right": return t_imgScheme.leftStickSeparate.right;
                    }
                }
            }
            else
            {
                switch (path)
                {
                    case "buttonnorth": return t_imgScheme.buttons.buttonNorth;
                    case "buttonwest": return t_imgScheme.buttons.buttonWest;
                    case "buttoneast": return t_imgScheme.buttons.buttonEast;
                    case "buttonsouth": return t_imgScheme.buttons.buttonSouth;
                    case "start": return t_imgScheme.start;
                    case "select": return t_imgScheme.select;
                    case "leftstickpress": return t_imgScheme.leftStick;
                    case "rightstickpress": return t_imgScheme.rightStick;
                    case "leftshoulder": return t_imgScheme.leftShoulder;
                    case "rightshoulder": return t_imgScheme.rightShoulder;
                    case "lefttrigger": return t_imgScheme.leftTrigger;
                    case "righttrigger": return t_imgScheme.rightTrigger;
                }
            }

            return t_imgScheme.unknownButton;
        }
    }
}