using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using CurvedUI;

#if CURVEDUI_VIVE
using Valve.VR;
#endif 

[ExecuteInEditMode]
#if CURVEDUI_GOOGLEVR
public class CurvedUIInputModule : GvrPointerInputModule {
#else
public class CurvedUIInputModule : StandaloneInputModule {
#endif





    //SETTINGS-------------------------------------------------//
    #region SETTINGS
    #pragma warning disable 414, 0649

    //Common
    [SerializeField]
    CUIControlMethod controlMethod;
    [SerializeField]
    string submitButtonName = "Fire1"; 

    //Gaze
    [SerializeField]
    bool gazeUseTimedClick = false;
    [SerializeField]
    float gazeClickTimer = 2.0f;
    [SerializeField]
    float gazeClickTimerDelay = 1.0f;
    [SerializeField]
    Image gazeTimedClickProgressImage;

    //World Space Mouse
    [SerializeField]
    float worldSpaceMouseSensitivity = 1;

    //SteamVR and Oculus
    [SerializeField]
    Hand usedHand = Hand.Right;

    //hidden
    static bool disableOtherInputModulesOnStart = true; //default true

    #endregion
    //---------------------------------------------------------//





    //COMMON VARIABLES-----------------------------------------//
    #region VARIABLES
   

    //Support Variables - common
    static CurvedUIInputModule instance;
    GameObject currentDragging;
    GameObject currentPointedAt;

    //Support Variables - gaze
    float gazeTimerProgress;

    //Support variables - custom ray
    Ray customControllerRay;

    //support variables - other
	float dragThreshold = 10.0f;
	bool pressedDown = false;
	bool pressedLastFrame = false;

    //support variables - world space mouse
    Vector3 lastMouseOnScreenPos = Vector2.zero;
    Vector2 worldSpaceMouseInCanvasSpace = Vector2.zero;
    Vector2 lastWorldSpaceMouseOnCanvas = Vector2.zero;
    Vector2 worldSpaceMouseOnCanvasDelta = Vector2.zero;
    //---------------------------------------------------------//





    //PLATFORM DEPENDANT VARIABLES AND SETTINGS----------------//

#if CURVEDUI_VIVE
    //Settings & References - SteamVR
    [SerializeField]
    SteamVR_ControllerManager steamVRControllerManager;

    //Support Variables - SteamVR
    private static SteamVR_ControllerManager controllerManager;
    private static CurvedUIViveController rightCont;
    private static CurvedUIViveController leftCont;
    private CurvedUIPointerEventData rightControllerData;
    private CurvedUIPointerEventData leftControllerData;

#endif


#if CURVEDUI_TOUCH
    //Settings & References - Oculus SDK
    [SerializeField]
    Transform TouchControllerTransform;
    [SerializeField]
    OVRInput.Button InteractionButton = OVRInput.Button.PrimaryIndexTrigger;
    [SerializeField]
    OVRCameraRig oculusCameraRig;


    //Support variables - Touch
    private OVRInput.Controller activeCont;
#endif



#pragma warning restore 414, 0649
    #endregion
    //---------------------------------------------------------//





#if CURVEDUI_GOOGLEVR


#else // CURVEDUI_GOOGLEVR ELSE

    protected override void Awake(){
		if (!Application.isPlaying) return;

		Instance = this;
		base.Awake ();


        //Gaze setup
        if (gazeTimedClickProgressImage != null)
            gazeTimedClickProgressImage.fillAmount = 0;

        //SteamVR setup
#if CURVEDUI_VIVE
		//SEtup controllers for vive
		if(ControlMethod == CUIControlMethod.STEAMVR)
		SetupViveControllers();
#endif
    }



    protected override void Start()
    {
        if (!Application.isPlaying) return;

        base.Start();


        //OculusVR setup
#if CURVEDUI_TOUCH
        if (oculusCameraRig == null)
        {
            //find the oculus rig - via manager or by findObjectOfType, if unavailable
            if (OVRManager.instance != null)
            {
                oculusCameraRig = OVRManager.instance.GetComponent<OVRCameraRig>();
            }

            if (oculusCameraRig == null)
            {
                oculusCameraRig = Object.FindObjectOfType<OVRCameraRig>();
            }

            if (oculusCameraRig == null && ControlMethod == CUIControlMethod.OCULUSVR)
            {
                Debug.LogError("OVRCameraRig prefab required. Import Oculus Utilities and drag OVRCameraRig prefab onto the scene.");
            }            
        }

#endif
    }



    #region EVENT PROCESSING - GENERAL
    /// <summary>
    /// Process is called by UI system to process events 
    /// </summary>
    public override void Process()
	{
        //processing mouse
        //eventSystem.SetSelectedGameObject(null, null);
        //base.Process();

        switch (controlMethod)
		{
		case CUIControlMethod.MOUSE:
			{
				base.Process();
				break;
			}
		case CUIControlMethod.GAZE:
			{
				ProcessGaze();
				break;
			}
		case CUIControlMethod.STEAMVR:
			{
				ProcessViveControllers();
				break;
			}
		case CUIControlMethod.OCULUSVR:
			{
				ProcessOculusVRController();
				break;
			}
		case CUIControlMethod.WORLD_MOUSE:
			{
				//touch can also be used as a world space mouse, although its probably not the best experience
				//Use standard mouse controller with touch.
				if (Input.touchCount > 0)
				{
					worldSpaceMouseOnCanvasDelta = Input.GetTouch(0).deltaPosition * worldSpaceMouseSensitivity;
				} else {
					worldSpaceMouseOnCanvasDelta = new Vector2((Input.mousePosition - lastMouseOnScreenPos).x, (Input.mousePosition - lastMouseOnScreenPos).y) * worldSpaceMouseSensitivity;
					lastMouseOnScreenPos = Input.mousePosition;
				}
				lastWorldSpaceMouseOnCanvas = worldSpaceMouseInCanvasSpace;
				worldSpaceMouseInCanvasSpace += worldSpaceMouseOnCanvasDelta;

				base.Process();

				break;
			}
		case CUIControlMethod.CUSTOM_RAY:
			{
				ProcessCustomRayController();
				break;
			}
		default: goto case CUIControlMethod.MOUSE;
		}
	}
        #endregion // EVENT PROCESSING - GENERAL



        #region EVENT PROCESSING - GAZE
	protected virtual void ProcessGaze()
	{
		bool usedEvent = SendUpdateEventToSelectedObject();

		if (eventSystem.sendNavigationEvents)
		{
			if (!usedEvent)
				usedEvent |= SendMoveEventToSelectedObject();

			if (!usedEvent)
				SendSubmitEventToSelectedObject();
		}

		ProcessMouseEvent();
	}
        #endregion // EVENT PROCESSING - GAZE



        #region EVENT PROCESSING - CUSTOM RAY
	protected virtual void ProcessCustomRayController(){

        var mouseData = GetMousePointerEventData(0);
        PointerEventData eventData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData;


        // send update events if there is a selected object - this is important for InputField to receive keyboard events
        SendUpdateEventToSelectedObject();

        // see if there is a UI element that is currently being pointed at
        PointerEventData ControllerData = eventData;

        currentPointedAt = ControllerData.pointerCurrentRaycast.gameObject;
        ProcessDownRelease(ControllerData, (pressedDown && !pressedLastFrame), (!pressedDown && pressedLastFrame));

        //Process move and drag if trigger is pressed
        ProcessMove(ControllerData);
        if (pressedDown)
        {
            ProcessDrag(ControllerData);

            if (!Mathf.Approximately(ControllerData.scrollDelta.sqrMagnitude, 0.0f))
            {
                var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(ControllerData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, ControllerData, ExecuteEvents.scrollHandler);
            }
        }

       

        //save button state for this frame
        pressedLastFrame = pressedDown;
	}

    /// <summary>
    /// Sends trigger down / trigger released events to gameobjects under the pointer.
    /// </summary>
    protected virtual void ProcessDownRelease(PointerEventData eventData, bool down, bool released)
    {
        var currentOverGo = eventData.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (down)
        {
            eventData.eligibleForClick = true;
            eventData.delta = Vector2.zero;
            eventData.dragging = false;
            eventData.useDragThreshold = true;
            eventData.pressPosition = eventData.position;
            eventData.pointerPressRaycast = eventData.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, eventData);

            if (eventData.pointerEnter != currentOverGo)
            {
                // send a pointer enter to the touched element if it isn't the one to select...
                HandlePointerExitAndEnter(eventData, currentOverGo);
                eventData.pointerEnter = currentOverGo;
            }

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);


            float time = Time.unscaledTime;

            if (newPressed == eventData.lastPress)
            {
                var diffTime = time - eventData.clickTime;
                if (diffTime < 0.3f)
                    ++eventData.clickCount;
                else
                    eventData.clickCount = 1;

                eventData.clickTime = time;
            }
            else
            {
                eventData.clickCount = 1;
            }

            eventData.pointerPress = newPressed;
            eventData.rawPointerPress = currentOverGo;

            eventData.clickTime = time;

            // Save the drag handler as well
            eventData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (eventData.pointerDrag != null)
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (released)
        {
            ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerUpHandler);

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (eventData.pointerPress == pointerUpHandler && eventData.eligibleForClick)
            {
                ExecuteEvents.Execute(eventData.pointerPress, eventData, ExecuteEvents.pointerClickHandler);
                //Debug.Log("click");
            }
            else if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, eventData, ExecuteEvents.dropHandler);
                //Debug.Log("drop");
            }

            eventData.eligibleForClick = false;
            eventData.pointerPress = null;
            eventData.rawPointerPress = null;

            if (eventData.pointerDrag != null && eventData.dragging)
            {
                ExecuteEvents.Execute(eventData.pointerDrag, eventData, ExecuteEvents.endDragHandler);
                //Debug.Log("end drag");
            }

            eventData.dragging = false;
            eventData.pointerDrag = null;

            // send exit events as we need to simulate this on touch up on touch device
            ExecuteEvents.ExecuteHierarchy(eventData.pointerEnter, eventData, ExecuteEvents.pointerExitHandler);
            eventData.pointerEnter = null;
        }
    }

    #endregion // EVENT PROCESSING - CUSTOM RAY



    #region EVENT PROCESSING - VIVE
    protected virtual void ProcessViveControllers()
	{
#if CURVEDUI_VIVE
        switch (usedHand)
        {
            case Hand.Right:
            {
                //in case only one controller is turned on, it will still be used to call events.
                if (controllerManager.right.activeInHierarchy)
                    ProcessController(controllerManager.right);
                else if (controllerManager.left.activeInHierarchy)
                    ProcessController(controllerManager.left);
                break;
            }
            case Hand.Left:
            {
                //in case only one controller is turned on, it will still be used to call events.
                if (controllerManager.left.activeInHierarchy)
                    ProcessController(controllerManager.left);
                else if (controllerManager.right.activeInHierarchy)
                    ProcessController(controllerManager.right);
                break;
            }
            case Hand.Both:
            {
                ProcessController(controllerManager.left);
                ProcessController(controllerManager.right);
                break;
            }
            default: goto case Hand.Right;
        }
    }


    /// <summary>
    /// Processes Events from given controller.
    /// </summary>
    /// <param name="myController"></param>
    void ProcessController(GameObject myController)
	{
        //do not process events from this controller if it's off or not visible by base stations.
        if (!myController.gameObject.activeInHierarchy) return;

        //get the assistant or add it if its missing.
        CurvedUIViveController myControllerAssitant = myController.AddComponentIfMissing<CurvedUIViveController>();

        // send update events if there is a selected object - this is important for InputField to receive keyboard events
        SendUpdateEventToSelectedObject();

        // see if there is a UI element that is currently being pointed at
        PointerEventData ControllerData;
        if (myControllerAssitant == Right)
            ControllerData = GetControllerPointerData(myControllerAssitant, ref rightControllerData);
        else
            ControllerData = GetControllerPointerData(myControllerAssitant, ref leftControllerData);


        currentPointedAt = ControllerData.pointerCurrentRaycast.gameObject;

        ProcessDownRelease(ControllerData, myControllerAssitant.IsTriggerDown, myControllerAssitant.IsTriggerUp);

        //Process move and drag if trigger is pressed
        if (!myControllerAssitant.IsTriggerUp)
        {
            ProcessMove(ControllerData);
            ProcessDrag(ControllerData);
        }

        if (!Mathf.Approximately(ControllerData.scrollDelta.sqrMagnitude, 0.0f))
        {
            var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(ControllerData.pointerCurrentRaycast.gameObject);
            ExecuteEvents.ExecuteHierarchy(scrollHandler, ControllerData, ExecuteEvents.scrollHandler);
            // Debug.Log("executing scroll handler");
        }

    }

    /// <summary>
    /// Create a pointerEventData that stores all the data associated with Vive controller.
    /// </summary>
    private CurvedUIPointerEventData GetControllerPointerData(CurvedUIViveController controller, ref CurvedUIPointerEventData ControllerData)
    {

        if (ControllerData == null)
            ControllerData = new CurvedUIPointerEventData(eventSystem);

        ControllerData.Reset();
        ControllerData.delta = Vector2.one; // to trick into moving
        ControllerData.position = Vector2.zero; // this will be overriden by raycaster
        ControllerData.Controller = controller.gameObject; // raycaster will use this object to override pointer position on screen. Keep it safe.
        ControllerData.scrollDelta = controller.TouchPadAxis - ControllerData.TouchPadAxis; // calcualte scroll delta
        ControllerData.TouchPadAxis = controller.TouchPadAxis; // assign finger position on touchpad

        eventSystem.RaycastAll(ControllerData, m_RaycastResultCache); //Raycast all the things!. Position will be overridden here by CurvedUIRaycaster

        //Get a current raycast to find if we're pointing at GUI object. 
        ControllerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        m_RaycastResultCache.Clear();

        return ControllerData;
    }


    private bool ShouldStartDrag(Vector2 pressPos, Vector2 currentPos, float threshold, bool useDragThreshold)
    {
        if (!useDragThreshold)
            return true;

        //this always returns false if override pointereventdata in curveduiraycster.cs is set to false. There is no past pointereventdata to compare with then.
        return (pressPos - currentPos).sqrMagnitude >= threshold * threshold;
    }

    /// <summary>
    /// Force selection of a gameobject.
    /// </summary>
    private void Select(GameObject go)
    {
        ClearSelection();
        if (ExecuteEvents.GetEventHandler<ISelectHandler>(go))
        {
            eventSystem.SetSelectedGameObject(go);
        }
    }

    /// <summary>
    /// Adds necessary components to Vive controller gameobjects. These will let us know what inputs are used on them.
    /// </summary>
    private void SetupViveControllers()
    {
        //find controller reference
        if (controllerManager == null)
                controllerManager = steamVRControllerManager;

        //Find Controller manager on the scene.
        if (controllerManager == null)
        {
            SteamVR_ControllerManager[] potentialManagers = Object.FindObjectsOfType<SteamVR_ControllerManager>();
            controllerManager = null;

            //ignore external camera created by externalcamera.cfg for mixed reality videos
            if (potentialManagers.GetLength(0) > 0)
            {
                for (int i = 0; i < potentialManagers.GetLength(0); i++)
                {
                    if (potentialManagers[i].gameObject.name != "External Camera")
                        controllerManager = potentialManagers[i];
                }
            }

            if (controllerManager == null)
                Debug.LogError("Can't find SteamVR_ControllerManager on scene. It is required to use VIVE control method. Make sure all SteamVR prefabs are present.");
        }
#endif
    }
    #endregion



    #region EVENT PROCESSING - OCULUS TOUCH
    protected virtual void ProcessOculusVRController()
    {
#if CURVEDUI_TOUCH

        activeCont = OVRInput.GetActiveController();


        //Find the currently used HandAnchor----------------------//
        //and set direction ray using its transform
        switch (activeCont)
        {
            //Oculus Touch
            case OVRInput.Controller.RTouch: CustomControllerRay = new Ray(oculusCameraRig.rightHandAnchor.position, oculusCameraRig.rightHandAnchor.forward); break;
            case OVRInput.Controller.LTouch: CustomControllerRay = new Ray(oculusCameraRig.leftHandAnchor.position, oculusCameraRig.leftHandAnchor.forward); break;
            //GearVR touchpad
            case OVRInput.Controller.Touchpad: CustomControllerRay = new Ray(oculusCameraRig.centerEyeAnchor.position, oculusCameraRig.centerEyeAnchor.forward); break;
            //GearVR controller / Oculus Go controller
            case OVRInput.Controller.RTrackedRemote: goto case OVRInput.Controller.RTouch;
            case OVRInput.Controller.LTrackedRemote: goto case OVRInput.Controller.LTouch;
            //edge cases
            default: CustomControllerRay = new Ray(OculusTouchUsedControllerTransform.position, OculusTouchUsedControllerTransform.forward); break;
        }



        //Check if interaction button is pressed ---------------//

        //find if we're using Rift with touch. If yes, we'll have to check if the interaction button is pressed on the proper hand.
        bool touchControllersUsed = (activeCont == OVRInput.Controller.Touch || activeCont == OVRInput.Controller.LTouch || activeCont == OVRInput.Controller.RTouch);

        if (usedHand == Hand.Both || !touchControllersUsed) 
        {
            //check if this button is pressed on any controller. Handles GearVR controller and Oculus Go controller.
            CustomControllerButtonDown = OVRInput.Get(InteractionButton);

            //on GearVR, also check touchpad, as a secondary, optional input.
            if (activeCont == OVRInput.Controller.Touchpad)
                CustomControllerButtonDown = CustomControllerButtonDown || OVRInput.Get(OVRInput.Button.PrimaryTouchpad);
        }
        else if (usedHand == Hand.Right) // Right Oculus Touch
        {
            CustomControllerButtonDown = OVRInput.Get(InteractionButton, OVRInput.Controller.RTouch);
        }
        else if (usedHand == Hand.Left)  // Left Oculus Touch
        {
            CustomControllerButtonDown = OVRInput.Get(InteractionButton, OVRInput.Controller.LTouch);
        }

        

        //process all events based on this data--------------//
        ProcessCustomRayController();
#endif // END OF CURVEDUI_TOUCH IF
    }
    #endregion


#endif // END OF CURVEDUI_GOOGLEVR IF - GOOGLEVR INPUT MODULE OVERRIDE









    #region HELPER FUNCTIONS
    static T EnableInputModule<T>() where T : BaseInputModule
    {
        bool moduleMissing = true;
        EventSystem eventGO = GameObject.FindObjectOfType<EventSystem>();

        if(eventGO == null)
        {
            Debug.LogError("CurvedUI: Your EventSystem component is missing from the scene! Unity Canvas will not track interactions without it.");
            return null as T;
        }

        foreach (BaseInputModule module in eventGO.GetComponents<BaseInputModule>())
        {
			if (module is T) {
				moduleMissing = false;
				module.enabled = true;
			} else if (disableOtherInputModulesOnStart) {
				module.enabled = false;

				#if CURVEDUI_GOOGLEVR //on GVR, we have to completely destroy the module because the code looks for first instance, even if it's disabled
				if(module is GvrPointerInputModule){
					if(Application.isPlaying){
						Destroy(module);
						Debug.LogError("CurvedUI: Fixed bad Input Module. Restart Play mode to continue.");
					} else {
						DestroyImmediate(module);
					}
				}
				#endif 
			}
        }

        if (moduleMissing)
            eventGO.gameObject.AddComponent<T>();

        return eventGO.GetComponent<T>();
    }
    #endregion  // HELPER FUNCTIONS










    #region SETTERS AND GETTERS
    public static CurvedUIInputModule Instance {
        get
        {
            if (instance == null)
                instance = EnableInputModule<CurvedUIInputModule>();

            return instance;
        }
        private set { instance = value; }
    }

    /// <summary>
    /// When in CUSTOM_RAY controller mode, RayCaster will use this worldspace Ray to determine which Canvas objects are being selected.
    /// </summary>
    public static Ray CustomControllerRay {
        get { return Instance.customControllerRay; }
        set  {  Instance.customControllerRay = value;  }
    }

    /// <summary>
    /// When in CUSTOM_RAY controller mode, Input module will use this wbool to determine whether interaction button is pressed.
    /// </summary>
    [System.Obsolete("Misspelled. Use CustomControllerButtonDown instead.")]
    public static bool CustromControllerButtonDown {
        get { return Instance.pressedDown; }
        set { Instance.pressedDown = value; }
    }

    /// <summary>
    /// When in CUSTOM_RAY controller mode, Input module will use this wbool to determine whether interaction button is pressed.
    /// </summary>
    public static bool CustomControllerButtonDown {
        get { return Instance.pressedDown; }
        set { Instance.pressedDown = value; }
    }

    /// <summary>
    /// Returns the position of the world space pointer in Canvas' local space. 
    /// You can use it to position an image on world space mouse pointer's position.
    /// </summary>
    public Vector2 WorldSpaceMouseInCanvasSpace {
        get { return worldSpaceMouseInCanvasSpace; }
        set
        {
            worldSpaceMouseInCanvasSpace = value;
            lastWorldSpaceMouseOnCanvas = value;
        }
    }

    /// <summary>
    /// The change in position of the world space mouse in canvas' units.
    /// Counted since the last frame.
    /// </summary>
    public Vector2 WorldSpaceMouseInCanvasSpaceDelta {
        get { return worldSpaceMouseInCanvasSpace - lastWorldSpaceMouseOnCanvas; }
    }

    /// <summary>
    /// How many units in Canvas space equals one unit in screen space.
    /// </summary>
    public float WorldSpaceMouseSensitivity {
        get { return worldSpaceMouseSensitivity; }
        set { worldSpaceMouseSensitivity = value; }
    }


    /// <summary>
    /// Current controller mode. Decides how user can interact with the canvas. 
    /// </summary>
    public static CUIControlMethod ControlMethod {
        get { return Instance.controlMethod; }
        set
        {
            if (Instance.controlMethod != value)
            {
                Instance.controlMethod = value;
#if CURVEDUI_VIVE
                if(value == CUIControlMethod.STEAMVR)
                    Instance.SetupViveControllers();
#endif 
            }
        }
    }

    /// <summary>
    /// Gameobject we're currently pointing at.
    /// </summary>
    public GameObject CurrentPointedAt {
        get { return currentPointedAt; }
    }

    /// <summary>
    /// Which VR Controller can be used to interact with canvas. Left, Right or Both. Default Right.
    /// </summary>
    public Hand UsedHand {
        get { return usedHand; }
        set { usedHand = value;  }
    }

    /// <summary>
    /// Gaze Control Method. Should execute OnClick events on button after user points at them?
    /// </summary>
    public bool GazeUseTimedClick {
        get { return gazeUseTimedClick; }
        set { gazeUseTimedClick = value; }
    }

    /// <summary>
    /// Gaze Control Method. How long after user points on a button should we click it? Default 2 seconds.
    /// </summary>
    public float GazeClickTimer {
        get { return gazeClickTimer; }
        set { gazeClickTimer = Mathf.Max(value, 0); }
    }

    /// <summary>
    /// Gaze Control Method. How long after user looks at a button should we start the timer? Default 1 second.
    /// </summary>
    public float GazeClickTimerDelay {
        get { return gazeClickTimerDelay; }
        set { gazeClickTimerDelay = Mathf.Max(value, 0); }
    }

    /// <summary>
    /// Gaze Control Method. How long till Click method is executed on Buttons under gaze? Goes 0-1.
    /// </summary>
    public float GazeTimerProgress {
        get { return gazeTimerProgress; }
    }

    /// <summary>
    /// Gaze Control Method. This Images's fill will be animated 0-1 when OnClick events are about
    /// to be executed on buttons under the gaze.
    /// </summary>
    public Image GazeTimedClickProgressImage {
        get { return gazeTimedClickProgressImage; }
        set { gazeTimedClickProgressImage = value; }
    }



#if CURVEDUI_VIVE
    /// <summary>
    /// Scene's controller manager. Used to get references for Vive controllers.
    /// </summary>
    public SteamVR_ControllerManager SteamVRControllerManager {
            get { return steamVRControllerManager; }
            set
            {
                if (steamVRControllerManager != value)  {
                    steamVRControllerManager = value;
                }
            }
        }

        /// <summary>
        /// Get or Set controller manager used by this input module.
        /// </summary>
        public SteamVR_ControllerManager ControllerManager {
            get { return controllerManager; }
            set
            {
                controllerManager = value;
                SetupViveControllers();
            }
        }
   
        /// <summary>
        /// Returns Right SteamVR Controller. Ask this component for any button states.;
        /// </summary>
        public static CurvedUIViveController Right {
            get {

                if (!rightCont)
                 rightCont = controllerManager.right.AddComponentIfMissing<CurvedUIViveController>();

                return rightCont ; }
        }

        /// <summary>
        /// Returns Left SteamVR Controller. Ask this component for any button states.;
        /// </summary>
        public static CurvedUIViveController Left {
            get {

                if (!leftCont)
                  leftCont = controllerManager.left.AddComponentIfMissing<CurvedUIViveController>();

                return leftCont; }
        }  
#endif // CURVEDUI_VIVE




#if CURVEDUI_TOUCH
    public OVRCameraRig OculusCameraRig {
        get { return oculusCameraRig; }
        set {  oculusCameraRig = value;  }
    }


    public OVRInput.Button OculusTouchInteractionButton {
        get { return InteractionButton; }
        set  {  InteractionButton = value;  }
    }

    public Transform OculusTouchUsedControllerTransform {
        get { return UsedHand == Hand.Left ? oculusCameraRig.leftHandAnchor : oculusCameraRig.rightHandAnchor; }
    }
#endif // CURVEDUI_TOUCH




#endregion // end of SETTERS AND GETTERS




#region ENUMS
    //enums
    public enum CUIControlMethod
{
	MOUSE = 0,
	GAZE = 1,
	WORLD_MOUSE = 2,
	CUSTOM_RAY = 3,
	STEAMVR = 4,
	OCULUSVR = 5,
	//DAYDREAM = 6,//deprecated, GoogleVR is now used for daydream.
	GOOGLEVR = 7,
}

public enum Hand
{
	Both = 0,
	Right = 1,
	Left = 2,
}
#endregion // ENUMS

}
