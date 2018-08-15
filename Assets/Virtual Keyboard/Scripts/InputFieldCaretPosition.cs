using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldCaretPosition : InputField {
	public int currentPosition = 0;

	public override void OnPointerUp (UnityEngine.EventSystems.PointerEventData eventData) {
		this.currentPosition = caretPosition; 
		base.OnPointerUp (eventData);
	}

	public override void OnPointerDown (UnityEngine.EventSystems.PointerEventData eventData) {
		this.currentPosition = caretPosition; 
		base.OnPointerDown (eventData);
	}

	public int GetLocalCaretPosition () {
		return currentPosition;	
	}
}
