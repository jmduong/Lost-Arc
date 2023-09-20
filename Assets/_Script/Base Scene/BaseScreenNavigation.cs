using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseScreenNavigation : MonoBehaviour
{
    /// Subscreens: 
    /// 1. Game Modes   - Story, Bosses, Events, Campaigns (Multi-Player), etc.
    /// 2. Components   - Character, Equipment, Formation, Library / Spell Book
    /// 3. Shop Counter - Shops, Quests, Recruit, Parallel Bonds (Friends), Inbox
    /// 3. Extras       - News, Settings

    public void ScreenChange(string scene)  =>  SceneManager.LoadScene(scene, LoadSceneMode.Additive);

    public void ScreenChange(int index)     =>  SceneManager.LoadScene(index, LoadSceneMode.Additive);
}