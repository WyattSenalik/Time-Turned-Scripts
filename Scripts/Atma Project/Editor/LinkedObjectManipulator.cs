using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LinkedObjectManipulator
{
    private const string ADD_LINKED_OBJ = "AddGameObjectToLinkedObjects";
    private const string REMOVE_LINKED_OBJ = "RemoveGameObjectFromLinkedObjects";


    public void AddToLinkedObjects(object t_firstObj, object t_secondObj)
    {
        t_firstObj.GetType().GetMethod(ADD_LINKED_OBJ).Invoke(t_firstObj, new object[] { t_secondObj });
        t_secondObj.GetType().GetMethod(ADD_LINKED_OBJ).Invoke(t_secondObj, new object[] { t_firstObj });
        EditorUtility.SetDirty(t_firstObj as Object);
        EditorUtility.SetDirty(t_secondObj as Object);
    }
    public void RemoveFromLinkedObjects(object t_firstObj, object t_secondObj)
    {
        t_firstObj.GetType().GetMethod(REMOVE_LINKED_OBJ).Invoke(t_firstObj, new object[] { t_secondObj });
        t_secondObj.GetType().GetMethod(REMOVE_LINKED_OBJ).Invoke(t_secondObj, new object[] { t_firstObj });
        EditorUtility.SetDirty(t_firstObj as Object);
        EditorUtility.SetDirty(t_secondObj as Object);
    }
}
