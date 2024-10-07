using System.Collections;
using UnityEngine;

using NaughtyAttributes;
using System;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Programmed
{
    /// <summary>
    /// <see cref="ConvoActionObject"/> that links to a <see cref="IEndpointProgrammedConvoAction"/> that can be programmed to do any special kind of behaviour.
    /// </summary>
    [CreateAssetMenu(fileName = "new ProgrammableAction", menuName = "ScriptableObjects/Dialogue/ConvoActions/ProgrammableAction")]
    public sealed class ProgrammableConvoAction : ConvoActionObject
    {
        // ProgrammableConvoActions always auto advance. Invoking the onFinish action from the custom behaviour is what advances to the next action.
        public override bool autoAdvance => true;
        public string uniqueID => m_uniqueID;

        [SerializeField] private string m_uniqueID = "";

#if UNITY_EDITOR
        [Button]
        private void RandomizeID() => m_uniqueID = UnityEditor.GUID.Generate().ToString();
#endif

        [SerializeField] private bool m_hideDialogue = true;


        private void OnEnable()
        {
#if UNITY_EDITOR
            m_uniqueID = UnityEditor.GUID.Generate().ToString();
#endif
        }

        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            
            ProgrammableConvoActionConnectionManager t_connectionMan = ProgrammableConvoActionConnectionManager.instance;
            if (m_hideDialogue)
            {
                ConversationDriver t_driver = ConversationDriver.GetInstanceSafe();
                if (t_driver.shouldSkip)
                {
                    convoData.dialougeBoxDisplay.Hide();
                    t_connectionMan.TellEndpointToBegin(uniqueID, convoData, onFinished);
                }
                else
                {
                    convoData.dialougeBoxDisplay.Hide(() =>
                    {
                        t_connectionMan.TellEndpointToBegin(uniqueID, convoData, onFinished);
                    });
                }
            }
            else
            {
                t_connectionMan.TellEndpointToBegin(uniqueID, convoData, onFinished);
            }
        }
        public override bool Advance(ConvoData convoData)
        {
            ProgrammableConvoActionConnectionManager t_connectionMan = ProgrammableConvoActionConnectionManager.instance;
            return t_connectionMan.TellEndpointToAdvance(uniqueID, convoData);
        }
    }
}