using System;
using System.Collections.Generic;
using UnityEngine;

using Timed;
using NaughtyAttributes;
using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
using static Atma.PushableBoxCollider;
using Helpers.UnityEnums;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Used to check if the object is a box. Can't use a tag since push box's tag is being used for other purposes already.
    /// Also restricts the rigidbody to only allow the box to be pushed in a single direction at a time.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SoundRecorder))]
    public sealed class PushBox : WindowRecorder<PushBoxWindow>
    {
        public IReadOnlyList<PushInfo> curPusherInfos => curWindow.pusherInfos;
        public bool isBeingPushed
        {
            get
            {
                if (curWindow == null) { return false; }

                foreach (PushInfo t_pushInfo in curPusherInfos)
                {
                    if (t_pushInfo.boxPusher != eBoxPusher.None && t_pushInfo.pushDir != ePushDirection.None && t_pushInfo.handsBeingUsed > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public PushBoxWindow curWindow { get; private set; } = null;

        [SerializeField, Required] private PushableBoxCollider m_pushableCollider = null;

        [SerializeField, Tag] private string m_playerTag = "Player";
        [SerializeField, Tag] private string m_cloneTag = "Clone";
        [SerializeField, Tag] private string m_enemyTag = "Enemy";

        [SerializeField] private bool m_isDebugging = false;

        private Rigidbody2D m_rb2D = null;
        private SoundRecorder m_soundRecorder = null;
        private readonly List<PushInfo> m_activeCollisionPushes = new List<PushInfo>();

        [SerializeField, ResizableTextArea, ReadOnly] private string m_debugWindows = "";


        protected override void Awake()
        {
            base.Awake();
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_pushableCollider, nameof(m_pushableCollider), this);
            #endregion Asserts
            m_rb2D = this.GetComponentSafe<Rigidbody2D>();
            m_soundRecorder = this.GetComponentSafe<SoundRecorder>();
        }
        private void Start()
        {
            curWindow = new PushBoxWindow();
            StartNewWindow(curWindow);

            ToggleSubscriptions(true);
        }
        private void OnDestroy()
        {
            ToggleSubscriptions(false);
        }
        private void FixedUpdate()
        {
            if (!isRecording) { return; }

            // Update the window, update the rigidbody, then clear the collision.
            UpdateSound();
            UpdateWindow();
            UpdateRBConstraintsBasedOnCurrentWindow();

            m_activeCollisionPushes.Clear();
        }
#if UNITY_EDITOR
        private void Update()
        {
            m_debugWindows = "";
            foreach (WindowData<PushBoxWindow> t_window in windowCollection)
            {
                m_debugWindows += $"{t_window.window}: {t_window.data}\n";
            }
        }
#endif

        protected override void SetToTimeRewindingDuringWindow(WindowData<PushBoxWindow> windowData)
        {
            base.SetToTimeRewindingDuringWindow(windowData);

            curWindow = windowData.data;
        }
        public override void OnRecordingStop(float time)
        {
            base.OnRecordingStop(time);

            m_rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        protected override void OnRecordingResumeDuringWindow(WindowData<PushBoxWindow> windowResumedDuring)
        {
            base.OnRecordingResumeDuringWindow(windowResumedDuring);

            curWindow = windowResumedDuring.data;
        }


        private void ToggleSubscriptions(bool cond)
        {
            if (m_pushableCollider != null)
            {
                m_pushableCollider.onCollision.ToggleSubscription(OnCustomColliderCollision, cond);
            }
        }
        private void OnCustomColliderCollision(CollisionInfo colInfo)
        {
            eBoxPusher t_pusher = DetermineBoxPusher(colInfo);
            ePushDirection t_pushAxis = DeterminePushAlongAxisCollisionActive(colInfo);
            if (t_pushAxis == ePushDirection.None || t_pusher == eBoxPusher.None) { return; }
            byte t_amountHands = DetermineHandAmount(colInfo, t_pusher);
            PushInfo t_info = new PushInfo(t_pushAxis, t_pusher, t_amountHands, colInfo.otherCollider.gameObject, colInfo);
            bool t_alreadyContains = false;
            for (int i = 0; i < m_activeCollisionPushes.Count; ++i)
            {
                PushInfo t_curInfo = m_activeCollisionPushes[i];
                if (t_curInfo.Equals(t_info))
                {
                    t_alreadyContains = true;
                    break;
                }
            }
            if (!t_alreadyContains)
            {
                m_activeCollisionPushes.Add(t_info);
            }
        }
        private eBoxPusher DetermineBoxPusher(CollisionInfo colInfo)
        {
            PushableBoxCollider t_otherCol = colInfo.otherCollider;
            if (t_otherCol.isImmobile) { return eBoxPusher.None; }
            Int2DTransform t_otherRootTrans = t_otherCol.rootTransform;
            #region Logs
            if (t_otherRootTrans == null)
            {
                //CustomDebug.LogErrorForComponent($"Expected a root transform for {t_otherCol.gameObject.GetFullName()} to exist", this);
                return eBoxPusher.None;
            }
            #endregion Logs
            GameObject t_otherRootGO = t_otherRootTrans.gameObject;
            if (t_otherRootGO.CompareTag(m_playerTag))
            {
                return eBoxPusher.Player;
            }
            else if (t_otherRootGO.CompareTag(m_cloneTag))
            {
                return eBoxPusher.Clone;
            }
            else if (t_otherRootGO.CompareTag(m_enemyTag))
            {
                return eBoxPusher.Chaser;
            }
            else if (t_otherRootGO.TryGetComponent(out PushBox _))
            {
                return eBoxPusher.OtherBox;
            }
            else
            {
                #region Logs
                //CustomDebug.LogErrorForComponent($"Collision occurred with {t_otherCol.gameObject.GetFullName()} but was unable to determine what kind of box pusher it was.", this);
                #endregion Logs
                return eBoxPusher.None;
            }
        }
        private ePushDirection DeterminePushAlongAxisCollisionActive(CollisionInfo colInfo)
        {
            Vector2 t_fourDirVector;
            Vector2 t_invertedDirMoved = -colInfo.directionIMoved;
            if (t_invertedDirMoved.x == 0.0f || t_invertedDirMoved.y == 0.0f)
            {
                t_fourDirVector = t_invertedDirMoved;
            }
            else
            {
                // Multi dir to move
                Vector2 t_pos2D = transform.position;
                Int2DTransform t_otherRootTrans = colInfo.otherCollider.rootTransform;
                if (t_otherRootTrans == null)
                {
                    return ePushDirection.None;
                }
                Vector2 t_diffToPusher = t_otherRootTrans.positionFloat - t_pos2D;
                if (Mathf.Abs(t_diffToPusher.x) > Mathf.Abs(t_diffToPusher.y))
                {
                    t_fourDirVector = new Vector2(t_diffToPusher.x, 0.0f);
                }
                else
                {
                    t_fourDirVector = new Vector2(0.0f, t_diffToPusher.y);
                }

                if (Vector2.Dot(t_fourDirVector, -colInfo.directionIMoved) < 0.0f)
                {
                    return ePushDirection.None;
                }
            }            

            eEightDirection t_closeFourDir = t_fourDirVector.ToClosestFourDirection();
            switch (t_closeFourDir)
            {
                case eEightDirection.Right: return ePushDirection.Right;
                case eEightDirection.Left: return ePushDirection.Left;
                case eEightDirection.Up: return ePushDirection.Up;
                case eEightDirection.Down: return ePushDirection.Down;
                default:
                {
                    CustomDebug.UnhandledEnum(t_closeFourDir, this);
                    return ePushDirection.None;
                }
            }
        }
        private byte DetermineHandAmount(CollisionInfo colInfo, eBoxPusher pusher)
        {
            switch (pusher)
            {
                case eBoxPusher.None: return 0;
                case eBoxPusher.Player:
                {
                    Int2DTransform t_rootTrans = colInfo.otherCollider.rootTransform;
                    PickupController t_pickupCont = t_rootTrans.GetComponentSafe<PickupController>();
                    return (byte)(t_pickupCont.isCarrying ? 1 : 2);
                }
                case eBoxPusher.Clone:
                {
                    Int2DTransform t_rootTrans = colInfo.otherCollider.rootTransform;
                    TimeClonePickup t_pickupCont = t_rootTrans.GetComponentSafe<TimeClonePickup>();
                    return (byte)(t_pickupCont.isCarrying ? 1 : 2);
                }
                case eBoxPusher.Chaser: return 2;
                case eBoxPusher.OtherBox: return 0;
                case eBoxPusher.Door: return 0;
                default:
                {
                    CustomDebug.UnhandledEnum(pusher, this);
                    return 2;
                }
            }
        }

        private void UpdateSound()
        {
            bool t_isBeingPushedByCharacter = false;
            foreach (PushInfo t_pushInfo in m_activeCollisionPushes)
            {
                switch (t_pushInfo.boxPusher)
                {
                    case eBoxPusher.Player:
                    case eBoxPusher.Clone:
                    case eBoxPusher.Chaser:
                    {
                        t_isBeingPushedByCharacter = true;
                        break;
                    }
                    case eBoxPusher.None:
                    case eBoxPusher.OtherBox:
                    case eBoxPusher.Door:
                    {
                        break;
                    }
                    default:
                    {
                        CustomDebug.UnhandledEnum(t_pushInfo.boxPusher, this);
                        break;
                    }
                }
                if (t_isBeingPushedByCharacter)
                {
                    break;
                }
            }

            if (t_isBeingPushedByCharacter)
            {
                // Play if not already playing.
                if (!m_soundRecorder.IsPlaying())
                {
                    m_soundRecorder.Play();
                }
            }
            else
            {
                // Stop if it is playing
                if (m_soundRecorder.IsPlaying())
                {
                    m_soundRecorder.Stop();
                }
            }
        }
        private void UpdateWindow()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(HasCurrentWindow(), $"a window to currently exist.", this);
            #endregion Asserts
            // Make sure the window isn't the same.
            PushBoxWindow t_potentialNewWindow = new PushBoxWindow(m_activeCollisionPushes.ToArray());
            if (!curWindow.Equals(t_potentialNewWindow))
            {
                EndCurrentWindow();
                curWindow = t_potentialNewWindow;
                StartNewWindow(curWindow);
            }
        }
        private void UpdateRBConstraintsBasedOnCurrentWindow()
        {
            IReadOnlyList<PushInfo> t_pushInfos = curWindow.pusherInfos;
            Vector2Int t_resultingVectors = Vector2Int.zero;
            foreach (PushInfo t_singleInfo in t_pushInfos)
            {
                t_resultingVectors += t_singleInfo.pushDir.ToVector();
            }
            float t_absX = Mathf.Abs(t_resultingVectors.x);
            float t_absY = Mathf.Abs(t_resultingVectors.y);
            // If both ARE NOT zero or both ARE zero, only restrict rotation
            if ((t_absX > 0 && t_absY > 0) || (t_absX == 0.0f && t_absY == 0.0f))
            {
                m_rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            // Only x is non zero
            else if (t_absX > 0)
            {
                m_rb2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            }
            // Only y is non zero
            else if (t_absY > 0)
            {
                m_rb2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                CustomDebug.ThrowAssertionFail($"unhandled case of if/else", this);
            }
        }
    }

    public enum ePushDirection { None, Left, Right, Up, Down }
    public enum eBoxPusher { None, Player, Clone, Chaser, OtherBox, Door }

    public sealed class PushBoxWindow
    {
        public PushInfo[] pusherInfos { get; private set; } = null;


        public PushBoxWindow()
        {
            pusherInfos = new PushInfo[0];
        }
        public PushBoxWindow(params PushInfo[] pusherInfos)
        {
            this.pusherInfos = pusherInfos;
        }


        public bool Equals(PushBoxWindow other) => Equals(other.pusherInfos);
        public bool Equals(params PushInfo[] pusherInfos)
        {
            if (this.pusherInfos.Length != pusherInfos.Length) { return false; }

            List<int> t_matchIndices = new List<int>(this.pusherInfos.Length);
            for (int i = 0; i < this.pusherInfos.Length; ++i)
            {
                PushInfo t_thisInfo = this.pusherInfos[i];
                for (int k = 0; k < pusherInfos.Length; ++k)
                {
                    PushInfo t_otherInfo = pusherInfos[k];
                    // Check for match
                    if (t_thisInfo.Equals(t_otherInfo))
                    {
                        // Ensure the match is unique
                        if (!t_matchIndices.Contains(k))
                        {
                            t_matchIndices.Add(k);
                            break;
                        }
                    }
                }
            }
            // Only return true if every pusher found a unique match.
            return t_matchIndices.Count == this.pusherInfos.Length;
        }
        public override string ToString()
        {
            string t_rtnStr = "[";
            if (pusherInfos.Length > 0)
            {
                t_rtnStr += $"({pusherInfos[0]})";
            }
            for (int i = 1; i < pusherInfos.Length; ++i)
            {
                t_rtnStr += $", ({pusherInfos[i]})";
            }
            t_rtnStr += "]";
            return t_rtnStr;
        }
    }
    public sealed class PushInfo
    {
        public ePushDirection pushDir { get; private set; } = ePushDirection.None;
        public eBoxPusher boxPusher { get; private set; } = eBoxPusher.None;
        public byte handsBeingUsed { get; private set; } = byte.MaxValue;
        public GameObject boxPusherObject { get; private set; } = null;
        public CollisionInfo collisionInfo { get; private set; } = null;

        public PushInfo(ePushDirection pushDir, eBoxPusher boxPusher, byte handsBeingUsed, GameObject boxPusherObject, CollisionInfo collisionInfo)
        {
            this.pushDir = pushDir;
            this.boxPusher = boxPusher;
            this.handsBeingUsed = handsBeingUsed;
            this.boxPusherObject = boxPusherObject;
            this.collisionInfo = collisionInfo;
        }


        public bool Equals(PushInfo other)
        {
            return Equals(other.pushDir, other.boxPusher, other.handsBeingUsed, other.boxPusherObject);
        }
        public bool Equals(ePushDirection otherPush, eBoxPusher otherBoxPusher, byte otherHandsBeingUsed, GameObject otherBoxPusherObject)
        {
            return pushDir == otherPush && boxPusher == otherBoxPusher && handsBeingUsed == otherHandsBeingUsed && boxPusherObject == otherBoxPusherObject;
        }
        public override string ToString()
        {
            return $"Dir:{pushDir}; Pusher:{boxPusher}; HandCount:{handsBeingUsed}; BoxPusherObject:{boxPusherObject}; CollisionInfo:({collisionInfo})";
        }
    }

    public static class PushDirectionExtensions
    {
        public static Vector2Int ToVector(this ePushDirection pushDir)
        {
            switch (pushDir)
            {
                case ePushDirection.None: return Vector2Int.zero;
                case ePushDirection.Left: return Vector2Int.left;
                case ePushDirection.Right: return Vector2Int.right;
                case ePushDirection.Up: return Vector2Int.up;
                case ePushDirection.Down: return Vector2Int.down;
                default:
                {
                    CustomDebug.UnhandledEnum(pushDir, nameof(PushDirectionExtensions));
                    return Vector2Int.zero;
                }
            }
        }
        public static ePushDirection[] ToPushDirections(this Vector2Int vector)
        {
            List<ePushDirection> t_dirs = new List<ePushDirection>();

            if (vector.x < 0)
            {
                t_dirs.Add(ePushDirection.Left);
            }
            else if (vector.x > 0)
            {
                t_dirs.Add(ePushDirection.Right);
            }

            if (vector.y < 0)
            {
                t_dirs.Add(ePushDirection.Down);
            }
            else if (vector.y > 0)
            {
                t_dirs.Add(ePushDirection.Up);
            }

            if (t_dirs.Count > 0)
            {
                return t_dirs.ToArray();
            }
            else
            {
                return new ePushDirection[1] { ePushDirection.None };
            }
        }
    }
}