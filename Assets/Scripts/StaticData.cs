using UnityEngine;

public class StaticData {
    public static int playerId {
        get{
            return PlayerPrefs.GetInt("playerId", 0);
        }
        set{
            PlayerPrefs.SetInt("playerId", value);
        }
    }

    public static string serverIp {
        get{
            return PlayerPrefs.GetString("serverIp", "192.168.0.3");
        }
        set{
            PlayerPrefs.SetString("serverIp", value);
        }
    }

    public static string serverPort {
        get{
            return PlayerPrefs.GetString("serverPort","1996");
        }
        set{
            PlayerPrefs.SetString("serverPort", value);
        }
    }
}