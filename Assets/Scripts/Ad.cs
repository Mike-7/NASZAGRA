using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class Ad : MonoBehaviour
{
    public RawImage adImage;
    public TextMeshProUGUI adText;

    string adUrl;

    public struct userAttributes { }
    public struct appAttrubutes { }

    void Awake()
    {
        ConfigManager.FetchCompleted += OnFetchConfigs;
        ConfigManager.FetchConfigs<userAttributes, appAttrubutes>
            (new userAttributes(), new appAttrubutes());
    }

    void OnDestroy()
    {
        ConfigManager.FetchCompleted -= OnFetchConfigs;
    }

    void OnFetchConfigs(ConfigResponse response)
    {
        if(response.status != ConfigRequestStatus.Success)
        {
            return;
        }

        if (ConfigManager.appConfig.HasKey(NaszaGra.AD_URL))
        {
            adUrl = ConfigManager.appConfig.GetString(NaszaGra.AD_URL);
        }

        if (ConfigManager.appConfig.HasKey(NaszaGra.AD_TEXT))
        {
            adText.text = ConfigManager.appConfig.GetString(NaszaGra.AD_TEXT);
        }

        if(ConfigManager.appConfig.HasKey(NaszaGra.AD_IMAGE_URL))
        {
            StartCoroutine(LoadAdImage(ConfigManager.appConfig.GetString(NaszaGra.AD_IMAGE_URL)));
        }
    }

    public void OnClick()
    {
        if(string.IsNullOrEmpty(adUrl))
        {
            return;
        }

        Application.OpenURL(adUrl);
    }

    IEnumerator LoadAdImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if(!request.isNetworkError && !request.isHttpError)
        {
            adImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
    }
}
