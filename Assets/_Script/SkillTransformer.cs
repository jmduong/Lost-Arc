using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/*
 * This script transforms the button attached to either show or hide information or other child gameobjects.
 */
public class SkillTransformer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> buttons = new List<GameObject>();
    public List<GameObject> Buttons
    {
        get { return buttons; }
        set { buttons = value; }
    }

    [SerializeField]
    public List<GameObject> conditionals = new List<GameObject>();
    [SerializeField]
    private float timeFinish = 1f;
    [SerializeField]
    private Vector2 rect, rectTransform;

    [SerializeField]
    private CanvasGroup group;

    public void AlphaZero()
    {
        if(group.alpha != 0)
            foreach(GameObject btn in buttons)
                btn.GetComponent<RectTransform>().sizeDelta = rect;
    }

    public void TransformRect(int value)
    {
        if(buttons[value].GetComponent<Button>() != null)buttons[value].GetComponent<Button>().interactable = false;
        bool transformed = buttons[value].GetComponent<RectTransform>().sizeDelta == rectTransform;
        StartCoroutine(Transforming(buttons[value], transformed));
    }

    public void TransformRectScale(int value)
    {
        if (buttons[value].GetComponent<Button>() != null) buttons[value].GetComponent<Button>().interactable = false;
        bool transformed = buttons[value].GetComponent<RectTransform>().localScale == new Vector3(rectTransform.x / rect.x, rectTransform.y / rect.y, buttons[value].GetComponent<RectTransform>().localScale.z);
        StartCoroutine(TransformingScale(buttons[value], transformed));
    }

    public void TransformRectBase(int value)
    {
        if (buttons[value].GetComponent<Button>() != null) buttons[value].GetComponent<Button>().interactable = false;
        bool transformed = buttons[value].GetComponent<RectTransform>().sizeDelta == rectTransform;
        StartCoroutine(Transforming(buttons[value], transformed, false));
    }

    public void TransformRectScaleBase(int value)
    {
        if (buttons[value].GetComponent<Button>() != null) buttons[value].GetComponent<Button>().interactable = false;
        bool transformed = buttons[value].GetComponent<RectTransform>().localScale == new Vector3(rectTransform.x / rect.x, rectTransform.y / rect.y, buttons[value].GetComponent<RectTransform>().localScale.z);
        StartCoroutine(TransformingScale(buttons[value], transformed, false));
    }

    public void TransformRectAll(int value)
    {
        bool transformed = buttons[value].GetComponent<RectTransform>().sizeDelta == rectTransform;
        StartCoroutine(TransformingAll(value, transformed));
    }

    public void ConditionalTransformRectAll(int cond)  // Works when set only 1 object
    {
        bool transformed = buttons[0].GetComponent<RectTransform>().sizeDelta == rectTransform;
        if(!transformed)                                            // Not transformed.
        {
            StartCoroutine(TransformingAll(0, transformed, false));
            UnityEngine.Debug.Log(1);
        }
        else if (conditionals[cond].activeSelf)                     // Transitioned and object active
        {
            StartCoroutine(TransformingAll(0, transformed, false));
            UnityEngine.Debug.Log(2);
        }
        else                                                        // Transitioned and object not active
        {
            UnityEngine.Debug.Log(3);
            for (int i = 0; i < conditionals.Count; i++)
            {
                conditionals[cond].SetActive(i == cond ? true : false);
            }
        }
    }

    public void TransformRectAllScale(int value)
    {
        bool transformed = buttons[value].GetComponent<RectTransform>().localScale == new Vector3(rectTransform.x / rect.x, rectTransform.y / rect.y, buttons[value].GetComponent<RectTransform>().localScale.z);
        StartCoroutine(TransformingAllScale(value, transformed));
    }

    public void TransformRectAllBase(int value)
    {
        bool transformed = buttons[value].GetComponent<RectTransform>().sizeDelta == rectTransform;
        StartCoroutine(TransformingAll(value, transformed, false));
    }

    public void TransformRectAllScaleBase(int value)
    {
        bool transformed = buttons[value].GetComponent<RectTransform>().localScale == new Vector3(rectTransform.x / rect.x, rectTransform.y / rect.y, buttons[value].GetComponent<RectTransform>().localScale.z);
        StartCoroutine(TransformingAllScale(value, transformed, false));
    }

    private IEnumerator Transforming(GameObject button, bool transformed, bool find = true)
    {
        if (find)
        {
            if (transformed) button.transform.Find("Transformed").gameObject.SetActive(false);
            else button.transform.Find("Original").gameObject.SetActive(false);
        }

        float t = 0f;
        while (t < timeFinish)
        {
            t += Time.deltaTime;
            button.GetComponent<RectTransform>().sizeDelta = transformed ?
                                                                Vector2.Lerp(rectTransform, rect, t / timeFinish) :
                                                                Vector2.Lerp(rect, rectTransform, t / timeFinish);
            yield return null;
        }
        button.GetComponent<RectTransform>().sizeDelta = transformed ? rect : rectTransform;

        if (find)
        {
            if (transformed) button.transform.Find("Original").gameObject.SetActive(true);
            else button.transform.Find("Transformed").gameObject.SetActive(true);
        }

        if (button.GetComponent<Button>() != null) button.GetComponent<Button>().interactable = true;
    }

    private IEnumerator TransformingScale(GameObject button, bool transformed, bool find = true)
    {
        if (find)
        {
            if (transformed) button.transform.Find("Transformed").gameObject.SetActive(false);
            else button.transform.Find("Original").gameObject.SetActive(false);
        }

        float t = 0f;
        while (t < timeFinish)
        {
            t += Time.deltaTime;
            button.GetComponent<RectTransform>().localScale = transformed ?
                                                                Vector2.Lerp(rectTransform / rect, new Vector2(1, 1), t / timeFinish) :
                                                                Vector2.Lerp(new Vector2(1, 1), rectTransform / rect, t / timeFinish);
            yield return null;
        }
        button.GetComponent<RectTransform>().localScale = transformed ? new Vector2(1, 1) : rectTransform / rect;

        if (find)
        {
            if (transformed) button.transform.Find("Original").gameObject.SetActive(true);
            else button.transform.Find("Transformed").gameObject.SetActive(true);
        }

        if (button.GetComponent<Button>() != null) button.GetComponent<Button>().interactable = true;
    }

    private IEnumerator TransformingAll(int value, bool transformed, bool find = true)
    {
        float t = 0f;

        // Disable all buttons.
        foreach (GameObject button in buttons)
        {
            if (button.GetComponent<Button>() != null) button.GetComponent<Button>().interactable = false;
        }

        // Button is already transformed. Return to normal.
        if (transformed)
        {
            if(find)    buttons[value].transform.Find("Transformed").gameObject.SetActive(false);

            while (t < timeFinish)
            {
                t += Time.deltaTime;
                buttons[value].GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(rectTransform, rect, t / timeFinish);
                yield return null;
            }
            buttons[value].GetComponent<RectTransform>().sizeDelta = rect;

            if (find)   buttons[value].transform.Find("Original").gameObject.SetActive(true);
        }
        // Transform button. Check others to see if any are transformed and revert.
        else
        {
            int index = buttons.FindIndex(x => x.GetComponent<RectTransform>().sizeDelta == rectTransform);

            if (find)
            {
                buttons[value].transform.Find("Original").gameObject.SetActive(false);
                if (index >= 0)
                {
                    buttons[index].transform.Find("Transformed").gameObject.SetActive(false);
                }
            }

            while (t < timeFinish)
            {
                t += Time.deltaTime;
                buttons[value].GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(rect, rectTransform, t / timeFinish);
                if(index >= 0)
                {
                    buttons[index].GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(rectTransform, rect, t / timeFinish);
                }
                yield return null;

            }

            if (find)
            {
                buttons[value].transform.Find("Transformed").gameObject.SetActive(true);
                if (index >= 0)
                {
                    buttons[index].transform.Find("Original").gameObject.SetActive(true);
                }
            }
        }

        // Disable all buttons.
        foreach (GameObject button in buttons)
        {
            if (button.GetComponent<Button>() != null) button.GetComponent<Button>().interactable = true;
        }
    }

    private IEnumerator TransformingAllScale(int value, bool transformed, bool find = true)
    {
        float t = 0f;

        // Disable all buttons.
        foreach (GameObject button in buttons)
        {
            if (button.GetComponent<Button>() != null) button.GetComponent<Button>().interactable = false;
        }

        // Button is already transformed. Return to normal.
        if (transformed)
        {
            if (find) buttons[value].transform.Find("Transformed").gameObject.SetActive(false);

            while (t < timeFinish)
            {
                t += Time.deltaTime;
                buttons[value].GetComponent<RectTransform>().localScale = Vector2.Lerp(rectTransform / rect, new Vector2(1, 1), t / timeFinish);
                yield return null;
            }
            buttons[value].GetComponent<RectTransform>().localScale = new Vector2(1, 1);

            if (find) buttons[value].transform.Find("Original").gameObject.SetActive(true);
        }
        // Transform button. Check others to see if any are transformed and revert.
        else
        {
            int index = buttons.FindIndex(x => x.GetComponent<RectTransform>().localScale == new Vector3( rectTransform.x / rect.x, rectTransform.y / rect.y, buttons[value].GetComponent<RectTransform>().localScale.z));

            if (find)
            {
                buttons[value].transform.Find("Original").gameObject.SetActive(false);
                if (index >= 0)
                {
                    buttons[index].transform.Find("Transformed").gameObject.SetActive(false);
                }
            }

            while (t < timeFinish)
            {
                t += Time.deltaTime;
                buttons[value].GetComponent<RectTransform>().localScale = Vector2.Lerp(new Vector2(1, 1), rectTransform / rect, t / timeFinish);
                if (index >= 0)
                {
                    buttons[index].GetComponent<RectTransform>().localScale = Vector2.Lerp(rectTransform / rect, new Vector2(1, 1), t / timeFinish);
                }
                yield return null;

            }

            if (find)
            {
                buttons[value].transform.Find("Transformed").gameObject.SetActive(true);
                if (index >= 0)
                {
                    buttons[index].transform.Find("Original").gameObject.SetActive(true);
                }
            }
        }

        // Disable all buttons.
        foreach (GameObject button in buttons)
        {
            if (button.GetComponent<Button>() != null) button.GetComponent<Button>().interactable = true;
        }
    }
}
