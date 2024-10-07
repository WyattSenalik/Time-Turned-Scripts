using UnityEngine;
using UnityEngine.InputSystem;

using Helpers.Events;
using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// A way to reference the player. Tags weren't working since we started using the "Player" tag for physics.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerSingleton : SingletonMonoBehaviour<PlayerSingleton>
    {
        public IEventPrimer<PlayerInput> onControlsChanged => m_onControlsChanged;

        [SerializeField] private MixedEvent<PlayerInput> m_onControlsChanged = new MixedEvent<PlayerInput>();

        #region Input Events
        private void OnControlsChanged(PlayerInput playerInp)
        {
            m_onControlsChanged.Invoke(playerInp);
        }
        #endregion Input Events
    }
}