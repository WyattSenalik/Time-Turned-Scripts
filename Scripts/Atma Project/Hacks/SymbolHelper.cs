using UnityEngine;

using NaughtyAttributes;

namespace Atma
{
    /// <summary>
    /// Just an editor helper to more quickly apply stuff
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SymbolHelper : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_onSymbolRend = null;
        [SerializeField] private SpriteRenderer m_offSymbolRend = null;

        [SerializeField] private int m_index = 0;
        [SerializeField] private bool m_isInverted = false;


        [Button]
        private void Apply()
        {
            ToggleSymbolsManager t_symbolsMan = FindSymbolsManager();
            ToggleSymbolsManager.SymbolGroup t_symbolGrp = t_symbolsMan.GetSymbolGroupAtIndex(m_index);
            if (m_onSymbolRend != null)
            {
                m_onSymbolRend.sprite = m_isInverted ? t_symbolGrp.inactiveSymbol : t_symbolGrp.activeSymbol;
            }
            if (m_offSymbolRend != null)
            {
                m_offSymbolRend.sprite = m_isInverted ? t_symbolGrp.activeSymbol : t_symbolGrp.inactiveSymbol;
            }
        }
        [Button]
        private void SetToPortalSymbols()
        {
            ToggleSymbolsManager t_symbolsMan = FindSymbolsManager();
            ToggleSymbolsManager.SymbolGroup t_symbolGrp = t_symbolsMan.GetPortalSymbols();
            if (m_onSymbolRend != null)
            {
                m_onSymbolRend.sprite = m_isInverted ? t_symbolGrp.inactiveSymbol : t_symbolGrp.activeSymbol;
            }
            if (m_offSymbolRend != null)
            {
                m_offSymbolRend.sprite = m_isInverted ? t_symbolGrp.activeSymbol : t_symbolGrp.inactiveSymbol;
            }
        }


        private ToggleSymbolsManager FindSymbolsManager()
        {
            ToggleSymbolsManager t_symbolsMan = ToggleSymbolsManager.instance;
            if (t_symbolsMan == null)
            {
                t_symbolsMan = FindObjectOfType<ToggleSymbolsManager>();
                if (t_symbolsMan == null)
                {
                    //CustomDebug.LogErrorForComponent($"No ToggleSymbolsManager in the scene", this);
                }
            }
            return t_symbolsMan;
        }
    }
}