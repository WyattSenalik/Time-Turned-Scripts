using UnityEngine;

using NaughtyAttributes;

using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Helpers.Moving
{
    /// <summary>
    /// Base class to slowly change the transform over time.
    /// </summary>
    [DisallowMultipleComponent]
    public class LinearTransformChanges : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private bool m_changePosition = true;
        [SerializeField, ShowIf(nameof(m_changePosition))]
        private eRelativeSpace m_posRelSpace = eRelativeSpace.World;
        [SerializeField, ShowIf(nameof(m_changePosition))]
        [Tooltip("Position Vel = (Unity units) / (seconds)")]
        private Vector3 m_posVelocity = Vector3.right;

        [Header("Rotation")]
        [SerializeField] private bool m_changeRotation = false;
        [SerializeField, ShowIf(nameof(m_changeRotation))]
        private eRelativeSpace m_rotRelSpace = eRelativeSpace.World;
        [SerializeField, ShowIf(nameof(m_changeRotation))]
        [Tooltip("Angular Vel = (Euler angles) / (seconds)")]
        private Vector3 m_rotAngularVelocity = Vector3.zero;

        [Header("Scale")]
        [SerializeField] private bool m_changeScale = false;
        [SerializeField, ShowIf(nameof(m_changeScale))]
        [Tooltip("Scale Vel = (Unity units) / (seconds)")]
        private Vector3 m_scaleVelocity = Vector3.zero;
        [SerializeField, ShowIf(nameof(m_changeScale))]
        private bool m_allowNegativeScale = false;


        /* For Fun
        private void Awake()
        {
            m_posVelocity = Random.insideUnitSphere * Random.Range(0.1f, 3.0f);
            m_rotAngularVelocity = new Vector3(0, 0, Random.Range(10.0f, 360.0f));
            m_scaleVelocity = Random.insideUnitSphere * Random.Range(0.1f, 3.0f);
        }
        */
        /// <summary>
        /// Updates position, rotation, and scale given the delta time.
        /// </summary>
        /// <param name="deltaTime">Amount of time to update the 
        /// transform data by. For smoothest results, it should be the time 
        /// in seconds since the last call to this function.</param>
        public void UpdateChanges(float deltaTime)
        {
            ChangePosition(deltaTime);
            ChangeRotation(deltaTime);
            ChangeScale(deltaTime);
        }
        /// <summary>
        /// Updates position by the given delta time.
        /// </summary>
        /// <param name="deltaTime">Amount of time to update the 
        /// position by. For smoothest results, it should be the time 
        /// in seconds since the last call to this function.</param>
        public void ChangePosition(float deltaTime)
        {
            if (!m_changePosition) { return; }

            Vector3 deltaPos = m_posVelocity * deltaTime;
            switch (m_posRelSpace)
            {
                case eRelativeSpace.Local:
                    transform.localPosition += deltaPos;
                    break;
                case eRelativeSpace.World:
                    transform.position += deltaPos;
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_posRelSpace, this);
                    break;
            }
        }
        /// <summary>
        /// Updates rotation by the given delta time.
        /// </summary>
        /// <param name="deltaTime">Amount of time to update the 
        /// rotation by. For smoothest results, it should be the time 
        /// in seconds since the last call to this function.</param>
        public void ChangeRotation(float deltaTime)
        {
            if (!m_changeRotation) { return; }

            Vector3 deltaRotAngles = m_rotAngularVelocity * deltaTime;
            switch (m_rotRelSpace)
            {
                case eRelativeSpace.Local:
                    transform.localEulerAngles += deltaRotAngles;
                    break;
                case eRelativeSpace.World:
                    transform.eulerAngles += deltaRotAngles;
                    break;
                default:
                    CustomDebug.UnhandledEnum(m_rotRelSpace, this);
                    break;
            }
        }
        /// <summary>
        /// Updates scale by the given delta time.
        /// </summary>
        /// <param name="deltaTime">Amount of time to update the 
        /// scale by. For smoothest results, it should be the time 
        /// in seconds since the last call to this function.</param>
        public void ChangeScale(float deltaTime)
        {
            if (!m_changeScale) { return; }

            Vector3 deltaScale = m_scaleVelocity * deltaTime;
            transform.localScale += deltaScale;
            // Make the scale 0 if its negative
            if (!m_allowNegativeScale)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Max(scale.x, 0.0f);
                scale.y = Mathf.Max(scale.y, 0.0f);
                scale.z = Mathf.Max(scale.z, 0.0f);
            }
        }
    }
}