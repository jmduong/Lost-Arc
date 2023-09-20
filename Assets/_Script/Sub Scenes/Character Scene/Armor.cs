using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableData;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class Armor : MonoBehaviour
{
    public static Armor s_armor;

    [HideInInspector]
    public int _currentType = 0;
    private string _currentInstance;
    [SerializeField]
    private Button _equipBtn, _unequipBtn;
    [SerializeField]
    private Sprite[] _lock = new Sprite[2];
    [SerializeField]
    private Transform[] _armorGrid = new Transform[4];
    [SerializeField]
    private GameObject _armoryButton;
    private static string _equipBase = "<b>Select Equipment</b><size=60%>\nStats of Armor\nEffects of Armor";
    [SerializeField]
    private TextMeshProUGUI _equip, _selected;

    private EquipmentInstance _instance;
    private Equipment_Scriptable _equipment;
    private string[] _codes = new string[5];

    // Start is called before the first frame update
    void Start()
    {
        s_armor = this;
        GenerateSelection();
    }

    public void GenerateSelection()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            foreach (KeyValuePair<string, EquipmentInstance> code in ScriptableDataSource.RuntimeDb.Armor[i])
            {
                GameObject btn = Instantiate(_armoryButton, _armorGrid[i]);
                btn.SetActive(true);

                btn.GetComponent<Button>().onClick.AddListener(() => SelectInstance(index, code.Key));
                btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.ArmorDB[i][index].Name;
                btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = code.Value.CharacterID >= 0 ? "<size=60%>" + ScriptableDataSource.Db.CharacterDB[code.Value.CharacterID].Name + "</size>\tLv." + (code.Value.Level + 1) : "Lv." + (code.Value.Level + 1);

                btn.transform.GetChild(2).GetComponent<Image>().color = Stats.ColorDict[code.Value.Rank];
                btn.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ChangeLock(code.Value, btn.transform.GetChild(3).GetChild(0).GetComponent<Image>()));
                btn.transform.GetChild(3).GetChild(0).GetComponent<Image>().sprite = _lock[code.Value.Locked ? 0 : 1];
            }
        }
    }

    private void ChangeLock(EquipmentInstance _instance, Image img)
    {
        _instance.Locked = !_instance.Locked;
        img.sprite = _lock[_instance.Locked ? 0 : 1];
    }

    public void SelectSet() =>  _codes = Character_Display.s_character_Display.Instance.ArmorEquipt;

    public void SelectType(int type)
    {
        _currentType = type;
        _currentInstance = null;
        _equipBtn.interactable = false;
        _selected.text = _equipBase;
        if(_codes[type] != null)
        {
            SetInstance(type, _codes[type]);
            _unequipBtn.interactable = true;
        }
        else
        {
            _equip.text = _equipBase;
            _unequipBtn.interactable = false;
        }
    }

    public void SetInstance(int type, string code)
    {
        type = Mathf.Min(type, 3);
        _instance = ScriptableDataSource.RuntimeDb.Armor[type][code];
        _equipment = ScriptableDataSource.Db.ArmorDB[type][_instance.ID];

        _equip.text = string.Format("<b>{0}<size=60%> Lv. {1}<b>\n| ", _equipment.Name, _instance.Level + 1);
        for (int i = 0; i < _equipment.StatTypes.Count; i++)
        {
            // Unlock new stat with every rank.
            if(_instance.Rank >= i)
                _equip.text += _equipment.StatTypes[i] + " " + _equipment.Stats[i] + " | ";
        }
        _selected.text = _equipBase;
    }

    public void SelectInstance(int type, string code)
    {
        _currentInstance = code;
        _equipBtn.interactable = true;
        _instance = ScriptableDataSource.RuntimeDb.Armor[type][code];
        _equipment = ScriptableDataSource.Db.ArmorDB[type][_instance.ID];

        _selected.text = string.Format("<b>{0}<size=60%> Lv. {1}</b>\n| ", _equipment.Name, _instance.Level + 1);
        for (int i = 0; i < _equipment.StatTypes.Count; i++)
        {
            // Unlock new stat with every rank.
            if (_instance.Rank >= i)
                _selected.text += _equipment.StatTypes[i] + " " + _equipment.Stats[i] + " | ";
        }
    }

    public void EquipInstance()
    {
        if(_currentInstance != null)
        {
            _codes[_currentType] = _currentInstance;
            Character_Display.s_character_Display.Instance.ArmorEquipt[_currentType] = _codes[_currentType];
            SetInstance(_currentType, _currentInstance);
            _unequipBtn.interactable = true;
            _currentInstance = null;
            _equipBtn.interactable = false;
            ScriptableDataSource.SaveGame();
        }
    }

    public void UnequipInstance()
    {
        _codes[_currentType] = null;
        Character_Display.s_character_Display.Instance.ArmorEquipt[_currentType] = null;
        _equip.text = _equipBase;
        _unequipBtn.interactable = false;
        ScriptableDataSource.SaveGame();
    }
}
