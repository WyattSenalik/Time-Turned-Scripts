using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Animation;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class GunFlareAnimator : MonoBehaviour
    {
        public float curTime => m_timeMan.curTime;
        public float latestBulletFireTime => m_gunBehav.bulletCont.latestBulletFireTime;

        [SerializeField, Required] private GunBehavior m_gunBehav = null;
        [SerializeField] private ManualSpriteAnimation m_flareAnim = new ManualSpriteAnimation();

        private GlobalTimeManager m_timeMan = null;
        private SpriteRenderer m_sprRend = null;


        private void Awake()
        {
            m_sprRend = GetComponent<SpriteRenderer>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_sprRend, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_gunBehav, nameof(m_gunBehav), this);
            #endregion Asserts
        }
        private void Start()
        {
            m_timeMan = GlobalTimeManager.instance;
            #region Asserts
            //CustomDebug.AssertSingletonIsNotNull(m_timeMan, this);
            #endregion Asserts
        }
        private void Update()
        {
            float t_animStartTime;
            // If recording, use the latest fire time.
            if (m_timeMan.shouldRecord)
            {
                t_animStartTime = latestBulletFireTime >= 0 ? latestBulletFireTime : float.PositiveInfinity;
            }
            // If rewinding, must figure out which fire time is most previous.
            else
            {
                t_animStartTime = GetMostRecentFireTimeUsingCurTime();
            }

            if (float.IsPositiveInfinity(t_animStartTime))
            {
                m_sprRend.sprite = null;
            }
            else
            {
                m_sprRend.sprite = m_flareAnim.GetSpriteForTime(t_animStartTime, curTime);
            }
        }


        private float GetMostRecentFireTimeUsingCurTime()
        {
            for (int i = m_gunBehav.prevFireTimesList.Count - 1; i >= 0; --i)
            {
                float t_fireTime = m_gunBehav.prevFireTimesList[i];
                if (curTime >= t_fireTime)
                {
                    return t_fireTime;
                }
            }
            return float.PositiveInfinity;
        }
    }
}