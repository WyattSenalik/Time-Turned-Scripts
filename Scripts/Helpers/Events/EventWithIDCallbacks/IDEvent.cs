using System;
using System.Collections.Generic;
// Original Authors - Wyatt Senalik

namespace Helpers.Events.IDEvents
{
    /// <summary>
    /// An event that ties its callbacks to ids instead of references to the callbacks themselves.
    /// For when you need to specify actions which are dependent on one another.
    /// </summary>
    //
    // Example of necessariness
    //   int subID = damagingCollider.GetUnusedID();
    //   int amountCallbacks = 0;
    //   // Declare the new callback in advance
    //   Action newContextCallback = () =>
    //   {
    //     int amountHits = damagingCollider.GetAmountHitsSinceEnable();
    //     if (amountHits <= ++amountCallbacks)
    //     {
    //       damagingCollider.UnsubscribeFromOnTriggerEnter(subID);
    //       effectContext.InvokeCallback();
    //     }
    //   };
    //   // Create a new context for the trigger event
    //   SkillEffectContext newContext = new SkillEffectContext(additionalEffect, effectContext.GetSkill(),
    //   effectContext.GetUserPawn(), effectContext.GetTargetPawns(), newContextCallback);
    //   // Action to call when the trigger
    //   Action onTriggerAction = () =>
    //   {
    //     PerformEffect(newContext);
    //   };
    //   // Subscribe to the collision
    //   damagingCollider.SubscribeToOnTriggerEnter(subID, onTriggerAction);
    public class IDEvent : IIDEvent
    {
        // Dictionary containing the callbacks
        private readonly Dictionary<int, Action> m_onTriggerEnterActions = null;
        // Handling if callbacks try to change the dictionary
        private bool m_isExecutingActions = false;
        private readonly List<int> m_actionsToUnsub = null;
        private readonly List<Tuple<int, Action>> m_actionsToSub = null;

        private readonly IDLibrary m_idLib = null;


        public IDEvent()
        {
            m_onTriggerEnterActions = new Dictionary<int, Action>();

            m_isExecutingActions = false;
            m_actionsToUnsub = new List<int>();
            m_actionsToSub = new List<Tuple<int, Action>>();

            m_idLib = new IDLibrary();
        }


        /************************************************************************************************************************/
        #region IIDEvent
        public void Invoke()
        {
            // Execute each action and mark it as executing so that no
            // actions may be added or removed during execution
            m_isExecutingActions = true;
            foreach (KeyValuePair<int, Action> pair in m_onTriggerEnterActions)
            {
                Action action = pair.Value;
                action?.Invoke();
            }
            m_isExecutingActions = false;
            // Sub actions from the invocations
            foreach (Tuple<int, Action> tuple in m_actionsToSub)
            {
                m_onTriggerEnterActions.Add(tuple.Item1, tuple.Item2);
            }
            m_actionsToSub.Clear();
            // Unsub actions from the invocations
            foreach (int id in m_actionsToUnsub)
            {
                m_onTriggerEnterActions.Remove(id);
            }
            m_actionsToUnsub.Clear();
        }

        /************************************************************************************************************************/
        #region IReadOnlyIDEvent
        public int GetUnusedID() => m_idLib.CheckoutID();
        public int Subscribe(Action triggerAction)
        {
            int t_id = GetUnusedID();
            Subscribe(t_id, triggerAction);
            return t_id;
        }
        public void Subscribe(int id, Action triggerAction)
        {
            // If we are currently executing actions, we cannot add
            // the action right away and must wait
            if (m_isExecutingActions)
            {
                m_actionsToSub.Add(new Tuple<int, Action>(id, triggerAction));
            }
            // Add it if we are not currently executing actions
            else
            {
                m_onTriggerEnterActions.Add(id, triggerAction);
            }
        }
        public void Unsubscribe(int id)
        {
            // If we are currently executing actions, we cannot remove
            // the action right away and must wait
            if (m_isExecutingActions)
            {
                m_actionsToUnsub.Add(id);
            }
            // Remove it if we are not currently executing actions
            else
            {
                m_onTriggerEnterActions.Remove(id);
            }

            // Return the id.
            m_idLib.ReturnID(id);
        }
        #endregion IReadOnlyIDEvent
        /************************************************************************************************************************/

        #endregion IEventWithIDCallbacks
        /************************************************************************************************************************/
    }
}
