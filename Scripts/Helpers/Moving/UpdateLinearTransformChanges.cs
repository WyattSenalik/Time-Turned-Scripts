using UnityEngine;

using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Helpers.Moving
{
    /// <summary>
    /// Slowly changes the transform over time.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LinearTransformChanges))]
    public class UpdateLinearTransformChanges : MonoBehaviour
    {
        [SerializeField]
        private eUnityUpdateChoice m_updateChoice = eUnityUpdateChoice.Update;

        private LinearTransformChanges m_linearTransChanges = null;


        // Domestic Initialization
        private void Awake()
        {
            m_linearTransChanges = GetComponent<LinearTransformChanges>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_linearTransChanges, this);
            #endregion Asserts
        }
        private void Update()
        {
            if (m_updateChoice != eUnityUpdateChoice.Update) { return; }

            m_linearTransChanges.UpdateChanges(Time.deltaTime);
        }
        private void LateUpdate()
        {
            if (m_updateChoice != eUnityUpdateChoice.LateUpdate) { return; }

            m_linearTransChanges.UpdateChanges(Time.deltaTime);
        }
        private void FixedUpdate()
        {
            if (m_updateChoice != eUnityUpdateChoice.FixedUpdate) { return; }

            m_linearTransChanges.UpdateChanges(Time.deltaTime);
        }
    }
}
