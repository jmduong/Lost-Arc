using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ScriptableData
{
    [Serializable]
    public class ScriptableStaticData
    {
        public Dictionary<int, Ability_Scriptable> AbilityDB = new Dictionary<int, Ability_Scriptable>();
        public Dictionary<int, Character_Scriptable> CharacterDB = new Dictionary<int, Character_Scriptable>();
        public Dictionary<int, Equipment_Scriptable>[] ArmorDB = new Dictionary<int, Equipment_Scriptable>[4]
        {
            new Dictionary<int, Equipment_Scriptable>(),
            new Dictionary<int, Equipment_Scriptable>(),
            new Dictionary<int, Equipment_Scriptable>(),
            new Dictionary<int, Equipment_Scriptable>()
        };
        public Dictionary<int, Materials_Scriptable>[] MaterialsDB = new Dictionary<int, Materials_Scriptable>[5]
        { 
            new Dictionary<int, Materials_Scriptable>(),    // Consumables
            new Dictionary<int, Materials_Scriptable>(),    // Materials
            new Dictionary<int, Materials_Scriptable>(),    // Events
            new Dictionary<int, Materials_Scriptable>(),    // Key Items
            new Dictionary<int, Materials_Scriptable>()     // Treasures
        };
        public Dictionary<int, Shop_Scriptable> ShopDB = new Dictionary<int, Shop_Scriptable>();
        public Dictionary<int, Sense_Scriptable> SenseDB = new Dictionary<int, Sense_Scriptable>();
        public Dictionary<int, Skill_Scriptable> SkillDB = new Dictionary<int, Skill_Scriptable>();
        public Dictionary<int, Burst_Scriptable> BurstDB = new Dictionary<int, Burst_Scriptable>();
        public Dictionary<int, SoulFragment_Scriptable> SoulFragDB = new Dictionary<int, SoulFragment_Scriptable>();
        public Dictionary<int, Quest_Scriptable>[] QuestDB = new Dictionary<int, Quest_Scriptable>[4]
        {
            new Dictionary<int, Quest_Scriptable>(),    // Daily
            new Dictionary<int, Quest_Scriptable>(),    // Weekly
            new Dictionary<int, Quest_Scriptable>(),    // Total
            new Dictionary<int, Quest_Scriptable>()     // Special
        };
        public Dictionary<int, List<Story_Scriptable>> StoryDB = new Dictionary<int, List<Story_Scriptable>>();
        public Dictionary<string, List<StoriesSub_Scriptable>> StoriesSubDB = new Dictionary<string, List<StoriesSub_Scriptable>>();
        public Dictionary<string, Portrait_Scriptable> PortraitDB = new Dictionary<string, Portrait_Scriptable>();
    }

    [Serializable]
    public class ScriptableData_Runtime
    {
        public Dictionary<int, CharacterInstance> Characters = new Dictionary<int, CharacterInstance>();
        public Dictionary<string, EquipmentInstance>[] Armor = new Dictionary<string, EquipmentInstance>[4]
        {
            new Dictionary<string, EquipmentInstance>(),
            new Dictionary<string, EquipmentInstance>(),
            new Dictionary<string, EquipmentInstance>(),
            new Dictionary<string, EquipmentInstance>()
        };
        public Dictionary<int, int> BannerSummonsCumulative = new Dictionary<int, int>(),
                                    SoulFrags = new Dictionary<int, int>();   // SF ID, equipted by.
        public Dictionary<int, int[]> Formations = new Dictionary<int, int[]>()
        {
            { 0, new int[4] { 0, -1, -1, -1 } }, { 1, new int[4] { -1, -1, -1, -1 } }, { 2, new int[4] { -1, -1, -1, -1 } }, { 3, new int[4] { -1, -1, -1, -1 } }, { 4, new int[4] { -1, -1, -1, -1 } }, { 5, new int[4] { -1, -1, -1, -1 } }, { 6, new int[4] { -1, -1, -1, -1 } }, { 7, new int[4] { -1, -1, -1, -1 } }, { 8, new int[4] { -1, -1, -1, -1 } }, { 9, new int[4] { -1, -1, -1, -1 } }
        };
        public Dictionary<int, (int, bool)>[] QuestProgress = new Dictionary<int, (int, bool)>[4]
        {
            new Dictionary<int, (int, bool)>(), // Daily
            new Dictionary<int, (int, bool)>(), // Weekly
            new Dictionary<int, (int, bool)>(), // Total
            new Dictionary<int, (int, bool)>()  // Special
        };
        public List<int>[] SpellBooks = new List<int>[4];
        public List<string> ParallelBonds = new List<string>(), ParallelBondsPending = new List<string>(); // Friends.
        public Inventory Inventory = new Inventory();
        public Dictionary<int, List<int>> SellRemaining = new Dictionary<int, List<int>>(); // 5 different currencies
        public Inbox Inbox = new Inbox();

        public bool WeeklyReset = false, MonthlyReset = false;  // Check if resets have been done on Sunday / 1st day of Month.
        public DateTime LastLogin;
        public int ProfileID, Level, EXP_Remain = 100, EXP_Cum, LoginStreak, LongestLoginStreak, TotalLogins, Representative, DailyBoosters = 3;
        public string UserID, PlayerName, ParallelCode, Message;
    }

    [Serializable]
    public class Settings
    {
        // Initial Acct Settings.
        public int ScreenRes = 1;   // 0. Low, 1. Std, 2. High
        public bool[] Toggles = new bool[6] 
        {
            false,  // Data Sharing Permissions
            false,  // Mute
            true,   // Costume Supt
            true,   // Subtitles
            true,   // Push Notifications
            true    // Right-Hand
        };
        public int[] SliderValues = new int[4]
        {
            1,  // TextSpeed
            7,  // BGM
            7,  // SFX
            7   // Voice
        };
        public string Country, TimeZone;
        public bool[] AutoLock = new bool[6] { false, false, false, false, false, true };
    }

    [Serializable]
    public class CharacterInstance
    {
        public CharacterInstance() { }

        public int ID, Level, Merges, Rarity, EXP_Remain = 100, EXP_Cum, Summons = 1, Points, StoryLine;
        public List<int> Senses = new List<int>();  // ID
        public int[] SensesEquipt = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 }, SoulFragments = new int[9] { -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        public int[,] Abilities = new int[3, 2];
        // ArmorEquipt will use unique instance IDs such as 864fdwe. When equipment is removed, check for ID and if not present, change to null.
        public string[] ArmorEquipt = new string[5];
    }

    [Serializable]
    public class Inventory
    {
        // Keep track on non-unique item counts.
        public Inventory() { }

        public int[] Currencies = new int[5];

        public Dictionary<int, int>[] MatCount = new Dictionary<int, int>[5]
        {
            new Dictionary<int, int>(), // Consumables
            new Dictionary<int, int>(), // Materials
            new Dictionary<int, int>(), // Events
            new Dictionary<int, int>(), // Treasures
            new Dictionary<int, int>()  // Skills
        };
        public List<int> KeyItems = new List<int>();

        public void AddMaterial(int type, int ID, int amount)
        {
            if (MatCount[type].ContainsKey(ID)) MatCount[type][ID] += amount;
            else MatCount[type].Add(ID, amount);
            ScriptableDataSource.SaveGame();
        }

        public void AddKeyItem(int ID)
        {
            if (!KeyItems.Contains(ID))
            {
                KeyItems.Add(ID);
                KeyItems.Sort();
            }
            ScriptableDataSource.SaveGame();
        }
    }

    [Serializable]
    public class Inbox
    {
        public Inbox() { }

        public List<(string, int, int, DateTime, string)> lst = new List<(string, int, int, DateTime, string)>();

        public void AddInboxItem(string type, int ID, string message = "", int amount = 1)
        {
            lst.Add((type, ID, amount, DateTime.Today.AddYears(1), message));
        }
    }

    [Serializable]
    public class EquipmentInstance
    {
        public EquipmentInstance() { }

        public bool Locked = false;
        public int ID, CharacterID = -1, Level, EXP_Remain = 100, EXP_Cum, Rank;
        public Dictionary<int, int> SubStats = new Dictionary<int, int>();  // Substat, Value.
    }
}
