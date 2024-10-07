using System.Collections.Generic;
using UnityEngine;

using Helpers.Extensions;
using Timed;
// Original Authors - Wyatt Senalik

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TimeClone))]
    public sealed class TimeCloneMoveDirections : MonoBehaviour
    {
        public TimeClone clone { get; private set; }

        [SerializeField] private List<MoveDirAtTime> m_debugList = new List<MoveDirAtTime>();

        private TimedVector2 m_timedMoveDir = null;


        private void Awake()
        {
            clone = GetComponent<TimeClone>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(clone, this);
            #endregion Asserts
            clone.onInitialized.ToggleSubscription(Initialize, true);
        }
        private void OnDestroy()
        {
            if (clone != null)
            {
                clone.onInitialized.ToggleSubscription(Initialize, false);
            }
        }
        private void Initialize()
        {
            GameObject t_playerObj = clone.cloneData.originalPlayerObj;
            PlayerMovement t_playerMove = t_playerObj.GetComponentSafe<PlayerMovement>();
            m_timedMoveDir = new TimedVector2(t_playerMove.timedMoveDir, true);
            foreach (Vector2Snapshot t_snap in m_timedMoveDir.scrapbook.GetInternalData())
            {
                m_debugList.Add(new MoveDirAtTime(t_snap.time, t_snap.data));
            }
        }


        public Vector2 GetMostRecentNonZeroMoveDirectionBeforeTime(float time)
        {
            for (int i = m_timedMoveDir.scrapbook.Count - 1; i >= 0; --i)
            {
                Vector2Snapshot t_snap = m_timedMoveDir.scrapbook.GetSnapshotAtIndex(i);
                if (t_snap.time < time && t_snap.data != Vector2.zero)
                {
                    return t_snap.data;
                }
            }
            return Vector2.zero;
        }
    }

    [System.Serializable]
    public sealed class MoveDirAtTime
    {
        [SerializeField] private float m_time = -1.0f;
        [SerializeField] private Vector2 m_dir = Vector2.negativeInfinity;

        public MoveDirAtTime(float time, Vector2 dir)
        {
            m_time = time;
            m_dir = dir;
        }
    }
}