using UnityEngine;
using UnityEngine.UI;

public class PortInputField : MonoBehaviour
{
    private InputFieldCaretPosition inputComp;

    private void Awake()
    {
        inputComp = GetComponent<InputFieldCaretPosition>();
        inputComp.text = StaticData.serverPort;
    }

    private void Update()
    {
        StaticData.serverPort = inputComp.text;
    }
}
