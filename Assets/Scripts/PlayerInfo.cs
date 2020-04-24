using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI playerInfoText;

    PlayerControls playerControls;

    void Update()
    {
        if(playerControls == null)
        {
            playerInfoText.text = "Zginales";

            return;
        }

        playerInfoText.text = string.Format(
            "Zycie: {0}               At1: {1}s               At2: {2}s               Special: {3}s",
            playerControls.GetHealth(),
            playerControls.GetCooldownTime(0).ToString("0.0"),
            playerControls.GetCooldownTime(1).ToString("0.0"),
            playerControls.GetCooldownTime(2).ToString("0.0"));
    }

    public void SetPlayer(PlayerControls playerControls)
    {
        this.playerControls = playerControls;
    }
}
