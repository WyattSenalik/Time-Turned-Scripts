using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations.Editor
{
    /// <summary>
    /// Editor Graphs for a TimedRecorder to use.
    /// </summary>
    public interface ITimedRecorderEditorGraphs
    {
        bool DrawAllDataAsCurve(Rect drawRect, float dataPointSize, float timeDotSize, out string errorMsg);
        void DisplayCurrentData();
    }
}