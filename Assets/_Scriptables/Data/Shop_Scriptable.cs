using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Shop", menuName = "ScriptableObjects/Shop")]
public class Shop_Scriptable : ScriptableObject
{
    public Shop_Scriptable() { }

    public int Currency;
    // Character, Equipment, Consumables, Key, Event
    public List<int> Type = new List<int>(), ID = new List<int>(), Count = new List<int>(), Cost = new List<int>();
}

#if UNITY_EDITOR
[CustomEditor(typeof(Shop_Scriptable))]
public class Shop_ScriptableEditor : Editor
{
    Shop_Scriptable comp;

    public void OnEnable() => comp = (Shop_Scriptable)target;

    public override void OnInspectorGUI()
    {
        comp.Currency = EditorGUILayout.IntField("Currency: ", comp.Currency);  // 0 - 4

        if (GUILayout.Button("+"))
        {
            comp.Type.Add(0);
            comp.ID.Add(0);
            comp.Count.Add(1);
            comp.Cost.Add(1);
        }
        GUILayout.Label("Items Available", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Type\t");
        GUILayout.Label("ID\t");
        GUILayout.Label("Count\t");
        GUILayout.Label("Cost\t");
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < comp.Type.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            comp.Type[i] = EditorGUILayout.IntField(comp.Type[i]);
            comp.ID[i] = EditorGUILayout.IntField(comp.ID[i]);
            comp.Count[i] = EditorGUILayout.IntField(comp.Count[i]);
            comp.Cost[i] = EditorGUILayout.IntField(comp.Cost[i]);
            if (GUILayout.Button("-"))
            {
                comp.Type.RemoveAt(i);
                comp.ID.RemoveAt(i);
                comp.Count.RemoveAt(i);
                comp.Cost.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorUtility.SetDirty(comp);
    }
}
#endif