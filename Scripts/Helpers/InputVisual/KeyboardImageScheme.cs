using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Helpers.InputVisual
{
    /// <summary>
    /// ScriptableObject that holds the Sprites for a keyboard.
    /// </summary>
    [CreateAssetMenu(fileName = "New KeyboardImageScheme", menuName = "ScriptableObjects/ImageScheme/Keyboard")]
    public sealed class KeyboardImageScheme : ScriptableObject
    {
        public Sprite backTick => m_backTick;
        public Sprite num1 => m_num1;
        public Sprite num2 => m_num2;
        public Sprite num3 => m_num3;
        public Sprite num4 => m_num4;
        public Sprite num5 => m_num5;
        public Sprite num6 => m_num6;
        public Sprite num7 => m_num7;
        public Sprite num8 => m_num8;
        public Sprite num9 => m_num9;
        public Sprite num0 => m_num0;
        public Sprite minus => m_minus;
        public Sprite equals => m_equals;
        public Sprite backspace => m_backspace;
        public Sprite tab => m_tab;
        public Sprite q => m_q;
        public Sprite w => m_w;
        public Sprite e => m_e;
        public Sprite r => m_r;
        public Sprite t => m_t;
        public Sprite y => m_y;
        public Sprite u => m_u;
        public Sprite i => m_i;
        public Sprite o => m_o;
        public Sprite p => m_p;
        public Sprite leftBracket => m_leftBracket;
        public Sprite rightBracket => m_rightBracket;
        public Sprite backSlash => m_backSlash;
        public Sprite caps => m_caps;
        public Sprite a => m_a;
        public Sprite s => m_s;
        public Sprite d => m_d;
        public Sprite f => m_f;
        public Sprite g => m_g;
        public Sprite h => m_h;
        public Sprite j => m_j;
        public Sprite k => m_k;
        public Sprite l => m_l;
        public Sprite semicolon => m_semicolon;
        public Sprite singleQuote => m_singleQuote;
        public Sprite enter => m_enter;
        public Sprite shift => m_shift;
        public Sprite z => m_z;
        public Sprite x => m_x;
        public Sprite c => m_c;
        public Sprite v => m_v;
        public Sprite b => m_b;
        public Sprite n => m_n;
        public Sprite m => m_m;
        public Sprite comma => m_comma;
        public Sprite period => m_period;
        public Sprite forwardSlash => m_forwardSlash;
        public Sprite ctrl => m_ctrl;
        public Sprite alt => m_alt;
        public Sprite space => m_space;
        public Sprite upArrow => m_upArrow;
        public Sprite downArrow => m_downArrow;
        public Sprite leftArrow => m_leftArrow;
        public Sprite rightArrow => m_rightArrow;
        public Sprite unknown => m_unknown;

        [SerializeField] private Sprite m_backTick = null;
        [SerializeField] private Sprite m_num1 = null;
        [SerializeField] private Sprite m_num2 = null;
        [SerializeField] private Sprite m_num3 = null;
        [SerializeField] private Sprite m_num4 = null;
        [SerializeField] private Sprite m_num5 = null;
        [SerializeField] private Sprite m_num6 = null;
        [SerializeField] private Sprite m_num7 = null;
        [SerializeField] private Sprite m_num8 = null;
        [SerializeField] private Sprite m_num9 = null;
        [SerializeField] private Sprite m_num0 = null;
        [SerializeField] private Sprite m_minus = null;
        [SerializeField] private Sprite m_equals = null;
        [SerializeField] private Sprite m_backspace = null;
        [SerializeField] private Sprite m_tab = null;
        [SerializeField] private Sprite m_q = null;
        [SerializeField] private Sprite m_w = null;
        [SerializeField] private Sprite m_e = null;
        [SerializeField] private Sprite m_r = null;
        [SerializeField] private Sprite m_t = null;
        [SerializeField] private Sprite m_y = null;
        [SerializeField] private Sprite m_u = null;
        [SerializeField] private Sprite m_i = null;
        [SerializeField] private Sprite m_o = null;
        [SerializeField] private Sprite m_p = null;
        [SerializeField] private Sprite m_leftBracket = null;
        [SerializeField] private Sprite m_rightBracket = null;
        [SerializeField] private Sprite m_backSlash = null;
        [SerializeField] private Sprite m_caps = null;
        [SerializeField] private Sprite m_a = null;
        [SerializeField] private Sprite m_s = null;
        [SerializeField] private Sprite m_d = null;
        [SerializeField] private Sprite m_f = null;
        [SerializeField] private Sprite m_g = null;
        [SerializeField] private Sprite m_h = null;
        [SerializeField] private Sprite m_j = null;
        [SerializeField] private Sprite m_k = null;
        [SerializeField] private Sprite m_l = null;
        [SerializeField] private Sprite m_semicolon = null;
        [SerializeField] private Sprite m_singleQuote = null;
        [SerializeField] private Sprite m_enter = null;
        [SerializeField] private Sprite m_shift = null;
        [SerializeField] private Sprite m_z = null;
        [SerializeField] private Sprite m_x = null;
        [SerializeField] private Sprite m_c = null;
        [SerializeField] private Sprite m_v = null;
        [SerializeField] private Sprite m_b = null;
        [SerializeField] private Sprite m_n = null;
        [SerializeField] private Sprite m_m = null;
        [SerializeField] private Sprite m_comma = null;
        [SerializeField] private Sprite m_period = null;
        [SerializeField] private Sprite m_forwardSlash = null;
        [SerializeField] private Sprite m_ctrl = null;
        [SerializeField] private Sprite m_alt = null;
        [SerializeField] private Sprite m_space = null;
        [SerializeField] private Sprite m_upArrow = null;
        [SerializeField] private Sprite m_downArrow = null;
        [SerializeField] private Sprite m_leftArrow = null;
        [SerializeField] private Sprite m_rightArrow = null;
        [SerializeField] private Sprite m_unknown = null;

        public Sprite GetSpriteForNumberChar(char numChar)
        {
            switch (numChar)
            {
                case '0': return num0;
                case '1': return num1;
                case '2': return num2;
                case '3': return num3;
                case '4': return num4;
                case '5': return num5;
                case '6': return num6;
                case '7': return num7;
                case '8': return num8;
                case '9': return num9;
                default:
                {
                    CustomDebug.UnhandledValue(numChar, this);
                    return null;
                }
            }
        }
        public Sprite GetSpriteForLetterChar(char letterChar)
        {
            letterChar = letterChar.ToString().ToLower()[0];

            switch (letterChar)
            {
                case 'a': return a;
                case 'b': return b;
                case 'c': return c;
                case 'd': return d;
                case 'e': return e;
                case 'f': return f;
                case 'g': return g;
                case 'h': return h;
                case 'i': return i;
                case 'j': return j;
                case 'k': return k;
                case 'l': return l;
                case 'm': return m;
                case 'n': return n;
                case 'o': return o;
                case 'p': return p;
                case 'q': return q;
                case 'r': return r;
                case 's': return s;
                case 't': return t;
                case 'u': return u;
                case 'v': return v;
                case 'w': return w;
                case 'x': return x;
                case 'y': return y;
                case 'z': return z;
                default:
                {
                    CustomDebug.UnhandledValue(letterChar, this);
                    return null;
                }
            }
        }
    }
}