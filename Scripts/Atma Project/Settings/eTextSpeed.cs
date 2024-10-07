// Original Authors - Wyatt Senalik

using Atma.Translation;

namespace Atma.Settings
{
    public enum eTextSpeed { Slow, Normal, Fast }


    public static class TextSpeedExtensions
    {
        public static float ToCharacterDelayMultiplier(this eTextSpeed textSpeed)
        {
            switch (textSpeed)
            {
                case eTextSpeed.Slow: return 2.0f;
                case eTextSpeed.Normal: return 1.0f;
                case eTextSpeed.Fast: return 0.5f;
                default:
                {
                    CustomDebug.UnhandledEnum(textSpeed, nameof(TextSpeedExtensions.ToCharacterDelayMultiplier));
                    return 1.0f;
                }
            }
        }
        public static string ToText(this eTextSpeed textSpeed)
        {
            TranslatorFileReader t_translator = TranslatorFileReader.instance;
            switch (textSpeed)
            {
                case eTextSpeed.Slow: return t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_TEXT_SPEED_OPT_SLOW);
                case eTextSpeed.Normal: return t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_TEXT_SPEED_OPT_NORMAL);
                case eTextSpeed.Fast: return t_translator.GetTranslatedTextForKey(eTranslationKey.MENU_TEXT_SPEED_OPT_FAST);
                default:
                {
                    CustomDebug.UnhandledEnum(textSpeed, nameof(TextSpeedExtensions.ToCharacterDelayMultiplier));
                    return "?";
                }
            }
        }
    }
}