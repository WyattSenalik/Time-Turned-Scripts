using System;
using System.Collections.Generic;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Dialogue.ConvoActions.Branching
{
    /// <summary>
    /// Controls branches. Similar to ConverstationDriver, but with branches.
    /// </summary>
    public sealed class BranchController : DynamicSingletonMonoBehaviour<BranchController>
    {
        private const bool IS_DEBUGGING = false;

        private ConditionManager m_conditionManager = null;
        private ConversationDriver m_convoDriver = null;

        private readonly Stack<Branch> m_branchStack = new Stack<Branch>();
        private ConvoData m_convoData = null;


        protected override void OnSingletonCreated()
        {
            base.OnSingletonCreated();

            m_conditionManager = ConditionManager.instance;
            m_convoDriver = ConversationDriver.GetInstanceSafe();
        }


        public void Begin(BranchConvoAction branchAction, ConvoData convoData, Action onFinished)
        {
            m_convoData = convoData;
            bool t_condition = m_conditionManager.GetCondition(branchAction);
            IReadOnlyList<ConvoActionObject> t_path = t_condition ? branchAction.path1 : branchAction.path2;
            Branch t_branchStackedInfo = new Branch(t_path, onFinished
#if UNITY_EDITOR
                ,branchAction.name
#endif
                );

            m_branchStack.Push(t_branchStackedInfo);
            DoNextAction();
        }
        public bool Advance()
        {
            // Are no more branches left.
            if (m_branchStack.Count == 0)
            {
                return true;
            }
            // All branches are finished.
            bool t_areAllBranchesFin = true;
            foreach (Branch t_curBranch in m_branchStack)
            {
                if (!t_curBranch.IsFinished())
                {
                    t_areAllBranchesFin = false;
                    break;
                }
            }
            if (t_areAllBranchesFin)
            {
                return true;
            }

            // Not done with the branch, so keep going with branch actions.
            return AdvanceBranchAction();
        }


        private bool DoNextAction()
        {
            if (!m_branchStack.TryPeek(out Branch t_curBranch))
            {
                #region Logs
                //CustomDebug.LogForComponent($"Failed to peek. We've run out of branches. Should be advacing.", this, IS_DEBUGGING);
                #endregion Logs
                // Fail to peek because there are not enough branches, so we can advance.
                return true;
            }
            else if (t_curBranch.IsFinished())
            {
                m_branchStack.Pop();
                if (m_branchStack.Count > 0)
                {
                    // Must move the previous branch's action index up 1.
                    Branch t_nextBranch = m_branchStack.Peek();
                    t_nextBranch.IncrementActionIndex();
                }
                t_curBranch.onFinished?.Invoke();
                // Check if we need to advance
                return DoNextAction();
            }
            else
            {
                ConvoActionObject t_action = t_curBranch.GetCurAction();
                #region Logs
                //CustomDebug.LogForComponent($"New action beginning ({t_action.name}).", this, IS_DEBUGGING);
                #endregion Logs
                #region Asserts
                //CustomDebug.AssertIsTrueForComponent(t_action != null, $"Convo Action specified in a conversation branch with index {t_curBranch.curIndex} to not be null.", this);
                #endregion Asserts
                Action t_onActionFinished = t_action.autoAdvance ? () => m_convoDriver.AdvanceDialogue() : null;
                t_action.Begin(m_convoData, t_onActionFinished);
            }
            return false;
        }
        private bool AdvanceBranchAction()
        {
            Branch t_curBranch = m_branchStack.Peek();
            ConvoActionObject t_action = t_curBranch.GetCurAction();
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(t_action != null, $"Convo Action specified in a conversation branch with index {t_curBranch.curIndex} to not be null.", this);
            #endregion Asserts
            if (t_action.Advance(m_convoData))
            {
                t_curBranch.IncrementActionIndex();
                #region Logs
                //CustomDebug.LogForComponent($"Advancing from the cur action ({t_action.name}) to the next one.", this, IS_DEBUGGING);
                #endregion Logs
                // Tell main Advance only to advance if there is no next action.
                return DoNextAction();
            }

            return false;
        }


        public sealed class Branch
        {
            public int curIndex { get; private set; }
#if UNITY_EDITOR
            public string name { get; private set; }
#endif

            public IReadOnlyList<ConvoActionObject> path { get; private set; }
            public Action onFinished { get; private set; }


            public Branch(IReadOnlyList<ConvoActionObject> path, Action onFinished
                #if UNITY_EDITOR
                ,string name
                #endif
                )
            {
                curIndex = 0;
                this.path = path;
                this.onFinished = onFinished;

#if UNITY_EDITOR
                this.name = name;
#endif
            }


            public bool IsFinished()
            {
                return curIndex >= path.Count;
            }
            public ConvoActionObject GetCurAction()
            {
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(curIndex, path, this);
                #endregion Asserts
                return path[curIndex];
            }
            public void IncrementActionIndex()
            {
                ++curIndex;
            }
        }
    }
}