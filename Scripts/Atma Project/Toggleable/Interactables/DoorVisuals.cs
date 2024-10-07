using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimedObject))]
    public sealed class DoorVisuals : TimedRecorder
    {
        [SerializeField, Required] private Animator m_animator = null;
        [SerializeField, AnimatorParam(nameof(m_animator))] private string m_isOpenedAnimParamBool = "isOpen";

        [SerializeField] private Door m_door = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_animator, nameof(m_animator), this);
            #endregion Asserts
        }
        private void Update()
        {
            // Did this because the exit door in 5-1 was starting the on animation once time started running normally.
            m_animator.SetBool(m_isOpenedAnimParamBool, m_door.isOn);
        }

        public void OnOpen()
        {
            //if (!isRecording) { return; }
            m_animator.SetBool(m_isOpenedAnimParamBool, true);
        }
        public void OnClose()
        {
            //if (!isRecording) { return; }
            m_animator.SetBool(m_isOpenedAnimParamBool, false);
        }
    }
}