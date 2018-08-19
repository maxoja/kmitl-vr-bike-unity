using UnityEngine;
using UnityEngine.UI;

public class TestCanvasController : MonoBehaviour {

    public Dropdown emotesDropdown;

    public PopupEmote emote;

    void Start() {
        emotesDropdown.options.Clear();
        foreach (var emoteName in emote.EmoteNames) {
            emotesDropdown.options.Add(new Dropdown.OptionData() {
                text = emoteName
            });
        }
        emotesDropdown.value = 0;
        emotesDropdown.RefreshShownValue();
    }

    public void ShowEmote() {
        emote.ShowEmote(emotesDropdown.options[emotesDropdown.value].text);
    }

    public void HideEmote() {
        emote.CloseEmote();
    }
}
