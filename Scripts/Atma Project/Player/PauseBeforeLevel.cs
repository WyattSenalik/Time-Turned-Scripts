using UnityEngine;

using Atma.UI;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(ITimeManipController))]
    public sealed class PauseBeforeLevel : MonoBehaviour
    {
        private ITimeManipController m_timeManipController = null;


        private void Awake()
        {
            m_timeManipController = GetComponent<ITimeManipController>();
            #region Asserts
            //CustomDebug.AssertIComponentIsNotNull(m_timeManipController, this);
            #endregion Asserts
        }
        private void Start()
        {
            if (!m_timeManipController.rewinder.hasStarted)
            {
                m_timeManipController.BeginTimeManipulation(true);
            }
        }
    }
}
