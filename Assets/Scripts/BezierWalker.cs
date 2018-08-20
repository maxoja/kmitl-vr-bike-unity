using UnityEngine;
using System.Collections.Generic;
using BezierSolution;

public class BezierWalker : MonoBehaviour
{

    #region Serializable private data field

    [SerializeField]
    private BezierSpline spline;

    [Range(20, 200), SerializeField]
    private float cacheQuality = 100;

    [Range(0, 0.15f), SerializeField]
    private float speed = 5f;

    [Range(0, 1), SerializeField]
    private float progress = 0f;

    [HideInInspector, SerializeField]
    private float[] cache;

    #endregion 

    public bool tickToCalculateCache;

    void Start()
    {
        tickToCalculateCache = true;
    }

    void Update()
    {
        InterpolatePositionAndRotation(progress);
        progress += Time.deltaTime * speed;

        if (progress > 1)
        {
            progress = 0;
        }
    }

    // helpers
    private void InterpolatePositionAndRotation(float percent)
    {
        float normalizedT = MapPercentToBeziereRatio(progress);
        transform.position = spline.GetPoint(normalizedT);
        transform.rotation = Quaternion.LookRotation(spline.GetTangent(normalizedT));
    }

    private float MapPercentToBeziereRatio(float percent)
    {
        PrepareCache();
        percent = Mathf.Clamp(percent, 0, 1);

        int cacheNum = cache.Length;
        int leftIndex = (int)(progress * cacheNum);
        int rightIndex = (leftIndex == cacheNum - 1) ? leftIndex : leftIndex + 1;
        float remainingIndex = progress * cacheNum - leftIndex;

        return Mathf.Lerp(cache[leftIndex], cache[rightIndex], remainingIndex);
    }

    public void SetSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }

    public void SetProgress(float newProgress)
    {
        this.progress = newProgress;
    }

    private void PrepareCache()
    {
        if (!tickToCalculateCache)
        {
            return;
        }

        tickToCalculateCache = false;

        List<float> list = new List<float>();
        float percent = 0;
        float detail = 1 / cacheQuality;

        do
        {
            list.Add(percent);
            spline.MoveAlongSpline(ref percent, detail);
        }
        while (percent < 1);

        cache = list.ToArray();
    }

    //pre processing in edit mode
    private void OnDrawGizmos()
    {
        PrepareCache();
    }
}