using System;
using UnityEditor;
using UnityEngine;

using Dialogue.ConvoActions.Pause;
using Dialogue.ConvoActions.Programmed;
using Dialogue.ConvoActions.Text;
// Original Authors - Wyatt Senalik and Eslis Vang

namespace Dialogue.Editor
{
    [CustomEditor(typeof(Conversation))]
    public sealed class ConversationEditor : UnityEditor.Editor
    {
        private eConvoActionType m_actionToMake = eConvoActionType.Null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            m_actionToMake = (eConvoActionType)EditorGUILayout.Popup("Type", (int)m_actionToMake, Enum.GetNames(typeof(eConvoActionType)));
            if (GUILayout.Button("Create New Action"))
            {
                ConvoActionObject t_newAction = CreateNewConvoActionOfType(m_actionToMake);
                if (t_newAction != null)
                {
                    CreateNecessaryFoldersIfNeeded();
                    string t_newAssetPath = AssetDatabase.GenerateUniqueAssetPath($"{GetPathToThisConvosActionsFolder()}/{m_actionToMake}.asset");
                    AssetDatabase.CreateAsset(t_newAction, t_newAssetPath);
                }
                Conversation t_convo = target as Conversation;
                t_convo.AddActionToList(t_newAction);
                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndHorizontal();
        }

        private ConvoActionObject CreateNewConvoActionOfType(eConvoActionType actionType)
        {
            Type t_typeToCreate = actionType.ConvertToLiteralType();
            if (t_typeToCreate == null)
            {
                return null;
            }
            else
            {
                return CreateInstance(t_typeToCreate) as ConvoActionObject;
            }
        }
        private string GetPathToThisConvosFolder()
        {
            string t_pathToAsset = AssetDatabase.GetAssetPath(target);
            int t_lastSlashIndex = t_pathToAsset.LastIndexOf('/');
            return t_pathToAsset.Remove(t_lastSlashIndex);
        }
        private string GetPathToThisConvosActionsFolder()
        {
            return $"{GetPathToThisConvosFolder()}/Actions";
        }
        private void CreateNecessaryFoldersIfNeeded()
        {
            CreateActionsFolderIfItDoesntExist();
        }
        private void CreateActionsFolderIfItDoesntExist()
        {
            string t_path = GetPathToThisConvosActionsFolder();
            if (!AssetDatabase.IsValidFolder(t_path))
            {
                string t_actionsFolderGUI = AssetDatabase.CreateFolder(GetPathToThisConvosFolder(), "Actions");
                Debug.Log($"Created actions folder ({t_actionsFolderGUI})");
            }
            else
            {
                Debug.Log("Actions folder already exists");
            }
        }
    }
    public enum eConvoActionType { Text, Pause, Programmable, Other, Null }
    public static class ConvoActionTypeExtensions
    {
        public static Type ConvertToLiteralType(this eConvoActionType enumVal)
        {
            switch (enumVal)
            {
                case eConvoActionType.Text: return typeof(DialogueText);
                case eConvoActionType.Pause: return typeof(PauseConvoAction);
                case eConvoActionType.Programmable: return typeof(ProgrammableConvoAction);
                case eConvoActionType.Other: return null;
                case eConvoActionType.Null: return null;
                default:
                {
                    CustomDebug.UnhandledEnum(enumVal, nameof(ConvertToLiteralType));
                    return null;
                }
            }
        }
        public static eConvoActionType DetermineTypeOfConvoAction(this ConvoActionObject convoAction)
        {
            if (convoAction == null)
            {
                return eConvoActionType.Null;
            }
            else if (convoAction is DialogueText)
            {
                return eConvoActionType.Text;
            }
            else if (convoAction is PauseConvoAction)
            {
                return eConvoActionType.Pause;
            }
            else if (convoAction is ProgrammableConvoAction)
            {
                return eConvoActionType.Programmable;
            }
            else
            {
                return eConvoActionType.Other;
            }
        }
    }
}