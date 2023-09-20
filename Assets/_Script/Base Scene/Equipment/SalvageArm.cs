using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using ScriptableData;

public class SalvageArm : MonoBehaviour
{
    [HideInInspector]
    public Equipment_Scriptable Equipment;
    [HideInInspector]
    public EquipmentInstance Instance;
    [HideInInspector]
    public int _type;
    [HideInInspector]
    public string _armCode;

    [SerializeField]
    private Button _confirm;
    [SerializeField]
    private GameObject _reset;
    private GameObject _armBtn;
    [SerializeField]
    private Image[] _salvageImgs = new Image[3];

    [SerializeField]
    private TextMeshProUGUI _title;
    [SerializeField]
    private TextMeshProUGUI[] _salvageCnts = new TextMeshProUGUI[3];

    private void OnEnable()
    {
        _title.text = "Salvage";
        SalvageInfo();
        _confirm.onClick.RemoveAllListeners();
        _confirm.onClick.AddListener(() => Salvage());
        _reset.SetActive(false);
    }

    private void SalvageInfo()
    {
        _confirm.interactable = !Instance.Locked;
        for (int i = 0; i < 3; i++)
        {
            if (Equipment.SalvageMats[i * 2] >= 0)
            {
                _salvageImgs[i].gameObject.SetActive(true);
                _salvageImgs[i].sprite = ScriptableDataSource.Db.MaterialsDB[0][Equipment.SalvageMats[i * 2]].Sprite;
                _salvageCnts[i].text = string.Format("{0} / {1}", Equipment.SalvageMats[(i * 2) + 1], ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][Equipment.SalvageMats[i * 2]]);
            }
            else
                _salvageImgs[i].gameObject.SetActive(false);
        }
    }

    public void Salvage()
    {
        if (Instance.CharacterID >= 0)
            ScriptableDataSource.RuntimeDb.Characters[Instance.CharacterID].ArmorEquipt[_type] = null;   // Remove equip.
        ScriptableDataSource.RuntimeDb.Armor[_type].Remove(_armCode);
        for (int i = 0; i < 3; i++)
            if (Equipment.SalvageMats[i * 2] >= 0)
                ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][Equipment.SalvageMats[i * 2]] += Equipment.SalvageMats[(i * 2) + 1];
        ScriptableDataSource.SaveGame();
        _armBtn.SetActive(false);
        _armBtn.transform.SetParent(null);
        SalvageInfo();
    }
}