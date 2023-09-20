using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using ScriptableData;

public class LevelUpArm : MonoBehaviour
{
    [HideInInspector]
    public EquipmentInstance Instance;

    private float _timer;
    private int _cap, _newLvl, _newEXP_cum, _newEXP_rem;
    private int[] _crystals = new int[4], _crystalEXP = new int[4];
    private static int[] s_expGainValues = new int[4] { 100, 1000, 10000, 100000 };

    [SerializeField]
    private Button _confirm;
    [SerializeField]
    private GameObject _reset, _arrow;
    [SerializeField]
    private Slider _EXPBar;
    [SerializeField]
    private TextMeshProUGUI _title, _lvlOld, _lvlNew, _lvlCap, _exp;
    [SerializeField]
    private TextMeshProUGUI[] _crystalCnt = new TextMeshProUGUI[4];

    private void OnEnable()
    {
        _title.text = "Level Up";
        LevelUpInfo();
        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(() => CrystalUse_Confirm());
        _reset.SetActive(true);
    }

    public void LevelUpInfo()
    {
        TriggerOffCrystal();
        CrystalUse_Reset();
        _arrow.SetActive(false);
        _lvlNew.gameObject.SetActive(false);
        _cap = Stats.RarityCaps[Instance.Rank];
        _lvlOld.text = (Instance.Level + 1).ToString();
        _lvlCap.text = (_cap + 1).ToString();
        _exp.text = Instance.EXP_Remain.ToString();
        _EXPBar.maxValue = Stats.EXP_Req[Instance.Level];
        _EXPBar.value = _EXPBar.maxValue - Instance.EXP_Remain;
        _confirm.interactable = Instance.Level < _cap ? true : false;
    }

    private void CrystalCount(int type)
    {
        _crystalCnt[type].text = (ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][type] - _crystals[type]).ToString();
        _crystalEXP[type] = _crystals[type] * s_expGainValues[type];
    }

    IEnumerator Change(int type, bool add)
    {
        while (true)
        {
            if (_timer == 0 || _timer >= 0.5f)
                if (add) ChangeCrystal(type, 1);
                else ChangeCrystal(type);
            _timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public void TriggerOffCrystal()
    {
        StopAllCoroutines();
        _timer = 0;
    }

    public void TriggerAddCrystal(int type)
    {
        TriggerOffCrystal();
        StartCoroutine(Change(type, true));
    }

    public void TriggerRemoveCrystal(int type)
    {
        TriggerOffCrystal();
        StartCoroutine(Change(type, false));
    }

    private void ChangeCrystal(int type, int diff = 1)
    {
        if ((diff == -1 && _crystals[type] <= 0) ||
            (diff == 1 && (_newLvl >= _cap || _crystals[type] >= ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][type])))
        {
            TriggerOffCrystal();
            return;
        }

        _crystals[type]++;
        CrystalCount(type);
        CalculateNewLvlEXP();
    }

    private void CalculateNewLvlEXP()
    {
        _newLvl = Instance.Level;
        int expCum = Instance.EXP_Cum + _crystalEXP.Sum();
        for (int i = _cap; i >= Instance.Level; i--)
            if (expCum >= Stats.EXP_Req_Cum[i])
            {
                _newLvl = Math.Min(i + 1, _cap);
                break;
            }
        _EXPBar.maxValue = Stats.EXP_Req[_newLvl];
        if (_newLvl == _cap)
        {
            _EXPBar.value = _EXPBar.maxValue;
            expCum = Stats.EXP_Req_Cum[_newLvl];
        }
        else if (_newLvl > 0)
            _EXPBar.value = expCum - Stats.EXP_Req_Cum[_newLvl - 1];
        else
            _EXPBar.value = expCum;
        _newEXP_cum = expCum;
        _newEXP_rem = Stats.EXP_Req_Cum[_newLvl] - expCum;
        _exp.text = _newEXP_rem.ToString();
        if (_newLvl > Instance.Level)
        {
            _arrow.SetActive(true);
            _lvlNew.gameObject.SetActive(true);
            _lvlNew.text = (_newLvl + 1).ToString();
        }
        else
        {
            _arrow.SetActive(false);
            _lvlNew.gameObject.SetActive(false);
        }
    }

    private void CrystalUse_Reset()
    {
        TriggerOffCrystal();
        for (int i = 0; i < _crystals.Length; i++)
        {
            _crystals[i] = 0;
            CrystalCount(i);
        }
        _newLvl = Instance.Level;
    }

    public void CrystalUse_Confirm()
    {
        if (_crystals[0] + _crystals[1] + _crystals[2] + _crystals[3] == 0)
            return;

        Instance.Level = _newLvl;
        Instance.EXP_Cum = _newEXP_cum;
        Instance.EXP_Remain = _newEXP_rem;
        Character_Display.s_character_Display.SetLevelStats();

        for (int i = 0; i < _crystals.Length; i++)
            ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][i] -= _crystals[i];
        ScriptableDataSource.SaveGame();
        LevelUpInfo();
    }
}
