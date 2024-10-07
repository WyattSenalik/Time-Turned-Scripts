using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RebindActionUI))]
    public sealed class ControlOption : NavigatableMenuOption
    {
        private RebindActionUI m_rebindActionUI = null;


        protected override void Awake()
        {
            base.Awake();

            m_rebindActionUI = GetComponent<RebindActionUI>();
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(m_rebindActionUI, nameof(m_rebindActionUI), this);
            #endregion Asserts
        }

        public override void OnChosen()
        {
            base.OnChosen();

            m_rebindActionUI.StartInteractiveRebind();
        }
    }
}
