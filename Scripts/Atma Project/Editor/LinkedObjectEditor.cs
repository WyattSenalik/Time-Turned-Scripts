using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine.Events;

namespace Atma.Editors
{
    /// <summary>
    /// Editor Window for manipulating pressure plates' linked objects.
    /// </summary>
    public class LinkedObjectEditor : EditorWindow
    {
        private const float BUTTON_COL_WIDTH = 50;
        private const string GIZMO_TOGGLED_KEY = "gizmosToggled";

        private static EditorWindow m_window = null;

        private LinkedObjectEditorUI m_editorGUI = null;
        private LinkedObjectManipulator m_lOManip = null;
        private int m_activatorSelectionIndex = -1;
        private List<int> m_toggleableSelectionIndices = new List<int>();
        private List<IActivator> m_activators = new List<IActivator>();
        private List<IToggleable> m_toggleables = new List<IToggleable>();
        private ListView m_activatorListView = null;
        private ListView m_toggleableListView = null;
        private ListView m_linkedObjectsListView = null;
        private Button m_addButton = null;
        private Button m_removeButton = null;
        private Button m_removeAllButton = null;
        private VisualElement m_buttons = null;
        private VisualElement m_selectionPanel = null;

        public static UnityEvent UpdateGizmos = new UnityEvent();

        public static void OpenEditor()
        {
            // Method is called after user selects the menu item in the Editor.
            m_window = GetWindow<LinkedObjectEditor>();
            m_window.titleContent = new GUIContent("Linked Objects Editor");
            // Limit window size.
            m_window.minSize = new Vector2(400, 300);
            m_window.maxSize = new Vector2(1080, 900);
        }

        public void CreateGUI()
        {
            UpdateObjectLists();
            m_lOManip = new LinkedObjectManipulator();

            // Build GUI
            m_editorGUI = new LinkedObjectEditorUI();
            m_editorGUI.BuildEditor(rootVisualElement);
            m_activatorListView = m_editorGUI.prsPlateListView;
            m_selectionPanel = m_editorGUI.selectionPanel;
            m_buttons = m_editorGUI.buttons;
            m_toggleableListView = m_editorGUI.doorListView;
            m_linkedObjectsListView = m_editorGUI.linkedObjectsListView;

            Button t_toggle = new Button(() =>
            {
                bool t_gizmoPref = EditorPrefs.GetBool(GIZMO_TOGGLED_KEY, true);
                t_gizmoPref = !t_gizmoPref;
                EditorPrefs.SetBool(GIZMO_TOGGLED_KEY, t_gizmoPref);
                UpdateGizmos.Invoke();
            });
            t_toggle.Add(new Label("Toggle Gizmos"));
            m_editorGUI.gizmoToggle.Add(t_toggle);

            m_editorGUI.PopulateListView(
                m_activatorListView,
                ref m_activators,
                OnPrsPlateSelectionChange,
                (items) => { m_activatorSelectionIndex = m_activatorListView.selectedIndex; }
                );

            m_editorGUI.PopulateListView(
                m_toggleableListView,
                ref m_toggleables,
                OnDoorSelectionChange,
                (items) => { m_toggleableSelectionIndices = m_toggleableListView.selectedIndices.ToList(); }
                );
            // Sets the selected pressure plate to be the same as the one
            // used to open the editor with.
            if (Selection.activeGameObject.TryGetComponent(out IActivator t_switch))
            {
                m_activatorSelectionIndex = m_activators.IndexOf(t_switch);
                m_activatorListView.SetSelection(m_activatorSelectionIndex);
            }

            // Making and adding buttons.
            #region AddButton
            m_addButton = new Button();
            m_addButton.Add(new Label(">>"));
            m_addButton.clicked += AddDoorsToPrsPlate;
            m_addButton.clicked += RefreshLists;
            #endregion

            #region RemoveButton
            m_removeButton = new Button();
            m_removeButton.Add(new Label("<<"));
            m_removeButton.clicked += RemoveDoorsFromPrsPlate;
            m_removeButton.clicked += RefreshLists;
            m_removeButton.clicked += m_linkedObjectsListView.ClearSelection;
            #endregion

            #region RemoveAllButton
            m_removeAllButton = new Button();
            m_removeAllButton.Add(new Label("Clear"));
            m_removeAllButton.clicked += RemoveAllDoorsFromPrsPlate;
            m_removeAllButton.clicked += RefreshLists;
            m_removeButton.clicked += m_linkedObjectsListView.ClearSelection;
            #endregion

            m_buttons.Add(m_addButton);
            m_buttons.Add(m_removeButton);
            m_buttons.Add(m_removeAllButton);
        }

        private void Update()
        {
            // Closes the window if m_window is set to null (Usually happens when reloading the editor).
            if (m_window == null)
            {
                try { Close(); } catch { /*Do nothing*/ }
                return;
            }
            // Adjusts the elements width to fit current window size.
            #region ResizeElements
            float t_heightPercent = (m_window.position.height - m_window.minSize.y) / (m_window.maxSize.y - m_window.minSize.y);
            m_activatorListView.style.width = m_window.position.width;
            m_activatorListView.style.height = 70 * (1 + t_heightPercent);
            m_selectionPanel.style.height = m_window.position.height - m_activatorListView.style.height.value.value;
            m_toggleableListView.style.width = (m_window.position.width - BUTTON_COL_WIDTH) / 2;
            m_linkedObjectsListView.style.width = (m_window.position.width - BUTTON_COL_WIDTH) / 2;
            m_buttons.style.width = BUTTON_COL_WIDTH;
            #endregion

            m_addButton.SetEnabled(false);
            m_removeButton.SetEnabled(false);
            m_removeAllButton.SetEnabled(false);

            // Clears selections
            #region ClearSelections
            if (Selection.activeObject == null)
            {
                m_activatorSelectionIndex = -1;
                m_activatorListView.ClearSelection();


                m_toggleableSelectionIndices.Clear();
                m_toggleableListView.ClearSelection();

                m_editorGUI.ClearListView(m_linkedObjectsListView);
                m_linkedObjectsListView.ClearSelection();

                LinkedObjectGizmoDrawer.DrawLineToGameObjects(null);
                return;
            }

            if (m_toggleableListView.selectedIndices.Count() == 0 && m_linkedObjectsListView.selectedIndices.Count() == 0)
            {
                LinkedObjectGizmoDrawer.DrawLineToGameObjects(null);
            }
            #endregion

            // Determines whether the link/unlink buttons are availible.
            #region ButtonStates
            if (m_activatorSelectionIndex < 0) { return; }
            if (m_toggleableSelectionIndices == null) { return; }

            object t_activator = m_activators[m_activatorSelectionIndex];
            IReadOnlyList<GameObject> t_linkedObjs = t_activator.GetType().GetProperty("linkedObjects").GetValue(t_activator) as IReadOnlyList<GameObject>;

            bool t_objIsLinked = false;
            foreach (int index in m_toggleableSelectionIndices)
            {
                if (t_linkedObjs.Contains((m_toggleables[index] as MonoBehaviour).gameObject))
                {
                    t_objIsLinked = true;
                    break;
                }
                else
                {
                    t_objIsLinked = false;
                }
            }
            bool t_objIsAddable = false;
            foreach (int index in m_toggleableSelectionIndices)
            {
                if (!t_linkedObjs.Contains((m_toggleables[index] as MonoBehaviour).gameObject))
                {
                    t_objIsAddable = true;
                    break;
                }
                else
                {
                    t_objIsAddable = false;
                }
            }

            if (t_objIsLinked) { m_removeButton.SetEnabled(true); }
            if (t_objIsAddable) { m_addButton.SetEnabled(true); }
            if (t_linkedObjs.Count > 0) { m_removeAllButton.SetEnabled(true); } 
            #endregion
        }

        private void OnSelectionChange()
        {
            // Changes active selection in the Scene GUI to be the
            // selected item in the list.
            if (Selection.gameObjects == null) { return; }
            if (Selection.activeGameObject == null) { ClearSelections(); return; }

            m_toggleableSelectionIndices.Clear();

            foreach (GameObject obj in Selection.gameObjects)
            {
                if (obj.TryGetComponent(out IActivator t_activator))
                {
                    if (!m_activators.Contains(t_activator)) { return; }

                    m_activatorSelectionIndex = m_activators.IndexOf(t_activator);
                    m_activatorListView.SetSelection(m_activatorSelectionIndex);
                }
                else if (obj.TryGetComponent(out IToggleable t_toggleable))
                {
                    if (!m_toggleables.Contains(t_toggleable)) { return; }

                    m_toggleableSelectionIndices.Add(m_toggleables.IndexOf(t_toggleable));

                    m_toggleableListView.SetSelection(m_toggleableSelectionIndices);
                }
            }
            RefreshLists();
        }

        private void OnHierarchyChange()
        {
            RefreshLists();
        }

        private void OnDestroy()
        {
            LinkedObjectGizmoDrawer.DrawLineToGameObjects(null);
            m_window = null;
        }

        private void RefreshLists()
        {
            // Refresh all the items in the lists.
            UpdateObjectLists();
            m_editorGUI.ClearListView(m_activatorListView);
            m_editorGUI.ClearListView(m_toggleableListView);
            m_editorGUI.ClearListView(m_linkedObjectsListView);
            UpdateLinkedObjectListView();
        }

        private void UpdateObjectLists()
        {
            Object[] t_monobehaviors = FindObjectsOfType(typeof(MonoBehaviour));
            List<IActivator> t_activatorsList = new List<IActivator>();
            List<IToggleable> t_toggleablesList = new List<IToggleable>();
            foreach (Object t_obj in t_monobehaviors)
            {
                if ((t_obj as MonoBehaviour).TryGetComponent(out IActivator t_activator))
                {
                    if (t_activatorsList.Contains(t_activator)) { continue; }
                    t_activatorsList.Add(t_activator);
                }
                if ((t_obj as MonoBehaviour).TryGetComponent(out IToggleable t_toggleable))
                {
                    if (t_toggleablesList.Contains(t_toggleable)) { continue; }
                    t_toggleablesList.Add(t_toggleable);
                }
            }
            m_activators = t_activatorsList;
            m_toggleables = t_toggleablesList;
        }

        private void UpdateLinkedObjectListView()
        {
            if (m_activatorSelectionIndex < 0) { return; }
            object t_activator = m_activators[m_activatorSelectionIndex];
            List<GameObject> t_linkedObjs = (t_activator.GetType().GetProperty("linkedObjects").GetValue(t_activator) as IReadOnlyList<GameObject>).ToList();
            m_editorGUI.PopulateListView(
                m_linkedObjectsListView,
                ref t_linkedObjs,
                OnLinkedObjectSelectionChange,
                (items) => {
                    List<int> t_selectedIndicies = new List<int>();
                    foreach (object t_obj in items)
                    {
                        GameObject t_go = t_obj as GameObject;
                        t_selectedIndicies.Add(m_toggleables.IndexOf(t_go.GetComponent<IToggleable>()));
                    }
                    m_toggleableSelectionIndices = t_selectedIndicies;
                });
        }

        private void OnPrsPlateSelectionChange(IEnumerable<object> selectedItems)
        {
            ClearSelections();
            if (m_activatorListView.selectedItem == null)
            {
                m_activatorListView.ClearSelection();
                return;
            }

            Object t_selectedItem = selectedItems.First() as
                Object;
            if (t_selectedItem == null) { return; }

            // Adds the selected pressure plate to the list of selected objects
            Selection.activeObject = t_selectedItem;

            UpdateLinkedObjectListView();
        }

        private void OnDoorSelectionChange(IEnumerable<object> selectedItems)
        {
            m_linkedObjectsListView.ClearSelection();
            if (m_toggleableListView.selectedItem == null)
            {
                m_toggleableListView.ClearSelection();
                return;
            }

            // Cast objects to GameObjects
            List<GameObject> t_selectedObjs = new List<GameObject>();
            foreach (object t_obj in selectedItems)
            {
                // Cast object to IToggleable because we can't cast directly to GameObject
                IToggleable t_toggleable = t_obj as IToggleable;
                t_selectedObjs.Add((t_toggleable as MonoBehaviour).gameObject);
            }

            // Draw lines from pressure plate to selected gameObjects
            LinkedObjectGizmoDrawer.DrawLineToGameObjects(t_selectedObjs);
            SceneView.RepaintAll();
        }
        
        private void OnLinkedObjectSelectionChange(IEnumerable<object> selectedItems)
        {
            m_toggleableListView.ClearSelection();
            if (m_linkedObjectsListView.selectedItem == null)
            {
                m_linkedObjectsListView.ClearSelection();
                return;
            }

            // Cast objects to GameObjects
            List<GameObject> t_selectedObjs = new List<GameObject>();
            foreach (object t_obj in selectedItems)
            {
                // Cast object to GameObject
                GameObject t_go = t_obj as GameObject;
                t_selectedObjs.Add(t_go);
            }

            // Draw lines from pressure plate to selected gameObjects
            LinkedObjectGizmoDrawer.DrawLineToGameObjects(t_selectedObjs);
            SceneView.RepaintAll();
        }

        private void AddDoorsToPrsPlate()
        {
            object t_activatorObj = m_activators[m_activatorSelectionIndex];
            object t_lOObject = t_activatorObj.GetType().GetProperty("linkedObjects").GetValue(t_activatorObj);
            List<GameObject> t_lOList = (t_lOObject as IReadOnlyList<GameObject>).ToList();

            List<IToggleable> t_linkedToggleables = new List<IToggleable>();
            foreach (GameObject t_obj in t_lOList)
            {
                if (t_obj.TryGetComponent(out IToggleable t_toggleable))
                {
                    t_linkedToggleables.Add(t_toggleable);
                }
            }
            foreach (int index in m_toggleableSelectionIndices)
            {
                if (t_linkedToggleables.Contains(m_toggleables[index])) { continue; }
                m_lOManip.AddToLinkedObjects(t_activatorObj, m_toggleables[index]);
            }
        } 

        private void RemoveDoorsFromPrsPlate()
        {
            object t_activatorObj = m_activators[m_activatorSelectionIndex];
            object t_lOObject = t_activatorObj.GetType().GetProperty("linkedObjects").GetValue(t_activatorObj);
            List<GameObject> t_lOList = (t_lOObject as IReadOnlyList<GameObject>).ToList();

            List<IToggleable> t_linkedToggleables = new List<IToggleable>();
            foreach (GameObject t_obj in t_lOList)
            {
                if (t_obj.TryGetComponent(out IToggleable t_toggleable))
                {
                    t_linkedToggleables.Add(t_toggleable);
                }
            }

            foreach (int index in m_toggleableSelectionIndices)
            {
                if (!t_linkedToggleables.Contains(m_toggleables[index])) { continue; }
                m_lOManip.RemoveFromLinkedObjects(t_activatorObj, m_toggleables[index]);
            }
        }

        private void RemoveAllDoorsFromPrsPlate()
        {
            object t_activatorObj = m_activators[m_activatorSelectionIndex];
            object t_lOObject = t_activatorObj.GetType().GetProperty("linkedObjects").GetValue(t_activatorObj);
            List<GameObject> t_lOList = (t_lOObject as IReadOnlyList<GameObject>).ToList();

            foreach (GameObject obj in t_lOList)
            {
                if (obj.TryGetComponent(out IToggleable t_toggleable))
                {
                    m_lOManip.RemoveFromLinkedObjects(t_activatorObj, t_toggleable);
                }
            }
        }

        private void ClearSelections()
        {
            m_toggleableListView.ClearSelection();
            m_linkedObjectsListView.ClearSelection();
            LinkedObjectGizmoDrawer.DrawLineToGameObjects(null);
        }
    }
}