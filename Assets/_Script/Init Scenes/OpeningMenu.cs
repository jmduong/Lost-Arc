using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class OpeningMenu : MonoBehaviour
{
    [SerializeField]
    private Button btn;

    public void LoadData()
    {
        Debug.Log(Application.persistentDataPath);
        btn.interactable = false;
        StartCoroutine(Loading());
    }

    private string AssetBundleFilePath(string assetBundleName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        filePath = Path.Combine(filePath, assetBundleName);
        return filePath;
    }

    // NOTE: When creating new scriptableobjects, DO NOT copy and paste an existing obj and edit. It will not register for some reason.
    IEnumerator Data(string type, string subtype = "")
    {
        var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(AssetBundleFilePath("scriptableobjects/" + type + (subtype != "" ? "/" + subtype : "")));
        yield return assetBundleCreateRequest;
        AssetBundle assetBundle = assetBundleCreateRequest.assetBundle;

        AssetBundleRequest asset = type switch
        {
            "abilities" => assetBundle.LoadAllAssetsAsync<Ability_Scriptable>(),
            "bursts" => assetBundle.LoadAllAssetsAsync<Burst_Scriptable>(),
            "characters" => assetBundle.LoadAllAssetsAsync<Character_Scriptable>(),
            "equipment" => assetBundle.LoadAllAssetsAsync<Equipment_Scriptable>(),
            "materials" => assetBundle.LoadAllAssetsAsync<Materials_Scriptable>(),
            "portraits" => assetBundle.LoadAllAssetsAsync<Portrait_Scriptable>(),
            "quests" => assetBundle.LoadAllAssetsAsync<Quest_Scriptable>(),
            "senses" => assetBundle.LoadAllAssetsAsync<Sense_Scriptable>(),
            "shop" => assetBundle.LoadAllAssetsAsync<Shop_Scriptable>(),
            "skills" => assetBundle.LoadAllAssetsAsync<Skill_Scriptable>(),
            "soulfragments" => assetBundle.LoadAllAssetsAsync<SoulFragment_Scriptable>(),
            "stories" => assetBundle.LoadAllAssetsAsync<Story_Scriptable>(),
            "substories" => assetBundle.LoadAllAssetsAsync<StoriesSub_Scriptable>()
        };
        yield return asset;

        Object[] loadedAssetObjs = asset.allAssets;

        Debug.Log(type + subtype + ": " + loadedAssetObjs.Length);
        switch (type)
        {
            case "abilities":
                Dictionary<int, Ability_Scriptable> abilityValue = new Dictionary<int, Ability_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Ability_Scriptable ability = loadedAssetObjs[i] as Ability_Scriptable;
                    abilityValue.Add(ability.ID, ability);
                }
                ScriptableDataSource.Db.AbilityDB = abilityValue;
                break;
            case "bursts":
                Dictionary<int, Burst_Scriptable> burstValue = new Dictionary<int, Burst_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Burst_Scriptable burst = loadedAssetObjs[i] as Burst_Scriptable;
                    burstValue.Add(burst.ID, burst);
                }
                ScriptableDataSource.Db.BurstDB = burstValue;
                break;
            case "characters":
                Dictionary<int, Character_Scriptable> characterValue = new Dictionary<int, Character_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Character_Scriptable character = loadedAssetObjs[i] as Character_Scriptable;
                    characterValue.Add(character.ID, character);
                }
                ScriptableDataSource.Db.CharacterDB = characterValue;
                break;
            case "equipment":
                Dictionary<int, Equipment_Scriptable> equipmentValue = new Dictionary<int, Equipment_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Equipment_Scriptable equipment = loadedAssetObjs[i] as Equipment_Scriptable;
                    equipmentValue.Add(equipment.ID, equipment);
                }
                ScriptableDataSource.Db.ArmorDB[subtype switch { "weapons" => 0, "helmets" => 1, "armor" => 2, _ => 3 }] = equipmentValue;
                break;
            case "materials":
                Dictionary<int, Materials_Scriptable> materialValue = new Dictionary<int, Materials_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Materials_Scriptable material = loadedAssetObjs[i] as Materials_Scriptable;
                    materialValue.Add(material.ID, material);
                }
                ScriptableDataSource.Db.MaterialsDB[subtype switch { "consumables" => 0, "materials" => 1, "events" => 2, "treasures" => 3, _ => 4 }] = materialValue;
                break;
            case "portraits":
                Dictionary<string, Portrait_Scriptable> portraitValue = new Dictionary<string, Portrait_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Portrait_Scriptable portrait = loadedAssetObjs[i] as Portrait_Scriptable;
                    portraitValue.Add(portrait.Name, portrait);
                }
                ScriptableDataSource.Db.PortraitDB = portraitValue;
                break;
            case "quests":
                Dictionary<int, Quest_Scriptable> questValue = new Dictionary<int, Quest_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                    questValue.Add(i, loadedAssetObjs[i] as Quest_Scriptable);
                ScriptableDataSource.Db.QuestDB[type switch { "daily" => 0, "weekly" => 1, "totalachievements" => 2, _ => 3 }] = questValue;
                break;
            case "senses":
                Dictionary<int, Sense_Scriptable> senseValue = new Dictionary<int, Sense_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Sense_Scriptable sense = loadedAssetObjs[i] as Sense_Scriptable;
                    senseValue.Add(sense.ID, sense);
                }
                ScriptableDataSource.Db.SenseDB = senseValue;
                break;
            case "shop":
                Dictionary<int, Shop_Scriptable> shopValue = new Dictionary<int, Shop_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Shop_Scriptable scriptable = loadedAssetObjs[i] as Shop_Scriptable;
                    shopValue.Add(scriptable.Currency, scriptable);
                }
                ScriptableDataSource.Db.ShopDB = shopValue;
                break;
            case "skills":
                Dictionary<int, Skill_Scriptable> skillValue = new Dictionary<int, Skill_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Skill_Scriptable skill = loadedAssetObjs[i] as Skill_Scriptable;
                    skillValue.Add(skill.ID, skill);
                }
                ScriptableDataSource.Db.SkillDB = skillValue;
                break;
            case "soulfragments":
                Dictionary<int, SoulFragment_Scriptable> soulValue = new Dictionary<int, SoulFragment_Scriptable>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    SoulFragment_Scriptable soul = loadedAssetObjs[i] as SoulFragment_Scriptable;
                    soulValue.Add(soul.ID, soul);
                }
                ScriptableDataSource.Db.SoulFragDB = soulValue;
                break;
            case "stories":
                Dictionary<int, List<Story_Scriptable>> storyValue = new Dictionary<int, List<Story_Scriptable>>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    Story_Scriptable story = loadedAssetObjs[i] as Story_Scriptable;
                    if (!storyValue.ContainsKey(story.ChapterID))
                        storyValue.Add(story.ChapterID, new List<Story_Scriptable>() { story });
                    else
                        storyValue[story.ChapterID].Add(story);
                }
                ScriptableDataSource.Db.StoryDB = storyValue;
                break;
            case "substories":
                Dictionary<string, List<StoriesSub_Scriptable>> subStoryValue = new Dictionary<string, List<StoriesSub_Scriptable>>();
                for (int i = 0; i < loadedAssetObjs.Length; i++)
                {
                    StoriesSub_Scriptable substory = loadedAssetObjs[i] as StoriesSub_Scriptable;
                    string code = $"{ substory.ChapterID }_{ substory.Part }";
                    if (!subStoryValue.ContainsKey(code))
                        subStoryValue.Add(code, new List<StoriesSub_Scriptable>() { substory });
                    else
                        subStoryValue[code].Add(substory);
                }
                ScriptableDataSource.Db.StoriesSubDB = subStoryValue;
                break;
            default:
                Debug.LogError("Data Type not matching.");
                break;
        }
    }
        
    public void InitAcct()
    {
        ScriptableDataSource.AddCharacter(0);
        ScriptableDataSource.AddEquipmentInstance(0, 0);
        ScriptableDataSource.AddSoulFrag(0);
        ScriptableDataSource.RuntimeDb.Inventory.MatCount[0].Add(0, 1);
        ScriptableDataSource.RuntimeDb.Inventory.MatCount[1].Add(0, 1);
        ScriptableDataSource.RuntimeDb.Inventory.KeyItems.Add(0);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Characters", 0, "Testing Add Characters");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Weapons", 0, "Testing Add Weapons");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Helmets", 0, "Testing Add Helmets");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Armor", 0, "Testing Add Armor");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Accessories", 0, "Testing Add Accessories");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("SoulFrags", 0, "Testing Add SoulFrags");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Currency_1", 0, "Testing Add Currency1", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Currency_2", 0, "Testing Add Currency2", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Currency_3", 0, "Testing Add Currency3", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Currency_4", 0, "Testing Add Currency4", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Currency_5", 0, "Testing Add Currency5", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Consumable", 0, "Testing Add Consumable", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Consumable", 1, "Testing Add Consumable", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Consumable", 2, "Testing Add Consumable", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Consumable", 3, "Testing Add Consumable", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Consumable", 4, "Testing Add Consumable", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Consumable", 7, "Testing Add Consumable", 500);
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Material", 0, "Testing Add Material");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("SkillBook", 3, "Testing Add Material");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("Event", 0, "Testing Add Event");
        ScriptableDataSource.RuntimeDb.Inbox.AddInboxItem("SkillBook", 0, "Testing Add Skill");

        Debug.Log("There is no save data! Created File.");
        ScriptableDataSource.SaveGame();
    }

    IEnumerator Loading()
    {
        yield return StartCoroutine(Data("abilities"));
        yield return StartCoroutine(Data("bursts"));
        yield return StartCoroutine(Data("characters"));
        yield return StartCoroutine(Data("equipment", "accessories"));
        yield return StartCoroutine(Data("equipment", "armor"));
        yield return StartCoroutine(Data("equipment", "helmets"));
        yield return StartCoroutine(Data("equipment", "weapons"));
        yield return StartCoroutine(Data("materials", "consumables"));
        yield return StartCoroutine(Data("materials", "materials"));
        yield return StartCoroutine(Data("materials", "events"));
        yield return StartCoroutine(Data("materials", "keys"));
        yield return StartCoroutine(Data("portraits"));
        yield return StartCoroutine(Data("quests", "daily"));
        yield return StartCoroutine(Data("quests", "special"));
        yield return StartCoroutine(Data("quests", "totalachievements"));
        yield return StartCoroutine(Data("quests", "weekly"));
        yield return StartCoroutine(Data("senses"));
        yield return StartCoroutine(Data("shop"));
        yield return StartCoroutine(Data("skills"));
        yield return StartCoroutine(Data("soulfragments"));
        yield return StartCoroutine(Data("stories"));
        yield return StartCoroutine(Data("substories"));

        if (File.Exists(Application.persistentDataPath + "/Data.dat"))
            ScriptableDataSource.LoadGame();
        else
            InitAcct();
        ScriptableDataSource.LoadSettings();
        WorldManager.Instance.LoadScene("Base");
    }
}
