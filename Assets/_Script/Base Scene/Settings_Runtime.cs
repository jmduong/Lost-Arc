using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ScriptableData;

public class Settings_Runtime : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _screenRes = new GameObject[3], _checkmarks = new GameObject[5];
    [SerializeField]
    private Slider[] _sliders = new Slider[4];
    private static string[] s_yesOrNo = new string[2] { "Yes", "No" };
    [SerializeField]
    private TextMeshProUGUI _confirmHand;
    [SerializeField]
    private TextMeshProUGUI[] _confirmations = new TextMeshProUGUI[5], _sliderText = new TextMeshProUGUI[4];

    [SerializeField]
    private TextMeshProUGUI _country, _timezone;

    // Start is called before the first frame update
    void Start()
    {
        Settings settings = ScriptableDataSource.SettingsDb;
        for(int i = 0; i < 3; i++)
            _screenRes[i].SetActive(settings.ScreenRes == i);

        for (int i = 0; i < 5; i++)
        {
            _checkmarks[i].SetActive(settings.Toggles[i]);
            _confirmations[i].text = settings.Toggles[i] ? s_yesOrNo[0] : s_yesOrNo[1];
        }

        for (int i = 0; i < 4; i++)
        {
            _sliders[i].value = settings.SliderValues[i];
            _sliderText[i].text = settings.SliderValues[i].ToString();
        }

        _confirmHand.text = ScriptableDataSource.SettingsDb.Toggles[5] ? "Right" : "Left";

        //_country.text = settings.Country;
        //_timezone.text = settings.TimeZone;
    }

    public void OnToggleValueChanged(int index)
    {
        _checkmarks[index].SetActive(!_checkmarks[index].activeSelf);
        _confirmations[index].text = _checkmarks[index].activeSelf ? s_yesOrNo[0] : s_yesOrNo[1];
        ScriptableDataSource.SettingsDb.Toggles[index] = _checkmarks[index].activeSelf;
        ScriptableDataSource.SaveSettings();
    }

    public void Toggle_Hand()
    {
        ScriptableDataSource.SettingsDb.Toggles[5] = !ScriptableDataSource.SettingsDb.Toggles[5];
        _confirmHand.text = ScriptableDataSource.SettingsDb.Toggles[5] ? "Right" : "Left";
        ScriptableDataSource.SaveSettings();
    }

    public void Sel_screenRes(int index)
    {
        for (int i = 0; i < 3; i++)
            _screenRes[i].SetActive(i == index ? true : false);
        ScriptableDataSource.SettingsDb.ScreenRes = index;
        ScriptableDataSource.SaveSettings();
    }

    public void OnSliderValueChanged(int index)
    {
        _sliderText[index].text = _sliders[index].value.ToString();
        ScriptableDataSource.SettingsDb.SliderValues[index] = (int)_sliders[index].value;
        ScriptableDataSource.SaveSettings();
    }
}
