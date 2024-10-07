using UnityEngine;

using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using Timed;
// Original Author - Sam Smith
// Tweaked heavily by Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Evaluates objects within a range against targeting conditions and sets the ModularTurretController's Target field to the nearest viable object
    /// Pre Conditions:
    /// </summary>
    [RequireComponent(typeof(ModularTurretController))]
    public class OmniTargeting : MonoBehaviour
    {
        private GlobalTimeManager m_timeMan = null;

        private Int2DTransform m_playerTrans = null;
        private CloneManager m_cloneMan = null;

        private ModularTurretController m_parentTurret = null;


        private void Awake()
        {
            m_parentTurret = this.GetComponentSafe<ModularTurretController>();
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            PlayerSingleton t_player = PlayerSingleton.GetInstanceSafe(this);
            m_playerTrans = t_player.GetComponentSafe<Int2DTransform>(this);
            m_cloneMan = t_player.GetComponentSafe<CloneManager>(this);
        }
        private void FixedUpdate()
        {
            if (!m_timeMan.shouldRecord) { return; }

            Vector2 t_diffToPlayer = m_playerTrans.transform.position - transform.position;
            float t_closestSqDist = t_diffToPlayer.sqrMagnitude;
            Int2DTransform t_closestTarget = m_playerTrans;

            foreach (TimeClone t_clone in m_cloneMan.existingClones)
            {
                // Don't care about dead clones.
                if (IsDeadClone(t_clone)) { continue; }

                Vector2 t_diffToClone = t_clone.transform.position - transform.position;
                float t_sqDistToClone = t_diffToClone.sqrMagnitude;
                if (t_sqDistToClone < t_closestSqDist)
                {
                    t_closestSqDist = t_sqDistToClone;
                    t_closestTarget = t_clone.intTransform;
                }
            }
            foreach (TimeClone t_clone in m_cloneMan.disconnectedClones)
            {
                // Don't care about dead clones.
                if (IsDeadClone(t_clone)) { continue; }

                Vector2 t_diffToClone = t_clone.transform.position - transform.position;
                float t_sqDistToClone = t_diffToClone.sqrMagnitude;
                if (t_sqDistToClone < t_closestSqDist)
                {
                    t_closestSqDist = t_sqDistToClone;
                    t_closestTarget = t_clone.intTransform;
                }
            }

            m_parentTurret.target = t_closestTarget;
        }


        private bool IsDeadClone(TimeClone clone)
        {
            TimeCloneHealth t_cloneHealth = clone.health;
            if (t_cloneHealth.isDead)
            {
                return true;
            }
            else if (t_cloneHealth.hitAfterDeathTime <= t_cloneHealth.curTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
