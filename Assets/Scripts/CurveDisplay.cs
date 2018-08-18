using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurveDisplay : MonoBehaviour {
    public RingUI upperRing, midRing, lowerRing;
    public CanvasGroup alphaController;
    private Mode currentMode = Mode.Waiting;
    private string rank = "no rank";
    private Vector3 startLocalPosition;

    public enum Mode
    {
        Waiting,
        Launching,
        Playing,
        Finished
    }

    private void Awake()
    {
        startLocalPosition = transform.localPosition;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(TransitToLaunching());
        yield return StartCoroutine(TransitToPlaying());
        yield return new WaitForSeconds(2);
        SetRank("1st");
        yield return StartCoroutine(TransitToFinished());
    }

    public void SetRank(string r)
    {
        this.rank = r;
    }

    public void ChangeMode(Mode mode)
    {
        if (mode == this.currentMode)
            return;

        this.currentMode = mode;
        switch(mode)
        {
            case Mode.Waiting: 
                break;
            case Mode.Launching:
                StopAllCoroutines();
                StartCoroutine(TransitToLaunching());
                break;
            case Mode.Playing:
                StopAllCoroutines();
                StartCoroutine(TransitToPlaying());
                break;
            case Mode.Finished:
                StopAllCoroutines();
                StartCoroutine(TransitToFinished());
                break;
        }
    }

    private IEnumerator TransitToLaunching()
    {
        upperRing.SetText("Get Ready!");
        lowerRing.SetText("Get Ready!");
        float startRotSpeed = Mathf.Abs(upperRing.rotateSpeed);
        float startMidRotSpeed = midRing.rotateSpeed;

        midRing.SetRotationSpeed(0);

        float r = 0;
        while(true)
        {
            r += Time.deltaTime/3;
            upperRing.SetRotationSpeed(Mathf.Lerp(-startRotSpeed, -1600, r));
            lowerRing.SetRotationSpeed(Mathf.Lerp(startRotSpeed, 1600,r));
            //midRing.SetRotationSpeed(Mathf.Lerp(startMidRotSpeed, 0, r));

            if (r >= 1)
                break;

            yield return null;
        }

        midRing.SetText("3              3               3");
        yield return new WaitForSeconds(1);
        midRing.SetText("2              2               2");
        yield return new WaitForSeconds(1);
        midRing.SetText("1              1               1");
        yield return new WaitForSeconds(1);
        midRing.SetText("Go!           Go!            Go!");

        r = 0;
        while(true)
        {
            r += Time.deltaTime * 4;
            float xz = Mathf.Lerp(1, 2, r);
            float y = Mathf.Lerp(1, 0, r);
            transform.localScale = new Vector3(xz, y, xz);
            alphaController.alpha = Mathf.Lerp(1, 0, r);

            if (r >= 1)
                break;

            yield return null;
        }

        upperRing.SetRotationSpeed(0);
        upperRing.SetRotationOffset(0);
        midRing.SetRotationSpeed(0);
        midRing.SetRotationOffset(0);
        lowerRing.SetRotationSpeed(0);
        lowerRing.SetRotationOffset(0);
    }

    private IEnumerator TransitToPlaying()
    {
        yield return null;
    }

    private IEnumerator TransitToFinished()
    {
        upperRing.SetRotationSpeed(20);
        upperRing.SetText("Finished!");

        midRing.SetRotationSpeed(40);
        midRing.SetText("You are  the " + rank);

        lowerRing.SetRotationSpeed(60);
        lowerRing.SetText("Finished!");

        float r = 0;

        while(true){
            r += Time.deltaTime * 4;
            float xz = Mathf.Lerp(2, 1, r);
            float y = Mathf.Lerp(0, 1, r);
            transform.localScale = new Vector3(xz, y, xz);
            alphaController.alpha = Mathf.Lerp(0, 1, r);

            if (r >= 1)
                break;

            yield return null;
        }
        yield return null;
    }
}
