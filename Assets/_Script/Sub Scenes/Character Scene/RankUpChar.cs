using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using ScriptableData;

public class RankUpChar : MonoBehaviour
{
    [HideInInspector]
    public Character_Scriptable Character;
    [HideInInspector]
    public CharacterInstance Instance;

    [SerializeField]
    private Button _confirm;
    private float _timer, _alpha, _diff;
    [SerializeField]
    private float _alphaMin = 0, _alphaMax = 1;
    [SerializeField]
    private GameObject _reset;
    [SerializeField]
    private Image[] _rankUpImgs = new Image[3], _stars = new Image[6];
    private int index;
    [SerializeField]
    private TextMeshProUGUI _title, _lvlOld, _lvlNew;
    [SerializeField]
    private TextMeshProUGUI[] _rankUpCnts = new TextMeshProUGUI[3];

    private void Awake()    =>  _diff = _alphaMax - _alphaMin;

    private void OnEnable()
    {
        _title.text = "Rank Up";
        RankUpInfo();
        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(() => RankUp());
        _reset.SetActive(false);
    }

    private void OnDisable()
    {
        _timer = 0;
        CancelRankUp();    
    }

    private void RankUpInfo()
    {
        CancelRankUp();
        int currentCap;
        int newCap;
        int[,] itemReqs = new int[3, 2];
        currentCap = Stats.RarityCaps[Instance.Rarity];
        newCap = Stats.RarityCaps[Mathf.Min(Instance.Rarity + 1, 5)];
        if (currentCap < newCap)
        itemReqs = new int[3, 2] {  {   Character.RankUpReqs[Instance.Rarity * 6],          Character.RankUpReqs[(Instance.Rarity * 6) + 1] },
                                    {   Character.RankUpReqs[(Instance.Rarity * 6) + 2],    Character.RankUpReqs[(Instance.Rarity * 6) + 3] },
                                    {   Character.RankUpReqs[(Instance.Rarity * 6) + 4],    Character.RankUpReqs[(Instance.Rarity * 6) + 5] } };
        _lvlOld.text = string.Format("Lvl. {0} / {1}", Instance.Level + 1, currentCap + 1);
        index = Instance.Rarity + 1;
        if (Instance.Rarity < 5)
        {
            _lvlNew.text = string.Format("Lvl. {0} / {1}", Instance.Level + 1, newCap + 1);
            StartCoroutine(Blink());
            _confirm.interactable = Instance.Level < currentCap ? false : true;
        }
        else
        {
            _lvlNew.text = "MAX CAP";
            _confirm.interactable = false;
        }
        for (int i = 0; i < 3; i++)
        {
            _stars[i].color = Instance.Rarity >= i ? Color.white : Color.clear;
            if (itemReqs[i, 0] >= 0 && currentCap < newCap)
            {
                _rankUpImgs[i].gameObject.SetActive(true);
                _rankUpImgs[i].sprite = ScriptableDataSource.Db.MaterialsDB[0][itemReqs[i, 0]].Sprite;
                _rankUpCnts[i].text = string.Format("{0} / {1}", itemReqs[i, 1], ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][itemReqs[i, 0]]);
                _rankUpCnts[i].color = itemReqs[i, 1] <= ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][itemReqs[i, 0]] ? Color.white : Color.red;
            }
            else
                _rankUpImgs[i].gameObject.SetActive(false);
        }
        for (int i = 3; i < 6; i++)
            _stars[i].color = Instance.Rarity >= i ? Color.white : Color.clear;
    }

    IEnumerator Blink()
    {
        while (true)
        {
            _timer += Time.deltaTime;
            _alpha = (Mathf.Cos(_timer * Mathf.PI) * _diff) + _diff / 2 + _alphaMin;
            _stars[index].color = new Color(1, 1, 1, _alpha);
            yield return null;
        }
    }

    public void RankUp()
    {
        int num = 0;
        for (int i = 0; i < 3; i++)
        {
            num = (Instance.Rarity * 6) + (i * 2);
            if (Character.RankUpReqs[num] >= 0)
                ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][Character.RankUpReqs[num]] -= Character.RankUpReqs[num + 1];
        }
        Instance.Rarity++;
        RankUpInfo();
        ScriptableDataSource.SaveGame();
    }

    public void CancelRankUp()
    {
        StopAllCoroutines();
        if(index <= 5)
            _stars[index].color = Color.clear;
    }
}