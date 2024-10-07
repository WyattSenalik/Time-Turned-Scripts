using UnityEngine;

using Atma.Settings;
// Original Authors - Wyatt Senalik

namespace Atma.UI
{
    [DisallowMultipleComponent]
    public sealed class LanguageMenu : MonoBehaviour
    {
        public void ChangeLanguageEnglish()
        {
            GameSettings.instance.SetLanguage(eLanguage.EN);
        }
        public void ChangeLanguageJapanese()
        {
            GameSettings.instance.SetLanguage(eLanguage.JP);
        }
    }
}