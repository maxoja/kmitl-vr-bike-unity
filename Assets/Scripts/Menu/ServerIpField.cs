using UnityEngine;
using UnityEngine.UI;

public class ServerIpField : MonoBehaviour {
    private InputFieldCaretPosition inputComp;

    private void Awake()
    {
        inputComp = GetComponent<InputFieldCaretPosition>();
        inputComp.text = StaticData.serverIp;
    }

    private void Update()
    {
        StaticData.serverIp = inputComp.text;
    }
}
