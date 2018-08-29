using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using UnityEngine.XR;

public class GameData
{
    public enum GameState : int
    {
        READY = 0,
        LAUNCHING = 1,
        PLAYING_NO_WINNER = 2,
        FIRST_FINISHED = 3,
        ALL_FINISHED = 4,
    };

    public static GameState gameState = GameState.READY;
    public static PlayerData[] players = new PlayerData[3] { new PlayerData(), new PlayerData(), new PlayerData() };

}

public class PlayerData
{
    public enum PlayerState : int
    {
        READY = 0,
        RIDING = 1,
        FINISHED = 2,
    }

    public PlayerState playerState = PlayerState.READY;
    public float zPosition = 0;
    public float zVelocity = 0;
    public Quaternion headsetRot = Quaternion.identity;
}


public class TCPClient : MonoBehaviour
{
    //public Transform camParentTrans;
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;

    public bool isMonitor = false;
    bool a = false;

    void Start()
    {
        Cursor.visible = false;
        setupSocket(StaticData.serverIp, int.Parse(StaticData.serverPort));
    }

    float counter = 0;
    void Update()
    {
        if (!a)
        {
            a = true;
            writeSocket("'tagClient', ");
        }

        counter += Time.deltaTime;
        if (counter >= 0.1f && !isMonitor)
        {
#if UNITY_EDITOR
            Quaternion headRot = FindObjectOfType<VRMouseLook>().transform.localRotation;
#else
            Quaternion headRot = InputTracking.GetLocalRotation(XRNode.Head);
#endif
            float x = headRot.x;
            float y = headRot.y;
            float z = headRot.z;
            float w = headRot.w;

            writeSocket("'setHeadset'," + x.ToString("F") + "," + y.ToString("F") + "," + z.ToString("F") + "," + w.ToString("F") + "," + StaticData.playerId);
        }

        if (Input.GetKeyDown("f"))
        {
            print("set frequency");
            writeSocket("'setFrequency', 5.0, 0");
            writeSocket("'setFrequency', 5.0, 1");
            writeSocket("'setFrequency', 5.0, 2");
        }

        if(Input.GetKeyDown("s"))
        {
            print("start game");
            writeSocket("'start',");
        }
        if(Input.GetKeyDown("r"))
        {
            print("reset game");
            writeSocket("'reset',");
        }
        
        while(theStream.DataAvailable)
        {
            string receivedString = readSocket();
            string[] listA = receivedString.Split(new char[] { '|' });
            GameData.gameState = (GameData.GameState)int.Parse(listA[0]);

            for (int i = 1; i < listA.Length; i++)
            {
                string playerString = listA[i];
                PlayerData playerData = GameData.players[i - 1];
                string[] dataString = playerString.Split(new char[] { ',' });

                playerData.playerState = (PlayerData.PlayerState)int.Parse(dataString[0]);
                playerData.zPosition = Mathf.Clamp(float.Parse(dataString[1]),0,1);
                playerData.zVelocity = float.Parse(dataString[2]);
                playerData.headsetRot = new Quaternion(
                    float.Parse(dataString[3]),
                    float.Parse(dataString[4]),
                    float.Parse(dataString[5]),
                    float.Parse(dataString[6])
                );
            }
        }
        //print(readSocket());
        //writeSocket("'getPosition'," + StaticData.playerId.ToString());
    }

    // **********************************************
    public void setupSocket(string Host, int Port)
    {
        try
        {
            print("try connect " + Host + " : " + Port);
            mySocket = new TcpClient(Host, Port);
            theStream = mySocket.GetStream();
            theWriter = new StreamWriter(theStream);
            theReader = new StreamReader(theStream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e);
        }
    }

    public void writeSocket(string theLine)
    {
        if (!socketReady)
            return;
        String foo = theLine + "\r\n";
        theWriter.Write(foo);
        theWriter.Flush();
    }

    public String readSocket()
    {
        if (!socketReady)
            return "";
        if (theStream.DataAvailable)
            return theReader.ReadLine();
        return "";
    }

    public void closeSocket()
    {
        if (!socketReady)
            return;
        theWriter.Close();
        theReader.Close();
        mySocket.Close();
        socketReady = false;
    }

    public void OnDestroy()
    {
        closeSocket();
    }

    public void OnApplicationQuit()
    {
        closeSocket();
    }
}