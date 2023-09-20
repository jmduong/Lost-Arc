using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;
using System.Runtime.Serialization.Formatters.Binary;

public class Initialization : MonoBehaviour
{
    [SerializeField]
    private GameObject _dataProcessing;
    [SerializeField]
    private GameObject[] _screenRes = new GameObject[3], _checkmarks = new GameObject[5];
    [SerializeField]
    private Slider[] _sliders = new Slider[4];
    private static string[] s_yesOrNo = new string[2] { "Yes", "No" };
    [SerializeField]
    private TextMeshProUGUI _confirmHand;
    [SerializeField]
    private TextMeshProUGUI[] _confirmations = new TextMeshProUGUI[5], _sliderText = new TextMeshProUGUI[4];
    private Settings settings = new Settings();
    
    public void CheckAcct()
    {
        if (File.Exists(Application.persistentDataPath + "/Settings.dat"))
            WorldManager.Instance.LoadScene("Login Menu");
        else
            _dataProcessing.SetActive(true);
    }

    public void Toggle(int index)
    {
        _checkmarks[index].SetActive(!_checkmarks[index].activeSelf);
        _confirmations[index].text = _checkmarks[index].activeSelf ? s_yesOrNo [0] : s_yesOrNo[1];
    }

    public void Sel_dataProcessing(bool confirm)
    {
        _checkmarks[0].SetActive(confirm);
        _confirmations[0].text = confirm ? s_yesOrNo[0] : s_yesOrNo[1];
    }

    public void Toggle_Hand()
    {
        settings.Toggles[5] = !settings.Toggles[5];
        _confirmHand.text = settings.Toggles[5] ? "Right" : "Left";
    }

    public void Sel_screenRes(int index)
    {
        for(int i = 0; i < 3; i++)
            _screenRes[i].SetActive(i == index ? true : false);
    }

    public void OnSliderValueChanged(int index) =>  _sliderText[index].text = _sliders[index].value.ToString();

    public void SaveInitSettings()
    {
        for (int i = 0; i < 3; i++)
        {
            if(_screenRes[i].activeSelf)
            {
                settings.ScreenRes = i;
                break;
            }
        }

        for (int i = 0; i < 5; i++)
            settings.Toggles[i] = _checkmarks[i].activeSelf;

        for(int i = 0; i < 4; i++)
            settings.SliderValues[i] = (int)_sliders[i].value;

        // Initialize the settings file.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Settings.dat");

        bf.Serialize(file, settings);
        file.Close();
        Debug.Log("Settings data saved!");
        WorldManager.Instance.LoadScene("Login Menu");
    }
}
