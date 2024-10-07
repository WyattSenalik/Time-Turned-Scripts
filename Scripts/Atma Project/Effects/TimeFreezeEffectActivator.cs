using UnityEngine;

using NaughtyAttributes;
using Atma.Events;
using Helpers.Events.GameEventSystem;

namespace Atma
{
    [DisallowMultipleComponent]
    public sealed class TimeFreezeEffectActivator : MonoBehaviour
    {
        private GameObject playerObj
        {
            get
            {
                InitializePlayerObjIfNull();
                return m_playerObj;
            }
        }

        [SerializeField, Required] private CloneDiedEventIdentifierSO m_cloneDiedEventIDSO = null;
        [SerializeField, Required] private IneptusDiedEventIdentifierSO m_ineptusDiedEventIDSO = null;
        [SerializeField, Required] private TimeFreezeVisualEffect m_timeFreezeEffect = null;
        [SerializeField, Required] private CameraController m_camCont = null;
        [SerializeField] private Vector2 m_offsetFromPlayer = new Vector2(0.0f, 0.5f);

        private GameObject m_playerObj = null;
        private PlayerHealth m_playerHealth = null;
        private SubManager<ICloneDiedContext> m_cloneDiedSubMan = null;
        private SubManager<IIneptusDiedContext> m_ineptusDiedSubMan = null;
        private int m_timeCloneDiedFrame = -1;
        private int m_ineptusDiedFrame = -1;


        private void Awake()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneDiedEventIDSO, nameof(m_cloneDiedEventIDSO), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_ineptusDiedEventIDSO, nameof(m_ineptusDiedEventIDSO), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeFreezeEffect, nameof(m_timeFreezeEffect), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_camCont, nameof(m_camCont), this);
            #endregion Asserts
        }
        private void Start()
        {
            InitializePlayerObjIfNull();

            m_cloneDiedSubMan = new SubManager<ICloneDiedContext>(m_cloneDiedEventIDSO, OnCloneDied);
            m_ineptusDiedSubMan = new SubManager<IIneptusDiedContext>(m_ineptusDiedEventIDSO, OnIneptusDied);
            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }


        public void StartEffect()
        {
            m_timeFreezeEffect.gameObject.SetActive(true);

            // Protect against race condition, we don't know if this is called before or after OnCloneDied
            if (m_timeCloneDiedFrame != Time.frameCount && m_ineptusDiedFrame != Time.frameCount)
            {
                // Move to the player's position.
                Vector2 t_playerPos2D = playerObj.transform.position;
                MoveTimeFreezeEffectToPosition(t_playerPos2D + m_offsetFromPlayer);
            }
        }
        public void ManuallyEndEffect()
        {
            // If its not already off, turn off the effect.
            m_timeFreezeEffect.gameObject.SetActive(false);
        }

        private void MoveTimeFreezeEffectToPosition(Vector2 position)
        {
            Vector3 t_pos = m_timeFreezeEffect.transform.position;
            t_pos.x = position.x;
            t_pos.y = position.y;
            m_timeFreezeEffect.transform.position = t_pos;
        }

        private void OnCloneDied(ICloneDiedContext context)
        {
            // Move to where the clone died
            MoveTimeFreezeEffectToPosition(context.positionAtDeath + m_offsetFromPlayer);
            // Screen shake
            m_camCont.ShakeCamera();

            m_timeCloneDiedFrame = Time.frameCount;
        }
        private void OnPlayerDeath()
        {
            Vector2 t_playerPos2D = m_playerObj.transform.position;
            MoveTimeFreezeEffectToPosition(t_playerPos2D + m_offsetFromPlayer);
            // Screen shake
            m_camCont.ShakeCamera();
        }
        private void OnIneptusDied(IIneptusDiedContext context)
        {
            // Move to where the clone died
            MoveTimeFreezeEffectToPosition(context.positionAtDeath + m_offsetFromPlayer);
            // Screen shake
            m_camCont.ShakeCamera();

            m_ineptusDiedFrame = Time.frameCount;
        }
        private void ToggleSubscriptions(bool cond)
        {
            m_cloneDiedSubMan.ToggleSubscription(cond);
            m_ineptusDiedSubMan.ToggleSubscription(cond);
            m_playerHealth.onDeath?.ToggleSubscription(OnPlayerDeath, cond);
        }

        private void InitializePlayerObjIfNull()
        {
            if (m_playerObj == null)
            {
                PlayerSingleton t_playerSingleton = PlayerSingleton.instance;
                #region Asserts
                //CustomDebug.AssertSingletonIsNotNull(t_playerSingleton, this);
                #endregion Asserts
                m_playerObj = t_playerSingleton.gameObject;
                m_playerHealth = m_playerObj.GetComponent<PlayerHealth>();
                #region Asserts
                //CustomDebug.AssertComponentOnOtherIsNotNull(m_playerHealth, m_playerObj, this);
                #endregion Asserts
            }
        }
    }
}