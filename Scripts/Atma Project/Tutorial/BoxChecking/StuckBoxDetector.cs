using System.Collections.Generic;
using UnityEngine;

using Timed;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider2D))]
    public sealed class StuckBoxDetector : TimedRecorder
    {
        public IEventPrimer onAmountBoxesContainedChanged => m_onAmountBoxesContainedChanged;

        [SerializeField, Min(1)] private int m_amountBoxesUntilStuck = 1;
        [SerializeField] private MixedEvent m_onAmountBoxesContainedChanged = new MixedEvent();

        private readonly List<PushBox> m_curContainedBoxes = new List<PushBox>();


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!IsOtherBox(other, out PushBox t_box)) { return; }
            SetBoxAsContained(t_box);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsOtherBox(other, out PushBox t_box)) { return; }
            SetBoxAsContained(t_box);
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (!IsOtherBox(other, out PushBox t_box)) { return; }
            RemoveBoxFromContained(t_box);
        }


        public bool DoesDetectStuckBox()
        {
            return m_curContainedBoxes.Count >= m_amountBoxesUntilStuck;
        }


        private bool IsOtherBox(Collider2D other, out PushBox box)
        {
            Rigidbody2D t_rb = other.attachedRigidbody;
            if (t_rb == null)
            {
                box = null;
                return false;
            }
            if (t_rb.TryGetComponent(out box))
            {
                return true;
            }
            return false;
        }
        private void SetBoxAsContained(PushBox box)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(box != null, $"box would be non-null when set as contained.", this);
            #endregion Asserts
            if (!m_curContainedBoxes.Contains(box))
            {
                m_curContainedBoxes.Add(box);
                m_onAmountBoxesContainedChanged.Invoke();
            }
        }
        private void RemoveBoxFromContained(PushBox box)
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(box != null, $"box would be non-null when no longer contained.", this);
            #endregion Asserts
            m_curContainedBoxes.Remove(box);
            m_onAmountBoxesContainedChanged.Invoke();
        }
        
    }
}