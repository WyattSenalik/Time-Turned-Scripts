using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using Timed;
using System;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma.Editors
{
    public sealed class PerformanceTimersDebugWindow : EditorWindow
    {
        private TimedObject[] m_timedObjects = new TimedObject[0];
        private readonly TimedObjectSorter m_timedObjSorter = new TimedObjectSorter();


        [MenuItem("Tools/PerformanceTimersDebug")]
        public static void ShowWindow()
        {
            GetWindow<PerformanceTimersDebugWindow>("Performance Timers Debug");
        }


        public void OnGUI()
        {
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Re-gather Timers"))
                {
                    RegatherTimers();
                }

                GUILayout.Space(10);

                GUILayout.BeginVertical();
                {
                    GUILayout.Label("TimedObjects");
                    GUILayout.Space(5);

                    DrawAllTimedObjectData();
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }


        private void RegatherTimers()
        {
            m_timedObjects = FindObjectsOfType<TimedObject>();
            SortTimedObjectsByTotalTicks();
        }
        private void SortTimedObjectsByTotalTicks()
        {
            Array.Sort(m_timedObjects, m_timedObjSorter);        
        }
        private void DrawAllTimedObjectData()
        {
            for (int i = m_timedObjects.Length - 1; i >= 0; --i)
            {
                TimedObject t_timedObj = m_timedObjects[i];
                if (t_timedObj == null) { continue; }
                GUILayout.BeginHorizontal();

                GUILayout.Label(t_timedObj.gameObject.GetFullName());
                GUILayout.Label($"SetToTime: {t_timedObj.setToTimeFuncTicks}");
                GUILayout.Label($"Request: {t_timedObj.requestSuspendRecordingFuncTicks}");
                GUILayout.Label($"CancelRequest: {t_timedObj.cancelSuspendRecordingRequestFuncTicks}");
                GUILayout.Label($"ForceSetBounds: {t_timedObj.forceSetTimeBoundsFuncTicks}");
                GUILayout.Label($"TrimData: {t_timedObj.trimDataAfterFuncTicks}");

                GUILayout.EndHorizontal();
            }
        }


        class TimedObjectSorter : IComparer<TimedObject>
        {
            public int Compare(TimedObject x, TimedObject y)
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else if (y == null)
                {
                    return 1;
                }
                else
                {
                    long t_xVal = CalculateTotalTicksForTimedObject(x);
                    long t_yVal = CalculateTotalTicksForTimedObject(y);
                    return t_xVal.CompareTo(t_yVal);
                }
            }

            public static long CalculateTotalTicksForTimedObject(TimedObject timedObject)
            {
                return timedObject.cancelSuspendRecordingRequestFuncTicks + timedObject.forceSetTimeBoundsFuncTicks + timedObject.requestSuspendRecordingFuncTicks + timedObject.setToTimeFuncTicks + timedObject.trimDataAfterFuncTicks;
            }
        }
    }
}