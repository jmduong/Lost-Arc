using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoulRemoval : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)  =>  SoulGraph.s_SoulGraph.SoulRemoval();
}
