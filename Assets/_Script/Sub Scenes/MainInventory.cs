using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System.IO;
using System;

public class MainInventory : MonoBehaviour
{
    private int _type = 0;

    [SerializeField]
    private TextMeshProUGUI _title;

    private List<GameObject> _itemSlots = new List<GameObject>();
    private List<TextMeshProUGUI> _itemCnts = new List<TextMeshProUGUI>();
    [SerializeField]
    private Transform _itemParent;

    // Item Panel
    [SerializeField]
    private GameObject _itemPanel, _item;
    [SerializeField]
    private Image _itemImg;
    [SerializeField]
    private TextMeshProUGUI _itemName, _itemAmt, _itemDescription;

    private void Start()    =>  GenerateInventory();

    public void ScreenChange() => WorldManager.Instance.LoadScene("Base");

    public void Option(int subtype)
    {
        if (_type != subtype)
        {
            _type = subtype;
            _title.text = _type switch 
            {
                1 => "<b>Inventory</b>\n<size=30>Materials",
                2 => "<b>Inventory</b>\n<size=30>Events",
                3 => "<b>Inventory</b>\n<size=30>Treasures",
                4 => "<b>Inventory</b>\n<size=30>Key Items",
                5 => "<b>Inventory</b>\n<size=30>Skills",
                _ => "<b>Inventory</b>\n<size=30>Consumables"
            };
            GenerateInventory();
        }
    }

    private void GenerateInventory()
    {
        int count = _type switch
        {
            4 => ScriptableDataSource.RuntimeDb.Inventory.KeyItems.Count,
            5 => ScriptableDataSource.Db.SkillDB.Count,
            _ => ScriptableDataSource.Db.MaterialsDB[_type].Count
        };
        int alt = 0;
        for (int i = 0; i < count; i++)
        {
            alt = _type != 4 ? i : ScriptableDataSource.RuntimeDb.Inventory.KeyItems[i];
            if (i < _itemSlots.Count)
            {
                _itemSlots[i].SetActive(true);
                _itemSlots[i].GetComponent<Image>().sprite = _type < 5 ? ScriptableDataSource.Db.MaterialsDB[_type][alt].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                _itemCnts[i].text = _type == 4 ? "" : ScriptableDataSource.RuntimeDb.Inventory.MatCount[_type == 5 ? 4 : _type].ContainsKey(i) ? ScriptableDataSource.RuntimeDb.Inventory.MatCount[_type == 5 ? 4 : _type][i].ToString() : "0";
            }
            else
            {
                GameObject obj = Instantiate(_item, _itemParent);
                //GameObject obj = ObjectPool.s_sharedInstance.GetPooledObject("Item", _itemParent);
                int index = i;
                obj.GetComponent<Image>().sprite = _type < 5 ? ScriptableDataSource.Db.MaterialsDB[_type][alt].Sprite : ScriptableDataSource.Db.SkillDB[i].Sprite;
                obj.GetComponent<Button>().onClick.AddListener(() => SelectItem(index));
                _itemSlots.Add(obj);
                _itemCnts.Add(obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
                _itemCnts[i].text = _type == 4 ? "" : ScriptableDataSource.RuntimeDb.Inventory.MatCount[_type == 5 ? 4 : _type].ContainsKey(i) ? ScriptableDataSource.RuntimeDb.Inventory.MatCount[_type == 5 ? 4 : _type][i].ToString() : "0";
            }
        }
        if (count < _itemSlots.Count)
            for (int i = count; i < _itemSlots.Count; i++)
                _itemSlots[i].SetActive(false);
    }

    public void SelectItem(int index)
    {
        _itemPanel.SetActive(true);
        if(_type != 5)
        {
            Materials_Scriptable mat = ScriptableDataSource.Db.MaterialsDB[_type][index];
            _itemImg.sprite = mat.Sprite;
            _itemName.text = mat.Name;
            _itemDescription.text = mat.Description;
            _itemAmt.text = "Count: " + _itemCnts[index].text;
        }
        else
        {
            Skill_Scriptable skill = ScriptableDataSource.Db.SkillDB[index];
            _itemImg.sprite = skill.Sprite;
            _itemName.text = skill.Name;
            _itemDescription.text = skill.Description;
            _itemAmt.text = "";
        }
    }
}
