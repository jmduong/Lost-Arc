using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "SoulFragment", menuName = "ScriptableObjects/SoulFragment")]
public class SoulFragment_Scriptable : ScriptableObject
{
    public SoulFragment_Scriptable() { }

    public string Name;
    public Sprite Sprite;
    public int ID, Color;
    public List<Stats.StatTypes> Stats = new List<Stats.StatTypes>();
    public List<int> StatValues = new List<int>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(SoulFragment_Scriptable))]
public class SoulFragment_ScriptableEditor : Editor
{
    SoulFragment_Scriptable comp;

    public void OnEnable()
    {
        comp = (SoulFragment_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));

        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.Color = EditorGUILayout.IntField("Color", comp.Color);

        GUILayout.Label("Stats", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
        {
            comp.Stats.Add((0));
            comp.StatValues.Add((0));
        }
        for (int i = 0; i < comp.Stats.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Stats[i] = (Stats.StatTypes)EditorGUILayout.EnumPopup(comp.Stats[i]);
            comp.StatValues[i] = EditorGUILayout.IntField(comp.StatValues[i]);
            if (GUILayout.Button("-"))
            {
                comp.Stats.RemoveAt(i);
                comp.StatValues.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtility.SetDirty(comp);
    }
}
#endif