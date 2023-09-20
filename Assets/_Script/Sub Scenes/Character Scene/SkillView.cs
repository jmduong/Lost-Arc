using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using ScriptableData;

public class SkillView : MonoBehaviour
{
    [HideInInspector]
    public Character_Scriptable Character;
    [HideInInspector]
    public CharacterInstance Instance;
    [HideInInspector]
    public static SkillView s_SkillView;

    [SerializeField]
    private Button _confirmRefine, _confirmReform;
    [SerializeField]
    private GameObject _options;
    [SerializeField]
    private Image _burst, _viewSelected;
    [SerializeField]
    private Image[] _abilities = new Image[3], _senses = new Image[10], _refineImgs = new Image[3];
    private int _abilityID;
    [SerializeField]
    private Sprite _skillSprite;    // Generic sprite filler for skills.
    [SerializeField]
    private TextMeshProUGUI _viewName, _viewInfo, _newAbilityTxt;
    [SerializeField]
    private TextMeshProUGUI[] _refineCnts = new TextMeshProUGUI[3];

    private void Awake()    =>  s_SkillView = this;

    public void ViewAbilitySet()
    {
        // Set icons for viewing under Skill View.
        for (int i = 0; i < _abilities.Length; i++)
            _abilities[i].sprite = ScriptableDataSource.Db.AbilityDB[Instance.Abilities[i, 0]].Sprite;
        for (int i = 0; i < _senses.Length; i++)
            _senses[i].sprite = Instance.SensesEquipt[i] >= 0 ? ScriptableDataSource.Db.SenseDB[Instance.SensesEquipt[i]].Sprite : _skillSprite;
        _burst.sprite = ScriptableDataSource.Db.BurstDB[Character.Burst].Sprite;
    }

    public void ViewAbility(int index)
    {
        Ability_Scriptable ability = ScriptableDataSource.Db.AbilityDB[Instance.Abilities[index, 0]];

        _viewSelected.sprite = ability.Sprite;
        _viewName.text = String.Format("<b>{0}</b> <size=60%>Lv. {1}</size>", ability.Name, Instance.Abilities[index, 1] + 1);
        _viewInfo.text = "[RP - " + ability.RP + "]\n\n<size=60%>";

        for (int i = 0; i < ability.Stats.Count; i++)
            _viewInfo.text += (ability.Stats[i] > 0 ? "INC " : "DEC ") + (ability.Attributes[i] != global::Stats.Attributes.None ? "[" + ability.Attributes[i] + "] " : "") + ability.Targets[i] + " " + ability.StatTypes[i] + " by " + Mathf.RoundToInt(Mathf.Lerp(ability.Stats[i], ability.mStats[i], Instance.Abilities[index, 1] / 9)) + ". ";

        _viewInfo.gameObject.SetActive(true);
        _options.SetActive(true);
    }

    public void ViewSense(int index)
    {
        if (Instance.SensesEquipt[index] >= 0)
        {
            Sense_Scriptable sense = ScriptableDataSource.Db.SenseDB[Instance.SensesEquipt[index]];

            _viewSelected.sprite = sense.Sprite;
            _viewName.text = sense.Name;
            _viewInfo.text = sense.Description;
        }
        else
        {
            _viewSelected.sprite = _skillSprite;
            _viewName.text = "Name";
            _viewInfo.text = "Sense Info";
        }
        _viewInfo.gameObject.SetActive(true);
        _options.SetActive(false);
    }

    public void ViewBurst()
    {
        Burst_Scriptable burst = ScriptableDataSource.Db.BurstDB[Character.Burst];

        _viewSelected.sprite = burst.Sprite;
        _viewName.text = burst.Name;
        _viewInfo.text = "[BP - " + burst.Cost + "]\n\n<size=60%>";

        string burstDMG = burst.BaseValue > 0 ? "Deal " + (Mathf.RoundToInt(burst.BaseValue * (Mathf.Pow(Instance.Merges / 4, 2) + 1))) + "% BURST DMG to " + burst.Target + (burst.AttributeMod != global::Stats.Attributes.None ? ". Inflict EXTRA " + (Mathf.RoundToInt(10 * (Mathf.Pow(Instance.Merges / 4, 2) + 1))) + "% BURST DMG to [" + burst.AttributeMod + "]. " : ". ") : "";

        string afterBurst = "";
        for (int i = 0; i < burst.StatMods.Count; i++)
        {
            if (burst.ActivationOrders[i] == global::Stats.ActivationOrder.Before)
                _viewInfo.text += (burst.StatMods[i] > 0 ? "INC " : " DEC ") + Mathf.Abs(burst.StatMods[i]) + " " + burst.Stats[i] + (burst.AttributeMods[i] != global::Stats.Attributes.None ? " of [" + burst.AttributeMods[i] + "] " : " of ") + burst.Targets[i] + ". ";
            else
                afterBurst += (burst.StatMods[i] > 0 ? "INC " : " DEC ") + Mathf.Abs(burst.StatMods[i]) + " " + burst.Stats[i] + (burst.AttributeMods[i] != global::Stats.Attributes.None ? " to [" + burst.AttributeMods[i] + "] " : " to ") + burst.Targets[i] + ". ";
        }
        _viewInfo.text += burstDMG + afterBurst;
        _viewInfo.gameObject.SetActive(true);
        _options.SetActive(false);
    }

    public void ViewReset()
    {
        _viewSelected.sprite = _skillSprite;
        _viewName.text = "Name";
        _viewInfo.text = "[RP / EP / BP]";
        _viewInfo.gameObject.SetActive(true);
        _options.SetActive(false);
    }

    private void RefineInfo()
    {
        Ability_Scriptable ability = ScriptableDataSource.Db.AbilityDB[Instance.Abilities[_abilityID, 0]];
        _confirmRefine.interactable = true;
        for (int i = 0; i < 3; i++)
        {
            if (Instance.Abilities[_abilityID, 1] == 8)        // Last refine.
            {
                if (i == 0)
                {
                    _refineImgs[i].gameObject.SetActive(true);
                    _refineImgs[i].sprite = ScriptableDataSource.Db.MaterialsDB[0][0].Sprite;   // Reference Refime Mat max level.
                    _refineCnts[i].text = "1 / " + ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][0];
                    if (ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][0] > 0)
                        _refineCnts[i].color = Color.white;
                    else
                    {
                        _refineCnts[i].color = Color.red;
                        _confirmRefine.interactable = false;
                    }
                }
                else
                    _refineImgs[i].gameObject.SetActive(false);
            }
            else if (Instance.Abilities[_abilityID, 1] == 9)   // Already max refined.
            {
                _refineImgs[i].gameObject.SetActive(false);
                _confirmRefine.interactable = false;
            }
            else
            {
                int matID = ScriptableDataSource.Db.AbilityDB[Instance.Abilities[_abilityID, 0]].RefineReqs[(Instance.Abilities[_abilityID, 1] * 6) + (i * 2)];
                int matAmount = ScriptableDataSource.Db.AbilityDB[Instance.Abilities[_abilityID, 0]].RefineReqs[(Instance.Abilities[_abilityID, 1] * 6) + (i * 2) + 1];
                if (matAmount >= 0)
                {
                    _refineImgs[i].gameObject.SetActive(true);
                    _refineImgs[i].sprite = ScriptableDataSource.Db.MaterialsDB[0][matID].Sprite;
                    _refineCnts[i].text = string.Format("{0} / {1}", matAmount, ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][matID]);
                    if (matAmount <= ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][matID])
                        _refineCnts[i].color = Color.white;
                    else
                    {
                        _refineCnts[i].color = Color.red;
                        _confirmRefine.interactable = false;
                    }
                }
                else
                    _refineImgs[i].gameObject.SetActive(false);
            }
        }
    }

    public void Refine()
    {
        if (Instance.Abilities[_abilityID, 1] == 8)
            ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][0] -= 1;   // Hard code specific material for maxing out refine skill.
        else
        {
            for (int i = 0; i < 3; i++)
            {
                int matID =     ScriptableDataSource.Db.AbilityDB[Instance.Abilities[_abilityID, 0]].RefineReqs[(Instance.Abilities[_abilityID, 1] * 6) + (i * 2)];
                int matAmount = ScriptableDataSource.Db.AbilityDB[Instance.Abilities[_abilityID, 0]].RefineReqs[(Instance.Abilities[_abilityID, 1] * 6) + (i * 2) + 1];
                ScriptableDataSource.RuntimeDb.Inventory.MatCount[0][matID] -= matAmount;
            }
        }
        Instance.Abilities[_abilityID, 1]++;
        ScriptableDataSource.SaveGame();
        RefineInfo();
    }

    private void ReformInfo()
    {
        int reformID = Instance.Abilities[_abilityID, 0];
        if (Character.ReformAbilities.Contains(reformID))
        {
            Ability_Scriptable newAbility = ScriptableDataSource.Db.AbilityDB[Character.TargetAbilities[Character.ReformAbilities.IndexOf(reformID)]];
            _newAbilityTxt.text = $"<B>{ newAbility.Name }</B><size=60%> Lv. 1 [RP { newAbility.RP }]\n";
            if (Instance.Abilities[_abilityID, 1] < 9)
            {
                _newAbilityTxt.text += "\nLevel not maxed out.";
                _confirmReform.interactable = false;
            }
            else
            {
                for (int j = 0; j < newAbility.Stats.Count; j++)
                {
                    _newAbilityTxt.text += (newAbility.Stats[j] > 0 ? "INC " : "DEC ") + (newAbility.Attributes[j] != global::Stats.Attributes.None ? "[" + newAbility.Attributes[j] + "] " : "") + newAbility.Targets[j] + " " + newAbility.StatTypes[j] + " by " + newAbility.Stats[j] + ". ";
                }
                _confirmReform.interactable = true;
            }
        }
        else
        {
            _newAbilityTxt.text = "No Reform Available.";
            _confirmReform.interactable = false;
        }
    }

    public void Reform()
    {
        Instance.Abilities[_abilityID, 0] = Character.TargetAbilities[Character.ReformAbilities.IndexOf(Instance.Abilities[_abilityID, 0])];
        Instance.Abilities[_abilityID, 1] = 0;
        ReformInfo();
    }
}
