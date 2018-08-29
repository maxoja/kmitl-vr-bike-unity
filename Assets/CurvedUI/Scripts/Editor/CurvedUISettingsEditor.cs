using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.EventSystems;

#if CURVEDUI_TMP || TMP_PRESENT
using TMPro;
#endif 


namespace CurvedUI { 
 

[CustomEditor(typeof(CurvedUISettings))]
    [ExecuteInEditMode]
    public class CurvedUISettingsEditor : Editor {

#pragma warning disable 414
        bool ShowRemoveCurvedUI = false;
        static bool ShowAdvaced = false;
		bool loadingCustomDefine = false;
        static bool CUIeventSystemPresent = false;
#pragma warning restore 414



        #region LIFECYCLE

        void Awake()
		{
			AddCurvedUIComponents();


        }
			
		void OnEnable()
		{

            loadingCustomDefine = false;

            //look for our custom eventsystem, if it makes sense to have it
            if (PlayerSettings.virtualRealitySupported)
            {
                CUIeventSystemPresent = (FindObjectsOfType(typeof(CurvedUIEventSystem)).Length > 0);
                //Debug.Log("OnEnable: found CUI Event system: " + CUIeventSystemPresent);
            }        

            //hacky way to make sure event is connected only once, but it works!
            EditorApplication.hierarchyWindowChanged -= AddCurvedUIComponents;
            EditorApplication.hierarchyWindowChanged -= AddCurvedUIComponents;
            EditorApplication.hierarchyWindowChanged += AddCurvedUIComponents;
        }

		#endregion 




        public override void OnInspectorGUI() {

        CurvedUISettings myTarget = (CurvedUISettings)target;

        if (target == null) return;

        //initial settings
        GUI.changed = false;
        EditorGUIUtility.labelWidth = 150;

            //show version
            GUILayout.Label("Version 2.6p2", EditorStyles.miniLabel);

            //vr event system warning
            if (PlayerSettings.virtualRealitySupported && CUIeventSystemPresent == false) //vr enabled reports wrong value on some versions
            {
                EditorGUILayout.HelpBox("Unity UI may become unresponsive in VR if game window loses focus. Use CurvedUIEventSystem instead of standard EventSystem component to solve this issue.", MessageType.Warning);
                GUILayout.BeginHorizontal();
                GUILayout.Space(146);
                if (GUILayout.Button("Use CurvedUI Event System")) SwapEventSystem();
                GUILayout.EndHorizontal();
                GUILayout.Space(30);
            }

            //global setting - control methods
            DrawControlMethods();

            //shape settings
            GUILayout.Label("Shape", EditorStyles.boldLabel);
            myTarget.Shape = (CurvedUISettings.CurvedUIShape)EditorGUILayout.EnumPopup("Canvas Shape", myTarget.Shape);
            switch (myTarget.Shape)
            {
                case CurvedUISettings.CurvedUIShape.CYLINDER:
                {
                    myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, -360, 360);
                    myTarget.PreserveAspect = EditorGUILayout.Toggle("Preserve Aspect", myTarget.PreserveAspect);

                    break;
                }
                case CurvedUISettings.CurvedUIShape.CYLINDER_VERTICAL:
                {
                    myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, -360, 360);
                    myTarget.PreserveAspect = EditorGUILayout.Toggle("Preserve Aspect", myTarget.PreserveAspect);

                    break;
                }
                case CurvedUISettings.CurvedUIShape.RING:
                {
                    myTarget.RingExternalDiameter = Mathf.Clamp(EditorGUILayout.IntField("External Diameter", myTarget.RingExternalDiameter), 1, 100000);
                    myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, 0, 360);
                    myTarget.RingFill = EditorGUILayout.Slider("Fill", myTarget.RingFill, 0.0f, 1.0f);
                    myTarget.RingFlipVertical = EditorGUILayout.Toggle("Flip Canvas Vertically", myTarget.RingFlipVertical);
                    break;
                }
                case CurvedUISettings.CurvedUIShape.SPHERE:
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(150);
                    EditorGUILayout.HelpBox("Sphere shape is more expensive than a Cyllinder shape. Keep this in mind when working on mobile VR.", MessageType.Info);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(10);

                    if (myTarget.PreserveAspect)
                    {
                        myTarget.Angle = EditorGUILayout.IntSlider("Angle", myTarget.Angle, -360, 360);
                    }
                    else {
                        myTarget.Angle = EditorGUILayout.IntSlider("Horizontal Angle", myTarget.Angle, 0, 360);
                        myTarget.VerticalAngle = EditorGUILayout.IntSlider("Vertical Angle", myTarget.VerticalAngle, 0, 180);
                    }

                    myTarget.PreserveAspect = EditorGUILayout.Toggle("Preserve Aspect", myTarget.PreserveAspect);
                    break;
                }
            }

            //180 degree warning
            if ((myTarget.Shape != CurvedUISettings.CurvedUIShape.RING && myTarget.Angle.Abs() > 180) ||
                (myTarget.Shape == CurvedUISettings.CurvedUIShape.SPHERE && myTarget.VerticalAngle > 180))
                Draw180DegreeWarning();


            //advanced settings
            GUILayout.Space(30);

        if (!ShowAdvaced) {
			//show advanced settings enable button
            if (GUILayout.Button("Show Advanced Settings")) {
                ShowAdvaced = true;
				loadingCustomDefine = false;
             }

        } else {
			// or just show advanced settings

			//hide advances settings button.
            if (GUILayout.Button("Hide Advanced Settings")) ShowAdvaced = false;
                GUILayout.Space(20);


			//common options
            GUILayout.Label("Other Options", EditorStyles.boldLabel);
            myTarget.Interactable = EditorGUILayout.Toggle("Interactable", myTarget.Interactable);
			myTarget.BlocksRaycasts = EditorGUILayout.Toggle("Blocks Raycasts", myTarget.BlocksRaycasts);
            myTarget.RaycastMyLayerOnly = EditorGUILayout.Toggle("Raycast My Layer Only", myTarget.RaycastMyLayerOnly);
            if(myTarget.Shape != CurvedUISettings.CurvedUIShape.SPHERE) myTarget.ForceUseBoxCollider = EditorGUILayout.Toggle("Force Box Colliders Use", myTarget.ForceUseBoxCollider);
            myTarget.Quality = EditorGUILayout.Slider("Quality", myTarget.Quality, 0.1f, 3.0f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(150);
            GUILayout.Label("Smoothness of the curve. Bigger values mean more subdivisions. Decrease for better performance. Default 1", EditorStyles.helpBox);
            GUILayout.EndHorizontal();


			//add components button
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Components", GUILayout.Width(146));
			if (GUILayout.Button("Add Effect To Children"))AddCurvedUIComponents();
            GUILayout.EndHorizontal();


			//remove components button
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(146));
            if (!ShowRemoveCurvedUI) {
                if (GUILayout.Button("Remove CurvedUI from Canvas")) ShowRemoveCurvedUI = true;
            } else {
                if (GUILayout.Button("Remove CurvedUI"))  {
                    RemoveCurvedUIComponents();
                }
                if (GUILayout.Button("Cancel"))  {
                    ShowRemoveCurvedUI = false;
                }
            }
            GUILayout.EndHorizontal();

            //documentation link
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Documentation", GUILayout.Width(146));
            if (GUILayout.Button("Open in web browser")) Help.BrowseURL("https://docs.google.com/document/d/10hNcvOMissNbGgjyFyV1MS7HwkXXE6270A6Ul8h8pnQ/edit");
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            //final settings
            if (GUI.changed && myTarget != null)
                EditorUtility.SetDirty(myTarget);

        }

	
    void SwapEventSystem()
    {
            if (Application.isPlaying)
            {
                Debug.LogError("Cant do this in Play mode!");
                return;
            }

            EventSystem system = FindObjectOfType<EventSystem>();
            if(!(system is CurvedUIEventSystem))
            {
                system.AddComponentIfMissing<CurvedUIEventSystem>();
                DestroyImmediate(system);
            }

            CUIeventSystemPresent = true;
        }

    void DrawControlMethods()
        {
            GUILayout.Label("Global Settings", EditorStyles.boldLabel);

            //controller
            CurvedUIInputModule.ControlMethod = (CurvedUIInputModule.CUIControlMethod)EditorGUILayout.EnumPopup("Control Method", CurvedUIInputModule.ControlMethod);
            GUILayout.BeginHorizontal();
            GUILayout.Space(150);
			GUILayout.BeginVertical();

            switch (CurvedUIInputModule.ControlMethod)
            {
                case CurvedUIInputModule.CUIControlMethod.MOUSE:
                {

#if CURVEDUI_GOOGLEVR
					EditorGUILayout.HelpBox("Enabling this control method will disable GoogleVR support.", MessageType.Warning);
					DrawCustomDefineSwitcher("");
#else
                    GUILayout.Label("Basic Controller. Mouse on screen", EditorStyles.helpBox);
                    #endif
                    break;
                }
                case CurvedUIInputModule.CUIControlMethod.GAZE:
                {
#if CURVEDUI_GOOGLEVR
					EditorGUILayout.HelpBox("Enabling this control method will disable GoogleVR support.", MessageType.Warning);
					DrawCustomDefineSwitcher("");
#else
                    GUILayout.Label("Center of Canvas's Event Camera acts as a pointer. This is a generic gaze implementation, to be used with any headset. If you're on cardboard, use GOOGLEVR control method for Reticle and GameObject interaction support.", EditorStyles.helpBox);
                    CurvedUIInputModule.Instance.GazeUseTimedClick = EditorGUILayout.Toggle("Use Timed Click", CurvedUIInputModule.Instance.GazeUseTimedClick);
                    if (CurvedUIInputModule.Instance.GazeUseTimedClick)
                    {
                        GUILayout.Label("Clicks a button if player rests his gaze on it for a period of time. You can assign an image to be used as a progress bar.", EditorStyles.helpBox);
                        CurvedUIInputModule.Instance.GazeClickTimer = EditorGUILayout.FloatField("Click Timer (seconds)", CurvedUIInputModule.Instance.GazeClickTimer);
                        CurvedUIInputModule.Instance.GazeClickTimerDelay = EditorGUILayout.FloatField("Timer Start Delay", CurvedUIInputModule.Instance.GazeClickTimerDelay);
                        CurvedUIInputModule.Instance.GazeTimedClickProgressImage = (UnityEngine.UI.Image)EditorGUILayout.ObjectField("Progress Image To FIll", CurvedUIInputModule.Instance.GazeTimedClickProgressImage, typeof(UnityEngine.UI.Image), true);
                    }
#endif
                    break;
                    
                }
                case CurvedUIInputModule.CUIControlMethod.WORLD_MOUSE:
                {

#if CURVEDUI_GOOGLEVR
					EditorGUILayout.HelpBox("Enabling this control method will disable GoogleVR support.", MessageType.Warning);
					DrawCustomDefineSwitcher("");
#else
                    GUILayout.Label("Mouse controller that is independent of the camera view. Use WorldSpaceMouseOnCanvas function to get its position.", EditorStyles.helpBox);
                    CurvedUIInputModule.Instance.WorldSpaceMouseSensitivity = EditorGUILayout.FloatField("Mouse Sensitivity", CurvedUIInputModule.Instance.WorldSpaceMouseSensitivity);
					#endif
					break;
                }
                case CurvedUIInputModule.CUIControlMethod.CUSTOM_RAY:
                {
#if CURVEDUI_GOOGLEVR
					EditorGUILayout.HelpBox("Enabling this control method will disable GoogleVR support.", MessageType.Warning);
					DrawCustomDefineSwitcher("");
#else
                    GUILayout.Label("Set a ray used to interact with canvas using CustomControllerRay function. Use CustomControllerButtonDown bool to set button pressed state. Find both in CurvedUIInputModule class", EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    //GUILayout.Space(20);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("View code snippet")) Help.BrowseURL("https://docs.google.com/document/d/10hNcvOMissNbGgjyFyV1MS7HwkXXE6270A6Ul8h8pnQ/edit#heading=h.b164qm67xp15");
                    GUILayout.EndHorizontal();
#endif
                    break;
                }
                case CurvedUIInputModule.CUIControlMethod.STEAMVR:
                {
#if CURVEDUI_VIVE
                    // vive enabled, we can show settings
                    GUILayout.Label("Use one or both vive controllers as to interact with canvas. Trigger acts a button", EditorStyles.helpBox);
                    CurvedUIInputModule.Instance.UsedHand = (CurvedUIInputModule.Hand)EditorGUILayout.EnumPopup("Used Controller", CurvedUIInputModule.Instance.UsedHand);

#else
                    DrawCustomDefineSwitcher("CURVEDUI_VIVE");
#endif
                    break;
                }
                case CurvedUIInputModule.CUIControlMethod.OCULUSVR:
                {

#if CURVEDUI_TOUCH
                    // oculus enabled, we can show settings
                    GUILayout.Label("Use Oculus Touch or GearVR controller to interact with canvas.", EditorStyles.helpBox);
                    //hand property
                    CurvedUIInputModule.Instance.UsedHand = (CurvedUIInputModule.Hand)EditorGUILayout.EnumPopup("Hand", CurvedUIInputModule.Instance.UsedHand);
                    //button property
                    CurvedUIInputModule.Instance.OculusTouchInteractionButton = (OVRInput.Button)EditorGUILayout.EnumPopup("Interaction Button", CurvedUIInputModule.Instance.OculusTouchInteractionButton);
#else
                    DrawCustomDefineSwitcher("CURVEDUI_TOUCH");
#endif
                    break;
                }
				case CurvedUIInputModule.CUIControlMethod.GOOGLEVR:
				{

#if CURVEDUI_GOOGLEVR
					GUILayout.Label("Use GoogleVR Reticle to interact with canvas. Requires GoogleVR SDK 1.110 or later.", EditorStyles.helpBox);
#else
                    DrawCustomDefineSwitcher("CURVEDUI_GOOGLEVR");
#endif
					break;
				}
            }

			GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
        }



		/// <summary>
		/// Draws the define switcher for different control methods. 
		/// Because different control methods use different API's that may not always be available, 
		/// CurvedUI needs to be recompile with different custom defines to fix this. This method 
		/// manages the defines.
		/// </summary>
		/// <param name="switcho">Switcho.</param>
		void DrawCustomDefineSwitcher(string switcho)
		{
			GUILayout.BeginVertical();
			GUILayout.Label("Press the button below to make CurvedUI recompile for this control method. It should take less than 30 seconds. Afterwards, you'll see its settings here.", EditorStyles.helpBox);

			GUILayout.BeginHorizontal();
			GUILayout.Space(50);
			if (GUILayout.Button(loadingCustomDefine ? "Please wait..." : "Enable."))
			{
				loadingCustomDefine = true;
				string str = "";
				str += PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

				//remove curvedui defines
				foreach(string define in new string[]{"CURVEDUI_VIVE","CURVEDUI_TOUCH","CURVEDUI_GOOGLEVR"}){
					if (str.Contains(define))
					{
						if(str.Contains((";" + define)))
							str = str.Replace((";" + define), "");
						else
 							str = str.Replace(define, "");
					}
				}


				//add this one, if not present.
				if (switcho != "" && !str.Contains(switcho))
                    str += ";" + switcho;

                //Debug.Log ("submitted str: " + str);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, str);

			}
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
        }

        void Draw180DegreeWarning()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(150);
            EditorGUILayout.HelpBox("Cavas with angle bigger than 180 degrees will not be interactable. \n" +
                "This is caused by Unity Event System requirements. Use two canvases facing each other for fully interactive 360 degree UI.", MessageType.Warning);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        bool CustomDefinePresent(string define)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Contains(define);
        }


    /// <summary>
    ///Travel the hierarchy and add CurvedUIVertexEffect to every gameobject that can be bent.
    /// </summary>
    private void AddCurvedUIComponents()
        {
            if (target == null) return;

            (target as CurvedUISettings).AddEffectToChildren();

        }



		/// <summary>
		/// Removes all CurvedUI components from this canvas.
		/// </summary>
        private void RemoveCurvedUIComponents()
   		{
	        if (target == null) return;

            //destroy TMP objects
            List<CurvedUITMP> tmps = new List<CurvedUITMP>();
            tmps.AddRange((target as CurvedUISettings).GetComponentsInChildren<CurvedUITMP>(true));
            for (int i = 0; i < tmps.Count; i++)
            {
                DestroyImmediate(tmps[i]);
            }

            List<CurvedUITMPSubmesh> submeshes = new List<CurvedUITMPSubmesh>();
            submeshes.AddRange((target as CurvedUISettings).GetComponentsInChildren<CurvedUITMPSubmesh>(true));
            for (int i = 0; i < submeshes.Count; i++)
            {
                DestroyImmediate(submeshes[i]);
            }

            //destroy curving componenets
            List<CurvedUIVertexEffect> comps = new List<CurvedUIVertexEffect>();
	        comps.AddRange((target as CurvedUISettings).GetComponentsInChildren<CurvedUIVertexEffect>(true));
	        for (int i = 0; i < comps.Count; i++)
	        {
	            if (comps[i].GetComponent<UnityEngine.UI.Graphic>() != null) comps[i].GetComponent<UnityEngine.UI.Graphic>().SetAllDirty();
	            DestroyImmediate(comps[i]);            
	        }
  

            //destroy raycasters
            List<CurvedUIRaycaster> raycasters = new List<CurvedUIRaycaster>();
	        raycasters.AddRange((target as CurvedUISettings).GetComponents<CurvedUIRaycaster>());
	        for (int i = 0; i < raycasters.Count; i++)
	        {
	            DestroyImmediate(raycasters[i]);
	        }

	        DestroyImmediate(target);
   		}

     
    }
}

