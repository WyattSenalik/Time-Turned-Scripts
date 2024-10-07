using UnityEngine;

using NaughtyAttributes;

using Timed;
using Timed.TimedComponentImplementations;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Used to fake the player staying in place during rewind on entering a new quadrant in the final level.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class FakePlayerVisuals : MonoBehaviour
    {
        public SpriteRenderer fakePlayerBodySprRend => m_fakePlayerBodySprRend;

        [SerializeField, Required, BoxGroup("Fakes")] private SpriteRenderer m_fakePlayerBodySprRend = null;
        [SerializeField, Required, BoxGroup("Fakes")] private SpriteRenderer m_fakeLeftHandSprRend = null;
        [SerializeField, Required, BoxGroup("Fakes")] private SpriteRenderer m_fakeRightHandSprRend = null;
        //[SerializeField, Required, BoxGroup("Fakes")] private SpriteRenderer m_fakeShadowSprRend = null;

        [SerializeField, Required, BoxGroup("Player References")] private SpriteRenderer m_playerBodySprRend = null;
        [SerializeField, Required, BoxGroup("Player References")] private SpriteRenderer m_leftHandSprRend = null;
        [SerializeField, Required, BoxGroup("Player References")] private SpriteRenderer m_rightHandSprRend = null;
        //[SerializeField, Required, BoxGroup("Player References")] private SpriteRenderer m_shadowSprRend = null;

        private TimedSpriteRenderer m_timedPlayerBodySprRend = null;
        private TimedSpriteRenderer m_timedLeftHandSprRend = null;
        private TimedSpriteRenderer m_timedRightHandSprRend = null;

        private bool m_areFakeVisualsActivated = false;

        private bool m_leftHandEnableStateOnActivate = false;
        private bool m_rightHandEnableStateOnActivate = false;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_fakePlayerBodySprRend, nameof(m_fakePlayerBodySprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_fakeLeftHandSprRend, nameof(m_fakeLeftHandSprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_fakeRightHandSprRend, nameof(m_fakeRightHandSprRend), this);
            ////CustomDebug.AssertSerializeFieldIsNotNull(m_fakeShadowSprRend, nameof(m_fakeShadowSprRend), this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_playerBodySprRend, nameof(m_playerBodySprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_leftHandSprRend, nameof(m_leftHandSprRend), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_rightHandSprRend, nameof(m_rightHandSprRend), this);
            ////CustomDebug.AssertSerializeFieldIsNotNull(m_shadowSprRend, nameof(m_shadowSprRend), this);
            #endregion Asserts

            m_timedPlayerBodySprRend = m_playerBodySprRend.GetComponentSafe<TimedSpriteRenderer>(this);
            m_timedLeftHandSprRend = m_leftHandSprRend.GetComponentSafe<TimedSpriteRenderer>(this);
            m_timedRightHandSprRend = m_rightHandSprRend.GetComponentSafe<TimedSpriteRenderer>(this);
        }
        private void Update()
        {
            if (m_areFakeVisualsActivated)
            {
                ToggleRealSpriteRenderers(false);
            }
        }


        public void Activate()
        {
            m_areFakeVisualsActivated = true;
            m_fakePlayerBodySprRend.sprite = m_playerBodySprRend.sprite;
            m_fakePlayerBodySprRend.transform.position = m_playerBodySprRend.transform.position;

            m_fakeLeftHandSprRend.sprite = m_leftHandSprRend.sprite;
            m_fakeLeftHandSprRend.transform.position = m_leftHandSprRend.transform.position;

            m_fakeRightHandSprRend.sprite = m_rightHandSprRend.sprite;
            m_fakeRightHandSprRend.transform.position = m_rightHandSprRend.transform.position;

            //m_fakeShadowSprRend.transform.position = m_shadowSprRend.transform.position;

            m_leftHandEnableStateOnActivate = m_leftHandSprRend.enabled;
            m_rightHandEnableStateOnActivate = m_rightHandSprRend.enabled;

            ToggleFakeSpriteRenderers(true);
            ToggleRealSpriteRenderers(false);
        }
        public void Deactivate()
        {
            m_areFakeVisualsActivated = false;
            ToggleFakeSpriteRenderers(false);
            ToggleRealSpriteRenderers(true);
        }


        private void ToggleFakeSpriteRenderers(bool toggle)
        {
            m_fakePlayerBodySprRend.enabled = toggle;
            m_fakeLeftHandSprRend.enabled = toggle && m_leftHandEnableStateOnActivate;
            m_fakeRightHandSprRend.enabled = toggle && m_rightHandEnableStateOnActivate;
            //m_fakeShadowSprRend.enabled = toggle;
        }
        private void ToggleRealSpriteRenderers(bool toggle)
        {
            m_timedPlayerBodySprRend.enabled = toggle;
            m_timedLeftHandSprRend.enabled = toggle;
            m_timedRightHandSprRend.enabled = toggle;

            m_playerBodySprRend.enabled = toggle;
            m_leftHandSprRend.enabled = toggle;
            m_rightHandSprRend.enabled = toggle;
            //m_shadowSprRend.enabled = toggle;
        }
    }
}