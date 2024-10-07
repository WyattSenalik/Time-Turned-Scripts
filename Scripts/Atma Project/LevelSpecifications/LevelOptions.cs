using UnityEngine;

using NaughtyAttributes;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    /// <summary>
    /// Options for the level.
    /// </summary>
    public sealed class LevelOptions : SingletonMonoBehaviour<LevelOptions>
    {
        public int maxCloneCharges => m_maxCloneCharges;
        public bool noTimeLimit => m_noTimeLimit;
        public float time => m_time;

        [SerializeField, Range(0, 3)] private int m_maxCloneCharges = 3;
        [SerializeField] private bool m_noTimeLimit = false;
        [SerializeField, Min(0.0f), ShowIf(nameof(DoesHaveTimeLimit))]
        private float m_time = 10.0f;
        [SerializeField, Scene, ReadOnly, BoxGroup("Obsolete")] private string m_nextLevel = "";
        [SerializeField, ReadOnly, BoxGroup("Obsolete")] private string m_levelFlavorName = "Unspecified Level";


        private bool DoesHaveTimeLimit() => !m_noTimeLimit;
    }
}