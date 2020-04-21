using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Options : MonoBehaviour
{
    public NetworkManager networkManager;
    public GameObject settings;

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown displayDropdown;
    public Button restartButton;
    int currentDisplay;

    public TMP_Dropdown qualityDropdown;

    public Slider volumeSlider;

    public TMP_InputField nickNameInputField;

    void Start()
    {
        nickNameInputField.text = networkManager.GetNickName();

        if(PlayerPrefs.HasKey(NaszaGra.VOLUME_KEY))
        {
            AudioListener.volume = PlayerPrefs.GetFloat(NaszaGra.VOLUME_KEY);
            volumeSlider.value = AudioListener.volume;
        }

        List<string> resolutions = new List<string>();
        foreach(var res in Screen.resolutions)
        {
            float hRatio = (res.height * 16f) / res.width;
            if (hRatio != 9 && hRatio != 10)
            {
                hRatio = (res.height * 683f) / res.width;
                if (hRatio != 384)
                {
                    continue;
                }
            }

            resolutions.Add(res.width + "x" + res.height);
        }
        resolutions.Reverse();
        resolutionDropdown.AddOptions(resolutions);

        string currentResolution = string.Format("{0}x{1}", Screen.width, Screen.height);
        for(int i = 0; i < resolutionDropdown.options.Count; i++)
        {
            if(currentResolution == resolutionDropdown.options[i].text)
            {
                resolutionDropdown.value = i;
                break;
            }
        }

        fullscreenToggle.isOn = Screen.fullScreen;

        List<string> displays = new List<string>();
        for(int i = 0; i < Display.displays.Length; i++)
        {
            displays.Add(i.ToString());
        }
        displayDropdown.AddOptions(displays);

        currentDisplay = PlayerPrefs.GetInt("UnitySelectMonitor");
        displayDropdown.value = currentDisplay;

        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    void ShowRestartButton(bool visible = true)
    {
        restartButton.gameObject.SetActive(visible);
    }

    public void OnNickNameChange()
    {
        if(string.IsNullOrEmpty(nickNameInputField.text))
        {
            return;
        }

        networkManager.SaveNickName(nickNameInputField.text);
        Restart();
    }

    public void OnResolutionChange()
    {
        string[] resolution = resolutionDropdown.captionText.text.Split('x');
        int width, height;
        if(!int.TryParse(resolution[0], out width) ||
           !int.TryParse(resolution[1], out height))
        {
            Debug.LogError("Wrong resolution");
            return;
        }

        FullScreenMode fullscreen = fullscreenToggle.isOn ?
            FullScreenMode.MaximizedWindow : FullScreenMode.Windowed;

        Screen.SetResolution(width, height, fullscreen);
    }

    public void OnDisplayChange()
    {
        if(displayDropdown.value == currentDisplay)
        {
            ShowRestartButton(false);
            return;
        }

        PlayerPrefs.SetInt("UnitySelectMonitor", displayDropdown.value);
        ShowRestartButton();
    }

    public void OnQualityChange()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void OnVolumeChange()
    {
        AudioListener.volume = volumeSlider.value;

        PlayerPrefs.SetFloat(NaszaGra.VOLUME_KEY, volumeSlider.value);
    }

    public void Settings()
    {
        settings.SetActive(!settings.activeSelf);
    }

    public void Restart()
    {
        System.Diagnostics.Process.Start(Application.dataPath + "/../NASZA GRA.exe");
        Application.Quit();
    }
}
