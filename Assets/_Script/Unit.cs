using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for anything that can appear battle in combat :: Character, Enemy, Inanimate Obj
public class Unit : ScriptableObject
{
    public string Name, Title;
    public int[] Stats = new int[9], Talents = new int[10], Resistances = new int[10];

    public Stats.Range Range;
    public Stats.Affinity Affinity;
    public List<Stats.Attributes> Attributes = new List<Stats.Attributes>();

    public Sprite Sprite;
    public GameObject Model;
}
