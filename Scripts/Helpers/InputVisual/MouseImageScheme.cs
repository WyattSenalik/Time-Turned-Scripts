using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.InputVisual
{
    [CreateAssetMenu(fileName = "New MouseImageScheme", menuName = "ScriptableObjects/ImageScheme/Mouse")]
    public sealed class MouseImageScheme : ScriptableObject
    {
        public Sprite fullMouse => m_fullMouse;
        public Sprite leftClick => m_leftClick;
        public Sprite rightClick => m_rightClick;
        public Sprite middleClick => m_middleClick;
        public Sprite scroll => m_scroll;

        [SerializeField] private Sprite m_fullMouse = null;
        [SerializeField] private Sprite m_leftClick = null;
        [SerializeField] private Sprite m_rightClick = null;
        [SerializeField] private Sprite m_middleClick = null;
        [SerializeField] private Sprite m_scroll = null;
    }
}