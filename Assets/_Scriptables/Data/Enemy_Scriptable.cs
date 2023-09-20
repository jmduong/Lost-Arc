using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy")]
public class Enemy_Scriptable : Unit
{
    public Enemy_Scriptable() { }

    public int ID, Burst;
    public List<int> AdditionalHealthBars = new List<int>(), Skills = new List<int>(), Abilities = new List<int>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Enemy_Scriptable))]
public class Enemy_ScriptableEditor : Editor
{
    Enemy_Scriptable comp;
    string[] affinities = new string[10] { "PHYS", "MAG", "SUPP", "FIRE", "WATER", "EARTH", "LGTNG", "WIND", "LGHT", "DARK" };

    public void OnEnable() =>   comp = (Enemy_Scriptable)target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Model = (GameObject)EditorGUILayout.ObjectField(comp.Model, typeof(GameObject), false, GUILayout.Width(75f), GUILayout.Height(75f));
        EditorGUILayout.EndHorizontal();

        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.Title = EditorGUILayout.TextField("Title", comp.Title);
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.Burst = EditorGUILayout.IntField("Burst", comp.Burst);

        GUILayout.Label("Stats", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 9; i++)
            GUILayout.Label(Stats.StatTxt[i] + "\t", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 9; i++)
            comp.Stats[i] = EditorGUILayout.IntField(comp.Stats[i]);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Talents (Top) & Resistances (Btm)", EditorStyles.boldLabel);  // Range is -10 to 10
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 10; i++)
            GUILayout.Label(affinities[i] + "\t");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 10; i++)
            comp.Talents[i] = EditorGUILayout.IntField(Mathf.Clamp(comp.Talents[i], -10, 10));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 10; i++)
            comp.Resistances[i] = EditorGUILayout.IntField(Mathf.Clamp(comp.Resistances[i], -10, 10));
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Additional Health Bars", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
            comp.AdditionalHealthBars.Add(0);
        for (int i = 0; i < comp.AdditionalHealthBars.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.AdditionalHealthBars[i] = EditorGUILayout.IntField(comp.AdditionalHealthBars[i]);
            if (GUILayout.Button("-"))
                comp.AdditionalHealthBars.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Abilities", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
            comp.Abilities.Add(0);
        for (int i = 0; i < comp.Abilities.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Abilities[i] = EditorGUILayout.IntField(comp.Abilities[i]);
            if (GUILayout.Button("-"))
                comp.Abilities.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Skills", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
            comp.Skills.Add(0);
        for (int i = 0; i < comp.Skills.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Skills[i] = EditorGUILayout.IntField(comp.Skills[i]);
            if (GUILayout.Button("-"))
                comp.Skills.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Label("Attributes", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
            comp.Attributes.Add(Stats.Attributes.Human);
        for (int i = 0; i < comp.Attributes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Attributes[i] = (Stats.Attributes)EditorGUILayout.EnumPopup(comp.Attributes[i]);
            if (GUILayout.Button("-"))
                comp.Attributes.RemoveAt(i);
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(comp);
    }
}
#endif