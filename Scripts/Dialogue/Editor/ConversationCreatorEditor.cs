using UnityEditor;
using UnityEngine;
// Original Authors - Wyatt Senalik

namespace Dialogue.Editor
{
    public sealed class ConversationCreatorEditor : EditorWindow
    {
        private const string CONVO_PARENT_FOLDER_PATH = "Assets/GameFiles/Dialogue/Conversations";
        private const string CONVO_SUFFIX = "_CONVO";

        private static bool s_shouldRefreshConvosList = false;

        private string m_nameOfNewConvo = "";
        private Conversation[] m_convoList = null;


        [MenuItem("Tools/ConversationCreator")]
        public static void ShowWindow()
        {
            GetWindow<ConversationCreatorEditor>("Conversation Creator");
            s_shouldRefreshConvosList = true;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name of New Convo");
            m_nameOfNewConvo = GUILayout.TextField(m_nameOfNewConvo);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Create New Convo"))
            {
                CreateNewConversation(m_nameOfNewConvo);
                s_shouldRefreshConvosList = true;
            }

            GUILayout.Space(50);
            GUILayout.Label("Convos List");
            if (s_shouldRefreshConvosList || m_convoList == null)
            {
                m_convoList = GatherExistingConversations();
            }
            foreach (Conversation t_convo in m_convoList)
            {
                if (t_convo == null) { continue; }
                string t_displayName = t_convo.name.Replace(CONVO_SUFFIX, "");
                if (GUILayout.Button(t_displayName))
                {
                    Debug.Log($"Open new window for editing {t_convo.name}");
                }
            }
        }



        private Conversation[] GatherExistingConversations()
        {
            string[] t_foundGuids = AssetDatabase.FindAssets($"t:{nameof(Conversation)}");
            Conversation[] t_existingConvos = new Conversation[t_foundGuids.Length];
            return t_existingConvos;
        }
        /// <summary>
        /// Returns false if the conversation already existed.
        /// </summary>
        private bool CreateNewConversation(string convoPrefix)
        {
            string t_folderPath = $"{CONVO_PARENT_FOLDER_PATH}/{convoPrefix}";
            if (!AssetDatabase.IsValidFolder(t_folderPath))
            {
                AssetDatabase.CreateFolder(CONVO_PARENT_FOLDER_PATH, convoPrefix);
            }

            string t_assetPath = $"{t_folderPath}/{convoPrefix}.asset";
            if (AssetDatabase.LoadAssetAtPath<Conversation>(t_assetPath) == null)
            {
                Conversation t_convo = CreateInstance<Conversation>();
                AssetDatabase.CreateAsset(t_convo, t_assetPath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}