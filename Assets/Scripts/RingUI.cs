using UnityEngine.UI;
using UnityEngine;

public class RingUI : MonoBehaviour 
{
    public float rotateSpeed = 1;
    HorizontalLayoutGroup horiGroup;
    public Text[] textElements;

    private RectTransform rt;
    private float startLeftAsset;
    private float startRightAsset;
    public float offset = 0;

    private void Awake()
    {
        horiGroup = GetComponent<HorizontalLayoutGroup>();
        rt = GetComponent<RectTransform>();

        startLeftAsset = rt.offsetMin.x;
        startRightAsset = rt.offsetMax.x;
    }
	
	void Update () 
    {
        offset += Time.deltaTime * rotateSpeed;
        ApplyOffset();
	}

    public void SetText(string text)
    {
        foreach (Text t in textElements)
            t.text = text;
    }

    public void SetRotationSpeed(float speed)
    {
        this.rotateSpeed = speed;
    }

    public void SetRotationOffset(float offset)
    {
        this.offset = offset;
    }

    private void ApplyOffset()
    {
        rt.offsetMin = new Vector2(startLeftAsset + offset, rt.offsetMin.y);
        rt.offsetMax = new Vector2(startRightAsset + offset, rt.offsetMax.y);
    }
}
