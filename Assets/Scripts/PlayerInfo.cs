using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI playerInfoText;

    Player player;

    void Update()
    {
        if(player == null)
        {
            return;
        }

        float attack1 = -(Time.time - player.attack1TimeStamp - player.attack1Cooldown);
        if(attack1 <= 0)
        {
            attack1 = 0;
        }
        attack1 = Mathf.Round(attack1 * 10f) / 10f;

        float attack2 = -(Time.time - player.attack2TimeStamp - player.attack2Cooldown);
        if (attack2 <= 0)
        {
            attack2 = 0;
        }
        attack2 = Mathf.Round(attack2 * 10f) / 10f;

        float attack3 = -(Time.time - player.attack3TimeStamp - player.attack3Cooldown);
        if (attack3 <= 0)
        {
            attack3 = 0;
        }
        attack3 = Mathf.Round(attack3 * 10f) / 10f;

        playerInfoText.text = string.Format(
            "Zycie: {0}               At1: {1}s               At2: {2}s               Special: {3}s",
            player.health,
            attack1.ToString("0.0"),
            attack2.ToString("0.0"),
            attack3.ToString("0.0"));
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
