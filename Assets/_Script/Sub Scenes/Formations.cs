using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Formations : MonoBehaviour
{
    private int _team = -1, _selected;

    [SerializeField]
    private GameObject _character;
    [SerializeField]
    private Image _selectedMember;
    [SerializeField]
    private Image[] _members = new Image[4];
    [SerializeField]
    private Sprite _empty;
    [SerializeField]
    private Transform _selectTransform;

    // Start is called before the first frame update
    void Start()
    {
        GenerateCharacters();
        LoadTeam();
    }

    public void ScreenChange() => WorldManager.Instance.LoadScene("Base");

    private void GenerateCharacters()
    {
        for(int i = 0; i < ScriptableDataSource.RuntimeDb.Characters.Count; i++)
        {
            int id = ScriptableDataSource.RuntimeDb.Characters[i].ID;
            GameObject obj = Instantiate(_character, _selectTransform);
            obj.GetComponent<Image>().sprite = ScriptableDataSource.Db.CharacterDB[id].Sprite;
            obj.GetComponent<Button>().onClick.AddListener(() => SetFormationSlot(id));
        }
    }

    public void LoadTeam(int teamNo = 0)
    {
        if(_team != teamNo)
        {
            _team = teamNo;
            for(int i = 0; i < 4; i++)
                SetFormationIndividual(i);
        }
    }

    private void SetFormationIndividual(int slot)
    {
        int index = ScriptableDataSource.RuntimeDb.Formations[_team][slot];
        _members[slot].sprite = index >= 0 ? ScriptableDataSource.Db.CharacterDB[index].Sprite : _empty;
    }

    public void SelectFormationSlot(int slot)
    {
        _selected = slot;
        _selectedMember.sprite = _members[slot].sprite;
    }

    public void RemoveFromFormation()
    {
        if (ScriptableDataSource.RuntimeDb.Formations[_team][_selected] != -1)
        {
            ScriptableDataSource.RuntimeDb.Formations[_team][_selected] = -1;
            _members[_selected].sprite = _empty;
            ScriptableDataSource.SaveGame();
        }
    }

    private void SetFormationSlot(int character)
    {
        int[] team = ScriptableDataSource.RuntimeDb.Formations[_team];
        team[_selected] = character;
        _selectedMember.sprite = ScriptableDataSource.Db.CharacterDB[character].Sprite;

        if (team[0] == character && _selected != 0)
        {
            team[0] = -1;
            ScriptableDataSource.RuntimeDb.Formations[_team] = team;
            SetFormationIndividual(0);
        }
        else if (team[1] == character && _selected != 1)
        {
            team[1] = -1;
            ScriptableDataSource.RuntimeDb.Formations[_team] = team;
            SetFormationIndividual(1);
        }
        else if (team[2] == character && _selected != 2)
        {
            team[2] = -1;
            ScriptableDataSource.RuntimeDb.Formations[_team] = team;
            SetFormationIndividual(2);
        }
        else if (team[3] == character && _selected != 3)
        {
            team[3] = -1;
            ScriptableDataSource.RuntimeDb.Formations[_team] = team;
            SetFormationIndividual(3);
        }
        else
            ScriptableDataSource.RuntimeDb.Formations[_team] = team;

        SetFormationIndividual(_selected);
        ScriptableDataSource.SaveGame();
    }
}
