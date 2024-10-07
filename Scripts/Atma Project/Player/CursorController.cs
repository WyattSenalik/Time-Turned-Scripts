using System.Collections.Generic;
using UnityEngine;

using Helpers;
using Helpers.UI;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class CursorController : TimedRecorder
    {
        [SerializeField] private CursorSpecs m_defaultCursor = null;
        [SerializeField] private CursorSpecs m_holingObjCursor = null;
        [SerializeField] private CursorSpecs m_timeSusCursor = null;

        private GamePauseController m_gamePauseCont = null;
        private PickupController m_pickupCont = null;

        private readonly List<CursorSpecs> m_cursorStack = new List<CursorSpecs>();


        protected override void Awake()
        {
            base.Awake();

            // Okay if its null, it just means we won't subscribe.
            m_pickupCont = GetComponent<PickupController>();
        }
        private void Start()
        {
            m_gamePauseCont = GamePauseController.instance;

            ToggleSubscriptions(true);
            AddCursorSpecsToStack(m_defaultCursor);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        private void AddCursorSpecsToStack(CursorSpecs specs)
        {
            m_cursorStack.Add(specs);
            UpdateCursor();
        }
        private void RemoveCursorFromStack(CursorSpecs specs)
        {
            int t_lastIndex = m_cursorStack.FindLastIndex((CursorSpecs cs) => cs == specs);
            if (t_lastIndex < 0)
            {
                #region Logs
                //CustomDebug.LogWarningForComponent($"Tried to remove CursorSpecs ({specs}) from the stack when it wasn't even in the stack.", this);
                #endregion Logs
            }
            m_cursorStack.RemoveAt(t_lastIndex);
            UpdateCursor();
        }
        private void UpdateCursor()
        {
            if (m_cursorStack.Count > 0)
            {
                CursorSpecs t_topSpecs = m_cursorStack[^1];
                Cursor.SetCursor(t_topSpecs.texture, t_topSpecs.hotspot, t_topSpecs.cursorMode);
            }
        }


        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            AddCursorSpecsToStack(m_timeSusCursor);
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            RemoveCursorFromStack(m_timeSusCursor);
        }

        private void ToggleSubscriptions(bool cond)
        {
            if (m_pickupCont != null)
            {
                m_pickupCont.onPickUp.ToggleSubscription(OnPickUpHoldObject, cond);
                m_pickupCont.onRelease.ToggleSubscription(OnReleaseHeldObject, cond);
            }
            if (m_gamePauseCont != null)
            {
                m_gamePauseCont.onPause.ToggleSubscription(OnGamePause, cond);
                m_gamePauseCont.onResume.ToggleSubscription(OnGameResume, cond);
            }
        }
        private void OnPickUpHoldObject(IPickUpObject pickupObj)
        {
            if (pickupObj.shouldChangeCursorIcon)
            {
                AddCursorSpecsToStack(m_holingObjCursor);
            }
        }
        private void OnReleaseHeldObject(IPickUpObject pickupObj)
        {
            if (pickupObj.shouldChangeCursorIcon)
            {
                RemoveCursorFromStack(m_holingObjCursor);
            }
        }
        private void OnGamePause()
        {
            AddCursorSpecsToStack(m_defaultCursor);
        }
        private void OnGameResume()
        {
            if (m_cursorStack.Count > 1)
            {
                RemoveCursorFromStack(m_defaultCursor);
            }
        }
    }
}