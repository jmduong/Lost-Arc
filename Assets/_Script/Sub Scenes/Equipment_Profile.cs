using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System.IO;
using System;

public class Equipment_Profile : MonoBehaviour
{
    public static Equipment_Profile s_Equipment_Display;

    // Profile Selection (besides btns and return)
    [SerializeField]
    private GameObject _selectionButton;
    [SerializeField]
    private Transform[] _selectionGrid = new Transform[4];
    [SerializeField]
    private LevelUpArm _levelUpArm;
    [SerializeField]
    private ReforgeArm _reforgeArm;
    [SerializeField]
    private SalvageArm _salvageArm;

    // Equipment Code selected
    private int _type;
    private string _current;

    [HideInInspector]
    public Equipment_Scriptable Equipment;
    [HideInInspector]
    public EquipmentInstance Instance;

    [SerializeField]
    private Button _confirm;
    private GameObject _armBtn;
    [SerializeField]
    private Image[] _rank = new Image[6];
    public Slider _expBar;
    [SerializeField]
    private Sprite[] _lock = new Sprite[2];
    public TextMeshProUGUI _name, _EXP, _character;
    [SerializeField]
    private TextMeshProUGUI[] _stats = new TextMeshProUGUI[2];

    private void Awake()    =>  s_Equipment_Display = this;

    // Start is called before the first frame update
    void Start()
    {
        GenerateSelection();
    }

    public void ScreenChange(string scene) => WorldManager.Instance.LoadScene(scene);

    private void GenerateSelection()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            foreach (KeyValuePair<string, EquipmentInstance> code in ScriptableDataSource.RuntimeDb.Armor[i])
            {
                GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Armor", _selectionGrid[i]);
                obj.GetComponent<Button>().onClick.AddListener(() => SelectInstance(obj, code.Key));

                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.ArmorDB[i][code.Value.ID].Name;
                obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = code.Value.ID >= 0 ? "<size=60%>" + ScriptableDataSource.Db.ArmorDB[index][code.Value.ID].Name + "</size>\tLv." + (code.Value.Level + 1) : "Lv." + (code.Value.Level + 1);
                obj.transform.GetChild(2).GetComponent<Image>().color = Stats.ColorDict[code.Value.Rank];
                obj.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => ChangeLock(code.Value, obj.transform.GetChild(3).GetChild(0).GetComponent<Image>()));
                obj.transform.GetChild(3).GetChild(0).GetComponent<Image>().sprite = _lock[code.Value.Locked ? 0 : 1];
            }
        }

    }

    private void ChangeLock(EquipmentInstance instance, Image img)
    {
        instance.Locked = !instance.Locked;
        img.sprite = _lock[instance.Locked ? 0 : 1];
        if (instance == Instance)   // Change when salvaging.
            _confirm.interactable = !instance.Locked;
    }

    public void SelectInstance(GameObject obj, string key)
    {
        _armBtn = obj;
        _current = key;

        Instance = ScriptableDataSource.RuntimeDb.Armor[_type][_current];
        Equipment = ScriptableDataSource.Db.ArmorDB[_type][Instance.ID];

        _levelUpArm.Instance = Instance;
        _reforgeArm.Instance = Instance;
        _salvageArm.Instance = Instance;
        _salvageArm.Equipment = Equipment;

        SetLevelStats();
    }

    public void SetLevelStats()
    {
        // Set profile details.
        _name.text = Equipment.Name + " <size=60%>Lvl. " + (Instance.Level + 1) + " / " + (Stats.RarityCaps[Instance.Rank] + 1);
        _character.text = Instance.CharacterID >= 0 ? ScriptableDataSource.Db.CharacterDB[Instance.CharacterID].Name : "";

        for (int i = 0; i < 6; i++)
            _rank[i].color = Instance.Rank >= i ? Color.white : Color.clear;

        _expBar.maxValue = Stats.EXP_Req[Instance.Level];
        _expBar.value = _expBar.maxValue - Instance.EXP_Remain;
        _EXP.text = Instance.EXP_Remain + " / " + _expBar.maxValue;

        for (int i = 0; i < 5; i++)
        {
            Debug.Log(i);
            if (i == 0) _stats[0].text = Mathf.Round(Mathf.Lerp(Equipment.Stats[i] / 10f, Equipment.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
            else        _stats[0].text += Mathf.Round(Mathf.Lerp(Equipment.Stats[i] / 10f, Equipment.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
        }
        for (int i = 5; i < Equipment.Stats.Length; i++)
        {
            Debug.Log(i);
            if (i == 5) _stats[1].text = Mathf.Round(Mathf.Lerp(Equipment.Stats[i] / 10f, Equipment.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
            else _stats[1].text += Mathf.Round(Mathf.Lerp(Equipment.Stats[i] / 10f, Equipment.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
        }
    }

    public void Reset(int tyoe)
    {
        _armBtn = null;
        _type = tyoe;
        _current = null;

        Instance = null;
        Equipment = null;

        _levelUpArm.Instance = null;
        _reforgeArm.Instance = null;
        _salvageArm.Instance = null;
        _salvageArm.Equipment = null;

        // Set profile details.
        _name.text = "Name <size=60%>Lvl. XXX / XXX";
        _character.text = "Character";

        _expBar.value = _expBar.maxValue;
        _EXP.text = "XXXXX / XXXXX";

        _stats[0].text = "####\n####\n####\n####\n####";
        _stats[1].text = "####\n####\n####\n####";

        for (int i = 0; i < 6; i++)
            _rank[i].color = Color.white;
    }
}
