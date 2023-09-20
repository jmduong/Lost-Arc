using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Story", menuName = "ScriptableObjects/Story")]
public class Story_Scriptable : ScriptableObject
{
    public Story_Scriptable() { }

    public string Name, Description;
    public int ChapterID, Part;
    public List<string> RewardType = new List<string>();
    public List<int> Reward = new List<int>(), Amount = new List<int>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Story_Scriptable))]
public class Story_ScriptableEditor : Editor
{
    Story_Scriptable comp;

    public void OnEnable()
    {
        comp = (Story_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.Description = EditorGUILayout.TextField("Description", comp.Description);

        comp.ChapterID = EditorGUILayout.IntField("ChapterID", comp.ChapterID);
        comp.Part = EditorGUILayout.IntField("Part", comp.Part);

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

            if (GUILayout.Button("-"))
            {
                comp.RewardType.RemoveAt(i);
                comp.Reward.RemoveAt(i);
                comp.Amount.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorUtility.SetDirty(comp);
    }
}
#endif