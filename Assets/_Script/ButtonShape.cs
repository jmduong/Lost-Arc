using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Changes the input range of the buttons from the entire ui object to the alpha threshold.
public class ButtonShape : MonoBehaviour
{
    void Start()    =>  GetComponent<Button>().targetGraphic.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
}
