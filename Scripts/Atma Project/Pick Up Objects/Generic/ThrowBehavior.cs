using UnityEngine;
using UnityEngine.Serialization;

using Helpers.UnityInterfaces;
using Helpers.Animation.BetterCurve;
using Timed;
using NaughtyAttributes;
using Timed.Animation.BetterCurve;
// Original Authors - Bryce Cernohous-Schrader
// Tweaked by Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IPickUpObject))]
    public sealed class ThrowBehavior : WindowRecorder<ThrowBehavWindowData>, IPickUpBehavior
    {
        private const bool IS_DEBUGGING = false;

        public IPickUpObject pickUpObject { get; private set; }
        public float windupAnimLength => m_windUpAnimCurve.GetEndTime();
        public float physicsAnimLength => m_physicsAnimLength;
        public eThrowState mostRecentState => mostRecentWindow != null ? mostRecentWindow.data.state : eThrowState.None;

        [SerializeField, Required] private TimedBetterCurveAnimation m_throwAnimation = null;
        [SerializeField, Min(0.0f), FormerlySerializedAs("speed"), FormerlySerializedAs("m_speed")] private float m_rbForceMag = 0;
        [SerializeField] private ContactFilter2D m_enemyCheckFilter = default;
        [SerializeField, Min(0.0f)] private float m_enemyCheckRadius = 0.1f;
        [SerializeField, Tag] private string m_enemyTag = "Enemy";
        [SerializeField, Min(0.0f)] private float m_bounceCooldown = 0.1f;
        [SerializeField, Min(0.0f)] private float m_physicsAnimLength = 0.8333333f;
        [SerializeField] private BetterCurve m_windUpAnimCurve = new BetterCurve();

        private Rigidbody2D m_rigidbody = null;
        private ITransform m_transform = null;

        private readonly Collider2D[] m_enemiesOverlappedArray = new Collider2D[32];
        private float m_bounceTime = float.NegativeInfinity;


        protected override void Awake()
        {
            base.Awake();

            pickUpObject = GetComponent<IPickUpObject>();
            m_rigidbody = GetComponent<Rigidbody2D>();
            m_transform = transform.ToITransform();
            #region Asserts
            //CustomDebug.AssertIComponentIsNotNull(pickUpObject, this);
            //CustomDebug.AssertComponentIsNotNull(m_rigidbody, this);

            //CustomDebug.AssertSerializeFieldIsNotNull(m_throwAnimation, nameof(m_throwAnimation), this);
            #endregion Asserts
        }
        private void Start()
        {
            StartNewWindow(new ThrowBehavWindowData());
        }


        public override void SetToTime(float time)
        {
            base.SetToTime(time);

            if (!isRecording) { return; }

            switch (mostRecentState)
            {
                case eThrowState.None: break;
                case eThrowState.Windup:
                {
                    float t_windupStartTime = mostRecentWindow.window.startTime;
                    float t_windupEndTime = t_windupStartTime + windupAnimLength;
                    if (curTime < t_windupEndTime)
                    {
                        PlayWindupAnim(curTime - t_windupStartTime);
                    }
                    else
                    {
                        StartPhysicsAnimation();
                    }
                    break;
                }
                case eThrowState.Physics:
                {
                    float t_physicsStartTime = mostRecentWindow.window.startTime;
                    float t_physicsEndTime = t_physicsStartTime + physicsAnimLength;
                    if (curTime >= t_physicsEndTime)
                    {
                        // End physics window
                        EndCurrentWindow();
                        StartNewWindow(new ThrowBehavWindowData());
                    }
                    // Check to see if we are hitting an enemy
                    else
                    {
                        int t_enemiesOverlappedCount = Physics2D.OverlapCircle(transform.position, m_enemyCheckRadius, m_enemyCheckFilter, m_enemiesOverlappedArray);
                        bool t_didHitEnemy = false;
                        // Compare tags to make sure its an enemy we hit
                        for (int i = 0; i < t_enemiesOverlappedCount; ++i)
                        {
                            if (m_enemiesOverlappedArray[i].CompareTag(m_enemyTag))
                            {
                                t_didHitEnemy = true;
                                break;
                            }
                        }
                        // We hit an enemy, bounce off them
                        if (t_didHitEnemy)
                        {
                            if (m_bounceTime + m_bounceCooldown < curTime)
                            {
                                m_rigidbody.velocity = -pickUpObject.direction * m_rigidbody.velocity.magnitude;
                                m_bounceTime = curTime;
                            }
                        }
                    }
                    break;
                }
                default:
                {
                    CustomDebug.UnhandledEnum(mostRecentState, this);
                    break;
                }
            }
        }
        public override void OnRecordingResume(float time)
        {
            base.OnRecordingResume(time);

            if (time <= m_bounceTime)
            {
                m_bounceTime = time;
            }
        }

        public void ThrowMe()
        {
            #region Logs
            //CustomDebug.LogForComponent($"{nameof(ThrowMe)} with force ({pickUpObject.direction * m_rbForceMag})", this, IS_DEBUGGING);
            #endregion Logs
            m_transform.parent = null;
            m_transform.localScale = Vector3.one;
            StartWindupAnimation();
        }
        public void TakeAction(bool onPressedOrReleased) { }
        public void OnPickUp() { }
        public void OnReleased() { }


        private void StartWindupAnimation()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(HasCurrentWindow(), $"a window to already exist before starting the windup animation.", this);
            //CustomDebug.AssertIsTrueForComponent(isRecording, $"only to start the windup animation when recording.", this);
            #endregion Asserts
            EndCurrentWindow();
            StartNewWindow(new ThrowBehavWindowData(eThrowState.Windup, pickUpObject));
        }
        private void StartPhysicsAnimation()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(HasCurrentWindow(), $"a window to already exist before starting the physics animation.", this);
            //CustomDebug.AssertIsTrueForComponent(isRecording, $"only to start the physics animation when recording.", this);
            #endregion Asserts
            EndCurrentWindow();
            StartNewWindow(new ThrowBehavWindowData(eThrowState.Physics, pickUpObject));

            m_rigidbody.AddForce(pickUpObject.direction * m_rbForceMag);
            m_throwAnimation.Play();
        }

        private void PlayWindupAnim(float elapsedTimeSinceAnimBegan)
        {
            Vector2 t_dir = mostRecentWindow.data.thrownDir;
            Vector2 t_center = mostRecentWindow.data.thrownObjPosWhenThrown;
            float t_mag = m_windUpAnimCurve.Evaluate(elapsedTimeSinceAnimBegan);
            transform.position = t_center + (t_dir * t_mag);
        }
    }

    public enum eThrowState { None, Windup, Physics }

    public sealed class ThrowBehavWindowData
    {
        public eThrowState state { get; private set; }
        public Vector2 thrownDir { get; private set; }
        public Vector2 thrownObjPosWhenThrown { get; private set; }


        public ThrowBehavWindowData()
        {
            state = eThrowState.None;
            thrownDir = Vector2.zero;
            thrownObjPosWhenThrown = Vector2.zero;
        }
        public ThrowBehavWindowData(eThrowState state, IPickUpObject thrownObj)
        {
            this.state = state;
            thrownDir = thrownObj.direction;
            thrownObjPosWhenThrown = thrownObj.transform.position;
        }
    }
}