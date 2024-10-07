using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Timed
{
    /// <summary>
    /// Implementation of <see cref="ITimedComponent"/> that can be extended to get 
    /// some easy references to the <see cref="ITimedObject"/> that this script is
    /// attached to.
    /// </summary>
    [RequireComponent(typeof(ITimedObject))]
    public abstract class TimedComponent : MonoBehaviour, ITimedComponent
    {
        public ITimedObject timedObject
        {
            get
            {
                InitializeTimedObject();
                return m_timedObject;
            }
        }
        public bool isRecording => timedObject.isRecording;
        public bool wasRecording => timedObject.wasRecording;
        public float curTime => timedObject.curTime;
        public float spawnTime => timedObject.spawnTime;
        public float furthestTime => timedObject.farthestTime;
        public float timeScale => timedObject.timeScale;
        public float deltaTime => timedObject.deltaTime;

        private ITimedObject m_timedObject = null;


        // Domestic Initialization
        protected virtual void Awake()
        {
            InitializeTimedObject();
        }

        private void InitializeTimedObject()
        {
            if (m_timedObject != null) { return; }
            m_timedObject = GetComponent<ITimedObject>();
            #region Asserts
            //CustomDebug.AssertIComponentIsNotNull(m_timedObject, this);
            #endregion Asserts
        }
    }
}