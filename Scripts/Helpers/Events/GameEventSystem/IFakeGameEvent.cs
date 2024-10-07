// Original Authors - Wyatt Senalik

namespace Helpers.Events.GameEventSystem.Internal
{
    /// <summary>
    /// Fake version of <see cref="IGameEvent"/> to temporarily hold
    /// callbacks if something tries to subscribe to an event before that
    /// event is created.
    /// </summary>
    public interface IFakeGameEvent : IReadOnlyGameEvent
    {
        /// <summary>
        /// Creates a real event with the same ID as the fake event. DOES NOT TRANSFER CALLBACKS.
        /// </summary>
        IGameEvent CreateReal();
        /// <summary>
        /// Clears all the callbacks being held 
        /// by the <see cref="IFakeGameEvent"/>.
        /// </summary>
        void Clear();
        /// <summary>
        /// Transfers the callbacks from the fake event to the given real one.
        /// </summary>
        void TransferCallbacksToRealEvent(IGameEvent realEventToTransferTo);
    }

    /// <summary>
    /// Fake version of <see cref="IGameEvent{T}"/> to temporarily hold
    /// callbacks if something tries to subscribe to an event before that
    /// event is created.
    /// </summary>
    public interface IFakeGameEvent<T> : IReadOnlyGameEvent<T>
    {
        /// <summary>
        /// Creates a real event with the same ID as the fake event. DOES NOT TRANSFER CALLBACKS.
        /// </summary>
        IGameEvent<T> CreateReal();
        /// <summary>
        /// Clears all the callbacks being held 
        /// by the <see cref="IFakeGameEvent{T}"/>.
        /// </summary>
        void Clear();
        /// <summary>
        /// Transfers the callbacks from the fake event to the given real one.
        /// </summary>
        void TransferCallbacksToRealEvent(IGameEvent<T> realEventToTransferTo);
    }
}