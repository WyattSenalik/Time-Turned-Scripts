using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.Animation
{
    /// <summary>
    /// Scriptable Object version of <see cref="RunCurves"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "new RunCurves", menuName = "ScriptableObjects/Curves/RunCurves")]
    public sealed class RunCurvesSO : ScriptableObject, IRunCurves
    {
        public RunCurves runCurves => m_runCurves;
        public float accelTime => m_runCurves.accelTime;
        public float decelTime => m_runCurves.decelTime;
        public float topSpeed => m_runCurves.topSpeed;
        public AnimationCurve accelCurve => m_runCurves.accelCurve;
        public AnimationCurve decelCurve => m_runCurves.decelCurve;

        [SerializeField] private RunCurves m_runCurves = new RunCurves();


        public float EvaluateAccelMove(float beginRunTime, float curTime)
        {
            return m_runCurves.EvaluateAccelMove(beginRunTime, curTime);
        }
        public float EvaluateDecel(float beginDecel, float curTime)
        {
            return m_runCurves.EvaluateDecel(beginDecel, curTime);
        }
    }
}