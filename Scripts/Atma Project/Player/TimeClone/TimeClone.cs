using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

using Helpers;
using Timed;
using Helpers.Events;
using Helpers.Events.CatchupEvents;
using Helpers.Extensions;
using Helpers.Physics.Custom2DInt;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Class that will have the init data injected into it for
    /// other time clone related classes to access.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ITimedObject))]
    public sealed class TimeClone : MonoBehaviour
    {
        public bool isDisconnected { get; set; } = false;
        public ITimedObject timedObject { get; private set; }
        public Int2DTransform intTransform { get; private set; } = null;
        public TimeCloneHealth health { get; private set; } = null;
        public TimeCloneDisappearAnimation disappearAnimation => disappearAnimationSpawner.disappearAnimation;
        public SpawnTimeCloneDisappearAnimation disappearAnimationSpawner { get; private set; } = null;
        public TimeCloneBlink cloneBlink { get; private set; } = null;
        public TimeCloneInitData cloneData { get; private set; }
        public IReadOnlyDictionary<int, float> earlyDisappearTimesDict => m_earlyDisappearTimesDict;
        public IEventPrimer onInitialized => m_onInitialized;

        private readonly CatchupEvent m_onInitialized = new CatchupEvent();

        private readonly Dictionary<int, float> m_earlyDisappearTimesDict = new Dictionary<int, float>();
        private readonly IDLibrary m_earlyDisappearTimeIdLibrary = new IDLibrary();


        private void Awake()
        {
            timedObject = this.GetIComponentSafe<ITimedObject>(this);
            intTransform = this.GetComponentSafe<Int2DTransform>(this);
            health = this.GetComponentSafe<TimeCloneHealth>(this);
            disappearAnimationSpawner = this.GetComponentSafe<SpawnTimeCloneDisappearAnimation>(this);
            cloneBlink = this.GetComponentSafe<TimeCloneBlink>(this);
        }
        private void Start()
        {
            #region Asserts
            //CustomDebug.AssertIsTrueForComponent(cloneData != null, $"Time Clone was spawned, but was not given data.", this);
            #endregion Asserts
        }


        public void Initialize(TimeCloneInitData initData)
        {
            cloneData = initData;
            m_onInitialized.Invoke();
        }

        /// <summary>
        /// NOT FOR ACTUAL DEATHS. 
        /// If clone is shot by a bullet or hit by an enemy, the clone's health takes damage and starts time manipulation. What this does is just makes the clone fade away earlier.
        /// </summary>
        public int RegisterEarlyDisappearTime(float timeOfDeath)
        {
            int t_deathID = m_earlyDisappearTimeIdLibrary.CheckoutID();
            m_earlyDisappearTimesDict.Add(t_deathID, timeOfDeath);
            return t_deathID;
        }
        public void UnregisterEarlyDisappearTime(int deathID)
        {
            m_earlyDisappearTimesDict.Remove(deathID);
            m_earlyDisappearTimeIdLibrary.ReturnID(deathID);
        }
        public bool HasEarlyDisappearanceBeforeOrAtTime(float time)
        {
            foreach (KeyValuePair<int, float> t_kvp in m_earlyDisappearTimesDict)
            {
                float t_deathTime = t_kvp.Value;

                if (t_deathTime <= time)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// If has no disappearance times, then returns positive infinity.
        /// </summary>
        public float DetermineEarliestDisappearanceTime()
        {
            float t_earliestDeathTime = float.PositiveInfinity;
            foreach (KeyValuePair<int, float> t_kvp in m_earlyDisappearTimesDict)
            {
                if (t_kvp.Value < t_earliestDeathTime)
                {
                    t_earliestDeathTime = t_kvp.Value;
                }
            }
            return t_earliestDeathTime;
        }



#if UNITY_EDITOR
        [SerializeField, ReadOnly] private bool m_hasEarlyDisappearance = false;
        [SerializeField, ReadOnly] private float m_earliestDisappearanceTime = float.NegativeInfinity;
        [SerializeField, ReadOnly] private int m_earlyDisappearanceAmount = 0;
        private void Update()
        {
            m_hasEarlyDisappearance = HasEarlyDisappearanceBeforeOrAtTime(GlobalTimeManager.instance.curTime);
            m_earliestDisappearanceTime = DetermineEarliestDisappearanceTime();
            m_earlyDisappearanceAmount = m_earlyDisappearTimesDict.Count;
        }
#endif
    }
}