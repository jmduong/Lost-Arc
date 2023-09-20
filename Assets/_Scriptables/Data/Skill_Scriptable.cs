using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Skill", menuName = "ScriptableObjects/Skill")]
public class Skill_Scriptable : ScriptableObject
{
    public Skill_Scriptable() { }

    public string Name, Description;
    public int ID, Cost, Mod;
    public Stats.OffensiveStat Stat;
    public Stats.Affinity Affinity;
    public Stats.Target Target;

    public Sprite Sprite;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Skill_Scriptable))]
public class Skill_ScriptableEditor : Editor
{
    Skill_Scriptable comp;

    public void OnEnable()
    {
        comp = (Skill_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.Description = EditorGUILayout.TextArea("Description", comp.Description);

        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.Cost = EditorGUILayout.IntField("Cost", comp.Cost);
        comp.Mod = EditorGUILayout.IntField("Mod", comp.Mod);

        comp.Affinity = (Stats.Affinity)EditorGUILayout.EnumPopup("Affinity", comp.Affinity);
        comp.Stat = (Stats.OffensiveStat)EditorGUILayout.EnumPopup("Stat", comp.Stat);
        comp.Target = (Stats.Target)EditorGUILayout.EnumPopup("Target", comp.Target);

        EditorUtility.SetDirty(comp);
    }
}
#endif