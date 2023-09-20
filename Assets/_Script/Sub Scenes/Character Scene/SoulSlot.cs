using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulSlot : MonoBehaviour, IDropHandler
{
    [HideInInspector]
    public SoulItem SoulItem;
    public Image Img;
    public Image Border;
    public int SoulIndex;

    void Start()
    {
        SoulItem = GetComponent<SoulItem>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("On Drop");
        GameObject obj = eventData.pointerDrag;
        Img.sprite = obj.GetComponent<Image>().sprite;
        SoulGraph.s_SoulGraph.SoulDrop(SoulIndex);
        SoulGraph.s_SoulGraph.SoulDrag = -1;
    }
}