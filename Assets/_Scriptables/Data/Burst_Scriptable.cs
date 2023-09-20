using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Burst", menuName = "ScriptableObjects/Burst")]
public class Burst_Scriptable : ScriptableObject
{
    public Burst_Scriptable() { }

    public string Name;

    public int ID, Cost, Affinity, BaseValue;
    public Stats.Target_Enemy Target;
    public Stats.Attributes AttributeMod;
    public List<Stats.StatTypes> Stats = new List<Stats.StatTypes>();
    public List<Stats.Target> Targets = new List<Stats.Target>();
    public List<Stats.ActivationOrder> ActivationOrders = new List<Stats.ActivationOrder>();
    public List<int> StatMods = new List<int>();
    public List<Stats.Attributes> AttributeMods = new List<Stats.Attributes>();

    public Sprite Sprite;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Burst_Scriptable))]
public class Burst_ScriptableEditor : Editor
{
    Burst_Scriptable comp;

    public void OnEnable()
    {
        comp = (Burst_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Name = EditorGUILayout.TextField("Name", comp.Name);
        comp.ID = EditorGUILayout.IntField("ID", comp.ID);
        comp.Cost = EditorGUILayout.IntField("Cost", comp.Cost);
        comp.BaseValue = EditorGUILayout.IntField("Value", comp.BaseValue);
        comp.Affinity = EditorGUILayout.IntField("Affinity", comp.Affinity);
        comp.AttributeMod = (Stats.Attributes)EditorGUILayout.EnumPopup("Attribute", comp.AttributeMod);
        comp.Target = (Stats.Target_Enemy)EditorGUILayout.EnumPopup("Target", comp.Target);

        GUILayout.Label("Additional Mods", EditorStyles.boldLabel);
        if (GUILayout.Button("+"))
        {
            comp.Stats.Add(Stats.StatTypes.ATK);
            comp.Targets.Add(Stats.Target.User);
            comp.StatMods.Add(0);
            comp.ActivationOrders.Add(Stats.ActivationOrder.After);
            comp.AttributeMods.Add(Stats.Attributes.None);
        }
        for (int i = 0; i < comp.Stats.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Stats[i] = (Stats.StatTypes)EditorGUILayout.EnumPopup(comp.Stats[i]);
            comp.Targets[i] = (Stats.Target)EditorGUILayout.EnumPopup(comp.Targets[i]);
            comp.ActivationOrders[i] = (Stats.ActivationOrder)EditorGUILayout.EnumPopup(comp.ActivationOrders[i]);
            comp.StatMods[i] = EditorGUILayout.IntField(comp.StatMods[i]);
            if(comp.Targets[i] != Stats.Target.User) comp.AttributeMods[i] = (Stats.Attributes)EditorGUILayout.EnumPopup(comp.AttributeMods[i]);
            if (GUILayout.Button("-"))
            {
                comp.Stats.RemoveAt(i);
                comp.Targets.RemoveAt(i);
                comp.StatMods.RemoveAt(i);
                comp.ActivationOrders.RemoveAt(i);
                comp.AttributeMods.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtility.SetDirty(comp);
    }
}
#endif