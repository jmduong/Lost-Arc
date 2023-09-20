using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class SoulItem : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject RemovalPanel;
    public Image Img;
    [HideInInspector]
    public int SoulIndex = -1;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SoulIndex >= 0) 
            SoulGraph.s_SoulGraph.SelectFragment(SoulIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(SoulIndex < 0)
            eventData.pointerDrag = null;
        else
        {
            Img.gameObject.SetActive(true);
            RemovalPanel.SetActive(true);
            Img.sprite = GetComponent<Image>().sprite;
            SoulGraph.s_SoulGraph.SoulDrag = SoulIndex;
        }
    }

    public void OnDrag(PointerEventData eventData)          =>  Img.transform.position = Input.mousePosition;

    public void OnEndDrag(PointerEventData eventData)
    {
        RemovalPanel.SetActive(false);
        Img.gameObject.SetActive(false);
    }

}
