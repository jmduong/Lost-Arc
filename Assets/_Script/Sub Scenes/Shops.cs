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
public class Shops : MonoBehaviour
{
    private int _type = 0;

    [SerializeField]
    private TextMeshProUGUI _title;

    // Inventory / Shop
    // Currency parts.
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

    // Item Panel
    [SerializeField]
    private GameObject _itemPanel, _insufficient, _item;
    [SerializeField]
    private Image _itemImg;
    [SerializeField]
    private Slider _slider;
    [SerializeField]
    private TextMeshProUGUI _itemName, _itemAmt, _timeRem, _itemDescription, _purchaseAmt, _purchaseCost;

    private void Start() => GenerateShop();

    public void ScreenChange() => WorldManager.Instance.LoadScene("Base");

    public void Option(int subtype)
    {
        if (_type != subtype)
        {
            _type = subtype;
            _title.text = _type switch
            {
                1 => "<b>Shop</b>\n<size=30>Currency",
                2 => "<b>Shop</b>\n<size=30>Currency",
                3 => "<b>Shop</b>\n<size=30>Currency",
                4 => "<b>Shop</b>\n<size=30>Currency",
                5 => "<b>Shop</b>\n<size=30>Currency",
                _ => "<b>Shop</b>\n<size=30>Currency"
            };
            GenerateShop();
        }
    }

    private void GenerateShop()
    {
        Shop_Scriptable shop = ScriptableDataSource.Db.ShopDB[_type];
        int count = shop.Type.Count;
        for (int i = 0; i < count; i++)
        {
            if (i < _itemSlots.Count)
            {
                _itemSlots[i].SetActive(true);
                //_itemSlots[i].GetComponent<Image>().sprite = shop.ItemType[i] < 5 ? ScriptableDataSource.Db.MaterialsDB[_subtype][i].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                _itemCnts[i].text = shop.Cost[i].ToString();
            }
            else
            {
                GameObject obj = Instantiate(_item, _itemParent);
                int index = i;
                //obj.GetComponent<Image>().sprite = shop.ItemType[i] < 5 ? ScriptableDataSource.Db.MaterialsDB[_subtype][i].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                obj.GetComponent<Button>().onClick.AddListener(() => SelectItem(index));
                _itemSlots.Add(obj);
                _itemCnts.Add(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                _itemCnts[i].text = shop.Cost[i].ToString();
            }
        }
        if (count < _itemSlots.Count)
            for (int i = count; i < _itemSlots.Count; i++)
                _itemSlots[i].SetActive(false);
    }

    public void SelectItem(int index)
    {
        _itemPanel.SetActive(true);
        //Materials_Scriptable mat = ScriptableDataSource.Db.MaterialsDB[_subtype][index];
        //_itemImg.sprite = mat.Sprite;
        //_itemName.text = mat.Name;
        //_itemDescription.text = mat.Description;
    }


    public void SetCurrency(int btn, int index) => _currencies[btn].text = ScriptableDataSource.RuntimeDb.Inventory.Currencies[index].ToString();

    public void Purchase()
    {
        // Get Currency Type, cost per unit, and total units
        //ScriptableDataSource.RuntimeDb.Inventory.Currencies[_subtype] -= 0;
    }
}
