using UnityEngine;

/// <summary>
/// Script for animating the wheels and sprocket of a bike procedurally
/// </summary>
public class BikeAnimator : MonoBehaviour {

    //Transforms for the wheels and sprocket
    [SerializeField]
    private Transform frontWheel;
    [SerializeField]
    private Transform rearWheel;
    [SerializeField]
    private Transform sprocket;

    //Axis to rotate the components in
    public Vector3 rotationAxis = new Vector3(0f, 1.0f, 0f);

    //Speed and scaling factors
    public float animationSpeed = 0f;
    public float scaleFactor = 1.0f;
    public float sprocketScaleFactor = 2.0f;

    //Check references
	void Start () {
        if (frontWheel == null || rearWheel == null || sprocket == null)
            throw new UnityEngine.UnassignedReferenceException("Component transforms not assigned");
	}
	
    //Animate the components
	void Update () {
        frontWheel.Rotate(rotationAxis * animationSpeed * scaleFactor);
        rearWheel.Rotate(rotationAxis * animationSpeed * scaleFactor);
        sprocket.Rotate(rotationAxis * animationSpeed * scaleFactor * sprocketScaleFactor);
    }
}
