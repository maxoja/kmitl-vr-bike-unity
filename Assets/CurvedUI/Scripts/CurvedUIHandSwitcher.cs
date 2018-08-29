using UnityEngine;
using System.Collections;

namespace CurvedUI {

    /// <summary>
    /// This script switches the hand controlling the UI when a click on the other controller's trigger is detected.
    /// This emulates the functionality seen in SteamVR overlay or Oculus Home.
    /// Works both for SteamVR and Oculus SDK.
    /// </summary>
    public class CurvedUIHandSwitcher : MonoBehaviour
    {

        [SerializeField]
        GameObject LaserBeam;

#pragma warning disable 414
        [SerializeField]
        [Tooltip("If true, when player clicks the trigger on the other hand, we'll instantly set it as UI controlling hand and move the pointer to it.")]
        bool autoSwitchHands = true;
#pragma warning restore 414


#if CURVEDUI_TOUCH
       //variables
        OVRInput.Controller activeCont;
        bool initialized = false;

        void Update()
        {
            if (CurvedUIInputModule.ControlMethod != CurvedUIInputModule.CUIControlMethod.OCULUSVR) return;

            activeCont = OVRInput.GetActiveController();

            if (!initialized && CurvedUIInputModule.Instance.OculusTouchUsedControllerTransform != null)
            {
                //Launch Hand Switch. This will place the laser pointer in the current hand.
                SwitchHandTo(CurvedUIInputModule.Instance.UsedHand);

                initialized = true;
            }


            //for Oculus Go and GearVR, switch automatically if a different controller is connected.
            //This covers the case where User changes hand setting in Oculus Go menu and gets back to our app.
            if (activeCont == OVRInput.Controller.LTrackedRemote && CurvedUIInputModule.Instance.UsedHand != CurvedUIInputModule.Hand.Left)
                SwitchHandTo(CurvedUIInputModule.Hand.Left);
            else if (activeCont == OVRInput.Controller.RTrackedRemote && CurvedUIInputModule.Instance.UsedHand != CurvedUIInputModule.Hand.Right)
                SwitchHandTo(CurvedUIInputModule.Hand.Right);

            if(autoSwitchHands){
                //For Oculus Rift, we wait for the click before we change the pointer.
                if (IsButtonDownOnController(OVRInput.Controller.LTouch) && CurvedUIInputModule.Instance.UsedHand != CurvedUIInputModule.Hand.Left)
                {
                   SwitchHandTo(CurvedUIInputModule.Hand.Left);
                }
                else if (IsButtonDownOnController(OVRInput.Controller.RTouch) && CurvedUIInputModule.Instance.UsedHand != CurvedUIInputModule.Hand.Right)
                {
                   SwitchHandTo(CurvedUIInputModule.Hand.Right);
                }
            }
            
        }

        void SwitchHandTo(CurvedUIInputModule.Hand newHand)
        {
            CurvedUIInputModule.Instance.UsedHand = newHand;
            LaserBeam.transform.SetParent(CurvedUIInputModule.Instance.OculusTouchUsedControllerTransform);
            LaserBeam.transform.ResetTransform();
        }

        bool IsButtonDownOnController(OVRInput.Controller cont, OVRInput.Controller cont2 = OVRInput.Controller.None)
        {
            return OVRInput.GetDown(CurvedUIInputModule.Instance.OculusTouchInteractionButton, cont) || (cont2 != OVRInput.Controller.None && OVRInput.GetDown(CurvedUIInputModule.Instance.OculusTouchInteractionButton, cont2));
        }
#elif CURVEDUI_VIVE
        void Start()
        {
            //connect to steamVR's OnModelLoaded events so we can update the pointer the moment controller is detected.
            CurvedUIInputModule.Right.ModelLoaded += OnModelLoaded;
            CurvedUIInputModule.Left.ModelLoaded += OnModelLoaded;
        }

        void OnModelLoaded(object sender)
        {
            SwitchHandTo(CurvedUIInputModule.Instance.UsedHand);
        }

        void Update()
        {
        
            if (CurvedUIInputModule.ControlMethod != CurvedUIInputModule.CUIControlMethod.STEAMVR) return;

            if(autoSwitchHands){

                if (CurvedUIInputModule.Right != null && CurvedUIInputModule.Right.IsTriggerDown && CurvedUIInputModule.Instance.UsedHand != CurvedUIInputModule.Hand.Right)
                {
                    SwitchHandTo(CurvedUIInputModule.Hand.Right);

                }
                else if (CurvedUIInputModule.Left != null && CurvedUIInputModule.Left.IsTriggerDown && CurvedUIInputModule.Instance.UsedHand != CurvedUIInputModule.Hand.Left)
                {
                    SwitchHandTo(CurvedUIInputModule.Hand.Left);

                }
            }
        }

        void SwitchHandTo(CurvedUIInputModule.Hand newHand)
        {
            if (newHand == CurvedUIInputModule.Hand.Right)
            {
                CurvedUIInputModule.Instance.UsedHand = CurvedUIInputModule.Hand.Right;
                LaserBeam.transform.SetParent(CurvedUIInputModule.Right.transform);
                LaserBeam.transform.ResetTransform();
                LaserBeam.transform.position = CurvedUIInputModule.Right.PointingOrigin;
                LaserBeam.transform.LookAt(LaserBeam.transform.position + CurvedUIInputModule.Right.PointingDirection);
            }
            else
            {
                CurvedUIInputModule.Instance.UsedHand = CurvedUIInputModule.Hand.Left;
                LaserBeam.transform.SetParent(CurvedUIInputModule.Left.transform);
                LaserBeam.transform.ResetTransform();
                LaserBeam.transform.position = CurvedUIInputModule.Left.PointingOrigin;
                LaserBeam.transform.LookAt(LaserBeam.transform.position + CurvedUIInputModule.Left.PointingDirection);
            }
        }
#endif

    }

}


