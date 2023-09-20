using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability_Scriptable : ScriptableObject
{
    public Ability_Scriptable() { }

    public string Name;
    public int ID, RP = 30;

    public List<Stats.StatTypes> StatTypes = new List<Stats.StatTypes>();
    public List<Stats.Target> Targets = new List<Stats.Target>();
    public List<int> Stats = new List<int>(), mStats = new List<int>();
    public List<Stats.Attributes> Attributes = new List<Stats.Attributes>();

    public int[] RefineReqs = new int[48]
{
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
};
    public Sprite Sprite;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Ability_Scriptable))]
public class Ability_ScriptableEditor : Editor
{
    Ability_Scriptable comp;

    public void OnEnable()
    {
        comp = (Ability_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.RP = EditorGUILayout.IntField("RP", comp.RP);

        GUILayout.Label("Refine Requirements", EditorStyles.boldLabel);
        for (int i = 0; i < 8; i++)
        {
            GUILayout.Label("Refine to Lvl. " + (i + 2), EditorStyles.boldLabel);
            for (int j = 0; j < 3; j++)
            {
                EditorGUILayout.BeginHorizontal();
                comp.RefineReqs[(i * 6) + (j * 2)] = EditorGUILayout.IntField("Item " + (j + 1), comp.RefineReqs[(i * 6) + (j * 2)]);
                if (comp.RefineReqs[(i * 6) + (j * 2)] != -1)
                    comp.RefineReqs[(i * 6) + (j * 2) + 1] = EditorGUILayout.IntField(comp.RefineReqs[(i * 6) + (j * 2) + 1]);
                EditorGUILayout.EndHorizontal();
            }
        }

        GUILayout.Label("Stats", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
        {
            comp.StatTypes.Add(Stats.StatTypes.ATK);
            comp.Targets.Add(Stats.Target.User);
            comp.Stats.Add(0);
            comp.mStats.Add(0);
            comp.Attributes.Add(Stats.Attributes.None);
        }
        for (int i = 0; i < comp.StatTypes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.StatTypes[i] = (Stats.StatTypes)EditorGUILayout.EnumPopup(comp.StatTypes[i]);
            comp.Targets[i] = (Stats.Target)EditorGUILayout.EnumPopup(comp.Targets[i]);
            comp.Stats[i] = EditorGUILayout.IntField(comp.Stats[i]);
            comp.mStats[i] = EditorGUILayout.IntField(comp.mStats[i]);
            if(comp.Targets[i] != Stats.Target.User)comp.Attributes[i] = (Stats.Attributes)EditorGUILayout.EnumPopup(comp.Attributes[i]);
            if (GUILayout.Button("-"))
            {
                comp.StatTypes.RemoveAt(i);
                comp.Targets.RemoveAt(i);
                comp.Stats.RemoveAt(i);
                comp.mStats.RemoveAt(i);
                comp.Attributes.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(comp);
    }
}
#endif