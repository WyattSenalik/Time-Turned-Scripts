using Helpers.Extensions;
using NaughtyAttributes;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Literally just spawns the specified prefab. Need to spawn this because it can't be the same object as the time clone because that time clone will turn itself off when it disappears, which then of course we can't see the object anymore, so the disappear animation won't show the ending half.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SpawnTimeCloneDisappearAnimation : MonoBehaviour
    {
        public TimeCloneDisappearAnimation disappearAnimation { get; private set; } = null;

        [SerializeField, Required] private GameObject m_timeCloneDisappearAnimPrefab = null;
        [SerializeField, Required] private TimeClone m_clone = null;
        [SerializeField, Required] private TimeCloneBlink m_cloneBlink = null;
        [SerializeField, Required] private TimeCloneHealth m_cloneHealth = null;

        private GameObject m_spawnedInstance = null;


        private void Start()
        {
            #region Asserts
            //CustomDebug.AssertSerializeFieldIsNotNull(m_timeCloneDisappearAnimPrefab, nameof(m_timeCloneDisappearAnimPrefab), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_clone, nameof(m_clone), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneBlink, nameof(m_cloneBlink), this);
            //CustomDebug.AssertSerializeFieldIsNotNull(m_cloneHealth, nameof(m_cloneHealth), this);
            #endregion Asserts

            m_spawnedInstance = Instantiate(m_timeCloneDisappearAnimPrefab);
            disappearAnimation = m_spawnedInstance.GetComponentSafe<TimeCloneDisappearAnimation>(this);
            disappearAnimation.Initialize(m_clone, m_cloneBlink, m_cloneHealth);
        }
        private void OnDestroy()
        {
            if (m_spawnedInstance != null)
            {
                Destroy(m_spawnedInstance);
            }
        }
    }
}