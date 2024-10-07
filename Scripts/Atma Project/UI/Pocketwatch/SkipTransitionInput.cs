using UnityEngine;
using UnityEngine.InputSystem;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Input for ending the stopwatch/timeline transition early.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BranchPlayerController))]
    public sealed class SkipTransitionInput : MonoBehaviour
    {
        public BranchPlayerController playerCont { get; private set; }


        private void Awake()
        {
            playerCont = GetComponent<BranchPlayerController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(playerCont, this);
            #endregion Asserts
        }


        #region InputMessages
        private void OnSkipTransition(InputValue value)
        {
            if (value.isPressed)
            {
                if (!playerCont.isFirstPause || playerCont.isTransitionToStopwatchPlaying)
                {
                    playerCont.EndTransitionEarly();
                }
            }
        }
        #endregion InputMessages
    }
}