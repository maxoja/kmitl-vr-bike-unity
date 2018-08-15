using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class TCPClient : MonoBehaviour {

    //setting the address information
    public string IP_ADDRESS;
    public int PORT;

    public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] buffer = new byte[1024];

    //response value from server
    private static String response = String.Empty;

    //connect to server
    void Start()
    {
        ConnectToServer();
    }
    

    private void ConnectToServer()
    {
        Debug.Log("Connecting to Server!!");
        IP_ADDRESS = StaticData.serverIp;
        PORT = int.Parse(StaticData.serverPort);
        clientSocket.BeginConnect(IP_ADDRESS, PORT, new AsyncCallback(OnConnected), clientSocket);
    }

    private void OnConnected(IAsyncResult result)
    {
        clientSocket.EndConnect(result);

        while (true)
        {
            ReceiveLoop();
        }
    }

    //recieve the data from server (Asynchronous)
    public void ReceiveLoop()
    {
        try
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.currentSocket = clientSocket;
            int offset = 0;
            SocketFlags flag = 0;
            // Begin asynchronous receiving
            clientSocket.BeginReceive(
                buffer.buffer, 
                offset, 
                PacketBuffer.BufferSize, 
                flag,
                new AsyncCallback(OnReceiveData), buffer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private void OnReceiveData(IAsyncResult ar)
    {
            PacketBuffer buffer = (PacketBuffer)ar.AsyncState;
            Socket client = buffer.currentSocket;
            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                buffer.stringBuilder.Append(Encoding.ASCII.GetString(buffer.buffer, 0, bytesRead));

                client.BeginReceive(buffer.buffer, 0, PacketBuffer.BufferSize, 0,
                    new AsyncCallback(OnReceiveData), buffer);
            }
            else
            {
                if (buffer.stringBuilder.Length > 1)
                {
                    response = buffer.stringBuilder.ToString();
                }
            }
    }

    public void sendString(string s)
    {
        sendData(Encoding.UTF8.GetBytes(s));
    }

    //send data to server
    public void sendData(byte[] data)
    {
        clientSocket.Send(data);
    }
}
