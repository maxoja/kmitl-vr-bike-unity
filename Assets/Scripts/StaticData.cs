using UnityEngine;

public class StaticData {
    public static int playerId {
        get{
            return PlayerPrefs.GetInt("playerId");
        }
        set{
            PlayerPrefs.SetInt("playerId", value);
        }
    }

    public static string serverIp {
        get{
            return PlayerPrefs.GetString("serverIp");
        }
        set{
            PlayerPrefs.SetString("serverIp", value);
        }
    }

    public static string serverPort {
        get{
            return PlayerPrefs.GetString("serverPort");
        }
        set{
            PlayerPrefs.SetString("serverPort", value);
        }
    }
}