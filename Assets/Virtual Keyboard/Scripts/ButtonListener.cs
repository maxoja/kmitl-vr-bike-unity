using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonListener : MonoBehaviour {
	public Text selectedText;

	public void MyClick (GameObject obj) 
	{
		Text text = obj.GetComponentInChildren<Text>();
		selectedText.text = "You selected " + (text != null ? text.text : obj.name);
		Debug.Log (selectedText.text); // for test purpose

		InputField[] ins = GameObject.FindObjectsOfType<InputField>();
		foreach (InputField i in ins) {
			Debug.Log ("in: " + i.name);  // for test purpose
			if (i.isFocused) {
				i.ActivateInputField();
				i.Select();
				i.MoveTextEnd(false);
				i.ProcessEvent(Event.KeyboardEvent("C"));
			}
		}
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.A)) {
			GameObject go = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
			if (go != null) {
				InputField inputField = go.GetComponent<InputField>();

				if (inputField != null) 
					inputField.ProcessEvent(Event.KeyboardEvent("I"));
			}
		}
	}
}
