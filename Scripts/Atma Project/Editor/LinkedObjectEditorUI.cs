using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Atma.Editors
{
    public class LinkedObjectEditorUI
    {

        private ListView m_prsPlateListView = null;
        private VisualElement m_selectionPanel = null;
        private VisualElement m_buttons = null;
        private VisualElement m_gizmoToggle = null;
        private ListView m_doorListView = null;
        private ListView m_linkedObjectsListView = null;
        public ListView prsPlateListView => m_prsPlateListView;
        public VisualElement selectionPanel => m_selectionPanel;
        public VisualElement buttons => m_buttons;
        public VisualElement gizmoToggle => m_gizmoToggle;
        public ListView doorListView => m_doorListView;
        public ListView linkedObjectsListView => m_linkedObjectsListView;

        public void BuildEditor(VisualElement rootVisualElement)
        {
            // Create the editor tree.
            m_prsPlateListView = new ListView();

            m_selectionPanel = new VisualElement();

            m_buttons = new VisualElement();
            m_doorListView = new ListView();
            m_linkedObjectsListView = new ListView();
            m_gizmoToggle = new VisualElement();

            VisualElement t_rightView = new VisualElement();
            VisualElement t_leftView = new VisualElement();
            t_rightView.Add(new Label("Linkable Objects"));
            t_rightView.style.unityTextAlign = TextAnchor.MiddleCenter;
            t_rightView.Q<Label>().style.unityFontStyleAndWeight = FontStyle.Bold;
            t_rightView.Q<Label>().style.borderBottomColor = Color.black;
            t_rightView.Q<Label>().style.borderBottomWidth = 1;
            t_rightView.Add(m_doorListView);
            t_leftView.Add(new Label("Linked Objects"));
            t_leftView.style.unityTextAlign = TextAnchor.MiddleCenter;
            t_leftView.Q<Label>().style.unityFontStyleAndWeight = FontStyle.Bold;
            t_leftView.Q<Label>().style.borderBottomColor = Color.black;
            t_leftView.Q<Label>().style.borderBottomWidth = 1;
            t_leftView.Add(m_linkedObjectsListView);

            m_selectionPanel.style.flexDirection = FlexDirection.Row;
            m_selectionPanel.style.borderTopColor = Color.black;
            m_selectionPanel.style.borderTopWidth = 1;
            m_doorListView.selectionType = SelectionType.Multiple;
            m_linkedObjectsListView.selectionType = SelectionType.Multiple;
            m_buttons.style.borderRightWidth = 1;
            m_buttons.style.borderLeftWidth = 1;
            m_buttons.style.borderRightColor = Color.black;
            m_buttons.style.borderLeftColor = Color.black;

            m_selectionPanel.Add(t_rightView);
            m_selectionPanel.Add(m_buttons);
            m_selectionPanel.Add(t_leftView);

            rootVisualElement.Add(m_gizmoToggle);
            rootVisualElement.Add(m_prsPlateListView);
            rootVisualElement.Add(m_selectionPanel);
        }

        /// <summary>
        /// Populates a <see cref="ListView"/> using the elements in a pre-existing <see cref="List{T}"/>.
        /// </summary>
        /// <param name="listView">ListView element</param>
        /// <param name="list">List to fill ListView</param>
        /// <param name="actions">Actions performed on selection change</param>
        public void PopulateListView<T>(ListView listView, ref List<T> list, params System.Action<IEnumerable<object>>[] actions )
        {
            listView.ClearSelection();
            listView.Clear();
            List<T> t_newList = list;
            listView.makeItem = () => new Label();
            listView.bindItem = (item, index) =>
            {
                try
                {
                    (item as Label).text =
                    (t_newList[index] as Object).name;
                    item.name = index.ToString();
                    item.style.unityTextAlign = TextAnchor.MiddleCenter;
                } catch { }
            };
            listView.itemsSource = list;
            foreach (System.Action<IEnumerable<object>> action in actions)
            {
                listView.onSelectionChange += action;
            }
        }

        public void ClearListView(ListView listView)
        {
            foreach (VisualElement e in listView.Children())
            {
                listView.Remove(e);
            }
        }
    }
}
