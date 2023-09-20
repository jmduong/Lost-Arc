using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableData;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using System.Linq;
using UnityEditor;

public static class ScriptableDataSource
{
    // Generic Data. Loaded in the beginning.
    private static ScriptableStaticData db = new ScriptableStaticData();
    public static ScriptableStaticData Db
    {
        get { return db; }
        set { db = value; }
    }

    // User Data. Runtime.
    private static ScriptableData_Runtime runtimeDb = new ScriptableData_Runtime();
    public static ScriptableData_Runtime RuntimeDb
    {
        get { return runtimeDb; }
        set { runtimeDb = value; }
    }

    // User Data. Settings.
    private static Settings settingsDb = new Settings();
    public static Settings SettingsDb
    {
        get { return settingsDb; }
        set { settingsDb = value; }
    }

    public static void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Data.dat");

        bf.Serialize(file, runtimeDb);
        file.Close();
        UnityEngine.Debug.Log("Game data saved!");
    }
    
    public static void LoadGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/Data.dat", FileMode.Open);
        ScriptableData_Runtime data = bf.Deserialize(file) as ScriptableData_Runtime;

        runtimeDb = data;
        file.Close();

        Reset();
        UnityEngine.Debug.Log("Game data loaded!");
    }

    public static void SaveSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Settings.dat");

        bf.Serialize(file, settingsDb);
        file.Close();
        UnityEngine.Debug.Log("Settings data saved!");
    }

    public static void LoadSettings()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/Settings.dat", FileMode.Open);
        Settings data = bf.Deserialize(file) as Settings;

        settingsDb = data;
        file.Close();
        UnityEngine.Debug.Log("Settings loaded!");
    }

    public static void AddCharacter(int ID)
    {
        if(runtimeDb.Characters.ContainsKey(ID))
        {
            // Add extra instance.
            runtimeDb.Characters[ID].Summons++;
            runtimeDb.Characters[ID].Points += (db.CharacterDB[ID].Rarity + 1) * 100;
        }
        else    // Add first time.
            runtimeDb.Characters.Add(ID, AddCharacterInstance(ID));
        SaveGame();
    }

    public static CharacterInstance AddCharacterInstance(int ID)
    {    
        Character_Scriptable character = db.CharacterDB[ID];
        CharacterInstance instance = new CharacterInstance();
        instance.ID = ID;
        instance.Rarity = character.Rarity;
        for (int i = 0; i < 3; i++)
        {
            instance.Senses.Add(character.SenseID[i]);
            instance.SensesEquipt[i] = character.SenseID[i];
            instance.Abilities[i, 0] = character.Abilities[i];
            instance.Abilities[i, 1] = 0;
        }

        return instance;
    }

    public static void AddEquipmentInstance(int type, int ID)
    {
        string identifier = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
        EquipmentInstance instance = new EquipmentInstance();
        instance.ID = ID;
        instance.Locked = settingsDb.AutoLock[db.ArmorDB[type][ID].Rarity];
        while (runtimeDb.Armor[type].ContainsKey(identifier))
            identifier = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Substring(0, 8);
        runtimeDb.Armor[type].Add(identifier, instance);
        SaveGame();
    }

    public static void AddSoulFrag(int ID)
    {
        if (runtimeDb.SoulFrags.ContainsKey(ID))
            runtimeDb.Inventory.Currencies[0] += 500;
        else if(ID >= 0)
            runtimeDb.SoulFrags.Add(ID, -1);
        SaveGame();
    }

    public static void AddCurrency(int type, int amount)
    {
        runtimeDb.Inventory.Currencies[type] += amount;
        SaveGame();
    }

    public static void AddReward(int reward, int type, int amount = 1, int subtype = 0, int id = 0)
    {
        switch(reward)
        {
            case 0:     // Character
                AddCharacterInstance(id);
                break;
            case 1:     // Equipment
                AddEquipmentInstance(type, id);
                break;
            default:    // Currency
                AddCurrency(type, amount);
                break;
        }
        SaveGame();
    }

    public static void RefreshQuests()
    {
        for(int i = 0; i < 4; i++)
            for (int j = 0; j < db.QuestDB[i].Count; j++)
                if (!runtimeDb.QuestProgress[i].ContainsKey(j))
                    runtimeDb.QuestProgress[i].Add(j, (0, false));
        SaveGame();
    }

    public static void Reset()
    {
        // Daily Check.
        if (DateTime.UtcNow.Date > runtimeDb.LastLogin)
        {
            UnityEngine.Debug.Log("Date check");
            for (int i = 0; i < runtimeDb.QuestProgress[0].Count; i++)
                runtimeDb.QuestProgress[0][i] = (0, false);

            // Reset Weekly/Monthly Checks.
            runtimeDb.MonthlyReset = false;
            runtimeDb.WeeklyReset = false;
        }

        // Weekly Check.
        if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday && !runtimeDb.WeeklyReset)
        {
            for (int i = 0; i < runtimeDb.QuestProgress[1].Count; i++)
                runtimeDb.QuestProgress[1][i] = (0, false);
            runtimeDb.WeeklyReset = true;
        }

        // Monthly Check.
        if (DateTime.UtcNow.Day == 1 && !runtimeDb.MonthlyReset)
        {
            for (int i = 0; i < db.ShopDB.Count; i++)       // Reset Shop Counts.
                runtimeDb.SellRemaining[i] = db.ShopDB[i].Count;
            runtimeDb.MonthlyReset = true;
        }

        runtimeDb.LastLogin = DateTime.UtcNow;
    }
}
