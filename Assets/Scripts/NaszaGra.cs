using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaszaGra
{
    public const string GAME_VERSION = "1";

    public const string NICK_NAME_KEY = "NickName";
    public const string DEFAULT_NICK_NAME = "Gracz";

    public const string VOLUME_KEY = "Volume";

    public const string AD_URL = "AdUrl";
    public const string AD_IMAGE_URL = "AdImageUrl";
    public const string AD_TEXT = "AdText";

    // Player's custom properties (Photon Network)
    public const string CHARACTER_ID = "CharacterID";

    public const string TEAM_ID = "TeamID";

    public static string GetTeamName(int teamID)
    {
        switch(teamID)
        {
            case 0:
                return "Zabawa";

            case 1:
                return "Nudy";
        }

        return null;
    }
}
