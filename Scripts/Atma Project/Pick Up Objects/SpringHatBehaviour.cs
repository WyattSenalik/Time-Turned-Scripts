using UnityEngine;

using NaughtyAttributes;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public class SpringHatBehaviour : PickupBehavior
    {
        [SerializeField, ValidateInput(nameof(GameObjectHasILeapObjectComponent), "Object must have an implementation of ILeapObject attached.")] 
        private GameObject m_leapGameObj = null;
        [SerializeField, Required] private SpriteRenderer m_pickupVisualSprRend = null;
        private ILeapObject m_leapObj = null;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_leapGameObj, nameof(m_leapGameObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pickupVisualSprRend, nameof(m_pickupVisualSprRend), this);
            #endregion Asserts
            m_leapObj = m_leapGameObj.GetComponent<ILeapObject>();
            #region Asserts
            //CustomDebug.AssertIComponentOnOtherIsNotNull(m_leapObj, m_leapGameObj, this);
            #endregion Asserts
        }


        public override void OnPickUp()
        {
            base.OnPickUp();

            m_leapObj.availableToUse = true;

            m_pickupVisualSprRend.enabled = false;
        }
        public override void OnReleased()
        {
            base.OnReleased();

            m_leapObj.availableToUse = false;

            m_pickupVisualSprRend.enabled = true;
        }


        #region Editor
        private bool GameObjectHasILeapObjectComponent(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            return go.GetComponent<ILeapObject>() != null;
        }
        #endregion Editor
    }
}