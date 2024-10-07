using System.Collections.Generic;
using UnityEngine;

using Helpers;
using Helpers.Singletons;
using Helpers.Extensions;
using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Manages the current time for all <see cref="ITimedObject"/>s.
    /// Updates them to the current time every frame.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GlobalTimeManager : DynamicSingletonMonoBehaviour<GlobalTimeManager>
    {
        private const bool IS_DEBUGGING = false;

        public float curTime { get; private set; } = 0.0f;
        public float prevTime { get; private set; } = 0.0f;
        public float farthestTime { get; private set; } = 0.0f;
        public bool shouldRecord { get; private set; } = true;
        public float timeScale
        {
            get => m_timeScale;
            set => m_timeScale = value;
        }
        public float deltaTime { get; private set; } = 0.0f;
        // Serialized just for testing TODO remove serialization
        [SerializeField] private float m_timeScale = 1.0f;

        [SerializeField, ReadOnly] private List<string> m_debugSubscribedObjects = new List<string>();

        private readonly List<ITimedObject> m_objects = new List<ITimedObject>();
        private readonly List<TimedClass> m_timedVariables = new List<TimedClass>();
        private readonly IDLibrary m_idLibrary = new IDLibrary();


        private void FixedUpdate()
        {
            float t_newTime;
            // Only increment time if we are recording
            if (shouldRecord)
            {
                // Update the current time and apply it to all timed objects
                t_newTime = curTime + m_timeScale * Time.fixedDeltaTime;
                t_newTime = Mathf.Max(0.0f, t_newTime);
                // Update farthest time if we reached a further time.
                if (farthestTime < t_newTime)
                {
                    farthestTime = t_newTime;
                }

                UpdateToTime(t_newTime);
            }
            else
            {
                t_newTime = curTime;
            }

            

            CustomDebug.RunDebugFunction(() =>
            {
                m_debugSubscribedObjects.Clear();
                foreach (ITimedObject t_obj in m_objects)
                {
                    if (t_obj is MonoBehaviour t_mono)
                    {
                        m_debugSubscribedObjects.Add($"{t_mono.gameObject.GetFullName()} ({t_mono.GetType().Name})");
                    }
                    else
                    {
                        m_debugSubscribedObjects.Add($"{t_obj.GetType().FullName}");
                    }
                }
                m_debugSubscribedObjects.Sort();
            }, IS_DEBUGGING);
        }


        /// <summary>
        /// Adds a <see cref="ITimedObject"/> to be synced to this
        /// <see cref="GlobalTimeManager"/>'s update.
        /// </summary>
        public void AddTimeObject(ITimedObject obj)
        {
            obj.SetToTime(curTime, deltaTime);
            m_objects.Add(obj);
        }
        /// <summary>
        /// Removes a <see cref="ITimedObject"/> to no longer be synced to this
        /// <see cref="GlobalTimeManager"/>'s update.
        /// </summary>
        public void RemoveTimeObject(ITimedObject obj)
        {
            m_objects.Remove(obj);
        }

        public void AddTimedVariable(TimedClass timedVariable)
        {
            m_timedVariables.Add(timedVariable);
        }
        public void RemoveTimedVariable(TimedClass timedVariable)
        {
            m_timedVariables.Remove(timedVariable);
        }

        public void SetTime(float newTime)
        {
            newTime = Mathf.Max(newTime, 0.0f);
            UpdateToTime(newTime);
        }

        public int RequestShouldNotRecord()
        {
            shouldRecord = false;
            SetTime(curTime);
            int t_checkedoutID = m_idLibrary.CheckoutID();
            #region Logs
            //CustomDebug.LogForComponent($"Request to not record with id ({t_checkedoutID}).", this, IS_DEBUGGING);
            #endregion Logs
            return t_checkedoutID;
        }
        public void CancelShouldNotRecordRequest(int checkoutID)
        {
            m_idLibrary.ReturnID(checkoutID);
            // Should record again if all ids have been returned
            shouldRecord = m_idLibrary.AreAllIDsReturned();
            #region Logs
            //CustomDebug.LogForComponent($"ID ({checkoutID}) has been returned. shouldRecord={shouldRecord}.", this, IS_DEBUGGING);
            #endregion Logs
        }


        private void UpdateToTime(float time)
        {
            prevTime = curTime;
            curTime = time;
            deltaTime = curTime - prevTime;
            UpdateTimedObjects();
            UpdateTimedClasses();
        }
        /// <summary>
        /// Calls <see cref="ITimedObject.SetToTime(float)"/> for each
        /// <see cref="ITimedObject"/>.
        /// </summary>
        private void UpdateTimedObjects()
        {
            #region Logs
            //CustomDebug.LogForComponent($"[{Time.frameCount}] [{curTime}] UpdatingTimedObjects", this, IS_DEBUGGING);
            #endregion Logs
            foreach (ITimedObject t_obj in m_objects)
            {
                t_obj.SetToTime(curTime, deltaTime);
            }
        }
        private void UpdateTimedClasses()
        {
            foreach (TimedClass t_var in m_timedVariables)
            {
                t_var.Update();
            }
        }
    }
}
