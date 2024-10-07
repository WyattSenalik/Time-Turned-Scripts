using System.Collections;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.UnityInterfaces
{
    /// <summary>
    /// Interface for <see cref="MonoBehaviour"/> if a new MonoBehaviour script is
    /// created with its own interface and you want to be able to do MonoBehaviour
    /// things from that interface.
    /// </summary>
    public interface IMonoBehaviour : IBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// Allow a specific instance of a MonoBehaviour to
        /// run in edit mode (only available in the editor).
        /// </summary>
        public bool runInEditMode { get; set; }
#endif //UNITY_EDITOR
        /// <summary>
        /// Disabling this lets you skip the GUI layout phase.
        /// </summary>
        public bool useGUILayout { get; set; }

        /// <summary>
        /// Cancels all Invoke calls on this MonoBehaviour.
        /// </summary>
        public void CancelInvoke();
        /// <summary>
        /// Invokes the method methodName in time seconds.
        /// 
        /// If time is set to 0, the method is invoked at the next Update cycle.
        /// In this case, it's better to call the function directly.
        /// 
        /// For better performance and maintability, use Coroutines instead.
        /// </summary>
        public void Invoke(string methodName, float time);
        /// <summary>
        /// Invokes the method methodName in time seconds,
        /// then repeatedly every repeatRate seconds.
        /// 
        /// Note : This does not work if you set the time scale to 0.
        /// </summary>
        public void InvokeRepeating(string methodName, float time, float repeatRate);
        /// <summary>
        /// Is any invoke on methodName pending?
        /// </summary>
        public bool IsInvoking(string methodName);
        /// <summary>
        /// Starts a Coroutine.
        /// 
        /// The execution of a coroutine can be paused at any point using the
        /// yield statement. When a yield statement is used, the coroutine pauses
        /// execution and automatically resumes at the next frame. See the Coroutines
        /// documentation for more details.
        /// Coroutines are excellent when modeling behavior over several frames.
        /// The StartCoroutine method returns upon the first yield return, however
        /// you can yield the result, which waits until the coroutine has finished
        /// execution. There is no guarantee coroutines end in the same order they
        /// started, even if they finish in the same frame.
        ///
        /// Yielding of any type, including null, results in the execution coming
        /// back on a later frame, unless the coroutine is stopped or has completed.
        /// Note: You can stop a coroutine using MonoBehaviour.StopCoroutine and
        /// MonoBehaviour.StopAllCoroutines.Coroutines are also stopped when the
        /// MonoBehaviour is destroyed or if the GameObject the MonoBehaviour is
        /// attached to is disabled. Coroutines are not stopped when a MonoBehaviour
        /// is disabled.
        ///
        /// See also: <seealso cref="Coroutine"/>, <seealso cref="YieldInstruction"/>
        /// </summary>
        public Coroutine StartCoroutine(IEnumerator routine);
        /// <summary>
        /// Starts a coroutine named methodName.
        /// 
        /// In most cases you want to use the StartCoroutine variation above.
        /// However StartCoroutine using a string method name lets you use
        /// StopCoroutine with a specific method name. The downside is that the
        /// string version has a higher runtime overhead to start the coroutine and
        /// you can pass only one parameter.
        /// </summary>
        public Coroutine StartCoroutine(string methodName, object value = null);
        /// <summary>
        /// Stops all coroutines running on this behaviour.
        /// 
        /// A MonoBehaviour can execute zero or more coroutines. Created coroutines
        /// can execute for a range of times. In the script example below two
        /// coroutines are created and run without stopping.However, StopAllCoroutines
        /// is used to stop both of them. This action can be made to happen on a script
        /// that runs multiple coroutines. No arguments are needed because all
        /// coroutines on a script are stopped.
        /// Note: StopAllCoroutines operates only on the one script it is attached to.
        /// </summary>
        public void StopAllCoroutines();
        /// <summary>
        /// Stops the first coroutine named methodName, or
        /// the coroutine stored in routine running on this behaviour.
        ///
        /// StopCoroutine takes one of three arguments which specify which
        /// coroutine is stopped:
        ///     A string function naming the active coroutine.
        ///     The IEnumerator variable used earlier to create the coroutine.
        ///     The Coroutine to stop the manually created Coroutine.
        ///
        /// Note: Do not mix the three arguments. If a string is used as the argument
        /// in StartCoroutine, use the string in StopCoroutine.Similarly, use the
        /// IEnumerator in both StartCoroutine and StopCoroutine. Finally, use
        /// StopCoroutine with the Coroutine used for creation.
        /// </summary>
        /// <param name="methodName">Name of coroutine.</param>
        public void StopCoroutine(string methodName);
        /// <summary>
        /// Stops the first coroutine named methodName, or
        /// the coroutine stored in routine running on this behaviour.
        ///
        /// StopCoroutine takes one of three arguments which specify which
        /// coroutine is stopped:
        ///     A string function naming the active coroutine.
        ///     The IEnumerator variable used earlier to create the coroutine.
        ///     The Coroutine to stop the manually created Coroutine.
        ///
        /// Note: Do not mix the three arguments. If a string is used as the argument
        /// in StartCoroutine, use the string in StopCoroutine.Similarly, use the
        /// IEnumerator in both StartCoroutine and StopCoroutine. Finally, use
        /// StopCoroutine with the Coroutine used for creation.
        /// </summary>
        /// <param name="routine">Name of the function in code,
        /// including coroutines.</param>
        public void StopCoroutine(IEnumerator routine);
        /// <summary>
        /// Stops the first coroutine named methodName, or
        /// the coroutine stored in routine running on this behaviour.
        ///
        /// StopCoroutine takes one of three arguments which specify which
        /// coroutine is stopped:
        ///     A string function naming the active coroutine.
        ///     The IEnumerator variable used earlier to create the coroutine.
        ///     The Coroutine to stop the manually created Coroutine.
        ///
        /// Note: Do not mix the three arguments. If a string is used as the argument
        /// in StartCoroutine, use the string in StopCoroutine.Similarly, use the
        /// IEnumerator in both StartCoroutine and StopCoroutine. Finally, use
        /// StopCoroutine with the Coroutine used for creation.
        /// </summary>
        /// <param name="routine">Name of the function in code,
        /// including coroutines.</param>
        public void StopCoroutine(Coroutine routine);
    }
}
