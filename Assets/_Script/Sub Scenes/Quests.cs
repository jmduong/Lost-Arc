using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System.IO;
using System;

public class Quests : MonoBehaviour
{
    private int _type = 0;

    // Top-left corner Info.
    [SerializeField]
    private TextMeshProUGUI _title;

    [SerializeField]
    private GameObject _item;
    [SerializeField]
    private Image[] _rewardsImg = new Image[7];
    [SerializeField]
    private TextMeshProUGUI _info;
    [SerializeField]
    private TextMeshProUGUI[] _rewardsValue;
    [SerializeField]
    private Transform _questParent;

    // Item Panel
    [SerializeField]
    private GameObject _itemPanel;
    [SerializeField]
    private Image _itemImg;
    [SerializeField]
    private TextMeshProUGUI _itemName, _itemAmt, _itemDescription;

    //private void Start() => GenerateInbox();

    public void ScreenChange() => WorldManager.Instance.LoadScene("Base");

    public void Option(int subtype)
    {
        if (_type != subtype)
        {
            _type = subtype;
            GenerateQuest();
        }
    }

    private void GenerateInbox()
    {
        List<(string, int, int, DateTime, string)> removal = new List<(string, int, int, DateTime, string)>();
        Debug.Log("inbox: " + ScriptableDataSource.RuntimeDb.Inbox.lst.Count);
        foreach ((string, int, int, DateTime, string) value in ScriptableDataSource.RuntimeDb.Inbox.lst)
        {
            if (DateTime.Today >= value.Item4)
                removal.Add(value);
            else
            {
                GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Type 2", _questParent);
                obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => obj.SetActive(false));
                obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.RuntimeDb.Inbox.lst.Remove(value));
                obj.transform.GetChild(2).gameObject.SetActive(false);

                switch (value.Item1)
                {
                    case "Characters":  // NOTE: Never add more than 1 instance.
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.CharacterDB[value.Item2].Name + " <size=60%>[x1]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddCharacter(value.Item2));
                        break;
                    case "Weapons":     // NOTE: Never add more than 1 instance.
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.ArmorDB[0][value.Item2].Name + " <size=60%>[x1]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddEquipmentInstance(0, value.Item2));
                        break;
                    case "Helmets":     // NOTE: Never add more than 1 instance.
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.ArmorDB[1][value.Item2].Name + " <size=60%>[x1]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddEquipmentInstance(1, value.Item2));
                        break;
                    case "Armor":       // NOTE: Never add more than 1 instance.
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.ArmorDB[2][value.Item2].Name + " <size=60%>[x1]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddEquipmentInstance(2, value.Item2));
                        break;
                    case "Accessories": // NOTE: Never add more than 1 instance.
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.ArmorDB[3][value.Item2].Name + " <size=60%>[x1]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddEquipmentInstance(3, value.Item2));
                        break;
                    case "SoulFrags":   // NOTE: Never add more than 1 instance. Only 1 instance can exist.
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.SoulFragDB[value.Item2].Name + " <size=60%>[x1]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddSoulFrag(value.Item2));
                        break;
                    case "Currency_1":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currency 1 <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddCurrency(0, value.Item3));
                        break;
                    case "Currency_2":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currency 2 <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddCurrency(1, value.Item3));
                        break;
                    case "Currency_3":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currency 3 <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddCurrency(2, value.Item3));
                        break;
                    case "Currency_4":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currency 4 <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddCurrency(3, value.Item3));
                        break;
                    case "Currency_5":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Currency 5 <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.AddCurrency(4, value.Item3));
                        break;
                    case "Consumable":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.MaterialsDB[0][value.Item2].Name + " <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.RuntimeDb.Inventory.AddMaterial(0, value.Item2, value.Item3));
                        break;
                    case "Material":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.MaterialsDB[1][value.Item2].Name + " <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.RuntimeDb.Inventory.AddMaterial(1, value.Item2, value.Item3));
                        break;
                    case "Event":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.MaterialsDB[2][value.Item2].Name + " <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.RuntimeDb.Inventory.AddMaterial(2, value.Item2, value.Item3));
                        break;
                    case "SkillBook":
                        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.SkillDB[value.Item2].Name + " <size=60%>[x " + value.Item3 + "]";
                        obj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => ScriptableDataSource.RuntimeDb.Inventory.AddMaterial(3, value.Item2, value.Item3));
                        break;
                    case "":
                        break;
                }
                //obj.GetComponent<Button>().onClick.AddListener(() => SetType2Info(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, "Claim before: " + value.Item4.ToString("d") + "\n\n" + value.Item5));
            }
        }
        foreach ((string, int, int, DateTime, string) value in removal)
            ScriptableDataSource.RuntimeDb.Inbox.lst.Remove(value);
        ScriptableDataSource.SaveGame();
    }

    private void GenerateQuest()
    {

    }

    public void SelectItem(int index)
    {
        _itemPanel.SetActive(true);
        //Materials_Scriptable mat = ScriptableDataSource.Db.MaterialsDB[_subtype][index];
        //_itemImg.sprite = mat.Sprite;
        //_itemName.text = mat.Name;
        //_itemDescription.text = mat.Description;
    }

    public void SelectQuest(int quest, int subquest)
    {

    }

    /*public void SetQuestType(int type)
    {
        if (_questTypeSelected != type)
        {
            Deselect(_type2Parent);
            _questTypeSelected = type;
            string date = null;
            date = type switch
            {
                0 => DateTime.Today.AddDays(1).AddSeconds(-1).ToString(),
                1 => DateTime.Today.AddDays(DateTime.Today.DayOfWeek switch { DayOfWeek.Monday => 7, DayOfWeek.Tuesday => 6, DayOfWeek.Wednesday => 5, DayOfWeek.Thursday => 4, DayOfWeek.Friday => 3, DayOfWeek.Saturday => 2, _ => 1 }).AddSeconds(-1).ToString(),
                3 => "Special Date",
                _ => date
            };
            for (int i = 0; i < ScriptableDataSource.RuntimeDb.QuestProgress[type].Count; i++)
                if (!ScriptableDataSource.RuntimeDb.QuestProgress[type][i].Item2)
                {
                    GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Type 2", _type2Parent);
                    obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.QuestDB[type][i].Title;

                    if (ScriptableDataSource.RuntimeDb.QuestProgress[type][i].Item1 >= ScriptableDataSource.Db.QuestDB[type][i].TargetAmount)
                    {
                        Button btn = obj.transform.GetChild(1).GetComponent<Button>();
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(() => obj.SetActive(false));
                        //btn.onClick.AddListener(() => ScriptableDataSource.AddReward(ScriptableDataSource.Db.QuestDB[type][i]));
                        btn.onClick.AddListener(() => ScriptableDataSource.RuntimeDb.QuestProgress[type][i] = (0, true));
                    }
                    else
                        obj.transform.GetChild(1).gameObject.SetActive(false);

                    obj.transform.GetChild(2).GetComponent<Image>().fillAmount = (float)ScriptableDataSource.RuntimeDb.QuestProgress[type][i].Item1 / ScriptableDataSource.Db.QuestDB[type][i].TargetAmount;

                    string str = (date != null ? "Available until: " + date + "\n\n" : "") + ScriptableDataSource.Db.QuestDB[type][i].Description + "\n\n[" + ScriptableDataSource.RuntimeDb.QuestProgress[type][i].Item1 + " / " + ScriptableDataSource.Db.QuestDB[type][i].TargetAmount + "]";
                    obj.GetComponent<Button>().onClick.AddListener(() => SetType2Info(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, str));
                }
        }
    }*/
}
