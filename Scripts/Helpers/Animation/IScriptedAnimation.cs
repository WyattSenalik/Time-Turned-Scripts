using System;
// Original Authors - Eslis Vang and Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Base class to extend when wanting to make an
    /// programmable animation.
    /// </summary>
    public interface IScriptedAnimation
    {
        public event Action onEnd;

        /// <summary>
        /// Starts the animation.
        /// </summary>
        /// <param name="shouldInterrupt">If the animation is currently playing,
        /// should it interrupt itself?</param>
        void Play(bool shouldInterrupt = false);
        /// <summary>
        /// Stops the animation if it is running.
        /// </summary>
        void Stop();
    }
}
