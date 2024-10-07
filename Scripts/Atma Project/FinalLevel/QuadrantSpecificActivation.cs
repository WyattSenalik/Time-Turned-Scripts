using System.Collections;
using System.Linq;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Extensions;
using Timed;

using static Atma.EnterNextQuadrantDetector;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimedObject))]
    public sealed class QuadrantSpecificActivation : MonoBehaviour
    {
        // Thought this was a good idea at the time, but its not.
        //
        //[SerializeField] private eQuadrant[] m_quadrantsToBeActiveIn = new eQuadrant[0];

        //private EnterNextQuadrantDetector m_nextQuadDetector = null;
        //private TimedObject m_timedObj = null;

        //[SerializeField, ReadOnly] private int m_turnOffRequestID = -1;


        //private void Awake()
        //{
        //    m_timedObj = this.GetComponentSafe<TimedObject>(this);
        //}
        //private IEnumerator Start()
        //{
        //    m_nextQuadDetector = EnterNextQuadrantDetector.instance;
        //    #region Asserts
        //    //CustomDebug.AssertSerializeFieldIsNotNull(m_nextQuadDetector, nameof(m_nextQuadDetector), this);
        //    #endregion Asserts
        //    ToggleSubscriptions(true);

        //    // Wait a frame so that the next quad detecter can grab a reference to the player
        //    yield return null;
        //    UpdateIfActive(m_nextQuadDetector.DetermineQuadrantPlayerIsIn());
        //}
        //private void OnDestroy()
        //{
        //    ToggleSubscriptions(false);
        //}


        //private void ToggleSubscriptions(bool toggle)
        //{
        //    if (m_nextQuadDetector != null)
        //    {
        //        m_nextQuadDetector.onBegunQuadrantTransition.ToggleSubscription(OnBegunQuadrantTransition, toggle);
        //        m_nextQuadDetector.onQuadrantTransitionEnded.ToggleSubscription(OnQuadrantTransitionEnded, toggle);
        //    }
        //}
        //private void OnBegunQuadrantTransition(QuadrantChangedEventData eventData)
        //{
        //    UpdateIfActive(eventData.oldQuadrant, eventData.newQuadrant);
        //}
        //private void OnQuadrantTransitionEnded(eQuadrant quadrantPlayerIsIn)
        //{
        //    UpdateIfActive(quadrantPlayerIsIn);
        //}


        //private void UpdateIfActive(params eQuadrant[] quadrants)
        //{
        //    bool t_shouldActivate = false;
        //    for (int i = 0; i < quadrants.Length; i++)
        //    {
        //        if (ShouldBeActiveInQuadrant(quadrants[i]))
        //        {
        //            t_shouldActivate = true;
        //        }
        //    }
        //    if (t_shouldActivate)
        //    {
        //        TurnOnObject();
        //    }
        //    else
        //    {
        //        TurnOffObject();
        //    }
        //}
        //private bool ShouldBeActiveInQuadrant(eQuadrant quadrantToCheck)
        //{
        //    return m_quadrantsToBeActiveIn.Contains(quadrantToCheck);
        //}
        //private void TurnOffObject()
        //{
        //    if (m_turnOffRequestID == -1)
        //    {
        //        m_turnOffRequestID = m_timedObj.RequestSuspendRecording();
        //    }
        //}
        //private void TurnOnObject()
        //{
        //    if (m_turnOffRequestID != -1)
        //    {
        //        m_timedObj.CancelSuspendRecordingRequest(m_turnOffRequestID);
        //        m_turnOffRequestID = -1;
        //    }
        //}
    }
}