using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Branching
{
    /// <summary>
    /// Convo action that branches the current conversation into 2 paths.
    /// </summary>
    [CreateAssetMenu(fileName = "new Branch", menuName = "ScriptableObjects/Dialogue/ConvoActions/Branch")]
    public sealed class BranchConvoAction : ConvoActionObject
    {
        // Branches can't autoadvance, it just doesn't make sense, they aren't themselves the thing that will act, their containing actions are.
        public override bool autoAdvance => false;
        public string conditionID => m_conditionRefID;
        public bool defaultBranchValue => m_defaultBranchValue;
        public IReadOnlyList<ConvoActionObject> path1 => new ReadOnlyCollection<ConvoActionObject>(m_pathA);
        public IReadOnlyList<ConvoActionObject> path2 => new ReadOnlyCollection<ConvoActionObject>(m_pathB);


        [SerializeField] private string m_conditionRefID = Guid.NewGuid().ToString();
        [SerializeField] private bool m_defaultBranchValue = false;
        #region Editor
        [Button] private void RandomizeID() => m_conditionRefID = Guid.NewGuid().ToString();
        #endregion Editor

        [SerializeField, Expandable] private ConvoActionObject[] m_pathA = new ConvoActionObject[1];
        [SerializeField, Expandable] private ConvoActionObject[] m_pathB = new ConvoActionObject[1];


        public override void Begin(ConvoData convoData, Action onFinished = null)
        {
            BranchController t_branchCont = BranchController.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_branchCont, this);
            #endregion Asserts
            t_branchCont.Begin(this, convoData, onFinished);
        }
        public override bool Advance(ConvoData convoData)
        {
            BranchController t_branchCont = BranchController.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(t_branchCont, this);
            #endregion Asserts
            return t_branchCont.Advance();
        }
    }
}