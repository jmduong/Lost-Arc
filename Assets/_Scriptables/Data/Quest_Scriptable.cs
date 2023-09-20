using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Quests", menuName = "ScriptableObjects/Quests")]
public class Quest_Scriptable : ScriptableObject
{
    public Quest_Scriptable() { }

    // Quest Info
    public string Title, Description;
    public int TargetType, TargetID, TargetAmount = 1;
    public List<int> RewardType = new List<int>(), RewardID = new List<int>(), RewardAmount = new List<int>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Quest_Scriptable))]
public class Quest_ScriptableEditor : Editor
{
    Quest_Scriptable comp;

    public void OnEnable()
    {
        comp = (Quest_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Title = EditorGUILayout.TextField("Title", comp.Title);
        comp.Description = EditorGUILayout.TextField("Description", comp.Description);

        comp.TargetType = EditorGUILayout.IntField("TargetType", comp.TargetType);
        comp.TargetID = EditorGUILayout.IntField("TargetID", comp.TargetID);
        comp.TargetAmount = EditorGUILayout.IntField("TargetAmount", comp.TargetAmount);

        if (GUILayout.Button("+"))
        {
            comp.RewardType.Add(0);
            comp.RewardID.Add(0);
            comp.RewardAmount.Add(1);
        }
        for (int i = 0; i < comp.RewardType.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.RewardType[i] = EditorGUILayout.IntField(comp.RewardType[i]);
            comp.RewardID[i] = EditorGUILayout.IntField(comp.RewardID[i]);
            comp.RewardAmount[i] = EditorGUILayout.IntField(comp.RewardAmount[i]);
            if (GUILayout.Button("-"))
            {
                comp.RewardType.RemoveAt(i);
                comp.RewardID.RemoveAt(i);
                comp.RewardAmount.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtility.SetDirty(comp);
    }
}
#endif