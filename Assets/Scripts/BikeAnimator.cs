using UnityEngine;

//Script for animating the wheels and sprocket of a bike procedurally
public class BikeAnimator : MonoBehaviour {

    #region Serializable private data field

    //Transforms for the wheels and sprocket
    [SerializeField]
    private Transform frontWheel;

    [SerializeField]
    private Transform rearWheel;

    [SerializeField]
    private Transform sprocket;

    #endregion 

    #region Animation Factors field
    //Speed and scaling factors
    public float animationSpeed = 0f;
    public float scaleFactor = 1.0f;
    public float sprocketScaleFactor = 2.0f;
    #endregion

    //Axis to rotate the components in
    public Vector3 rotationAxis = new Vector3(0f, 1.0f, 0f);

    //Check references
    void Start () {
        if (frontWheel == null || rearWheel == null || sprocket == null)
        {
            throw new UnityEngine.UnassignedReferenceException("Component transforms not assigned");
        }
	}
	
    //Animate the components
	void Update () {
        frontWheel.Rotate(rotationAxis * animationSpeed * scaleFactor);

        rearWheel.Rotate(rotationAxis * animationSpeed * scaleFactor);

        sprocket.Rotate(rotationAxis * animationSpeed * scaleFactor * sprocketScaleFactor);
    }
}
