using UnityEngine;

public class StaticData
{
    public static int playerId
    {
        get
        {
            return PlayerPrefs.GetInt("playerId", 0);
        }
        set
        {
            PlayerPrefs.SetInt("playerId", value);
        }
    }

    public static string serverIp
    {
        get
        {
#if UNITY_EDITOR
            return "localhost";
#else
            return PlayerPrefs.GetString("serverIp", "192.168.0.3");
#endif
        }
        set
        {
            PlayerPrefs.SetString("serverIp", value);
        }
    }

    public static string serverPort
    {
        get
        {
#if UNITY_EDITOR
            return "1996";
#else
            return PlayerPrefs.GetString("serverPort","1996");
#endif
        }
        set{
            PlayerPrefs.SetString("serverPort", value);
        }
    }
}