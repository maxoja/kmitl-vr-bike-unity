using UnityEngine;
using System.Collections;

public class VirtualKeyboardASCII : MonoBehaviour 
{
	public VirtualKeyboard canvasKeyboard;
	private bool ShiftDown = false;
	private bool AltDown = false;
	public GameObject alphaBoardUnsfhifted;
	public GameObject alphaBoardSfhifted;
	public GameObject numberBoardUnshifted;
	public GameObject numberBoardShifted;

	void Awake() {
		Refresh();
	}
	
	void Refresh() {
		alphaBoardUnsfhifted.SetActive(!AltDown && !ShiftDown);
		alphaBoardSfhifted.SetActive(!AltDown && ShiftDown);
		numberBoardUnshifted.SetActive(AltDown && !ShiftDown);
		numberBoardShifted.SetActive(AltDown && ShiftDown);
	}

	public void OnKeyDown(GameObject kb) {
		if (kb.name == "DONE")
		{
			if (canvasKeyboard != null)
				canvasKeyboard.CloseKeyboard();
		}
		else if (kb.name == "ALT")
		{
			AltDown = !AltDown;
			ShiftDown = false;
			Refresh ();
		}
		else if (kb.name == "SHIFT")
		{
			ShiftDown = !ShiftDown;
			Refresh ();
		}
		else
		{
			if (canvasKeyboard != null) {
				string s;
				if (kb.name == "BACKSPACE")
				{
					s = "\x08";
				}
				else if (kb.name == "SPACE")
				{
					s = " ";
				}
				else
				{
					s = kb.name;
				}
				canvasKeyboard.SendKeyString(s);
			}
		}
		
	}

}