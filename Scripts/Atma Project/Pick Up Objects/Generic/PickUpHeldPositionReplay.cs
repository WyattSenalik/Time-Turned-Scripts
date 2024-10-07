using System;
using System.Collections.Generic;
using UnityEngine;

using Timed;
using Timed.TimedComponentImplementations;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Overrides the position of the pickup set by <see cref="TimedTransform"/>
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(PickUpStateManager))]
    [RequireComponent(typeof(IPickUpObject))]
    public sealed class PickUpHeldPositionReplay : SnapshotRecorder<HeldInfoSnapshot, HeldInfoSnapshotData>
    {
        public PickUpStateManager pickupStateMan { get; private set; }
        public IPickUpObject pickupObj { get; private set; }

        private HeldInfoSnapshotData m_recentSnapData = null;
#if UNITY_EDITOR
        [SerializeField] private DebugHeldInfoSnapshotData m_debugRecentSnapData = null;
        [SerializeField] private DebugHeldInfoScrapbook m_debugScrapbook = null;
#endif


        protected override void Awake()
        {
            base.Awake();

            pickupStateMan = GetComponent<PickUpStateManager>();
            pickupObj = GetComponent<IPickUpObject>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(pickupStateMan, this);
            //CustomDebug.AssertIComponentIsNotNull(pickupObj, this);
            #endregion Asserts
        }
#if UNITY_EDITOR
        private void Start()
        {
            m_debugScrapbook = new DebugHeldInfoScrapbook(scrapbook);
        }
#endif
        private void LateUpdate()
        {
#if UNITY_EDITOR
            m_debugScrapbook.UpdateScrapbook(scrapbook);
#endif
            // Set object's position in late update to go after TimedTransform and override position of the object.
            // Only set if the object is being held and we are not currently recording.
            if (!isRecording && pickupStateMan.curState == ePickupState.Held && m_recentSnapData.holder != null)
            {
                if (m_recentSnapData.holder is UnityEngine.Object t_holderObj)
                {
                    if (t_holderObj != null)
                    {
                        pickupObj.direction = m_recentSnapData.dir;
                        pickupObj.UpdateCarryObjPosition(m_recentSnapData.holdPosCenterWorld, m_recentSnapData.pickupSize);
                    }
                }
            }
        }


        protected override void ApplySnapshotData(HeldInfoSnapshotData snapData)
        {
            m_recentSnapData = snapData;
#if UNITY_EDITOR
            m_debugRecentSnapData = new DebugHeldInfoSnapshotData(m_recentSnapData);
#endif
        }
        protected override HeldInfoSnapshotData GatherCurrentData()
        {
            return new HeldInfoSnapshotData(pickupObj);
        }
        protected override HeldInfoSnapshot CreateSnapshot(HeldInfoSnapshotData data, float time)
        {
            return new HeldInfoSnapshot(data, time, eInterpolationOption.Linear);
        }
    }

    public class HeldInfoSnapshot : ISnapshot<HeldInfoSnapshotData, HeldInfoSnapshot>
    {
        public HeldInfoSnapshotData data { get; private set; } = null;
        public float time { get; private set; } = -1.0f;
        public eInterpolationOption interpolationOpt { get; set; } = eInterpolationOption.Linear;


        public HeldInfoSnapshot(IPickUpObject pickUpObj, float time)
        {
            data = new HeldInfoSnapshotData(pickUpObj);
            this.time = time;
        }
        public HeldInfoSnapshot(HeldInfoSnapshotData data, float time, eInterpolationOption interpolationOpt)
        {
            this.data = data.Clone();
            this.time = time;
            this.interpolationOpt = interpolationOpt;
        }

        public HeldInfoSnapshot Clone() => Clone(time);
        public HeldInfoSnapshot Clone(float timeForClone)
        {
            return new HeldInfoSnapshot(data, time, interpolationOpt);
        }
        public HeldInfoSnapshot Interpolate(HeldInfoSnapshot other, float targetTime)
        {
            SnapshotHelpers.GatherInterpolationInfo<HeldInfoSnapshotData, HeldInfoSnapshot>(this, other, targetTime, out HeldInfoSnapshot t_earlierSnap, out HeldInfoSnapshot t_laterSnap, out eInterpolationOption t_intOpt);

            Vector2 t_interpolatedDir;
            Vector2 t_interpolatedHoldPos;
            float t_interpolatedSize;
            IPickUpHolder t_interpolatedHolder;
            eInterpolateKind t_interpolateKind;
            TimeFrame t_snapFrame = new TimeFrame(t_earlierSnap.time, t_laterSnap.time);
            if (t_snapFrame.ContainsTime(targetTime, eTimeFrameContainsOption.EndExclusive))
            {
                if (t_earlierSnap.data.holder == null)
                {
                    t_interpolateKind = eInterpolateKind.Empty;
                    t_interpolatedHolder = null;
                }
                else if (t_laterSnap.data.holder == null)
                {
                    t_interpolateKind = eInterpolateKind.Early;
                    t_interpolatedHolder = t_earlierSnap.data.holder;
                }
                else
                {
                    t_interpolateKind = eInterpolateKind.Lerp;
                    t_interpolatedHolder = t_earlierSnap.data.holder;
                }
            }
            else if (targetTime >= t_laterSnap.time)
            {
                if (t_laterSnap.data.holder == null)
                {
                    t_interpolateKind = eInterpolateKind.Empty;
                    t_interpolatedHolder = null;
                }
                else if (t_earlierSnap.data.holder == null)
                {
                    t_interpolateKind = eInterpolateKind.Late;
                    t_interpolatedHolder = t_laterSnap.data.holder;
                }
                else
                {
                    t_interpolateKind = eInterpolateKind.Lerp;
                    t_interpolatedHolder = t_laterSnap.data.holder;
                }
            }
            else
            {
                //CustomDebug.LogError($"target time ({targetTime}) is before both snaps ({t_earlierSnap}) and ({t_laterSnap})");
                t_interpolateKind = eInterpolateKind.Lerp;
                t_interpolatedHolder = t_earlierSnap.data.holder;
            }

            // Should really be an enum
            switch (t_interpolateKind)
            {

                case eInterpolateKind.Empty:
                {
                    t_interpolatedDir = Vector2.zero;
                    t_interpolatedHoldPos = Vector2.negativeInfinity;
                    t_interpolatedSize = float.NaN;
                    break;
                }
                case eInterpolateKind.Early:
                {
                    t_interpolatedDir = t_earlierSnap.data.dir;
                    t_interpolatedHoldPos = t_earlierSnap.data.holdPosCenterWorld;
                    t_interpolatedSize = t_earlierSnap.data.pickupSize;
                    break;
                }
                case eInterpolateKind.Late:
                {
                    t_interpolatedDir = t_laterSnap.data.dir;
                    t_interpolatedHoldPos = t_laterSnap.data.holdPosCenterWorld;
                    t_interpolatedSize = t_laterSnap.data.pickupSize;
                    break;
                }
                case eInterpolateKind.Lerp:
                {
                    // Direction
                    // Utilize VectorSnapshot for ease
                    Vector2Snapshot t_earlierDirSnap = new Vector2Snapshot(t_earlierSnap.time, t_earlierSnap.data.dir, t_earlierSnap.interpolationOpt);
                    Vector2Snapshot t_laterDirSnap = new Vector2Snapshot(t_laterSnap.time, t_laterSnap.data.dir, t_laterSnap.interpolationOpt);
                    Vector2Snapshot t_interpolatedDirSnap = t_earlierDirSnap.Interpolate(t_laterDirSnap, targetTime);
                    t_interpolatedDir = t_interpolatedDirSnap.data;

                    // HoldPos
                    // Utilize VectorSnapshot for ease
                    Vector2Snapshot t_earlierHoldPosSnap = new Vector2Snapshot(t_earlierSnap.time, t_earlierSnap.data.holdPosCenterWorld, t_earlierSnap.interpolationOpt);
                    Vector2Snapshot t_laterHoldPosSnap = new Vector2Snapshot(t_laterSnap.time, t_laterSnap.data.holdPosCenterWorld, t_laterSnap.interpolationOpt);
                    Vector2Snapshot t_interpolatedHoldPosSnap = t_earlierHoldPosSnap.Interpolate(t_laterHoldPosSnap, targetTime);
                    t_interpolatedHoldPos = t_interpolatedHoldPosSnap.data;

                    // Size
                    FloatSnapshot t_earlierSizeSnap = new FloatSnapshot(t_earlierSnap.data.pickupSize, t_earlierSnap.time, t_earlierSnap.interpolationOpt);
                    FloatSnapshot t_laterSizeSnap = new FloatSnapshot(t_laterSnap.data.pickupSize, t_laterSnap.time, t_laterSnap.interpolationOpt);
                    FloatSnapshot t_interpolatedSizeSnap = t_earlierSizeSnap.Interpolate(t_laterSizeSnap, targetTime);
                    t_interpolatedSize = t_interpolatedSizeSnap.data;
                    break;
                }
                default:
                {
                    t_interpolatedDir = Vector2.zero;
                    t_interpolatedHoldPos = Vector2.negativeInfinity;
                    t_interpolatedSize = float.NaN;
                    t_interpolatedHolder = null;
                    CustomDebug.UnhandledEnum(t_interpolateKind, nameof(HeldInfoSnapshot));
                    break;
                }
            }

            return new HeldInfoSnapshot(new HeldInfoSnapshotData(t_interpolatedDir, t_interpolatedHolder, t_interpolatedHoldPos, t_interpolatedSize), targetTime, t_intOpt);
        }
        public override string ToString()
        {
            return $"(time={time}; data={data}; interpolation={interpolationOpt})";
        }

        private enum eInterpolateKind { Empty, Early, Late, Lerp }
    }
    public class HeldInfoSnapshotData : IEquatable<HeldInfoSnapshotData> 
    {
        public Vector2 dir { get; private set; } = Vector2.zero;
        public IPickUpHolder holder { get; private set; } = null;
        public Vector2 holdPosCenterWorld { get; private set; } = Vector2.negativeInfinity;
        public float pickupSize { get; private set; } = float.NaN;

        public HeldInfoSnapshotData(IPickUpObject pickUpObj)
        {
            dir = pickUpObj.direction;
            holder = pickUpObj.holder;
            if (holder is UnityEngine.Object t_holderObj)
            {
                if (t_holderObj != null)
                {
                    holdPosCenterWorld = holder.holdPosCenterWorld;
                    pickupSize = holder.pickupSize;
                }
            }
        }
        public HeldInfoSnapshotData(Vector2 dir, IPickUpHolder holder, Vector3 holdPosCenterWorld, float pickupSize)
        {
            this.dir = dir;
            this.holder = holder;
            this.holdPosCenterWorld = holdPosCenterWorld;
            this.pickupSize = pickupSize;
        }


        public HeldInfoSnapshotData Clone()
        {
            return new HeldInfoSnapshotData(dir, holder, holdPosCenterWorld, pickupSize);
        }
        public HeldInfoSnapshotData Interpolate(HeldInfoSnapshotData other, float t, eInterpolationOption interpolationOpt)
        {
            Vector2 t_interpolatedDir;
            Vector2 t_interpolatedHoldPos;
            float t_interpolatedSize;
            IPickUpHolder t_interpolatedHolder;
            if (holder == other.holder)
            {
                // Holders are the same, so we actually lerp as normal
                switch (interpolationOpt)
                {
                    case eInterpolationOption.Linear:
                    {
                        t_interpolatedDir = Vector2.LerpUnclamped(dir, other.dir, t);
                        t_interpolatedHoldPos = Vector2.LerpUnclamped(holdPosCenterWorld, other.holdPosCenterWorld, t);
                        t_interpolatedSize = Mathf.LerpUnclamped(pickupSize, other.pickupSize, t);
                        t_interpolatedHolder = holder;
                        break;
                    }
                    case eInterpolationOption.Step:
                    {
                        t_interpolatedDir = dir;
                        t_interpolatedHoldPos = holdPosCenterWorld;
                        t_interpolatedSize = pickupSize;
                        t_interpolatedHolder = holder;
                        break;
                    }
                    default:
                    {
                        t_interpolatedDir = dir;
                        t_interpolatedHoldPos = holdPosCenterWorld;
                        t_interpolatedSize = pickupSize;
                        t_interpolatedHolder = holder;
                        CustomDebug.UnhandledEnum(interpolationOpt, nameof(HeldInfoSnapshotData));
                        break;
                    }
                }
            }
            else
            {
                if (holder == null)
                {
                    t_interpolatedDir = Vector2.zero;
                    t_interpolatedHoldPos = Vector2.negativeInfinity;
                    t_interpolatedSize = float.NaN;
                    t_interpolatedHolder = null;
                }
                else
                {
                    t_interpolatedDir = dir;
                    t_interpolatedHoldPos = holdPosCenterWorld;
                    t_interpolatedSize = pickupSize;
                    t_interpolatedHolder = holder;
                }
            }
            return new HeldInfoSnapshotData(t_interpolatedDir, t_interpolatedHolder, t_interpolatedHoldPos, t_interpolatedSize);
        }

        public bool Equals(HeldInfoSnapshotData other)
        {
            if (holder == null && other.holder == null)
            {
                return true;
            }
            return dir == other.dir && holder == other.holder && holdPosCenterWorld == other.holdPosCenterWorld && pickupSize == other.pickupSize;
        }
        public override string ToString()
        {
            string t_holderName = "null";
            if (holder is UnityEngine.Object t_holderObj)
            {
                if (t_holderObj != null)
                {
                    t_holderName = t_holderObj.name;
                }
            }
            return $"(dir={dir}; holder={t_holderName}; holdPos={holdPosCenterWorld}; size={pickupSize})";
        }
    }

#if UNITY_EDITOR
    [System.Serializable]
    public sealed class DebugHeldInfoSnapshotData
    {
        [SerializeField] private Vector3 m_recentDir = Vector3.negativeInfinity;
        [SerializeField] private PickupController m_recentHolderPlayer = null;
        [SerializeField] private TimeClonePickup m_recentHolderClone = null;
        [SerializeField] private Vector3 m_recentHoldPosCenterWorld = Vector3.negativeInfinity;
        [SerializeField] private float m_recentPickupSize = float.NaN;

        public DebugHeldInfoSnapshotData(HeldInfoSnapshotData realData)
        {
            m_recentDir = realData.dir;
            m_recentHolderPlayer = realData.holder as PickupController;
            m_recentHolderClone = realData.holder as TimeClonePickup;
            m_recentHoldPosCenterWorld = realData.holdPosCenterWorld;
            m_recentPickupSize = realData.pickupSize;
        }
    }
    [System.Serializable]
    public sealed class DebugHeldInfoSnapshot
    {
        [SerializeField] private float m_time = float.NaN;
        [SerializeField] private DebugHeldInfoSnapshotData m_data = null;

        public DebugHeldInfoSnapshot(HeldInfoSnapshot snap)
        {
            m_time = snap.time;
            m_data = new DebugHeldInfoSnapshotData(snap.data);
        }
    }
    [System.Serializable]
    public sealed class DebugHeldInfoScrapbook
    {
        [SerializeField] private List<DebugHeldInfoSnapshot> m_scrapbook = null;


        public DebugHeldInfoScrapbook(IReadOnlySnapshotScrapbook<HeldInfoSnapshot, HeldInfoSnapshotData> ogScrapbook)
        {
            m_scrapbook = new List<DebugHeldInfoSnapshot>(ogScrapbook.Count);
            foreach (HeldInfoSnapshot t_snap in ogScrapbook.GetInternalData())
            {
                m_scrapbook.Add(new DebugHeldInfoSnapshot(t_snap));
            }
        }

        public void UpdateScrapbook(IReadOnlySnapshotScrapbook<HeldInfoSnapshot, HeldInfoSnapshotData> ogScrapbook)
        {
            m_scrapbook.Clear();
            foreach (HeldInfoSnapshot t_snap in ogScrapbook.GetInternalData())
            {
                m_scrapbook.Add(new DebugHeldInfoSnapshot(t_snap));
            }
        }
    }
#endif
}