using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using NaughtyAttributes;

using Timed;
using Helpers.Animation;
using Helpers.Extensions;
using Helpers.Events;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Time-Aware controller for a bullet that does NOT utilize TimedObject.
    /// </summary>
    [RequireComponent(typeof(DamageDealer))]
    public class LinearBullet : MonoBehaviour
    {
        public BulletController owner { get; set; }
        public bool isDebugging { get; set; } = false;

        public IEventPrimer<GameObject> onHit => m_onHit;
        public IEventPrimer onShowBullet => m_onShowBullet;
        public IEventPrimer onHideBullet => m_onHideBullet;

        private float curTime => timeManager.curTime;
        private float farthestTime => timeManager.farthestTime;
        private bool shouldRecord => timeManager.shouldRecord;
        private GlobalTimeManager timeManager
        {
            get
            {
                if (m_timeManager == null)
                {
                    m_timeManager = GlobalTimeManager.instance;
                }
                return m_timeManager;
            }
        }


        [SerializeField, Required] private GameObject m_bulletRotationalCorrectionObj = null;
        [SerializeField, Required] private SpriteRenderer m_bulletVisual = null;
        [SerializeField, Tag] private string[] m_hitTags = new string[] { "Player", "Clone", "Wall" };
        [SerializeField, BoxGroup("CircleCast")] private LayerMask m_circleCastLayerMask = 0;
        [SerializeField, Min(0.0f), BoxGroup("CircleCast")]
        private float m_circleCastRadius = 0.05f;
        [SerializeField, BoxGroup("CircleCast")]
        private Vector2 m_circleCastOffset = Vector2.zero;
        [SerializeField, Min(0.0f)] private float m_spawnCheckBacktrackDistance = 0.05f;
        [SerializeField, Min(0.0f)] private float m_boxCheckDistance = 1.0f;
        [SerializeField, Required] private Sprite m_normalBulletSprite = null;
        [SerializeField] private ManualSpriteAnimation m_disperseAnimation = new ManualSpriteAnimation();
        [SerializeField] private MixedEvent<GameObject> m_onHit = new MixedEvent<GameObject>();
        [SerializeField] private MixedEvent m_onShowBullet = new MixedEvent();
        [SerializeField] private MixedEvent m_onHideBullet = new MixedEvent();
        [SerializeField, Required, BoxGroup("Sounds")] private SoundRecorder m_enemyKillSound = null;
        [SerializeField, Required, BoxGroup("Sounds")] private SoundRecorder m_wallHitSound = null;
        [SerializeField, BoxGroup("Debugging")] private bool m_printDebugUpdateLogs = false;
        [SerializeField, BoxGroup("Debugging")] private bool m_printDebugHitLogs = false;
        [SerializeField, BoxGroup("Debugging")] private bool m_showBulletPathDebug = false;
        [SerializeField, BoxGroup("Debugging"), ResizableTextArea, ReadOnly] private string m_debugFrames = "";

        private GlobalTimeManager m_timeManager = null;

        private DamageDealer m_dmgDealer = null;

        private readonly List<FiredFrame> m_activeFrames = new List<FiredFrame>();
        private bool m_wasRecording = false;
        private float m_prevPosTime = float.NegativeInfinity;
        private Vector2 m_prevPos = Vector2.negativeInfinity;
        private readonly BulletPhysicsManager.Hit[] m_circleCastHits = new BulletPhysicsManager.Hit[4];


        // Domestic Initialization
        private void Awake()
        {
            m_dmgDealer = this.GetComponentSafe<DamageDealer>();
        }
        // Foreign Initialization
        private void Start()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_bulletRotationalCorrectionObj, nameof(m_bulletRotationalCorrectionObj), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_bulletVisual, nameof(m_bulletVisual), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_normalBulletSprite, nameof(m_normalBulletSprite), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_enemyKillSound, nameof(m_enemyKillSound), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_wallHitSound, nameof(m_wallHitSound), this);
            #endregion Asserts

            m_wasRecording = shouldRecord;
            // Set previous pos and prev pos time
            m_prevPosTime = curTime;
            m_prevPos = CalculatePositionForTime(GetActiveFrame(), m_prevPosTime);
            HideBullet();
        }
        private void FixedUpdate()
        {
            // Just resumed this frame
            if (shouldRecord && !m_wasRecording)
            {
                // Fake resume -> trim
                int t_index = m_activeFrames.Count - 1;
                while (t_index >= 0)
                {
                    FiredFrame t_frame = m_activeFrames[t_index];
                    if (curTime < t_frame.fireTime)
                    {
                        // Rewinded to a point where it hasn't been fired yet.
                        m_activeFrames.RemoveAt(t_index);
                    }
                    else if (curTime < t_frame.hitTime)
                    {
                        // It's been fired, but hasn't hit anything yet.
                        t_frame.hitTime = float.PositiveInfinity;
                    }

                    --t_index;
                }

                // Previous time and previous position will be reset.
                ResetPrevTimeAndPrevPos();
            }

            FiredFrame t_curFrame = GetActiveFrame();
            // During this time, the bullet is not active, so hide it
            if (t_curFrame == null)
            {
                #region Logs
                //CustomDebug.LogForComponent($"curTime ({curTime}) NOT BETWEEN any FiredFrames", this, m_printDebugUpdateLogs);
                #endregion Logs
                HideBullet();
            }
            // If we have data saying we should hit something in the future,
            // get rid of that data since we only care if we hit something during
            // this recording session.
            else if (curTime < t_curFrame.hitTime)
            {
                #region Logs
                //CustomDebug.LogForComponent($"curTime ({curTime}) BEFORE hitTime ({t_curFrame.hitTime}).", this, m_printDebugUpdateLogs);
                #endregion Logs

                transform.position = CalculateWhatPositionShouldBeNow(t_curFrame);
                UpdateRotation(t_curFrame);
                m_bulletVisual.sprite = m_normalBulletSprite;
                ShowBullet();

                // If recording, check if we hit anything
                if (shouldRecord)
                {
                    t_curFrame.ResetHit();
                    CheckForHits(t_curFrame);
                    m_prevPosTime = curTime;
                    m_prevPos = transform.position;
                }
            }
            // Bullet didn't hit something with a death animation (enemy/player/target), need to play the bullet disperse animation
            else if (!t_curFrame.didHitBody && curTime < t_curFrame.hitTime + m_disperseAnimation.animLength)
            {
                transform.position = CalculatePositionForTime(t_curFrame, t_curFrame.hitTime);
                UpdateRotation(t_curFrame);
                m_bulletVisual.sprite = m_disperseAnimation.GetFrameForTime(t_curFrame.hitTime, curTime).sprite;
                ShowBullet();
            }
            // If we have data saying we hit something earlier in the timeline,
            // we want to hide the bullet and not move it.
            else
            {
                #region Logs
                //CustomDebug.LogForComponent($"curTime ({curTime}) AFTER hitTime ({t_curFrame.hitTime})", this, m_printDebugUpdateLogs);
                #endregion Logs
                HideBullet();
            } 

            m_wasRecording = shouldRecord;

//#if UNITY_EDITOR
//            m_debugFrames = "";
//            foreach (FiredFrame t_frame in m_activeFrames)
//            {
//                m_debugFrames += $"{t_frame}\n";
//            }
//#endif
        }


        public bool IsAvailable()
        {
            TimeFrame t_checkAgainstFrame = new TimeFrame(curTime - m_disperseAnimation.animLength, curTime + m_disperseAnimation.animLength);
            foreach (FiredFrame t_frame in m_activeFrames)
            {
                if (t_frame.timeFrame.HasOverlap(t_checkAgainstFrame))
                {
                    return false;
                }
            }
            return true;
        }
        public void AddFireTime(Vector2 startPos, Vector2 velocity)
        {
            if (!shouldRecord)
            {
                #region Logs
                //CustomDebug.LogErrorForComponent($"Tried to add a new fire time at {curTime} even though we are not recording.", this);
                #endregion Logs
                return;
            }
            #region Logs
            if (!IsAvailable())
            {
                //CustomDebug.LogErrorForComponent($"Added a new fire time at {curTime} even though a frame already contains this time.", this);
            }
            #endregion Logs

            FiredFrame t_newFrame = new FiredFrame(curTime, startPos, velocity);
            m_activeFrames.Add(t_newFrame);
            // Reset the prev frame stuff
            ResetPrevTimeAndPrevPos();

            // Check for new hit right away to detect if the bullet spawned inside a wall.
            Vector2 t_dir = velocity.normalized;
            Vector2 t_backtrackPos = startPos - t_dir * m_spawnCheckBacktrackDistance;
            if (GetClosestHitFromCircleCast(t_backtrackPos, t_dir, m_spawnCheckBacktrackDistance, out BulletPhysicsManager.Hit t_closestHit))
            {
                Hit(t_newFrame, t_closestHit.collider.gameObject, curTime);

                //CustomDebug.DrawRay(t_backtrackPos, t_dir * m_spawnCheckBacktrackDistance, Color.magenta, 1.0f, true);
            }
        }
        public float GetLatestFireTime()
        {
            // Assumes that the frames are chronological by their fire time.
            if (m_activeFrames.Count <= 0)
            {
                return float.NegativeInfinity;
            }
            return m_activeFrames[^1].fireTime;
        }


        /// <summary>
        /// Must be called after position is updated.
        /// Does a raycast to search for things that was hit by this bullet's movement.
        /// </summary>
        private void CheckForHits(FiredFrame curFrame)
        {
            // Cur pos != WhatPositionShouldBeNow because we expect this is called before cur pos is updated.
            Vector2 t_pos2D = transform.position;
            Vector2 t_prevPos = m_prevPos;
            Vector2 t_curPos = t_pos2D;
            Vector2 t_diff = t_curPos - t_prevPos;
            float t_dist = t_diff.magnitude;
            Vector2 t_dir = t_diff / t_dist;

            if (!GetClosestHitFromCircleCast(t_prevPos + GetCircleCastOffset(), t_dir, t_dist, out BulletPhysicsManager.Hit t_closestHit))
            {
                // No hits
                return;
            }
            // Do the thing on the closest hit.
            
            float t_prevPosTime = m_prevPosTime;
            float t_curPosTime = curTime;
            float t_percentageDist = t_closestHit.distance / t_dist;
            float t_exactHitTime = Mathf.Lerp(t_prevPosTime, t_curPosTime, t_percentageDist);
            #region Asserts
            //CustomDebug.LogForComponent($"prevPos:{m_prevPos}. curPos:{t_curPos}. prevPosTime:{t_prevPosTime}. curPosTime:{t_curPosTime}. percentageDist = hitDist:{t_closestHit.distance} / distFromPrevToCur:{t_dist} = {t_percentageDist}. exactHitTime:{t_exactHitTime}", this, m_printDebugHitLogs);
            #endregion Asserts
            Hit(curFrame, t_closestHit.collider.gameObject, t_exactHitTime);
        }
        /// <summary>
        /// Checks if the bullet that is at origin will hit anything if it moves in direciton distance amount. Returns true if it hit something, false if it hit nothing.
        /// </summary>
        /// <param name="closestHit">Closest thing it hit.</param>
        private bool GetClosestHitFromCircleCast(Vector2 origin, Vector2 direction, float distance, out BulletPhysicsManager.Hit closestHit)
        {
            int t_hitCount = BulletPhysicsManager.instance.CircleCastNonAlloc(origin, m_circleCastRadius, direction, m_circleCastHits, distance, owner.overrideStaticColliders ? owner.staticCollidersOverride : null);
            BulletPhysicsManager.Hit? t_closestHit = null;
            bool t_willHitBox = false;
            BulletPhysicsManager.Hit? t_closestBoxHit = null;
            #region Logs
            if (t_hitCount > m_circleCastHits.Length)
            {
                //CustomDebug.LogWarningForComponent($"Bullet hit more than the hardcoded, expected number of hits (max:{m_circleCastHits.Length}) (actuallyHit:{t_hitCount}).", this);
            }
            #endregion Logs
            for (int i = 0; i < t_hitCount; ++i)
            {
                BulletPhysicsManager.Hit t_hit = m_circleCastHits[i];
                // Check to see if we should ignore.
                if (owner.ignoreColliders.Contains(t_hit.collider)) { continue; }

                GameObject t_hitObj = t_hit.collider.gameObject;
                #region Logs
                //CustomDebug.LogForComponent($"hit object named {t_hitObj.name}", this, m_printDebugHitLogs);
                #endregion Logs
                float t_closeDist = t_closestHit.HasValue ? t_closestHit.Value.distance : float.PositiveInfinity;

                if (t_hit.collider.targetType == BulletCollider.eBulletTarget.Box)
                {
                    if (t_willHitBox)
                    {
                        if (t_closestBoxHit.Value.distance < t_hit.distance)
                        {
                            t_closestBoxHit = t_hit;
                        }
                    }
                    else
                    {
                        t_willHitBox = true;
                        t_closestBoxHit = t_hit;
                    }
                }
                if (t_hit.distance < t_closeDist && t_hit.distance < distance)
                {
                    t_closestHit = t_hit;
                }
            }
            if (t_closestHit.HasValue)
            {
                bool t_wasBoxHitAlready = t_willHitBox;
                if (!t_willHitBox)
                {
                    // Check extra distance for if there is a box.
                    t_hitCount = BulletPhysicsManager.instance.CircleCastNonAlloc(origin, m_circleCastRadius, direction, m_circleCastHits, t_closestHit.Value.distance + m_boxCheckDistance, owner.overrideStaticColliders ? owner.staticCollidersOverride : null);
                    for (int i = 0; i < t_hitCount; ++i)
                    {
                        BulletPhysicsManager.Hit t_hit = m_circleCastHits[i];
                        if (t_hit.collider.targetType == BulletCollider.eBulletTarget.Box)
                        {
                            t_willHitBox = true;
                        }
                    }
                }

                
                if (t_willHitBox)
                {
                    // Box was hit by the box check distance, so actually ignore.
                    if (!t_wasBoxHitAlready)
                    {
                        closestHit = t_closestHit.Value;
                        // Going to hit a box
                        if (t_closestHit.Value.collider.targetType == BulletCollider.eBulletTarget.Box)
                        {
                            //CustomDebug.Log($"Closest hit was already a box", isDebugging);
                            // Closest hit was the box, so we are good.
                            return true;
                        }
                        else
                        {
                            //CustomDebug.Log($"Ignore the pusher cause a box was close behind it.", isDebugging);
                            // Closest hit was NOT the box, so we ignore it.
                            return false;
                        }
                    }
                    else
                    {
                        // A box was simply among the hit, only ignore it if its too far away from the actual hit.
                        float t_distDiff = Mathf.Abs(t_closestHit.Value.distance - t_closestBoxHit.Value.distance);
                        if (t_distDiff <= m_boxCheckDistance)
                        {
                            //CustomDebug.Log($"Hit Box", isDebugging);
                            // It hits the box
                            closestHit = t_closestBoxHit.Value;
                            return true;
                        }
                        else
                        {
                            //CustomDebug.Log($"Hit Pusher", isDebugging);
                            // It hits the pusher
                            closestHit = t_closestHit.Value;
                            return true;
                        }
                    }
                }
                else
                {
                    // We aren't going to hit a box, so its just a normal hit.
                    closestHit = t_closestHit.Value;
                    return true;
                }
            }
            else
            {
                closestHit = default;
                return false;
            }
        }
        /// <summary>
        /// Calls <see cref="CalculatePositionForTime"/> using curTime as time.
        /// </summary>
        private Vector2 CalculateWhatPositionShouldBeNow(FiredFrame curFrame) => CalculatePositionForTime(curFrame, curTime);
        /// <summary>
        /// Calculate position based on starting position, velocity, given time, and time since fired.
        /// </summary>
        private Vector2 CalculatePositionForTime(FiredFrame curFrame, float time)
        {
            float t_fireTime = 0.0f;
            Vector2 t_vel = Vector2.zero;
            Vector2 t_startPos = Vector2.zero;
            if (curFrame != null)
            {
                t_fireTime = curFrame.fireTime;
                t_vel = curFrame.velocity;
                t_startPos = curFrame.startPos;
            }

            // p_0 + (t-t_0)*v = p
            // where p is position we should be now, t is curTime, v is velocity, p_0 is startPos, and t_0 is firedTime.
            float t_timeSinceFired = time - t_fireTime;
            t_timeSinceFired = Mathf.Max(t_timeSinceFired, 0.0f);
            return t_startPos + t_vel * t_timeSinceFired;
        }
        ///// <summary>
        ///// Calculates the time it should be if the bullet was at the given position.
        ///// NOT TESTED SUPER WELL.
        ///// </summary>
        //private float CalculateTimeFromPosition(Vector2 position)
        //{
        //    // This is t*v = p - p_0 + t_0*v
        //    // where t is the time we want to find, v is velocity, p is given position, p_0 is startPos, and t_0 is firedTime.
        //    // Found from solving CalculatePositionForTime equation for t
        //    Vector2 t_vt = position - startPos + (velocity * firedTime);
        //    // The vector we have now is velocity scaled by the time we want.
        //    if (velocity.x != 0.0f)
        //    {
        //        return t_vt.x / velocity.x;
        //    }
        //    else if (velocity.y != 0.0f)
        //    {
        //        return t_vt.y / velocity.y;
        //    }
        //    else
        //    {
        //        //CustomDebug.LogErrorForComponent($"velocity is <0, 0>", this);
        //        return 0.0f;
        //    }
        //}
        /// <summary>
        /// Returns true if the given GameObject has any of the hit tags.
        /// </summary>
        //private bool HasHitTag(GameObject hitObj)
        //{
        //    foreach (string t_tag in m_hitTags)
        //    {
        //        if (hitObj.CompareTag(t_tag))
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        /// <summary>
        /// The bullet hits something.
        /// </summary>
        private void Hit(FiredFrame curFrame, GameObject hitObject, float determinedHitTime)
        {            
            #region Logs
            //CustomDebug.LogForComponent($"bullet will deal damage to {hitObject.name} at {determinedHitTime} seconds. CurTime is {curTime}.", this, m_printDebugHitLogs);
            #endregion Logs
            transform.position = CalculatePositionForTime(curFrame, determinedHitTime);
            if (!m_dmgDealer.DealDamage(hitObject, determinedHitTime, out bool t_hasTagToDmg))
            {
                // Used to set hit time in here, we no longer do that.
                // In Update, changed:
                //      firedTime <= curTime && curTime < hitTime
                // to be
                //      firedTime <= curTime && curTime <= hitTime
                // So now we just set time no matter if its supposed to be consumable or not.
                return;
            }
            curFrame.didHitBody = t_hasTagToDmg;
            curFrame.hitTime = determinedHitTime;
            //if (curTime > hitTime)
            //{
            //    HideBullet();
            //}

            if (curFrame.didHitBody)
            {
                m_enemyKillSound.Play();
            }
            else
            {
                m_wallHitSound.Play();
            }

            m_onHit.Invoke(hitObject);
        }
        /// <summary>
        /// Hides the bullet so it is no longer seen.
        /// </summary>
        private void HideBullet()
        {
            m_bulletVisual.enabled = false;
            m_onHideBullet.Invoke();
        }
        /// <summary>
        /// Shows the bullet so it is no longer seen.
        /// </summary>
        private void ShowBullet()
        {
            m_bulletVisual.enabled = true;
            m_onShowBullet.Invoke();
        }
        private Vector2 GetCircleCastOffset()
        {
            return m_bulletRotationalCorrectionObj.transform.localRotation * m_circleCastOffset;
        }
        private void ResetPrevTimeAndPrevPos()
        {
            // Note: using Time.deltaTime isn't super correct here, but all we need is a value close to what it would have been at the Update before the new curTime, so this is probably close enough.
            m_prevPosTime = Mathf.Max(0.0f, curTime - Time.deltaTime);
            m_prevPos = CalculatePositionForTime(GetMostRecentFrame(), m_prevPosTime);
        }
        private void UpdateRotation(FiredFrame curFrame)
        {
            // Set rotation from velocity
            Vector2 t_vel = curFrame.velocity;
            float t_angle = Mathf.Atan2(t_vel.y, t_vel.x) * Mathf.Rad2Deg;
            m_bulletRotationalCorrectionObj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, t_angle);
        }

        private FiredFrame GetActiveFrame()
        {
            foreach (FiredFrame t_frame in m_activeFrames)
            {
                if (curTime >= t_frame.fireTime && curTime < t_frame.hitTime + m_disperseAnimation.animLength)
                {
                    return t_frame;
                }
            }
            return null;
        }
        private FiredFrame GetMostRecentFrame()
        {
            FiredFrame t_mostRecentFrame = null;
            foreach (FiredFrame t_frame in m_activeFrames)
            {
                if (curTime >= t_frame.fireTime)
                {
                    if (t_mostRecentFrame == null)
                    {
                        t_mostRecentFrame = t_frame;
                    }
                    else if (t_frame.fireTime > t_mostRecentFrame.fireTime)
                    {
                        t_mostRecentFrame = t_frame;
                    }
                }
            }
            return t_mostRecentFrame;
        }


        public sealed class FiredFrame
        {
            public bool printDebugHitLogs { get; set; } = false;

            public float fireTime => timeFrame.startTime;
            public float hitTime
            {
                get => timeFrame.endTime;
                set => timeFrame = new TimeFrame(fireTime, value);
            }
            public bool didHitBody { get; set; } = false;
            public TimeFrame timeFrame { get; private set; } = TimeFrame.NaN;
            public Vector2 startPos { get; private set; } = new Vector2(float.NaN, float.NaN);
            public Vector2 velocity { get; private set; } = new Vector2(float.NaN, float.NaN);


            public FiredFrame(float fireTime, Vector2 startPos, Vector2 velocity, bool printDebugHitLogs = false)
            {
                this.timeFrame = new TimeFrame(fireTime, float.PositiveInfinity);
                this.startPos = startPos;
                this.velocity = velocity;
                this.printDebugHitLogs = printDebugHitLogs;
            }


            /// <summary>
            /// Gets rid of previous hit information.
            /// </summary>
            public void ResetHit()
            {
                #region Logs
                if (hitTime != float.PositiveInfinity)
                {
                    //CustomDebug.LogForObject($"Resetting the hit after it has had a value of {hitTime}.", this, printDebugHitLogs);
                }
                #endregion Logs
                hitTime = float.PositiveInfinity;
                didHitBody = false;
            }

            public override string ToString()
            {
                return $"{timeFrame}: pos0:{startPos}; vel:{velocity}; didHitBody:{didHitBody}";
            }
        }

        #region Editor Functions
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector2 t_pos2D = transform.position;
            Vector2 t_curPosWOffset = t_pos2D + GetCircleCastOffset();
            Gizmos.DrawWireSphere(t_curPosWOffset, m_circleCastRadius);
            if (Application.isPlaying)
            {
                foreach (FiredFrame t_frame in m_activeFrames)
                {
                    // Show some additional info about the circle cast
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(t_curPosWOffset, t_frame.velocity.normalized);

                    if (m_showBulletPathDebug)
                    {
                    
                            float t_farthestTime = Mathf.Min(farthestTime, t_frame.hitTime);
                            Vector2 t_endPos = CalculatePositionForTime(t_frame, t_farthestTime);
                            Gizmos.color = Color.white;
                            Gizmos.DrawLine(t_frame.startPos, t_endPos);
                            Gizmos.color = Color.blue;
                            Gizmos.DrawWireSphere(t_frame.startPos, m_circleCastRadius * 2);
                            Gizmos.color = t_frame.hitTime <= farthestTime ? Color.red : Color.white;
                            Gizmos.DrawWireSphere(t_endPos, m_circleCastRadius * 2);
                    
                    }
                }
            }
        }
        #endregion Editor Functions
    }
}