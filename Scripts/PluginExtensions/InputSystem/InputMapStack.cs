using System.Collections.Generic;
// Original Authors - Wyatt Senalik and Eslis Vang
// Borrowed code from SquaredUp

namespace UnityEngine.InputSystem.Extension
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PlayerInput))]
    public sealed class InputMapStack : MonoBehaviour
    {
        private const bool IS_DEBUGGING = false;

        public string defaultMapName => m_defaultMapName;
        public PlayerInput playerInput => m_playerInputRef;
        public string activeMap => m_activeMapNames.Count > 0 ? m_activeMapNames[^1] : "";

        // Name of the default map
        [SerializeField] private string m_defaultMapName = "Player";
        // Reference to the player input
        private PlayerInput m_playerInputRef = null;

        // Stack of active input maps.
        private List<string> m_activeMapNames = new List<string>();


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_playerInputRef = GetComponent<PlayerInput>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_playerInputRef, this);
            #endregion Asserts
            ResetInputMap();
        }


        /// <summary>Adds the given input map to the stack and swaps to it.</summary>
        /// <param name="inputMapName">Name of the input map to add to the stack.</param>
        public void SwitchInputMap(string inputMapName)
        {
            #region Logs
            //CustomDebug.LogForComponent($"SwitchInputMap({inputMapName})", this, IS_DEBUGGING);
            #endregion Logs
            m_activeMapNames.Add(inputMapName);
            UpdateActiveInputMap();
        }
        /// <summary>Pops the input map with the given name off the stack and updates the input map to the one before it.</summary>
        /// <param name="inputMapName">Name of the input map to remove from the stack.</param>
        public void PopInputMap(string inputMapName)
        {
            #region Logs
            //CustomDebug.LogForComponent($"PopInputMap({inputMapName})", this, IS_DEBUGGING);
            #endregion Logs
            m_activeMapNames.Remove(inputMapName);
            UpdateActiveInputMap();
        }
        /// <summary>Resets the input map to just be the starting input map.</summary>
        public void ResetInputMap()
        {
            #region Logs
            //CustomDebug.LogForComponent($"ResetInputMap", this, IS_DEBUGGING);
            #endregion Logs
            // Initialize active map names to have the default map in it.
            m_activeMapNames = new List<string>(new string[] { m_defaultMapName });
            UpdateActiveInputMap();
        }
        /// <summary>Gets the name of the currently active map. Map at top of stack.</summary>
        public string GetActiveInputMapName()
        {
            #region Logs
            //CustomDebug.LogForComponent($"GetActiveInputMapName", this, IS_DEBUGGING);
            #endregion Logs
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_activeMapNames.Count > 0, $"to have at least 1 active map.", this);
            #endregion Asserts
            return m_activeMapNames[m_activeMapNames.Count - 1];
        }

        /// <summary>Updates the action map to be the map at the top of the stack.</summary>
        private void UpdateActiveInputMap()
        {
            if (m_activeMapNames.Count > 0)
            {
                string t_mapToChangeTo = m_activeMapNames[m_activeMapNames.Count - 1];
                m_playerInputRef.SwitchCurrentActionMap(t_mapToChangeTo);
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(m_playerInputRef.currentActionMap.name == t_mapToChangeTo, $"Was not able to change action map to {t_mapToChangeTo}", this);
                #endregion Asserts
            }
            else
            {
                #region Logs
                //CustomDebug.LogErrorForComponent("Last input map was removed from the stack", this);
                #endregion Logs
            }
        }
    }
}
