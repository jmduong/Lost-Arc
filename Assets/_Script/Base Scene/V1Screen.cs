using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System.IO;
using System;

// Inventory, Shop, Inbox, Quest
public class V1Screen : MonoBehaviour
{
    private int _type = 0, _subtype = 0;

    // Top-left corner Info.
    [SerializeField]
    private Image _screenIcon;
    [SerializeField]
    private TextMeshProUGUI _screenType;

    [SerializeField]
    private GameObject _optsMenu;
    [SerializeField]
    private Image[] _options = new Image[7];
    [SerializeField]    // Ned to get sprites for all of these.
    private Sprite[] _screenIcons = new Sprite[3], _inventoryIcons = new Sprite[7], _shopIcons = new Sprite[7], _questIcons = new Sprite[7];


    // Inventory / Shop
    // Currency parts.
    [SerializeField]
    private GameObject _currencyObj;
    [SerializeField]
    private Image[] _currencyImg = new Image[2];
    [SerializeField]
    private Sprite[] _currencySprite = new Sprite[5];
    [SerializeField]
    private TextMeshProUGUI[] _currencies = new TextMeshProUGUI[2];
    private (int, int, int) _purchaseValues = (-1, -1, -1); // Type, Index, Currency
    // Items
    private List<GameObject> _itemSlots = new List<GameObject>();
    private List<TextMeshProUGUI> _itemCnts = new List<TextMeshProUGUI>();
    [SerializeField]
    private Transform _itemParent;

    // Inbox
    [SerializeField]
    private GameObject _inbox, _infoObj;   // _info also used by Quest
    [SerializeField]
    private TextMeshProUGUI _info;          // Also used by Quest
    [SerializeField]
    private Transform _InboxParent;

    // Quest
    [SerializeField]
    private GameObject _quest;
    [SerializeField]
    private Image[] _rewardsImg = new Image[7];
    [SerializeField]
    private TextMeshProUGUI[] _rewardsValue;
    [SerializeField]
    private Transform _questParent;

    // Item Panel
    [SerializeField]
    private GameObject _itemPanel, _insufficient, _confirm;
    [SerializeField]
    private Image _itemImg;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _itemName, _itemAmt, _timeRem, _itemDescription, _purchaseAmt, _purchaseCost;

    public void SelectScreenType(int type)
    {
        _type = type;
        _subtype = 0;
        _screenIcon.sprite = _screenIcons[type];
        _itemPanel.SetActive(false);

        switch (type)
        {
            case 0:     // Inventory
                SetObjectsActive(true, false, false, false, false, false, false, true, false);
                for (int i = 0; i < 7; i++)
                    _options[i].sprite = _inventoryIcons[i];
                _screenType.text = "<b>Inventory</b>\n<size=30>Consumables";
                GenerateInventory();
                break;
            case 1:     // Shop
                SetObjectsActive(true, true, false, false, false, true, true, true, true);
                for (int i = 0; i < 7; i++)
                {
                    _options[i].sprite = _shopIcons[i];
                    if (i < 2)
                    {
                        _currencyImg[i].sprite = _currencySprite[i];
                        SetCurrency(i, i);
                    }
                }
                _screenType.text = "<b>Shop</b>\n<size=30>Recommend";
                GenerateShop();
                break;
            case 2:    // Inbox
                SetObjectsActive(false, false, true, false, true, false, false, false, false);
                for (int i = 0; i < 7; i++)
                    _rewardsImg[i].gameObject.SetActive(false);
                _screenType.text = "<b>Inbox</b>\n<size=30>XXX / 100";
                _info.text = "<size=160%>Title[x]</size>\n\nDescription";
                GenerateInbox();
                break;
            default:    // Quest
                SetObjectsActive(true, false, false, true, true, false, false, false, false);
                _slider.gameObject.SetActive(false);
                for (int i = 0; i < 7; i++)
                {
                    if (i < 6) _options[i].sprite = _questIcons[i];
                    _rewardsImg[i].gameObject.SetActive(false);
                }
                _screenType.text = "<b>Quests</b>\n<size=30>Daily";
                _info.text = "Location - Sublocation\n<size=160%>Quest Title</size>\n\n<indent=10%>\u2022 Task 1\n\u2022 Task 2\n\u2022 Task 3</indent>\n\nQuest Description";
                GenerateQuest();
                break;
        }
    }

    private void SetObjectsActive(bool optsMenu, bool currencyObj, bool inbox, bool quest, bool infoBox, bool itemAmt, bool timeRem, bool itemParent, bool slider)
    {
        _insufficient.SetActive(false);

        _optsMenu.SetActive(optsMenu);
        _currencyObj.SetActive(currencyObj);
        _inbox.SetActive(inbox);
        _quest.SetActive(quest);
        _infoObj.SetActive(infoBox);
        _itemAmt.gameObject.SetActive(itemAmt);
        _timeRem.gameObject.SetActive(timeRem);
        _itemParent.gameObject.SetActive(itemParent);
        _slider.gameObject.SetActive(slider);
    }

    public void Option(int subtype)
    {
        if (_subtype != subtype)
        {
            _subtype = subtype;
            switch (_type)
            {
                case 0:
                    GenerateInventory();
                    break;
                case 1:
                    GenerateShop();
                    break;
                case 2:
                    GenerateInbox();
                    break;
                case 3:
                    GenerateQuest();
                    break;
            }
        }
        _subtype = subtype;
    }

    private void GenerateInventory()
    {
        // 0. Consumables, 1. Materials, 2. Events, 3. Treasures (Valuable Items), 4. Key, 5. Skills
        int count = _subtype switch
        {
            4 => ScriptableDataSource.RuntimeDb.Inventory.KeyItems.Count,
            5 => ScriptableDataSource.Db.SkillDB.Count,
            _ => ScriptableDataSource.Db.MaterialsDB[_subtype].Count
        };
        int alt = 0;
        for (int i = 0; i < count; i++)
        {
            alt = _subtype != 4 ? i : ScriptableDataSource.RuntimeDb.Inventory.KeyItems[i];
            if (i < _itemSlots.Count)
            {
                _itemSlots[i].SetActive(true);
                _itemSlots[i].GetComponent<Image>().sprite = _subtype < 5 ? ScriptableDataSource.Db.MaterialsDB[_subtype][alt].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                _itemCnts[i].text = _subtype == 4 ? "" : ScriptableDataSource.RuntimeDb.Inventory.MatCount[_subtype == 5 ? 4 : _subtype].ContainsKey(i) ? "x " + ScriptableDataSource.RuntimeDb.Inventory.MatCount[_subtype == 5 ? 4 : _subtype][i] : "x 0";
            }
            else
            {
                GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Item", _itemParent);
                int index = i;
                obj.GetComponent<Image>().sprite = _subtype < 5 ? ScriptableDataSource.Db.MaterialsDB[_subtype][alt].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                obj.GetComponent<Button>().onClick.AddListener(() => SelectItem(index));
                _itemSlots.Add(obj);
                _itemCnts.Add(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                _itemCnts[i].text = _subtype == 4 ? "" : ScriptableDataSource.RuntimeDb.Inventory.MatCount[_subtype == 5 ? 4 : _subtype].ContainsKey(i) ? "x " + ScriptableDataSource.RuntimeDb.Inventory.MatCount[_subtype == 5 ? 4 : _subtype][i] : "x 0";
            }
        }
        if(count < _itemSlots.Count)
            for (int i = count; i < _itemSlots.Count; i++)
                _itemSlots[i].SetActive(false);
    }

    private void GenerateShop()
    {
        Shop_Scriptable shop = ScriptableDataSource.Db.ShopDB[_subtype];
        // 0. Consumables, 1. Materials, 2. Events, 3. Treasures (Valuable Items), 4. Key, 5. Skills
        int count = shop.Type.Count;
        for (int i = 0; i < count; i++)
        {
            if (i < _itemSlots.Count)
            {
                _itemSlots[i].SetActive(true);
                //_itemSlots[i].GetComponent<Image>().sprite = shop.ItemType[i] < 5 ? ScriptableDataSource.Db.MaterialsDB[_subtype][i].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                _itemCnts[i].text = ScriptableDataSource.RuntimeDb.SellRemaining[_subtype][i] + " / " + shop.Count[i];
            }
            else
            {
                GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Item", _itemParent);
                int index = i;
                //obj.GetComponent<Image>().sprite = shop.ItemType[i] < 5 ? ScriptableDataSource.Db.MaterialsDB[_subtype][i].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                obj.GetComponent<Button>().onClick.AddListener(() => SelectItem(index));
                _itemSlots.Add(obj);
                _itemCnts.Add(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                _itemCnts[i].text = ScriptableDataSource.RuntimeDb.SellRemaining[_subtype][i] + " / " + shop.Count[i];
            }
        }
        if (count < _itemSlots.Count)
            for (int i = count; i < _itemSlots.Count; i++)
                _itemSlots[i].SetActive(false);
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
                GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Type 2", _InboxParent);
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
        switch(_type)
        {
            case 0:     // Inventory
                //    private TextMeshProUGUI _itemAmt
                Materials_Scriptable mat = ScriptableDataSource.Db.MaterialsDB[_subtype][index];
                _itemImg.sprite = mat.Sprite;
                _itemName.text = mat.Name;
                _itemDescription.text = mat.Description;
                break;
        }
    }


    public void SetCurrency(int btn, int index) => _currencies[btn].text = ScriptableDataSource.RuntimeDb.Inventory.Currencies[index].ToString();

    public void Purchase()
    {
        // Get Currency Type, cost per unit, and total units
        ScriptableDataSource.RuntimeDb.Inventory.Currencies[_subtype] -= 0;
    }

    public void SelectQuest(int quest, int subquest)
    {

    }
}
