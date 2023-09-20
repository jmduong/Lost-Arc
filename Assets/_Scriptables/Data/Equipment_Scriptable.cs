using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipment")]
public class Equipment_Scriptable : ScriptableObject
{
    public Equipment_Scriptable() { }

    public string Name;
    public int ID, Rarity = 0;
    public List<Stats.StatTypes> StatTypes = new List<Stats.StatTypes>();
    public int[] Stats = new int[9];
    public int[] SalvageMats = new int[6]
    {
        -1, 0, -1, 0, -1, 0
    };
}

#if UNITY_EDITOR
[CustomEditor(typeof(Equipment_Scriptable))]
public class Equipment_ScriptableEditor : Editor
{
    Equipment_Scriptable comp;

    public void OnEnable()  =>  comp = (Equipment_Scriptable)target;

    public override void OnInspectorGUI()
    {
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.Rarity = EditorGUILayout.IntField("Rarity", comp.Rarity);

        GUILayout.Label("Stats", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 9; i++)
            GUILayout.Label(Stats.StatTxt[i] + "\t", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 9; i++)
            comp.Stats[i] = EditorGUILayout.IntField(comp.Stats[i]);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Salvage Mats", EditorStyles.boldLabel);
        for(int i = 0; i < 3; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.SalvageMats[i * 2] = EditorGUILayout.IntField("Item " + (i + 1), comp.SalvageMats[i * 2]);
            if (comp.SalvageMats[i * 2] >= 0)
                comp.SalvageMats[(i * 2) + 1] = EditorGUILayout.IntField(comp.SalvageMats[(i * 2) + 1]);
            else
                comp.SalvageMats[(i * 2) + 1] = 0;
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(comp);
    }
}
#endif