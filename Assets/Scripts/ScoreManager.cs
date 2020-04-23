using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Image scoreBar;

    public float lerpSpeed = 5f;

    // Min score: 0
    // Max score: 100
    int score = 50;

    void FixedUpdate()
    {
        var desiredPosition =
            new Vector2(CalculateScoreBarX(score),
                scoreBar.rectTransform.anchoredPosition.y);

        scoreBar.rectTransform.anchoredPosition =
            Vector2.Lerp(scoreBar.rectTransform.anchoredPosition,
                desiredPosition, lerpSpeed * Time.deltaTime);
    }

    float CalculateScoreBarX(int score)
    {
        float scorePercent = score / 100.0f;
        float scoreX = -scoreBar.rectTransform.sizeDelta.x + scorePercent * scoreBar.rectTransform.sizeDelta.x;

        return scoreX;
    }

    public bool CanRespawn(int teamID)
    {
        switch(teamID)
        {
            case 0:
                if(score > 0)
                {
                    return true;
                }
                return false;

            case 1:
                if(score < 100)
                {
                    return true;
                }
                return false;
        }

        return false;
    }

    [PunRPC]
    public void AddPointsToTeam(int teamID, int points)
    {
        switch (teamID)
        {
            // Zabawa team
            case 0:
                score += points;
                break;

            // Nudy team
            case 1:
                score -= points;
                break;
        }

        if(score > 100)
        {
            score = 100;
        }
        else if(score < 0)
        {
            score = 0;
        }
    }
}
