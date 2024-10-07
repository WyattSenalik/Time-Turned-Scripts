using UnityEngine;

using Timed;
using Helpers.Extensions;

namespace Atma
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class ControlPanelElement : MonoBehaviour
    {
        public SpriteRenderer sprRend { get; private set; } = null;
        public eElementType type => m_type;

        [SerializeField] eElementType m_type = eElementType.Button;
        [SerializeField] private Sprite[] m_sprites = new Sprite[2];
        private TimedInt m_sprIndex = null;


        private void Awake()
        {
            sprRend = GetComponent<SpriteRenderer>();
            #region Asserts
            //CustomDebug.AssertComponentIsNotNull(sprRend, this);
            #endregion Asserts
        }
        private void Start()
        {
            m_sprIndex = new TimedInt(Random.Range(0, m_sprites.Length), eInterpolationOption.Step);
            UpdateSprite();
        }
        private void Update()
        {
            if (!m_sprIndex.isRecording)
            {
                UpdateSprite();
            }
        }


        public void Change()
        {
            if (Random.Range(0, 2) == 0)
            {
                GoPrevious();
            }
            else
            {
                GoNext();
            }
        }
        public void GoNext()
        {
            int t_newIndex = m_sprites.WrapIndex(m_sprIndex.curData + 1);
            m_sprIndex.curData = t_newIndex;
            UpdateSprite();
        }
        public void GoPrevious()
        {
            int t_newIndex = m_sprites.WrapIndex(m_sprIndex.curData - 1);
            m_sprIndex.curData = t_newIndex;
            UpdateSprite();
        }


        private void UpdateSprite()
        {
            sprRend.sprite = m_sprites[m_sprIndex.curData];
        }


        public enum eElementType { Button, BigButton, Lever, Valve, Light }
    }
}