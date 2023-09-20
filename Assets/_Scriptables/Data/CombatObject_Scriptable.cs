using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
[CreateAssetMenu(fileName = "Combat Obj", menuName = "ScriptableObjects/CombatObj")]
public class CombatObject_Scriptable : Unit
{

}

#if UNITY_EDITOR
[CustomEditor(typeof(CombatObject_Scriptable))]
public class CombatObject_ScriptableEditor : Editor
{
    CombatObject_Scriptable comp;

    public void OnEnable()
    {
        comp = (CombatObject_Scriptable)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        comp.Sprite = (Sprite)EditorGUILayout.ObjectField(comp.Sprite, typeof(Sprite), false, GUILayout.Width(75f), GUILayout.Height(75f));
        comp.Model = (GameObject)EditorGUILayout.ObjectField(comp.Model, typeof(GameObject), false, GUILayout.Width(75f), GUILayout.Height(75f));
        EditorGUILayout.EndHorizontal();

        EditorUtility.SetDirty(comp);
    }
}
#endif