using UnityEngine;
using UnityEngine.UI;

public class PortInputField : MonoBehaviour
{
    private InputField inputComp;

    private void Awake()
    {
        inputComp = GetComponent<InputField>();
        inputComp.text = StaticData.serverPort;
    }

    private void Update()
    {
        StaticData.serverPort = inputComp.text;
    }
}
