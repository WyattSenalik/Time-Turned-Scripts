using System;
using UnityEngine;

using NaughtyAttributes;

using Helpers.Singletons;
// Original Authors - Wyatt Senalik

namespace Atma
{
    public sealed class ToggleSymbolsManager : SingletonMonoBehaviour<ToggleSymbolsManager>
    {
        [SerializeField] private SymbolGroup[] m_symbols = new SymbolGroup[0];

        [SerializeField] private SymbolGroup m_portalSymbols = null;

        public SymbolGroup GetSymbolGroupAtIndex(int index)
        {
            int t_clampedIndex = Mathf.Clamp(index, 0, m_symbols.Length - 1);
            return m_symbols[t_clampedIndex];
        }
        public SymbolGroup GetPortalSymbols()
        {
            return m_portalSymbols;
        }



        [Serializable]
        public sealed class SymbolGroup
        {
            public Sprite activeSymbol => m_activeSymbol;
            public Sprite inactiveSymbol => m_inactiveSymbol;

            [SerializeField] private Sprite m_activeSymbol = null;
            [SerializeField] private Sprite m_inactiveSymbol = null;
        }
    }
}