using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class FollowMainCamera : MonoBehaviour
    {
        private Transform m_mainCamTrans = null;


        private void Start()
        {
            m_mainCamTrans = Camera.main.transform;
        }
        private void LateUpdate()
        {
            transform.position = m_mainCamTrans.position;
        }
    }
}