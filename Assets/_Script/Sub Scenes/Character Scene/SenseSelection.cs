using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using ScriptableData;

public class SenseSelection : MonoBehaviour
{
    [HideInInspector]
    public CharacterInstance Instance;
    [HideInInspector]
    public static SenseSelection s_SenseSelection;

    [SerializeField]
    private Button _equip, _unequip, _learn;
    private int _senseSelected = -1, _senseToReplaceValue = -1, _senseToLearnID;    // Index out of 10, Sense ID, Sense to learn
    [SerializeField]
    private GameObject _sensePrefab;
    [SerializeField]
    private Image _senseImgSelected, _senseImgToReplace;
    [SerializeField]
    private Image[] _senseSet = new Image[10];
    [SerializeField]
    private Sprite _senseSprite;    // Generic sprite filler for senses.
    [SerializeField]
    private List<GameObject> _sensesAvailable = new List<GameObject>(), _learnAvailable = new List<GameObject>();
    private static string _senseNull = "<B>Sense Name</B><size=60%>\nSense Info";
    [SerializeField]
    private TextMeshProUGUI _senseCurrent, _senseToReplace;
    [SerializeField]
    private Transform _sensesParent, _learnParent;

    private void Awake()
    {
        s_SenseSelection = this;
        LearnGen();
    }

    public void SenseSet()
    {
        // Set senses available in Senses Selection.
        for (int i = 0; i < Instance.Senses.Count; i++)
        {
            if (i < _sensesAvailable.Count)
            {
                _sensesAvailable[i].SetActive(true);
                _sensesAvailable[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.SenseDB[Instance.Senses[i]].Name;
            }
            else
            {
                GameObject obj = Instantiate(_sensePrefab, _sensesParent);
                int index = i;
                obj.GetComponent<Button>().onClick.AddListener(() => SelectSense_List(index));
                obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.SenseDB[Instance.Senses[i]].Name;
                _sensesAvailable.Add(obj);
            }
        }
        if (Instance.Senses.Count < _sensesAvailable.Count)
            for (int i = Instance.Senses.Count; i < _sensesAvailable.Count; i++)
                _sensesAvailable[i].SetActive(false);

    }

    public void ViewSenseSet()
    {
        for (int i = 0; i < _senseSet.Length; i++)
            _senseSet[i].sprite = Instance.SensesEquipt[i] >= 0 ? ScriptableDataSource.Db.SenseDB[Instance.SensesEquipt[i]].Sprite : _senseSprite;
    }

    public void SenseSelectionReset()
    {
        _senseSelected = -1;
        _senseToReplaceValue = -1;
        _senseImgSelected.sprite = _senseSprite;
        _senseImgToReplace.sprite = _senseSprite;
        _senseCurrent.text = _senseNull;
        _senseToReplace.text = _senseNull;
        _unequip.interactable = false;
        _equip.interactable = false;
        LearnInfo();
    }

    public void SelectSense_Set(int index)
    {
        if (!_senseImgToReplace.gameObject.activeInHierarchy)
            return;

        _senseSelected = index;
        if (Instance.SensesEquipt[index] >= 0)
        {
            Sense_Scriptable sense = ScriptableDataSource.Db.SenseDB[Instance.SensesEquipt[index]];
            _senseImgSelected.sprite = sense.Sprite;
            _senseCurrent.text = "<B>" + sense.Name + "</B><size=60%>\n" + sense.Description;
            _unequip.interactable = true;
        }
        else
        {
            _senseImgSelected.sprite = _senseSprite;
            _senseCurrent.text = _senseNull;
            _unequip.interactable = false;
        }
    }

    public void SelectSense_List(int index)
    {
        _senseToReplaceValue = Instance.Senses[index];
        Sense_Scriptable sense = ScriptableDataSource.Db.SenseDB[_senseToReplaceValue];
        _senseImgToReplace.sprite = sense.Sprite;
        _senseToReplace.text = "<B>" + sense.Name + "</B><size=60%>\n" + sense.Description;
        _equip.interactable = true;
    }

    public void UnequipSense()
    {
        Instance.SensesEquipt[_senseSelected] = -1;
        _senseSet[_senseSelected].sprite = _senseSprite;
        _senseImgSelected.sprite = _senseSprite;
        _senseCurrent.text = _senseNull;
        _unequip.interactable = false;
        ScriptableDataSource.SaveGame();
    }

    public void EquipSense()
    {
        // No need for action if sense is already set at slot.
        if (_senseSelected == -1 || Instance.SensesEquipt[_senseSelected] == _senseToReplaceValue)
        {
            _senseToReplaceValue = -1;
            _senseImgToReplace.sprite = _senseSprite;
            _senseToReplace.text = _senseNull;
            _equip.interactable = false;
            return;
        }

        // Check if senses equiped contain the sense already.
        if (Instance.SensesEquipt.Contains(_senseToReplaceValue))
        {
            int indexOfSense = Array.IndexOf(Instance.SensesEquipt, _senseToReplaceValue);
            Instance.SensesEquipt[indexOfSense] = Instance.SensesEquipt[_senseSelected];
            _senseSet[indexOfSense].sprite = _senseImgSelected.sprite;
        }
        Instance.SensesEquipt[_senseSelected] = _senseToReplaceValue;
        _senseImgSelected.sprite = _senseImgToReplace.sprite;
        _senseSet[_senseSelected].sprite = _senseImgToReplace.sprite;
        _senseCurrent.text = _senseToReplace.text;
        _unequip.interactable = true;

        _senseToReplaceValue = -1;
        _senseImgToReplace.sprite = _senseSprite;
        _senseToReplace.text = _senseNull;
        _equip.interactable = false;
        ScriptableDataSource.SaveGame();
    }

    private void LearnGen()
    {
        for (int i = 0; i < ScriptableDataSource.RuntimeDb.Inventory.MatCount[4].Count; i++)
        {
            GameObject obj = Instantiate(_sensePrefab, _learnParent);
            int index = i;
            obj.GetComponent<Button>().onClick.AddListener(() => LearnInfo(index));
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ScriptableDataSource.Db.SenseDB[i].Name;
            _learnAvailable.Add(obj);
        }
    }

    public void CanLearn()
    {
        for (int i = 0; i < _learnAvailable.Count; i++)
            _learnAvailable[i].SetActive(Instance.Senses.Contains(i) ? false : true);
    }

    private void LearnInfo(int senseToLearn = -1)
    {
        Debug.Log("Learn Info");
        _senseToLearnID = senseToLearn;
        if (_senseToLearnID == -1)      // Sense not selected.
        {
            _senseImgSelected.sprite = _senseSprite;
            _senseCurrent.text = _senseNull;
            _learn.interactable = false;
        }
        else
        {
            Sense_Scriptable sense = ScriptableDataSource.Db.SenseDB[_senseToLearnID];
            _senseImgSelected.sprite = sense.Sprite;
            _senseCurrent.text = "<B>" + sense.Name + "</B><size=60%>\n" + sense.Description;
            _learn.interactable = ScriptableDataSource.RuntimeDb.Inventory.MatCount[3][_senseToLearnID] > 0 ? true : false;
        }
    }

    public void Learn()
    {
        ScriptableDataSource.RuntimeDb.Inventory.MatCount[3][_senseToLearnID]--;
        Instance.Senses.Add(_senseToLearnID);
        for (int i = 0; i < 10; i++)
        {
            if (Instance.SensesEquipt[i] == -1)
            {
                Instance.SensesEquipt[i] = _senseToLearnID;
                break;
            }
        }
        LearnInfo();
        ScriptableDataSource.SaveGame();
    }
}
