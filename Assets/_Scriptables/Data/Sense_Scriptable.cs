using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Sense", menuName = "ScriptableObjects/Sense")]
public class Sense_Scriptable : ScriptableObject
{
    public Sense_Scriptable() { }

    public string Name, Description;
    public int ID, UniqueID = -1, Mod, Value, Time = -1, Condition = -1;

    public Sprite Sprite;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Sense_Scriptable))]
public class Sense_ScriptableEditor : Editor
{
    Sense_Scriptable comp;

    public void OnEnable()  =>  comp = (Sense_Scriptable)target;

    public override void OnInspectorGUI()
    {
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        GUILayout.Label("Description");
        comp.Description = EditorGUILayout.TextArea(comp.Description, GUILayout.Height(100f));

        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.UniqueID = EditorGUILayout.IntField("UniqueID", comp.UniqueID);
        comp.Mod = EditorGUILayout.IntField("Mod", comp.Mod);
        comp.Value = EditorGUILayout.IntField("Value", comp.Value);
        comp.Time = EditorGUILayout.IntField("Time", comp.Time);
        comp.Condition = EditorGUILayout.IntField("Condition", comp.Condition);

        EditorUtility.SetDirty(comp);
    }
}
#endif