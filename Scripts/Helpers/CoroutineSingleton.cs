using System;
using System.Collections;

using Helpers.Singletons;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers
{
    /// <summary>
    /// DynamicSingletonMonoBehaviour that you can use to call StartCoroutine and StopCoroutine on.
    /// </summary>
    public sealed class CoroutineSingleton : DynamicSingletonMonoBehaviour<CoroutineSingleton>
    {
        public void InvokeAfterSingleFrameDelay(Action action)
        {
            StartCoroutine(InvokeActionAfterSingleFrameDelayCoroutine(action));
        }
        public void InvokeAfterWaitForFixedUpdateDelay(Action action)
        {
            StartCoroutine(InvokeAfterWaitForFixedUpdateDelayCoroutine(action));
        }

        private IEnumerator InvokeActionAfterSingleFrameDelayCoroutine(Action action)
        {
            yield return null;
            action?.Invoke();
        }
        private IEnumerator InvokeAfterWaitForFixedUpdateDelayCoroutine(Action action)
        {
            yield return new WaitForFixedUpdate();
            yield return null;
            action?.Invoke();
        }
    }
}