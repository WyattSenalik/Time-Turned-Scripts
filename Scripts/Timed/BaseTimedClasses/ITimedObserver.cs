// Original Authors - Wyatt Senalik

namespace Timed
{
    public interface ITimedObserver : ITimedComponent
    {
        /// <summary>
        /// Called every frame by <see cref="ITimedObject"/> when recording.
        /// </summary>
        /// <param name="deltaTime">Time since the last TimedUpdate.</param>
        void TimedUpdate(float deltaTime);
    }
}