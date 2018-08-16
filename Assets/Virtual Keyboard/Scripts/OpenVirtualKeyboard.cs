using UnityEngine;
using System.Collections;

public class OpenVirtualKeyboard : MonoBehaviour {
	public Canvas VirtualKeyboardObject;
	public GameObject inputObject;

	public void OpenKeyboard() 
	{		
		VirtualKeyboard.Open(VirtualKeyboardObject, inputObject != null ? inputObject : gameObject);
	}

	public void CloseKeyboard() 
	{		
		VirtualKeyboard.Close ();
	}
}