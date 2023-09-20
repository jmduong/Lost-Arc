using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using ScriptableData;

public class MergeChar : MonoBehaviour
{
    [HideInInspector]
    public CharacterInstance Instance;

    [SerializeField]
    private Button _confirm;
    private float _timer, _alpha, _diff;
    [SerializeField]
    private float _alphaMin = 0, _alphaMax = 1;
    [SerializeField]
    private GameObject _reset;
    private int index;
    [SerializeField]
    private Image[] _merges = new Image[4];
    [SerializeField]
    private TextMeshProUGUI _title, _ptCost;

    private void Awake()    =>  _diff = _alphaMax - _alphaMin;

    private void OnEnable()
    {
        _title.text = "Merge";
        MergeInfo();
        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(() => Merge());
        _reset.SetActive(false);
    }

    private void OnDisable()
    {
        _timer = 0;
        StopAllCoroutines();
        if (index < 4)
            _merges[index].color = Color.clear;
    }

    private void MergeInfo()
    {
        StopAllCoroutines();
        if (Instance.Merges == 4) // Maxed out
        {
            _ptCost.text = "MAX Merges";
            _ptCost.color = Color.white;
            _confirm.interactable = false;
            index = 4;
            for (int i = 0; i < _merges.Length; i++)
                _merges[i].color = Color.white;
        }
        else
        {
            _confirm.interactable = Instance.Points >= 100 ? true : false;
            _ptCost.color = Instance.Points >= 100 ? Color.white : Color.red;
            _ptCost.text = string.Format("{0} / 100", Instance.Points);
            index = Instance.Merges;
            StartCoroutine(Blink());
            for (int i = 0; i < _merges.Length; i++)
                _merges[i].color = index > i ? Color.white : Color.clear;
        }
    }

    IEnumerator Blink()
    {
        while(true)
        {
            _timer += Time.deltaTime;
            _alpha = (Mathf.Cos(_timer * Mathf.PI) * _diff) + _diff / 2 + _alphaMin;
            _merges[index].color = new Color(1, 1, 1, _alpha);
            yield return null;
        }
    }

    public void Merge()
    {
        Instance.Points -= 100;
        Instance.Merges++;
        MergeInfo();
        ScriptableDataSource.SaveGame();
    }
}
