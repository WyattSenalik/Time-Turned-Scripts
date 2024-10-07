using System;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Base class for mono behaviours being <see cref="IScriptedAnimation"/>
    /// so that they may be serialized.
    /// </summary>
    public abstract class ScriptedAnimationMonoBehaviour : MonoBehaviour, IScriptedAnimation
    {
        public abstract event Action onEnd;
        public abstract void Play(bool shouldInterrupt = false);
        public abstract void Stop();
    }
}
