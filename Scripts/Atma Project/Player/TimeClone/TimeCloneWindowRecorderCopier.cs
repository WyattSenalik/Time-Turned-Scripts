using System;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Copies <see cref="WindowData{T}"/> from the specified <see cref="TCopyFromBehaviour"/> into and replays it for the time clone.
    /// </summary>
    /// <typeparam name="TCopyFromBehaviour">Behaviour to copy from.</typeparam>
    /// <typeparam name="TData"></typeparam>
    [RequireComponent(typeof(TimeClone))]
    public abstract class TimeCloneWindowRecorderCopier<TCopyFromBehaviour, TData> : TimedRecorder
        where TCopyFromBehaviour : WindowRecorder<TData>
        where TData : IEquatable<TData>
    {
        private const bool IS_DEBUGGING = false;

        [SerializeField] private bool m_doesPlayerHaveMultipleOfCopyFromBehaviour = false;
        [SerializeField, Min(0), ShowIf(nameof(m_doesPlayerHaveMultipleOfCopyFromBehaviour))] private int m_componentIndex = 0;

        private TimeClone m_clone = null;

        private IReadOnlyList<WindowData<TData>> m_windows = null;


        protected override void Awake()
        {
            base.Awake();

            m_clone = GetComponent<TimeClone>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_clone, this);
            #endregion Asserts
            m_clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        protected virtual void OnDestroy()
        {
            if (m_clone != null)
            {
                m_clone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }
        private void Initialize()
        {
            TCopyFromBehaviour t_copyFromBehav = RetrieveCopyFromBehaviour();
            // Get all the snapshots after this object's spawn time.
            WindowData<TData>[] t_windowsArr = t_copyFromBehav.windowCollection.GetAllWindows();
            #region Logs
            //CustomDebug.LogForComponent($"Found {t_windowsArr.Length} windows after time {spawnTime}. There were {t_windowsArr.Length} original snapshots.", this, IS_DEBUGGING);
            #endregion Logs
            List<WindowData<TData>>  t_windowsLists = new List<WindowData<TData>>(t_windowsArr.Length);
            for (int i = 0; i < t_windowsArr.Length; ++i)
            {
                WindowData<TData> t_window = t_windowsArr[i];
                t_windowsLists.Add(new WindowData<TData>(t_window.window, t_window.data));
            }
            m_windows = t_windowsLists;
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);
            #region Logs
            //CustomDebug.LogForComponent(nameof(SetToTime), this, IS_DEBUGGING);
            #endregion Logs
            // If haven't yet initialized
            if (m_windows == null) { return; }
            // If early disappearance before this time, do nothing
            if (m_clone.HasEarlyDisappearanceBeforeOrAtTime(time)) { return; }

            TData t_curWindowData = GetWindowDataAtTime(time);
            ApplyData(t_curWindowData);
        }


        protected abstract void ApplyData(TData windowData);


        private TCopyFromBehaviour RetrieveCopyFromBehaviour()
        {
            GameObject t_origPlayerObj = m_clone.cloneData.originalPlayerObj;
            TCopyFromBehaviour t_copyFromBehav;
            if (m_doesPlayerHaveMultipleOfCopyFromBehaviour)
            {
                t_copyFromBehav = t_origPlayerObj.GetComponent<TCopyFromBehaviour>();
                #region Asserts
                //CustomDebug.AssertComponentOnOtherIsNotNull(t_copyFromBehav, t_origPlayerObj, this);
                #endregion Asserts
            }
            else
            {
                TCopyFromBehaviour[] t_copyFromBehavArr = t_origPlayerObj.GetComponents<TCopyFromBehaviour>();
                #region Asserts
                //CustomDebug.AssertIndexIsInRange(m_componentIndex, t_copyFromBehavArr, this);
                #endregion Asserts
                t_copyFromBehav = t_copyFromBehavArr[m_componentIndex];
            }
            return t_copyFromBehav;
        }
        private TData GetWindowDataAtTime(float time)
        {
            foreach (WindowData<TData> t_window in m_windows)
            {
                if (t_window.ContainsTime(time))
                {
                    return t_window.data;
                }
            }
            #region Asserts
            string t_additionalInfo = "No windows at all.";
            if (m_windows.Count > 0)
            {
                t_additionalInfo = $"Earliest window: ({m_windows[0].window}). Most recent window: ({m_windows[^1].window}).";
            }
            CustomDebug.ThrowAssertionFail($"a window to exist at time ({time}) but none did. {t_additionalInfo}", this);
            #endregion Asserts
            return default;
        }
    }
}