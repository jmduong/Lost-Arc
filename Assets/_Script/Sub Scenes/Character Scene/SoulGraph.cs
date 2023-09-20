using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Linq;
using System;

public class SoulGraph : MonoBehaviour
{
    public static SoulGraph s_SoulGraph;

    [SerializeField]
    private GameObject _fragment, _removalPanel;
    [SerializeField]
    private Image _dragImg;

    [SerializeField]
    private SoulSlot[] _soulSlots = new SoulSlot[9];
    [HideInInspector]
    public int SoulDrag = -1;
    [HideInInspector]
    public int[] SoulIndex = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
    [SerializeField]
    private Sprite _blankSprite;
    private static string _fragStringBase = "<b>Select Fragment</b>\n<size=60%>None\nNone</size>";
    [SerializeField]
    private TextMeshProUGUI _fragDescription;
    private Dictionary<int, TextMeshProUGUI> _selText = new Dictionary<int, TextMeshProUGUI>();
    [SerializeField]
    private Transform _parent;

    // Start is called before the first frame update
    void Start()
    {
        s_SoulGraph = this;
        InitializeFragmentList();
    }

    public void Reset()
    {
        SoulDrag = -1;
        _fragDescription.text = _fragStringBase;
    }

    public void CharacterSet()
    {
        SoulIndex = Character_Display.s_character_Display.Instance.SoulFragments;
        for (int i = 0; i < _soulSlots.Length; i++)
        {
            _soulSlots[i].SoulItem.SoulIndex = SoulIndex[i];
            _soulSlots[i].Img.sprite = SoulIndex[i] >= 0 ? ScriptableDataSource.Db.SoulFragDB[SoulIndex[i]].Sprite : _blankSprite;
            _soulSlots[i].Border.color = SoulIndex[i] >= 0 ? Stats.ColorDict[ScriptableDataSource.Db.SoulFragDB[SoulIndex[i]].Color] : Color.white;
        }
    }

    public void InitializeFragmentList()
    {
        List<int> removals = new List<int>();
        foreach (KeyValuePair<int, int> entry in ScriptableDataSource.RuntimeDb.SoulFrags)
        {
            Debug.Log("Key: " + entry.Key + "  Value: " + entry.Value);
            if (entry.Key < 0)
                removals.Add(entry.Key);
            else
                FragmentInstance(entry.Key, entry.Value);
        }
        if (removals.Count > 0)
        {
            foreach (int index in removals)
                ScriptableDataSource.RuntimeDb.SoulFrags.Remove(index);
            ScriptableDataSource.SaveGame();
        }
    }

    public GameObject FragmentInstance(int index, int instanceEquipt)
    {
        GameObject fragment = Instantiate(_fragment, _parent);
        fragment.SetActive(true);
        SoulFragment_Scriptable sf = ScriptableDataSource.Db.SoulFragDB[index];
        // 0 - Sprite Image, 1 - Soul Color, 2 - Name of Soul, 3 - Name of Character w/ soul
        fragment.transform.GetChild(0).GetComponent<Image>().sprite = sf.Sprite;
        fragment.transform.GetChild(0).GetComponent<SoulItem>().Img = _dragImg;
        fragment.transform.GetChild(0).GetComponent<SoulItem>().SoulIndex = index;
        fragment.transform.GetChild(0).GetComponent<SoulItem>().RemovalPanel = _removalPanel;
        fragment.transform.GetChild(1).GetComponent<Image>().color = Stats.ColorDict[sf.Color];
        fragment.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = sf.Name;
        _selText.Add(index, fragment.transform.GetChild(3).GetComponent<TextMeshProUGUI>());
        _selText[index].text = instanceEquipt >= 0 ? ScriptableDataSource.Db.CharacterDB[instanceEquipt].Name : "";

        return fragment;
    }

    public void SelectFragment(int index)
    {
        if(index >= 0)
        {
            SoulFragment_Scriptable frag = ScriptableDataSource.Db.SoulFragDB[index];
            _fragDescription.text = "<b>" + frag.Name + "</b>\n<size=60%>| ";
            for (int i = 0; i < frag.Stats.Count; i++)
                _fragDescription.text += frag.Stats[i] + " " + frag.StatValues[i] + " | ";
        }
        else
            _fragDescription.text = _fragStringBase;
    }

    public void SoulDrop(int index = -1)
    {
        // Scenario 1: Soul at index is the same as the one being dropped. 
        if (SoulIndex[index] == SoulDrag)
            return;

        // Scenario 2: Soul Graph already contains the soul being dropped.
        if(SoulIndex.Contains(SoulDrag))
        {
            int indexOfSoulDrag = Array.IndexOf(SoulIndex, SoulDrag);
            int container = SoulIndex[index];
            SoulIndex[indexOfSoulDrag] = container;
            _soulSlots[indexOfSoulDrag].SoulItem.SoulIndex = container;

            // Scenario 2.1: The new slot already contains a soul. Switch slots.
            if (container >= 0 && container != SoulDrag)
            {
                SoulFragment_Scriptable sfSwap = ScriptableDataSource.Db.SoulFragDB[container];
                _soulSlots[indexOfSoulDrag].Img.sprite = sfSwap.Sprite;
                _soulSlots[indexOfSoulDrag].Border.color = Stats.ColorDict[sfSwap.Color];
            }
            // Scenario 2.2: The new slot does not already contain a soul.
            else
            {
                _soulSlots[indexOfSoulDrag].Img.sprite = _blankSprite;
                _soulSlots[indexOfSoulDrag].Border.color = Color.white;
            }
        }

        SoulIndex[index] = SoulDrag;
        Character_Display.s_character_Display.Instance.SoulFragments = SoulIndex;
        _soulSlots[index].SoulItem.SoulIndex = SoulDrag;
        ScriptableDataSource.RuntimeDb.SoulFrags[SoulDrag] = Character_Display.s_character_Display.Current;
        ScriptableDataSource.SaveGame();

        SoulFragment_Scriptable sf = ScriptableDataSource.Db.SoulFragDB[SoulDrag];
        _soulSlots[index].Img.sprite = sf.Sprite;
        _soulSlots[index].Border.color = Stats.ColorDict[sf.Color];
        _selText[SoulDrag].text = Character_Display.s_character_Display.Character.Name;
    }

    public void SoulRemoval()
    {
        if (SoulIndex.Contains(SoulDrag)) // This case is for if user starts from list and drops back in list.
        {
            int indexOfSoulRemoval = Array.IndexOf(SoulIndex, SoulDrag);
            SoulIndex[indexOfSoulRemoval] = -1;
            Character_Display.s_character_Display.Instance.SoulFragments = SoulIndex;
            _soulSlots[indexOfSoulRemoval].SoulItem.SoulIndex = -1;
            ScriptableDataSource.RuntimeDb.SoulFrags[SoulDrag] = -1;
            ScriptableDataSource.SaveGame();

            _soulSlots[indexOfSoulRemoval].Img.sprite = _blankSprite;
            _soulSlots[indexOfSoulRemoval].Border.color = Color.white;
            _selText[SoulDrag].text = "";
        }
    }
}
