using System;
using UnityEngine;

using NaughtyAttributes;

using Dialogue.ConvoActions.Programmed;
using Dialogue;
using Timed;
using UnityEngine.InputSystem.Extension;
using System.Collections;
using Atma.Dialogue;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma.Tutorial.Clone
{
    [DisallowMultipleComponent]
    public sealed class CloneMadeSuccessCloneTut : MonoBehavEndpointProgrammedConvoAction
    {
        public bool hasBegun { get; private set; }

        [SerializeField, Required] private SrAgentMover m_srAgentMover = null;
        [SerializeField, Required] private TimeRewinder m_timeRewinder = null;
        [SerializeField, Required] private InputMapStack m_inpMapStack = null;

        [SerializeField] private string m_movingInpMapName = "Default";

        private ConversationSkipper m_convoSkipper = null;
        private TimedObject m_srAgentTimedObj = null;


        protected override void Awake()
        {
            base.Awake();

            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_srAgentMover, nameof(m_srAgentMover), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeRewinder, nameof(m_timeRewinder), this);
            #endregion Asserts

            m_srAgentTimedObj = m_srAgentMover.GetComponentSafe<TimedObject>(this);
        }
        private void Start()
        {
            m_convoSkipper = ConversationSkipper.GetInstanceSafe(this);
        }


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            // NOT SAVING onFinished because last action.

            // Unfreeze time
            m_timeRewinder.CancelRewind();

            // Senior Agent goes through portal.
            if (!m_convoSkipper.ShouldSkipDialogue())
            {
                m_srAgentMover.PlayMoveAnimation(2, () =>
                {
                    StartCoroutine(GivePlayerControllerAfterFrameCoroutine());
                    IEnumerator GivePlayerControllerAfterFrameCoroutine()
                    {
                        yield return new WaitForFixedUpdate();
                        RestorePlayerControl();
                    }
                });
            }
            else
            {
                m_srAgentTimedObj.RequestSuspendRecording();
                RestorePlayerControl();
            }

            hasBegun = true;
        }
        public override bool Advance(ConvoData convoData)
        {
            // Last action, so it doesn't matter (nothing to advance to).
            return false;
        }


        private void RestorePlayerControl()
        {
            // Let player move again
            m_inpMapStack.SwitchInputMap(m_movingInpMapName);
            // Make it so player can't rewind before this point because recording Senior Agent's movement / making it so he can resay dialogue is annoying. We will playtest and see what people think first.
            m_timeRewinder.SetEarliestTime(m_timeRewinder.curTime);
        }
    }
}