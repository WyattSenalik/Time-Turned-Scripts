using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Moment that holds if an ILeapObject is available or unavailable.
    /// </summary>
    public class LeapAvailableMoment : IMoment<LeapAvailableMoment>
    {
        public float time { get; private set; } = 1.0f;
        /// <summary>
        /// Available/unavailable state during and after the moment.
        /// </summary>
        public bool newState { get; private set; } = false;
        /// <summary>
        /// Available/unavailable state directly before the moment.
        /// </summary>
        public bool prevState { get; private set; } = false;
        /// <summary>
        /// ILeapObject that is available/unavailable at this moment.
        /// </summary>
        public ILeapObject leapObject { get; private set; } = null;


        public LeapAvailableMoment(float occurTime, bool stateAfterMoment, bool stateBeforeMoment, ILeapObject leapObj)
        {
            time = occurTime;
            newState = stateAfterMoment;
            prevState = stateBeforeMoment;
            leapObject = leapObj;
        }


        public LeapAvailableMoment Clone() => new LeapAvailableMoment(time, newState, prevState, leapObject);
        public void Destroy(float destroyTime) { } // Nothing to clean up
        public void Do()
        {
            // Set leap object's available state to the moment's state.
            leapObject.availableToUse = newState;
        }
        public void Undo()
        {
            // Set leap object's available state to the state before the moment.
            leapObject.availableToUse = prevState;
        }
    }
}
