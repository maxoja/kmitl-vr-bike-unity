using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;

public class TCPClient : MonoBehaviour
{
    internal Boolean socketReady = false;
    TcpClient mySocket;
    NetworkStream theStream;
    StreamWriter theWriter;
    StreamReader theReader;

    public BezierWalker[] walker;

    bool a = false;

    void Start()
    {
        setupSocket(StaticData.serverIp, int.Parse(StaticData.serverPort));
    }

    void Update()
    {
        if(!a)
        {
            a = true;
            writeSocket("'tagClient', ");
            writeSocket("'setFrequency',5,"+StaticData.playerId.ToString());
        }

        while(theStream.DataAvailable)
        {
            string receivedString = readSocket();
            string[] listA = receivedString.Split(new char[] { '|' });
            for (int i = 0; i < listA.Length; i++)
            {
                string s = listA[i];
                string[] subList = s.Split(new char[] { ',' });
                float position = float.Parse(subList[0]);
                float velocity = float.Parse(subList[1]);

                Debug.Log("player:"+i + " pos:" + position + " velo:" + velocity);
                if (i < walker.Length)
                {
                    walker[i].SetProgress(position);
                    walker[i].SetSpeed(velocity);
                }
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
} // end class s_TCP