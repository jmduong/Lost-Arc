using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "StorySub", menuName = "ScriptableObjects/StorySub")]
public class StoriesSub_Scriptable : ScriptableObject
{
    public StoriesSub_Scriptable() { }

    public string Name, Description;
    public int ChapterID, Part, SubPart;
    public List<string> RewardType = new List<string>();
    public List<int> Reward = new List<int>(), Amount = new List<int>();

    public List<string> Enemies_1 = new List<string>(), Enemies_2 = new List<string>(), Enemies_3 = new List<string>();
    // If more than one character, use '/' to separate names and expressions. If using the same expression, no '/' needed.
    public List<string> Characters = new List<string>(), Expressions = new List<string>(), Dialogue = new List<string>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(StoriesSub_Scriptable))]
public class StoriesSub_ScriptableEditor : Editor
{
    StoriesSub_Scriptable comp;

    public void OnEnable()
    {
        comp = (StoriesSub_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.Description = EditorGUILayout.TextField("Description", comp.Description);

        comp.ChapterID = EditorGUILayout.IntField("ChapterID", comp.ChapterID);
        comp.Part = EditorGUILayout.IntField("Part", comp.Part);
        comp.SubPart = EditorGUILayout.IntField("SubPart", comp.SubPart);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Rewards", EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            comp.RewardType.Add("");
            comp.Reward.Add(0);
            comp.Amount.Add(1);
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.RewardType.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.RewardType[i] = EditorGUILayout.TextField(comp.RewardType[i]);
            comp.Reward[i] = EditorGUILayout.IntField(comp.Reward[i]);
            comp.Amount[i] = EditorGUILayout.IntField(comp.Amount[i]);

            if (GUILayout.Button("-", GUILayout.Width(50f)))
            {
                comp.RewardType.RemoveAt(i);
                comp.Reward.RemoveAt(i);
                comp.Amount.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Round 1", EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            comp.Enemies_1.Add("Add Enemy");
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.Enemies_1.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Enemies_1[i] = EditorGUILayout.TextField(comp.Enemies_1[i]);
            if (GUILayout.Button("-", GUILayout.Width(50f)))
            {
                comp.Enemies_1.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Round 2", EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            comp.Enemies_2.Add("Add Enemy");
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.Enemies_2.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Enemies_2[i] = EditorGUILayout.TextField(comp.Enemies_2[i]);
            if (GUILayout.Button("-", GUILayout.Width(50f)))
            {
                comp.Enemies_2.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Round 3", EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            comp.Enemies_3.Add("Add Enemy");
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.Enemies_3.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Enemies_3[i] = EditorGUILayout.TextField(comp.Enemies_3[i]);
            if (GUILayout.Button("-", GUILayout.Width(50f)))
            {
                comp.Enemies_3.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Dialogue", EditorStyles.boldLabel);
        if (GUILayout.Button("+", GUILayout.Width(100f)))
        {
            comp.Characters.Add("CHARACTERNAME");
            comp.Expressions.Add("DEFAULT");
            comp.Dialogue.Add("DIALOGUE");
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.Dialogue.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Characters[i] = EditorGUILayout.TextField("Character / Expression" , comp.Characters[i]);
            comp.Expressions[i] = EditorGUILayout.TextField(comp.Expressions[i], GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
            comp.Dialogue[i] = EditorGUILayout.TextField("Line " + (i + 1), comp.Dialogue[i]);
            if (GUILayout.Button("-", GUILayout.Width(50f)))
            {
                comp.Characters.RemoveAt(i);
                comp.Expressions.RemoveAt(i);
                comp.Dialogue.RemoveAt(i);
            }
        }

        EditorUtility.SetDirty(comp);
    }
}
#endif