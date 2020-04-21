using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI onlineStatus;

    float timer = 0f;

    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = NaszaGra.GAME_VERSION;
            PhotonNetwork.NickName = GetNickName();
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Update()
    {
        if(timer > 2f)
        {
            if(onlineStatus.text.IndexOf("<color=\"green\">Connected</color>") == 0)
            {
                onlineStatus.text = "<color=\"green\">Connected</color>\n";
                onlineStatus.text += "Ping: " + PhotonNetwork.GetPing();
            }
            timer = 0;
        }

        timer += Time.deltaTime;
    }

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        onlineStatus.text = "<color=\"green\">Connected</color>";
    }

    public string GetNickName()
    {
#if UNITY_EDITOR
        return "UnityEditor";
#else
        // Get nick name from command line arguments
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length >= 3)
        {
            if (args[1] == "-n" && !string.IsNullOrEmpty(args[2]))
            {
                return args[2];
            }
        }

        // Get nick name from player prefs
        if(!PlayerPrefs.HasKey(NaszaGra.NICK_NAME_KEY))
        {
            if(string.IsNullOrEmpty(Environment.UserName))
            {
                return NaszaGra.DEFAULT_NICK_NAME;
            }

            PlayerPrefs.SetString(NaszaGra.NICK_NAME_KEY, Environment.UserName);

            return Environment.UserName;
        }

        return PlayerPrefs.GetString(NaszaGra.NICK_NAME_KEY);
#endif
    }

    public void SaveNickName(string newNickName)
    {
        if(string.IsNullOrEmpty(newNickName))
        {
            return;
        }

        PlayerPrefs.SetString("NickName", newNickName);
    }
}
