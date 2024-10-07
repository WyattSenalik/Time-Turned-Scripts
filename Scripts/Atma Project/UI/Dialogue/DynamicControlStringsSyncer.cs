using UnityEngine;
using UnityEngine.InputSystem;

using NaughtyAttributes;

using Atma.Settings;
using Dialogue;
using Helpers.Extensions;
using System.Collections.Generic;

namespace Atma.Dialogue
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    public sealed class DynamicControlStringsSyncer : MonoBehaviour
    {
        public static string mostRecentControlScheme { get; private set; } = PlayerInputExtensions.MOUSE_CTRL_SCHEME;

        [SerializeField, Required] private DynamicStringReference m_createCloneControlDynamicString = null;
        [SerializeField, Required] private DynamicStringReference m_pauseTimeControlDynamicString = null;
        [SerializeField, Required] private DynamicStringReference m_restartControlDynamicString = null;

        private GameSettings m_settings = null;
        private PlayerInput m_playerInp = null;

        private bool m_wasStartCalled = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_createCloneControlDynamicString, nameof(m_createCloneControlDynamicString), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pauseTimeControlDynamicString, nameof(m_pauseTimeControlDynamicString), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_restartControlDynamicString, nameof(m_restartControlDynamicString), this);
            #endregion Asserts
            m_playerInp = this.GetComponentSafe<PlayerInput>();
        }
        private void Start()
        {
            m_wasStartCalled = true;

            m_settings = GameSettings.instance;
            ToggleSubscriptions(true);

            m_playerInp.SwitchCurrentControlScheme(mostRecentControlScheme);

            UpdateDynamicStrings();
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_settings != null)
            {
                m_settings.onSettingsChanged.ToggleSubscription(UpdateDynamicStrings, cond);
            }
        }
        private void UpdateDynamicStrings()
        {
            string t_createCloneSprAtlasText = eInputAction.CreateClone.GetSpriteAtlasTextBasedOnCurrentBindings();
            string t_pauseTimeSprAtlasText = eInputAction.FreezeTime.GetSpriteAtlasTextBasedOnCurrentBindings();
            string t_restartSprAtlasText = eInputAction.Restart.GetSpriteAtlasTextBasedOnCurrentBindings();

            //List<(string actionName, string atlasText)> t_prntList = new List<(string actionName, string atlasText)>();
            //for (int i = 0; i < 27; ++i)
            //{
            //    eInputAction t_curAction = (eInputAction)i;
            //    string t_atlasText = t_curAction.GetSpriteAtlasTextBasedOnCurrentBindings();
            //    t_prntList.Add((t_curAction.ToString(), t_atlasText));
            //}
            //string t_prntStr = "";
            //foreach ((string t_actionName, string t_atlasText) in t_prntList)
            //{
            //    t_prntStr += $"[action: {t_actionName}, atlas:{t_atlasText}], ";
            //}
            //t_prntStr = t_prntStr.Substring(0, t_prntStr.Length - 2);
            //Debug.Log($"{t_prntStr}");


            //Debug.Log($"CreateClonePath ({t_createCloneSprAtlasText}); FreezeTimePath ({t_pauseTimeSprAtlasText}); RestartPath ({t_restartSprAtlasText});");

            m_createCloneControlDynamicString.SetDynamicStringValue(t_createCloneSprAtlasText);
            m_pauseTimeControlDynamicString.SetDynamicStringValue(t_pauseTimeSprAtlasText);
            m_restartControlDynamicString.SetDynamicStringValue(t_restartSprAtlasText);
        }
        private void ResetStringsToDefault()
        {
            m_createCloneControlDynamicString.RemoveDynamicString();
            m_pauseTimeControlDynamicString.RemoveDynamicString();
            m_restartControlDynamicString.RemoveDynamicString();
        }

        #region Input Driven
        private void OnControlsChanged(PlayerInput playerInput)
        {
            // Wait for singletons to initialize themself in case this was called on or before awake (which is definitely possible).
            if (!m_wasStartCalled) { return; }
            mostRecentControlScheme = playerInput.GetCurrentControlScheme().GetPlayerInputName();
            UpdateDynamicStrings();
        }
        #endregion Input Driven
    }
}