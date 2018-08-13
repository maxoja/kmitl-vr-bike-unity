using UnityEngine;
using UnityEngine.UI;

public class ServerIpField : MonoBehaviour {
    private InputField inputComp;

    private void Awake()
    {
        inputComp = GetComponent<InputField>();
        inputComp.text = StaticData.serverIp;
    }

    private void Update()
    {
        StaticData.serverIp = inputComp.text;
    }
}
