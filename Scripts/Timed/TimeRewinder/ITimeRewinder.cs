using Helpers.Events;
using Helpers.UnityInterfaces;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Allows for manipulation of time going forward, backward, and
    /// standing still (pausing).
    /// </summary>
    public interface ITimeRewinder : IMonoBehaviour
    {
        /// <summary>
        /// Event called when the rewind starts.
        /// </summary>
        IEventPrimer onRewindBegin { get; }
        /// <summary>
        /// Event called when the time rewinder reaches the earliest time.
        /// </summary>
        IEventPrimer onBeginReached { get; }
        /// <summary>
        /// Event called when the time rewinder reaches the farthest time.
        /// </summary>
        IEventPrimer onEndReached { get; }
        /// <summary>
        /// Event called when the navigation direction changes. Parameter is the new navigation direction.
        /// </summary>
        IEventPrimer<float> onNavDirChanged { get; }

        /// <summary>
        /// If this time rewinder is currently manipulating time.
        /// If <see cref="StartRewind"/> has been called and
        /// <see cref="CancelRewind"/> has not been called after.
        /// </summary>
        bool hasStarted { get; }
        /// <summary>
        /// Earliest time the time rewinder is allowed to rewind to. Default is 0.
        /// </summary>
        float earliestTime { get; }
        /// <summary>
        /// The furthest time the time rewinder is allowed to go until.
        /// Specified in <see cref="StartRewind"/> as an optional parameter.
        /// </summary>
        float farthestTime { get; }
        /// <summary>
        /// Which direction in time the rewinder is moving. 
        /// Negative is rewinding. Positive is fast forwarding. Zero is paused.
        /// </summary>
        float navigationDir { get; }
        /// <summary>
        /// Current time that is being manipulated.
        /// </summary>
        float curTime { get; }

        /// <summary>
        /// Begins the manipulation of time in the direction of the given
        /// navigation direction.
        /// </summary>
        /// <param name="startNavDir">Positive values go forward in time 
        /// (fast forward). Negative values decrease time (rewind). 
        /// Zero will pause time.</param>
        void StartRewind(float startNavDir = 0.0f, float? farthestTime = null, float?
            earliestTime = null);
        /// <summary>
        /// Stops the manipulation of time and returns 
        /// time to its normal flow.
        /// </summary>
        void CancelRewind();
        /// <summary>
        /// Changes the current navigation direction to the given value.
        /// <see cref="StartRewind"/> must be called first for 
        /// this to have any effect.
        /// </summary>
        /// <param name="navDir">Positive values go forward in time 
        /// (fast forward). Negative values decrease time (rewind). 
        /// Zero will pause time.</param>
        void ChangeNavigationDirection(float navDir);
        /// <summary>
        /// Sets the current time to the given value.
        /// <see cref="StartRewind"/> must be called first for this to have any effect.
        /// </summary>
        /// <param name="time">Time to set to. Assumes it is between 0 and the specified 'max' time.</param>
        void ForceSetTime(float time);
    }
}
