using UnityEngine;

using Helpers.Singletons;
using Atma.Settings;
using Helpers;
// Original Authors - Wyatt Senalik

namespace Atma.Translation
{
    [DisallowMultipleComponent]
    public sealed class TranslatorFileReader : DynamicSingletonMonoBehaviourPersistant<TranslatorFileReader>
    {
        private GameSettings m_gameSettings = null;
        private AutoUnloadResource<TextAsset> m_translationCSVResource = null;
        private AutoUnloadResource<TextAsset> m_levelNamesCSVResource = null;


        protected override void OnSingletonCreated()
        {
            base.OnSingletonCreated();

            m_gameSettings = GameSettings.instance;

            m_translationCSVResource = new AutoUnloadResource<TextAsset>("Language Assets/TranslationTable");
            m_levelNamesCSVResource = new AutoUnloadResource<TextAsset>("Language Assets/LevelNamesTable");
        }
        private void Update()
        {
            m_translationCSVResource.Update();
            m_levelNamesCSVResource.Update();
        }


        public string GetTranslatedTextForKey(eTranslationKey key, eLanguage? languageOverride = null)
        {
            // Get which language is currently being used.
            eLanguage t_curLang = languageOverride.HasValue ? languageOverride.Value : m_gameSettings.language;           

            string t_fileText = m_translationCSVResource.resource.text;
            return CSVReader.QueryTable(t_fileText, key.ToString(), t_curLang.ToString());
        }
        public string GetTranslatedLevelName(string enLevelName)
        {
            eLanguage t_curLang = m_gameSettings.language;

            string t_fileText = m_levelNamesCSVResource.resource.text;
            return CSVReader.QueryTable(t_fileText, enLevelName, t_curLang.ToString());
        }
    }
}