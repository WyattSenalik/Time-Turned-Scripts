using System;

using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Helpers.StateManager
{
    /// <summary>
    /// Generic interface for a state manager.
    /// </summary>
    /// <typeparam name="TEnum">The enum used by the state manager.</typeparam>
    public interface IStateManager<TEnum>
        where TEnum : Enum
    {
        public TEnum curState { get; }
        public IEventPrimer<TEnum> onInitialStateSet { get; }
        public IEventPrimer<TEnum, TEnum> onStateChange { get; }
    }
}
