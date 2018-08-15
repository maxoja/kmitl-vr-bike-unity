using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VirtualKeyboard : MonoBehaviour {
	#region VirtualKeyboard Instantiation

	public enum VirtualKeyboardType {
		ASCIICapable
	}
	
    public static VirtualKeyboard Open(Canvas canvas, GameObject inputObject = null, VirtualKeyboardType keyboardType = VirtualKeyboardType.ASCIICapable) {
		VirtualKeyboard keyboard = GameObject.FindObjectOfType<VirtualKeyboard>();
		if (keyboard == null || (keyboard != null && keyboard.inputObject != inputObject))
		{
			Close();
			keyboard = Instantiate<VirtualKeyboard>(Resources.Load<VirtualKeyboard>("VirtualKeyboard"));
			keyboard.transform.SetParent(canvas.transform, false);
			keyboard.inputObject = inputObject;
		}
		return keyboard;
	}
	
	public static void Close() {
		VirtualKeyboard[] kbs = GameObject.FindObjectsOfType<VirtualKeyboard>();
		foreach (VirtualKeyboard kb in kbs)
		{
			kb.CloseKeyboard();
		}
	}
	
	public static bool IsOpen  {
		get {
			return GameObject.FindObjectsOfType<VirtualKeyboard>().Length != 0;
		}
	}

	#endregion

	public GameObject inputObject;

	public string text {
		get {
			if (inputObject != null) {
				Component[] components = inputObject.GetComponents(typeof(Component));

				foreach (Component component in components) {
					PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
					if (prop != null) {
						return prop.GetValue(component, null)  as string;;
					}
				}
				return inputObject.name;
			}
			return "";
		}
		
		set {
			if (inputObject != null)  {
				Component[] components = inputObject.GetComponents(typeof(Component));
				foreach (Component component in components) {
					PropertyInfo prop = component.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public);
					if (prop != null) {
						prop.SetValue(component, value, null);
						return;
					}
				}
				inputObject.name = value;
			}
		}
	}

	#region Keyboard Receiving Input

	public void SendKeyString(string keyString) {
		InputFieldCaretPosition inputField = inputObject.GetComponent<InputFieldCaretPosition> ();

		if (keyString.Length == 1 && keyString[0] == 8) {
            inputField.text = "";
			//if (text.Length > 0 && inputField.GetLocalCaretPosition () - 1 > -1) {
			//	text = text.Remove (inputField.GetLocalCaretPosition () - 1, 1); 
			//	inputField.currentPosition--;
			//}
		}
		else {
            inputField.text = text.Trim() + keyString.Trim();
		}

        //ReactivateInputField(inputObject.GetComponent<InputField>());
        print(StaticData.serverPort);
	}

	public void CloseKeyboard() {
		Destroy(gameObject);
	}

	#endregion


	#region Steal Focus Workaround

	void ReactivateInputField(InputField inputField) {
		if (inputField != null)
			StartCoroutine(ActivateInputFieldWithoutSelection(inputField));
	}

	IEnumerator ActivateInputFieldWithoutSelection(InputField inputField) {
		inputField.ActivateInputField();
		yield return new WaitForEndOfFrame();

		//if (EventSystem.current.currentSelectedGameObject == inputField.gameObject)
		//	inputField.MoveTextEnd(true);
	}

	#endregion

}