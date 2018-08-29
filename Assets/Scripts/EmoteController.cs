using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteController : MonoBehaviour {
    private PopupEmote[] emotes;
	// Use this for initialization
    private IEnumerator Start () {
        emotes = GetComponents<PopupEmote>();
		while(true)
        {
            yield return new WaitForSeconds(Random.Range(5, 10));
            foreach(PopupEmote emote in emotes)
                emote.ShowEmote(emote.EmoteNames[Random.Range(0, 30)]);

        }
	}
}
