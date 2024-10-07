using UnityEngine;

using Helpers;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(BranchPlayerController))]
    public class PlayerHealth : MonoBehaviour, IHealth
    {
        public bool isDead => m_playerStateMan.curState == ePlayerState.Dead;

        public IEventPrimer onDeath => m_onDeath;
        public IEventPrimer<IDamageContext> onDeathWContext => m_onDeathWContext;

        [SerializeField] private ePlayerState[] m_invincibleStates =
            { ePlayerState.Leap, ePlayerState.Dead };
        [SerializeField] private MixedEvent m_onDeath = new MixedEvent();
        [SerializeField] private MixedEvent<IDamageContext> m_onDeathWContext = new MixedEvent<IDamageContext>();

        private PlayerStateManager m_playerStateMan = null;
        private BranchPlayerController m_branchController = null;


        private void Awake()
        {
            m_playerStateMan = GetComponent<PlayerStateManager>();
            m_branchController = GetComponent<BranchPlayerController>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_playerStateMan, this);
            //CustomDebug.AssertComponentIsNotNull(m_branchController, this);
            #endregion Asserts
        }


        public bool TakeDamage(IDamageContext context)
        {
            if (m_playerStateMan.TrySetState(ePlayerState.Dead, eAllowListType.Blacklist,
                m_invincibleStates))
            {
                m_branchController.ForceBeginTimeManipulation(context.damageTime);
                m_branchController.timeRewinder.ForceSetTime(context.damageTime);
                m_onDeath.Invoke();
                m_onDeathWContext.Invoke(context);
            }

            // Don't want bullet to stop because time will pause instead of actually tanking the bullet.
            return false;
        }
    }
}