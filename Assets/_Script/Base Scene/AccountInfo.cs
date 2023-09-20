using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableData;
using System;
using TMPro;
using UnityEngine.UI;

public class AccountInfo : MonoBehaviour
{
    [SerializeField]
    private Image _profileMain, _profileAcc;
    [SerializeField]
    private Slider _expBarMain, _expBarAcc;
    [SerializeField]
    private TMP_InputField _playerNameAcc, _message;
    [SerializeField]
    private TextMeshProUGUI _playerNameMain, _levelMain, _levelAcc, _exp, _info, _userID;

    void Start()
    {
        _profileMain.sprite = ScriptableDataSource.Db.CharacterDB[ScriptableDataSource.RuntimeDb.ProfileID].Sprite;
        _profileAcc.sprite = _profileMain.sprite;

        _levelMain.text = "Lvl. " + (ScriptableDataSource.RuntimeDb.Level + 1);
        _levelAcc.text = _levelMain.text;

        _expBarMain.maxValue = Stats.EXP_Req[ScriptableDataSource.RuntimeDb.Level];
        _expBarMain.value = _expBarMain.maxValue - ScriptableDataSource.RuntimeDb.EXP_Remain;
        _expBarAcc.maxValue = _expBarMain.maxValue;
        _expBarAcc.value = _expBarMain.value;
        _exp.text = (_expBarMain.maxValue  - ScriptableDataSource.RuntimeDb.EXP_Remain) + " / " + _expBarMain.maxValue;

        _playerNameMain.text = ScriptableDataSource.RuntimeDb.PlayerName;
        _playerNameAcc.text = _playerNameMain.text;
        _message.text = ScriptableDataSource.RuntimeDb.Message;
        _userID.text = "UID: " + ScriptableDataSource.RuntimeDb.UserID;
    }


    public void SetInfo()
    {
        Inventory inventory = ScriptableDataSource.RuntimeDb.Inventory;

        _info.text = string.Format("{0} - {1} - {2}\n{3} / {4}\n\n{5}\n{6}\n{7}\n{8}\n{9}\n\n{10} / 3 Remaining\n{11} Days\n{12} Days\n{13} Days",
                0, 0, 0,
                ScriptableDataSource.RuntimeDb.Characters.Count,
                ScriptableDataSource.Db.CharacterDB.Count,
                ScriptableDataSource.RuntimeDb.Inventory.Currencies[0],
                ScriptableDataSource.RuntimeDb.Inventory.Currencies[1],
                ScriptableDataSource.RuntimeDb.Inventory.Currencies[2],
                ScriptableDataSource.RuntimeDb.Inventory.Currencies[3],
                ScriptableDataSource.RuntimeDb.Inventory.Currencies[4],
                ScriptableDataSource.RuntimeDb.DailyBoosters,
                ScriptableDataSource.RuntimeDb.LongestLoginStreak,
                ScriptableDataSource.RuntimeDb.LoginStreak,
                ScriptableDataSource.RuntimeDb.TotalLogins);
    }

    public void OnMessageChanged()
    {
        ScriptableDataSource.RuntimeDb.Message = _message.text;
        ScriptableDataSource.SaveGame();
    }

    public void OnNameChanged()
    {
        ScriptableDataSource.RuntimeDb.PlayerName = _playerNameAcc.text;
        _playerNameMain.text = _playerNameAcc.text;
        ScriptableDataSource.SaveGame();
    }

    public void OnProfileChanged(int index)
    {
        ScriptableDataSource.RuntimeDb.ProfileID = index;
        _profileMain.sprite = ScriptableDataSource.Db.CharacterDB[index].Sprite;
        _profileAcc.sprite = _profileMain.sprite;
        ScriptableDataSource.SaveGame();
    }
}
