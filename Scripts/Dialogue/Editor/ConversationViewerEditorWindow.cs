using Dialogue.ConvoActions.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dialogue.Editor
{
    public sealed class ConversationViewerEditorWindow : EditorWindow
    {
        private static ConversationViewerEditorWindow m_window = null;


        private TwoPaneSplitView m_paneView = null;
        private TwoPaneSplitView m_convoActionPaneView = null;

        // Left Pane Elements
        private VisualElement m_leftPane = null;
        private Label m_leftPaneTitle = null;
        private ListView m_listView = null;

        // Right Pane Elements
        private VisualElement m_rightPane = null;
        private Label m_rightPaneTitle = null;
        private VisualElement m_rightPaneDefaultInspector = null;
        private VisualElement m_rightPaneConvoActionsContent = null;
        private VisualElement m_rightPaneConvoActionsInspector = null;
        private VisualElement m_rightPaneSubTextContent = null;
        private VisualElement m_rightPaneSubTextInspector = null;


        [MenuItem("Tools/ConversationViewer")]
        public static void OpenEditor()
        {
            // Method is called after user selects the menu item in the Editor.
            m_window = GetWindow<ConversationViewerEditorWindow>();
            m_window.titleContent = new GUIContent("Conversation Viewer");
            // Limit window size.
            m_window.minSize = new Vector2(500, 300);
            m_window.maxSize = new Vector2(1080, 900);
        }

        private void CreateGUI()
        {
            m_paneView = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
            m_convoActionPaneView = new TwoPaneSplitView(0, 140, TwoPaneSplitViewOrientation.Horizontal);
            m_leftPane = new VisualElement();
            m_leftPaneTitle = new Label("Conversations");
            m_listView = new ListView();
            m_listView.reorderable = true;
            m_rightPane = new VisualElement();
            m_rightPaneTitle = new Label("Inspector");
            m_rightPaneDefaultInspector = new VisualElement();
            m_rightPaneConvoActionsContent = new VisualElement();
            m_rightPaneConvoActionsContent.style.flexGrow = 1;
            m_rightPaneConvoActionsContent.style.borderBottomColor = Color.black;
            m_rightPaneConvoActionsContent.style.borderBottomWidth = 1;
            m_rightPaneConvoActionsInspector = new VisualElement();
            m_rightPaneSubTextContent = new VisualElement();
            m_rightPaneSubTextContent.style.flexGrow = 1;
            m_rightPaneSubTextContent.style.borderBottomColor = Color.black;
            m_rightPaneSubTextContent.style.borderBottomWidth = 1;
            m_rightPaneSubTextInspector = new VisualElement();
            //m_rightPaneConvoActionsInspector.style.backgroundColor = Color.red;

            // Left Pane
            m_leftPaneTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            m_leftPaneTitle.style.borderBottomColor = Color.gray;
            m_leftPaneTitle.style.borderBottomWidth = 1;
            m_leftPane.Add(m_leftPaneTitle);
            string[] t_assetGuids = AssetDatabase.FindAssets("_CONVO");
            List<Object> t_listObjects = new List<Object>();
            for (int i = 0; i < t_assetGuids.Length; ++i)
            {
                string t_path = AssetDatabase.GUIDToAssetPath(t_assetGuids[i]);
                Object t_object = AssetDatabase.LoadAssetAtPath<Object>(t_path);
                
                if (t_object != null && !AssetDatabase.IsValidFolder(t_path))
                {
                    t_listObjects.Add(t_object);
                }
            }
            if (t_listObjects.Count > 0)
            {
                m_listView.makeItem = () => new Label();
                m_listView.bindItem = (item, index) =>
                {
                    (item as Label).text = t_listObjects[index].name;
                };
                m_listView.itemsSource = t_listObjects;
            }
            m_leftPane.Add(m_listView);

            // Right Pane
            m_rightPaneTitle.style.unityFontStyleAndWeight = FontStyle.Bold;
            m_rightPaneTitle.style.borderBottomColor = Color.gray;
            m_rightPaneTitle.style.borderBottomWidth = 1;
            m_rightPane.Add(m_rightPaneTitle);
            m_rightPane.Add(m_rightPaneDefaultInspector);
            m_rightPane.Add(m_rightPaneConvoActionsContent);
            m_rightPaneSubTextInspector.Add(m_rightPaneSubTextContent);


            m_listView.onSelectionChange += OnListItemSelectionChange;


            m_paneView.Add(m_leftPane);
            m_paneView.Add(m_rightPane);
            rootVisualElement.Add(m_paneView);
        }

        private void OnListItemSelectionChange(IEnumerable<object> selectedItems)
        {
            m_convoActionPaneView.Clear();
            m_rightPaneDefaultInspector.Clear();
            //m_rightPaneConvoActionsContent.Clear();
            m_rightPaneConvoActionsInspector.Clear();

            IEnumerator<object> enumerator = selectedItems.GetEnumerator();
            if (enumerator.MoveNext())
            {
                Object t_convo = enumerator.Current as Object;
                if (t_convo == null) { return; }

                DrawConvoInspector(t_convo);
            }
        }

        private void DrawConvoInspector(Object t_obj)
        {
            Foldout t_foldout = new Foldout();
            t_foldout.text = "Conversation Inspector";
            t_foldout.value = false;
            t_foldout.style.borderBottomColor = Color.gray;
            t_foldout.style.borderBottomWidth = 1;
            InspectorElement t_inspector = new InspectorElement(t_obj);
            t_foldout.Add(t_inspector);
            VisualElement t_leftPane = new VisualElement();
            ListView t_arrayElementBar = new ListView();
            if (t_obj is Conversation)
            {
                Conversation t_convo = t_obj as Conversation;
                ConvoActionObject[] t_convoActions = t_convo.conversationActions.ToArray();

                if (t_convoActions.Length > 0)
                {
                    t_arrayElementBar.makeItem = () => new Label();
                    t_arrayElementBar.bindItem = (item, index) =>
                    {
                        (item as Label).text = t_convoActions[index].name;
                    };
                    t_arrayElementBar.itemsSource = t_convoActions;
                }
                t_arrayElementBar.onSelectionChange += OnConvoActionListSelectionChange;
            }
            t_leftPane.Add(t_arrayElementBar);
            m_convoActionPaneView.Add(t_leftPane);
            m_convoActionPaneView.Add(m_rightPaneConvoActionsInspector);
            m_rightPaneDefaultInspector.Add(t_foldout);
            m_rightPaneConvoActionsContent.Add(m_convoActionPaneView);
        }
        

        private void OnConvoActionListSelectionChange(IEnumerable<object> selectedItems)
        {
            m_rightPaneConvoActionsInspector.Clear();
            m_rightPaneSubTextInspector.Clear();
            m_rightPaneSubTextContent.Clear();

            IEnumerator<object> enumerator = selectedItems.GetEnumerator();
            if (enumerator.MoveNext())
            {
                Object t_obj = enumerator.Current as Object;
                if (t_obj == null) { return; }

                DrawConvoActionInspector(t_obj);
            }

        }

        private void DrawConvoActionInspector(Object t_obj)
        {
            Foldout t_foldout = new Foldout();
            t_foldout.text = "ConvoAction Inspector";
            t_foldout.value = true;
            t_foldout.style.borderBottomColor = Color.gray;
            t_foldout.style.borderBottomWidth = 1;
            InspectorElement t_inspector = new InspectorElement(t_obj);
            t_foldout.Add(t_inspector);
            TwoPaneSplitView t_rightPaneTwoPane = new TwoPaneSplitView(0, 100, TwoPaneSplitViewOrientation.Horizontal);
            VisualElement t_leftPane = new VisualElement();
            ListView t_arrayElementBar = new ListView();
            if (t_obj is DialogueText)
            {
                DialogueText t_dialogueText = t_obj as DialogueText;
                DialogueSubText[] t_subTexts = t_dialogueText.subTexts.ToArray();

                if (t_subTexts.Length > 0)
                {
                    t_arrayElementBar.makeItem = () => new Label();
                    t_arrayElementBar.bindItem = (item, index) =>
                    {
                        (item as Label).text = t_subTexts[index].name;
                    };
                    t_arrayElementBar.itemsSource = t_subTexts;
                }
                t_arrayElementBar.onSelectionChange += OnSubTextSelectionChange;
            }
            t_leftPane.Add(t_arrayElementBar);
            t_rightPaneTwoPane.Add(t_leftPane);
            t_rightPaneTwoPane.Add(m_rightPaneSubTextInspector);
            m_rightPaneConvoActionsInspector.Add(t_rightPaneTwoPane);
            m_rightPaneSubTextInspector.Add(t_foldout);

        }

        private void OnSubTextSelectionChange(IEnumerable<object> selectedItems)
        {
            m_rightPaneSubTextInspector.Clear();
            m_rightPaneSubTextContent.Clear();

            IEnumerator<object> enumerator = selectedItems.GetEnumerator();
            if (enumerator.MoveNext())
            {
                Object t_obj = enumerator.Current as Object;
                if (t_obj == null) { return; }

                DrawSubTextInspector(t_obj);
            }
        }

        private void DrawSubTextInspector(Object t_obj)
        {
            Foldout t_foldout = new Foldout();
            t_foldout.text = "Sub Text Inspector";
            t_foldout.value = true;
            t_foldout.style.borderBottomColor = Color.gray;
            t_foldout.style.borderBottomWidth = 1;
            InspectorElement t_inspector = new InspectorElement(t_obj);
            t_foldout.Add(t_inspector);
            m_rightPaneSubTextInspector.Add(t_foldout);
        }

        private void OnProjectChange()
        {
            GetWindow<ConversationViewerEditorWindow>().Close();
        }

        private void Update()
        {

            // Closes the window if m_window is set to null (Usually happens when reloading the editor).
            if (m_window == null)
            {
                try { Close(); } catch { /*Do nothing*/ }
                return;
            }
        }
    } 
}
