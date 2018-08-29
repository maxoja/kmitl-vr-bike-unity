using UnityEngine;
using System.Collections.Generic;
using BezierSolution;

public class BezierWalker : MonoBehaviour
{
    [SerializeField]
    private BezierSpline spline;

    [Range(20, 200), SerializeField]
    private float cacheQuality = 100;


    //[Range(0, 0.15f), SerializeField]
    //private float speed = 5f;

    [Range(0, 1), SerializeField]
    private float progress = 0f;

    private bool tickToCalculateCache = true;

    //[HideInInspector, SerializeField]
     private float[] cache;

    private void Start()
    {
        tickToCalculateCache = true;
        PrepareCache();
    }

    void Update()
    {
        InterpolatePositionAndRotation(progress);

        if(progress > 1)
            progress = 0;
    }

    public float GetProgress()
    {
        return progress;
    }

    public void SetSpeed(float newSpeed)
    {
        //this.speed = newSpeed;
    }

    public void SetProgress(float newProgress)
    {
        this.progress = newProgress;
    }

    // helpers
    private void InterpolatePositionAndRotation(float percent)
    {
        float normalizedT = MapPercentToBeziereRatio(progress);
        Vector3 resultPosition = spline.GetPoint(normalizedT);

        //do raycast and lerp y
        Ray ray = new Ray(transform.position + Vector3.up*3, Vector3.down);
        RaycastHit hit;
          
        //if the ray has hit something  
        if(Physics.Raycast(ray.origin,ray.direction, out hit, 10))//cast the ray 5 units at the specified direction    
        {
            float hitY = hit.point.y;
            float posY = transform.position.y;
            float properY = Mathf.Lerp(posY, hitY, Time.deltaTime*10);
            resultPosition.y = properY;
        }

        transform.position = resultPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(spline.GetTangent(normalizedT)), Time.deltaTime*5);
    }

    private float MapPercentToBeziereRatio(float percent)
    {
        if (percent >= 1)
            return 1f;
        PrepareCache();
        percent = Mathf.Clamp(percent, 0, 1);

        int cacheNum = cache.Length;
        int leftIndex = (int)(progress * cacheNum);
        int rightIndex = (leftIndex == cacheNum - 1) ? leftIndex : leftIndex + 1;
        float remainingIndex = progress * cacheNum - leftIndex;

        return Mathf.Lerp(cache[leftIndex], cache[rightIndex], remainingIndex);
    }

    private void PrepareCache()
    {
        if (!tickToCalculateCache)
            return;

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
        print("calculated with size of " + cache.Length);
    }

    //pre processing in edit mode
    private void OnDrawGizmos()
    {
        PrepareCache();
    }
}