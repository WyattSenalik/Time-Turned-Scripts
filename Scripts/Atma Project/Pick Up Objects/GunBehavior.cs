using System.Collections.Generic;
using UnityEngine;

using Timed;
using NaughtyAttributes;
using Helpers.Math;
// Original Authors - Bryce Cernohous-Schrader
// Tweaked by Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimedObject))]
    public sealed class GunBehavior : PickupBehavior, ITimedRecorder
    {
        private bool IS_DEBUGGING = false;

        public ITimedObject timedObject { get; private set; } = null;
        public bool isRecording => timedObject.isRecording;
        public IReadOnlyList<float> prevFireTimesList => m_prevFireTimesList;
        public BulletController bulletCont => m_bulletController;

        [SerializeField, Required] private BulletController m_bulletController = null;

        [SerializeField] private float m_velocity = 1.0f;
        [SerializeField, Min(0.0f)] private float m_fireCooldown = 1.0f;
        [SerializeField, Required] private Transform m_rotateTrans = null;
        [SerializeField, Required] private Transform m_spawnTrans = null;
        [SerializeField, Min(0.0f)] private float m_insideWallCheckCircleRadius = 0.01f;
        [SerializeField, Tag] private string m_wallTag = "Wall";

        private Rigidbody2D m_rigidbody = null;

        private bool m_isFiring = false;
        private readonly List<float> m_prevFireTimesList = new List<float>();
        private float m_lastRotationBeforeUpdate = 0.0f;
        private int m_lastRotationUpdateFrame = -1;


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_bulletController, nameof(m_bulletController), this);
            #endregion Asserts

            m_rigidbody = GetComponent<Rigidbody2D>();
            timedObject = GetComponent<ITimedObject>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(m_rigidbody, this);
            //CustomDebug.AssertIComponentIsNotNull(timedObject, this);
            #endregion Asserts
        }


        //when holding down button, auto fire is on
        public override void TakeAction(bool onPressedOrReleased)
        {
            base.TakeAction(onPressedOrReleased);

            m_isFiring = onPressedOrReleased;
            #region Logs
            string t_onOrOff = m_isFiring ? "On" : "Off";
            //CustomDebug.Log($"Gun auto {t_onOrOff}", IS_DEBUGGING);
            #endregion Logs
        }
        public override void OnPickUp()
        {
            base.OnPickUp();

            BulletCollider t_holderCollider = pickUpObject.holder.GetComponentInChildren<BulletCollider>();
            m_bulletController.AddIgnoreCollider(t_holderCollider);
        }
        public override void OnReleased()
        {
            base.OnReleased();

            m_bulletController.ClearIgnoreColliders();

            if (Time.frameCount == m_lastRotationUpdateFrame)
            {
                m_rotateTrans.localScale = Vector3.one;
                m_rotateTrans.rotation = Quaternion.Euler(0.0f, 0.0f, m_lastRotationBeforeUpdate);
            }
        }

        public void SetToTime(float time)
        {
            if (!isRecording) { return; }
            if (!isHeld) { return; }

            UpdateGunRotation();
            UpdateFire();
        }
        public void OnRecordingStop(float time)
        {
            m_isFiring = false;
        }
        public void OnRecordingResume(float time) { }
        public void TrimDataAfter(float time)
        {
            for (int i = 0; i < m_prevFireTimesList.Count; ++i)
            {
                float t_fireTime = m_prevFireTimesList[i];
                if (t_fireTime > time)
                {
                    m_prevFireTimesList.RemoveAt(i);
                    --i;
                }
            }
        }


        private void UpdateGunRotation()
        {
            Vector2 t_holdDir = pickUpObject.aimDirection;
            float t_angle = Mathf.Atan2(t_holdDir.y, t_holdDir.x) * Mathf.Rad2Deg;
            t_angle = AngleHelpers.RestrictAngle(t_angle);
            bool t_lessThan90 = 90.0f >= t_angle;
            bool t_greaterThanNeg90 = t_angle >= -90.0f;
            if (t_lessThan90 && t_greaterThanNeg90)
            {
                m_rotateTrans.localScale = Vector3.one;
            }
            else
            {
                m_rotateTrans.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

                if (t_lessThan90)
                {
                    t_angle -= 180.0f;
                }
                else if (t_greaterThanNeg90)
                {
                    t_angle += 180.0f;
                }
                else
                {
                    #region Logs
                    //CustomDebug.LogErrorForComponent($"Fell into an expected else. Unhandled condition. {nameof(t_lessThan90)}:{t_lessThan90}. {nameof(t_greaterThanNeg90)}:{t_greaterThanNeg90}", this);
                    #endregion Logs
                }
            }
            m_lastRotationBeforeUpdate = m_rotateTrans.rotation.z;
            m_rotateTrans.rotation = Quaternion.Euler(0.0f, 0.0f, t_angle);

            m_lastRotationUpdateFrame = Time.frameCount;
        }
        private void UpdateFire()
        {
            // Not firing.
            if (!m_isFiring) { return; }
            // On cooldown.
            if (GetMostRecentFireTime() + m_fireCooldown > timedObject.curTime) { return; }
            // SpawnTrans is inside a wall, so don't fire.
            if (IsBulletSpawnInsideWall()) { return; }

            // Are firing, not on cooldown, not in a wall, fire the bullet.
            m_bulletController.FireBullet(m_spawnTrans.position, pickUpObject.aimDirection * m_velocity);
            m_prevFireTimesList.Add(timedObject.curTime);
        }
        private float GetMostRecentFireTime()
        {
            if (m_prevFireTimesList.Count < 1)
            {
                return float.NegativeInfinity;
            }
            return m_prevFireTimesList[^1];
        }
        private bool IsBulletSpawnInsideWall()
        {
            Collider2D[] t_hits = Physics2D.OverlapCircleAll(m_spawnTrans.position, m_insideWallCheckCircleRadius);
            foreach (Collider2D t_singleHit in t_hits)
            {
                GameObject t_hitObj = t_singleHit.gameObject;
                // Found a wall
                if (t_hitObj.CompareTag(m_wallTag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}