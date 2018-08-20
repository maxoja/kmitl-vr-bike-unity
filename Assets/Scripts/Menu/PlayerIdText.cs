using UnityEngine;
using UnityEngine.UI;

public class PlayerIdText : MonoBehaviour {
    private Text textComp;

    private void Update()
    {
        if (textComp == null)
            textComp = GetComponent<Text>();

        textComp.text = "player id : " + StaticData.playerId.ToString();
    }

    public void AddValue()
    {
        StaticData.playerId += 1;

        if (StaticData.playerId > 10)
            StaticData.playerId = 10;
    }

    public void SubstractValue()
    {
        StaticData.playerId -= 1;

        if (StaticData.playerId < 0)
            StaticData.playerId = 0;
    }
}
