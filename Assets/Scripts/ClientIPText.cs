using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;

public class ClientIPText : MonoBehaviour
{
    private Text textComponent;

    private void Awake()
    {
        if (textComponent == null)
            textComponent = GetComponent<Text>();

        textComponent.text = "Client IP: " + GetIP();
    }
    private string GetIP()
    {
        string strHostName = System.Net.Dns.GetHostName();

        print(strHostName);
        IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

        IPAddress[] addr = ipEntry.AddressList;

        return addr[addr.Length - 1].ToString();

    }
}
