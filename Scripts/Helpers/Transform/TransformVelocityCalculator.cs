using UnityEngine;
// Original Authors - Wyatt Senalik and Aaron Duffey

namespace Helpers.Transforms
{
    /// <summary>
    /// Calculates velocity from the transform.
    /// </summary>
    public class TransformVelocityCalculator : MonoBehaviour
    {
        private Vector3 m_lastPosition = Vector3.negativeInfinity;
        private Vector3 m_velocitySinceLastFrame = Vector3.zero;

        public Vector3 velocitySinceLastFrame => m_velocitySinceLastFrame;


        // Called 0th
        // Domestic Initialization
        private void Awake()
        {
            m_lastPosition = transform.position;
        }
        // Update is called once per frame
        private void LateUpdate()
        {
            Vector3 temp_curPos = transform.position;

            m_velocitySinceLastFrame = (temp_curPos - m_lastPosition) / Time.deltaTime;

            m_lastPosition = temp_curPos;
        }
    }
}
