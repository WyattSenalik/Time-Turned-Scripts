using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Helpers.Extensions;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace Dialogue.ConvoActions.Text.Editor
{
    [CustomEditor(typeof(DialogueText))]
    public class DialogueTextEditor : UnityEditor.Editor
    {
        private string m_desiredText = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Desired Text");
            string t_desiredText = EditorGUILayout.TextArea(BuildEditorText());
            ApplyDesiredText(t_desiredText);
        }


        private string BuildEditorText()
        {
            DialogueText t_diaText = target as DialogueText;
            RichText t_text = t_diaText.BuildRichTextFromSubTexts(new ConvoDefaults(Color.white, 24, 0));
            return t_text.GetFormattedText();
        }
        private void ApplyDesiredText(string desiredText)
        {
            m_desiredText = desiredText;
        }

        //private static bool IsDesiredTextValid(string desiredText)
        //{
        //    Stack<eRichTag> t_foundTagOPs = new Stack<eRichTag>();
        //    Stack<eRichTag> t_foundTagEDs = new Stack<eRichTag>();

        //    List<int> t_colorOPIndices = desiredText.FindAllIndicesOf(RichSubText.COLOR_TAG_OP_BEGIN_PART);
        //    List<int> t_colorEDIndices = desiredText.FindAllIndicesOf(RichSubText.COLOR_TAG_ED);
        //    List<int> t_colorEDIndices = desiredText.FindAllIndicesOf(RichSubText.COLOR_TAG_ED);
        //}
        //private void CreateScriptableObjectsFromDesiredText(string desiredText)
        //{
        //    // TODO: Clear the dialogue sub texts.

        //    CreateSubTextForDesiredText(desiredText);
        //}
        //private void CreateSubTextForDesiredText(string desiredText)
        //{
        //    int t_firstTagIndex = desiredText.IndexOf('<');
        //    if (t_firstTagIndex < 0)
        //    {
        //        DialogueSubText t_subTextSO = CreateInstance<DialogueSubText>();

        //    }
        //}
    }
}