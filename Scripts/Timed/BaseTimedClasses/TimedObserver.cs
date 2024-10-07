// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Implementation of <see cref="ITimedObserver"/> that can be extended to get 
    /// some easy references to the <see cref="ITimedObject"/> that this script is
    /// attached to. Also makes the various functions required by 
    /// <see cref="ITimedObserver"/> optional (virtual).
    /// </summary>
    public abstract class TimedObserver : TimedComponent, ITimedObserver
    {
        public virtual void TimedUpdate(float deltaTime) { }
    }
}
