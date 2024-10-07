using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Common curves for run speed.
    /// </summary>
    public interface IRunCurves
    {
        /// <summary>How long (in seconds) since the run began until top speed is reached.</summary>
        public float accelTime { get; }
        /// <summary>How long (in seconds) since the run input ended until movement is completely stopped.</summary>
        public float decelTime { get; }
        /// <summary>Fastest speed (in unity units per second) the run should reach.</summary>
        public float topSpeed { get; }
        public AnimationCurve accelCurve { get; }
        public AnimationCurve decelCurve { get; }

        /// <summary>
        /// Determines how fast the run should be at the current time (if still accelerating or at top speed).
        /// </summary>
        /// <param name="beginRunTime">Time the run began.</param>
        /// <param name="curTime">Current time. Must be after or equal to <paramref name="beginRunTime"/>.</param>
        /// <returns>Speed the run should be.</returns>
        public float EvaluateAccelMove(float beginRunTime, float curTime);
        /// <summary>
        /// Determines how fast the slow down from the run should be at the current time (if still decelerating or finished and should be stopped (0 speed).
        /// </summary>
        /// <param name="beginDecel">Time input for the run ended.</param>
        /// <param name="curTime">Current time. Must be after or equal to <paramref name="beginDecel"/>.</param>
        /// <returns>Speed the character should be sliding after the run.</returns>
        public float EvaluateDecel(float beginDecel, float curTime);
    }
}