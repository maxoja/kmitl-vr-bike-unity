using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteController : MonoBehaviour {
    private PopupEmote emote;
	// Use this for initialization
    private IEnumerator Start () {
        emote = GetComponent<PopupEmote>();
		while(true)
        {
            yield return new WaitForSeconds(Random.Range(3, 12));
            emote.ShowEmote(emote.EmoteNames[Random.Range(0, 30)]);

        }
	}
}
