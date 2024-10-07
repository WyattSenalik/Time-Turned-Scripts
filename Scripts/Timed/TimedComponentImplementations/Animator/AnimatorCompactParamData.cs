using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed.TimedComponentImplementations
{
    public sealed class AnimatorCompactParamData
    {
        public AnimatorControllerParameterType type => m_type;
        public int id => m_id;

        private readonly AnimatorControllerParameterType m_type = AnimatorControllerParameterType.Float;
        private readonly int m_id = int.MinValue;


        public AnimatorCompactParamData(AnimatorControllerParameterType paramType, int paramID)
        {
            m_type = paramType;
            m_id = paramID;
        }
    }
}