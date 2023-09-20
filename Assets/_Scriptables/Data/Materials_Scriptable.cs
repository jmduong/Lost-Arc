using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Materials", menuName = "ScriptableObjects/Materials")]
public class Materials_Scriptable : ScriptableObject
{
    public Materials_Scriptable() { }

    // User Account Info
    public string Name;
    public Sprite Sprite;
    public int ID;
    public string Description;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Materials_Scriptable))]
public class Materials_ScriptableEditor : Editor
{
    Materials_Scriptable comp;

    public void OnEnable()  =>  comp = (Materials_Scriptable)target;

    public override void OnInspectorGUI()
    {
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));

        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        EditorGUILayout.LabelField("Description");
        comp.Description = EditorGUILayout.TextArea(comp.Description, GUILayout.Height(100));

        EditorUtility.SetDirty(comp);
    }
}
#endif