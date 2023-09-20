using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Portrait", menuName = "ScriptableObjects/Portrait")]
public class Portrait_Scriptable : ScriptableObject
{
    public Portrait_Scriptable() { }
    public string Name;
    public Dictionary<string, Sprite> Expressions = new Dictionary<string, Sprite>() 
        { { "Default", null }, { "Happy", null }, { "Sad", null }, { "Surprise", null }, { "Anger", null }, { "Battle", null }, { "Injured", null } };
}

#if UNITY_EDITOR
[CustomEditor(typeof(Portrait_Scriptable))]
public class Portrait_ScriptableEditor : Editor
{
    Portrait_Scriptable comp;

    public void OnEnable()
    {
        comp = (Portrait_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.Expressions["Default"] = (Sprite)EditorGUILayout.ObjectField("Default", comp.Expressions["Default"], typeof(Sprite), false);
        comp.Expressions["Happy"] = (Sprite)EditorGUILayout.ObjectField("Happy", comp.Expressions["Happy"], typeof(Sprite), false);
        comp.Expressions["Sad"] = (Sprite)EditorGUILayout.ObjectField("Sad", comp.Expressions["Sad"], typeof(Sprite), false);
        comp.Expressions["Surprise"] = (Sprite)EditorGUILayout.ObjectField("Surprise", comp.Expressions["Surprise"], typeof(Sprite), false);
        comp.Expressions["Anger"] = (Sprite)EditorGUILayout.ObjectField("Anger", comp.Expressions["Anger"], typeof(Sprite), false);
        comp.Expressions["Battle"] = (Sprite)EditorGUILayout.ObjectField("Battle", comp.Expressions["Battle"], typeof(Sprite), false);
        comp.Expressions["Injured"] = (Sprite)EditorGUILayout.ObjectField("Injured", comp.Expressions["Injured"], typeof(Sprite), false);

        EditorUtility.SetDirty(comp);
    }
}
#endif