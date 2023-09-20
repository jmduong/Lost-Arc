using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System.IO;
using System;

public class Character_Display : MonoBehaviour
{
    public static Character_Display s_character_Display;

    // Profile Selection (besides btns and return)
    [SerializeField]
    private GameObject _model, _selectionButton;
    [SerializeField]
    private Sprite[] _rarityBorders = new Sprite[6];
    [SerializeField]
    private Transform _selectionGrid;
    [SerializeField]
    private LevelUpChar _levelUpChar;
    [SerializeField]
    private RankUpChar _rankUpChar;
    [SerializeField]
    private MergeChar _mergeChar;

    // Character selected
    private int _current = 0;
    public int Current
    {
        get {   return _current;    }
        set
        {
            if(_current != value)
            {
                _current = value;
                SelectInstance();
                Armor.s_armor.SelectSet();
                SoulGraph.s_SoulGraph.CharacterSet();
            }
        }
    }

    [HideInInspector]
    public Character_Scriptable Character;
    [HideInInspector]
    public CharacterInstance Instance;

    // Character profile
    [SerializeField]
    private Image[] _rarity = new Image[6], _merges = new Image[4];
    public Slider _expBar;
    public TextMeshProUGUI _name, _EXP;
    [SerializeField]
    private TextMeshProUGUI _attributes;
    [SerializeField]
    private TextMeshProUGUI[] _stats = new TextMeshProUGUI[2];

    private void Awake()
    {
        s_character_Display = this;
        GenerateSelection();
    }

    private void Start()    =>  SelectInstance();

    public void ScreenChange(string scene) => WorldManager.Instance.LoadScene(scene);

    public void GenerateSelection()
    {
        for(int i = 0; i < ScriptableDataSource.Db.CharacterDB.Count; i++)
        {
            if(ScriptableDataSource.RuntimeDb.Characters.ContainsKey(i))
            {
                GameObject btn = Instantiate(_selectionButton, _selectionGrid);
                CharacterInstance instance = ScriptableDataSource.RuntimeDb.Characters[i];
                Character_Scriptable character = ScriptableDataSource.Db.CharacterDB[instance.ID];

                btn.SetActive(true);
                btn.GetComponent<Button>().onClick.AddListener(() => Current = instance.ID);
                btn.GetComponent<Image>().sprite = character.Sprite;
                btn.transform.GetChild(0).GetComponent<Image>().sprite = _rarityBorders[instance.Rarity];
            }
        }
    }

    public void SelectInstance()
    {
        Debug.Log(ScriptableDataSource.RuntimeDb.Characters.Count);
        Instance = ScriptableDataSource.RuntimeDb.Characters[_current];
        Character = ScriptableDataSource.Db.CharacterDB[Instance.ID];

        _levelUpChar.Instance = Instance;
        _rankUpChar.Instance = Instance;
        _mergeChar.Instance = Instance;
        SkillView.s_SkillView.Instance = Instance;
        SenseSelection.s_SenseSelection.Instance = Instance;

        _rankUpChar.Character = Character;
        SkillView.s_SkillView.Character = Character;

        SenseSelection.s_SenseSelection.SenseSet();
        SenseSelection.s_SenseSelection.CanLearn();

        SetLevelStats();

        // Rarity Count (6), Merge Count (4).
        for (int i = 0; i < 6; i++)
        {
            if (i < 4)
                _merges[i].color = Instance.Merges > i ? Color.white : Color.clear;
            _rarity[i].color = Instance.Rarity >= i ? Color.white : Color.clear;
        }

        // Must have at least 1 attribute or error occurs.
        _attributes.text = "| " + Character.Attributes[0] + " | ";
        for(int i = 1; i < Character.Attributes.Count; i++)
            _attributes.text += Character.Attributes[i] + " | ";

        SkillView.s_SkillView.ViewAbilitySet();
        SenseSelection.s_SenseSelection.ViewSenseSet();
    }

    public void SetLevelStats()
    {
        // Set profile details.
        _name.text = Character.Name + " <size=60%>Lvl. " + (Instance.Level + 1) + " / " + (Stats.RarityCaps[Instance.Rarity] + 1);

        _expBar.maxValue = Stats.EXP_Req[Instance.Level];
        _expBar.value = _expBar.maxValue - Instance.EXP_Remain;
        _EXP.text = Instance.EXP_Remain + " / " + _expBar.maxValue;

        for (int i = 0; i < 5; i++)
        {
            if (i == 0) _stats[0].text =    Mathf.Round(Mathf.Lerp(Character.Stats[i] / 10f, Character.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
            else        _stats[0].text +=   Mathf.Round(Mathf.Lerp(Character.Stats[i] / 10f, Character.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
        }
        for (int i = 5; i < Character.Stats.Length; i++)
        {
            if (i == 5) _stats[1].text =    Mathf.Round(Mathf.Lerp(Character.Stats[i] / 10f, Character.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
            else        _stats[1].text +=   Mathf.Round(Mathf.Lerp(Character.Stats[i] / 10f, Character.Stats[i], (Instance.Level - 1) * 1f / 9)) + "\n";
        }
    }
}