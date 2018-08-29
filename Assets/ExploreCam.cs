using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreCam : MonoBehaviour {
    public Transform[] targets;
    Transform currentTarget;
    Vector3 currentDistance;
    float t = 0;

    private void Awake()
    {
        RandomNewTarget();
    }

    void Update () {
        t += Time.deltaTime;
		if(t >= 10)
        {
            t = 0;
            RandomNewTarget();
        }

        transform.rotation = Quaternion.identity;
        transform.position = currentTarget.position + currentDistance;
        currentDistance -= currentDistance.normalized * Time.deltaTime/1.5f;
        transform.LookAt(currentTarget);
	}

    void RandomNewTarget()
    {
        currentTarget = targets[Random.Range(0, targets.Length)];
        currentDistance = new Vector3(Random.Range(-20, 20), Random.Range(10f,15f), Random.Range(-20, 20));
    }
}
