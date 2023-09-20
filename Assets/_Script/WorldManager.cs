 using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance;
    
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else    Destroy(gameObject);
    }

    public void DebugMessage(string str) => UnityEngine.Debug.Log(str);
    public void LoadScene(string sceneName) =>  SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    public void LoadScene(int index) => SceneManager.LoadScene(index, LoadSceneMode.Single);
    public void OpenURL(string url) => Application.OpenURL(url);
    public void ToggleInstance(GameObject obj) => obj.SetActive(!obj.activeSelf);
}