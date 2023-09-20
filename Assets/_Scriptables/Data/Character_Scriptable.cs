using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character")]
public class Character_Scriptable : Unit
{
    public Character_Scriptable() { }

    public int ID, UniqueID, Rarity = 0, Burst;
    public int[] SenseID = new int[3], Abilities = new int[3];
    public int[] RankUpReqs = new int[30]
    {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1
    };
    public List<int> ReformAbilities = new List<int>(), TargetAbilities = new List<int>(), ReformQuestNo = new List<int>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Character_Scriptable))]
public class Character_ScriptableEditor : Editor
{
    Character_Scriptable comp;
    bool variant = false;
    int abilityToReplace = -1, newAbility = -1, questNo = -1;
    string[] affinities = new string[10] { "PHYS", "MAG", "SUPP", "FIRE", "WATER", "EARTH", "LGTNG", "WIND", "LGHT", "DARK" };

    public void OnEnable() =>   comp = (Character_Scriptable)target;

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Model = (GameObject)EditorGUILayout.ObjectField(comp.Model, typeof(GameObject), false, GUILayout.Width(75f), GUILayout.Height(75f));
        EditorGUILayout.EndHorizontal();

        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.Title = EditorGUILayout.TextField("Title", comp.Title);
        comp.Rarity = EditorGUILayout.IntField("Rarity", comp.Rarity);

        EditorGUILayout.BeginHorizontal();
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        variant = EditorGUILayout.Toggle("Variant", variant);
        EditorGUILayout.EndHorizontal();
        if (variant)
            comp.UniqueID = EditorGUILayout.IntField("Unique ID", comp.UniqueID);
        else
            comp.UniqueID = comp.ID;

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

        GUILayout.Label("Abilities", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 3; i++)
            comp.Abilities[i] = EditorGUILayout.IntField(comp.Abilities[i]);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Senses", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 3; i++)
            comp.SenseID[i] = EditorGUILayout.IntField(comp.SenseID[i]);
        EditorGUILayout.EndHorizontal();

        comp.Burst = EditorGUILayout.IntField("Burst", comp.Burst);
        comp.Range = (Stats.Range)EditorGUILayout.EnumPopup("Range", comp.Range);
        comp.Affinity = (Stats.Affinity)EditorGUILayout.EnumPopup("Affinity", comp.Affinity);

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

        GUILayout.Label("Rank Up Requirements", EditorStyles.boldLabel);
        for (int i = 0; i < 5; i++)
            if(comp.Rarity <= i)
            {
                GUILayout.Label("Rank " + (i + 1), EditorStyles.boldLabel);
                for (int j = 0; j < 3; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    comp.RankUpReqs[(i * 6) + (j * 2)] = EditorGUILayout.IntField("Item " + (j + 1), comp.RankUpReqs[(i * 6) + (j * 2)]);
                    if (comp.RankUpReqs[(i * 6) + (j * 2)] != -1)
                        comp.RankUpReqs[(i * 6) + (j * 2) + 1] = EditorGUILayout.IntField(comp.RankUpReqs[(i * 6) + (j * 2) + 1]);
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
                for(int j = 0; j < 3; j++)
                {
                    comp.RankUpReqs[(i * 6) + (j * 2)] = -1;
                    comp.RankUpReqs[(i * 6) + (j * 2) + 1] = 0;
                }

        GUILayout.Label("Reform Skills List", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        abilityToReplace = EditorGUILayout.IntField("Current to New ID", abilityToReplace);
        newAbility = EditorGUILayout.IntField(newAbility);
        questNo = EditorGUILayout.IntField(questNo);
        if (GUILayout.Button("+"))
            if (abilityToReplace >= 0 && newAbility >= 0 && !comp.ReformAbilities.Contains(abilityToReplace))
            {
                comp.ReformAbilities.Add(abilityToReplace);
                comp.TargetAbilities.Add(newAbility);
                comp.ReformQuestNo.Add(questNo);
            }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.ReformAbilities.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(comp.ReformAbilities[i] + " replaced by " + comp.TargetAbilities[i]);
            if (GUILayout.Button("-"))
            {
                comp.ReformAbilities.RemoveAt(i);
                comp.TargetAbilities.RemoveAt(i);
                comp.ReformQuestNo.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtility.SetDirty(comp);
    }
}
#endif