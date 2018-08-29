using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BezierWalker))]
public class PlayerSync : MonoBehaviour 
{
    private BezierWalker walkerComp;
    public Transform cameraTrans;
    public Transform cameraParent;
    private Transform headTrans;
    private CurveDisplay curveDisplay;
    private FrontDisplay frontDisplay;
    private Animator animator;

    public int id;
    public bool isMonitor = false;

    void Awake()
    {
        walkerComp = GetComponent<BezierWalker>();
        headTrans = (GetComponentInChildren(typeof(HeadTransform),true) as HeadTransform).transform;
        curveDisplay = GetComponentInChildren(typeof(CurveDisplay),true) as CurveDisplay;
        frontDisplay = GetComponentInChildren(typeof(FrontDisplay),true) as FrontDisplay;
        animator = GetComponentInChildren(typeof(Animator),true) as Animator;
           
        if(isMonitor)
        {
            //regardless to whoever
            cameraTrans.gameObject.SetActive(true);
            cameraTrans.parent = cameraParent;
            cameraTrans.position = cameraParent.position;
            cameraTrans.rotation = cameraParent.rotation;
            curveDisplay.gameObject.SetActive(true);
            frontDisplay.gameObject.SetActive(true);
        }
        else
        {
            if (id == StaticData.playerId)
            {
                cameraTrans.parent = cameraParent;
                cameraTrans.localPosition = Vector3.zero;
                cameraTrans.localRotation = Quaternion.identity;
                curveDisplay.gameObject.SetActive(true);
                frontDisplay.gameObject.SetActive(true);
            }
            else
            {
                curveDisplay.gameObject.SetActive(false);
                frontDisplay.gameObject.SetActive(false);
            }
        }

    }

    void Update()
    {
        switch(GameData.gameState)
        {
            case GameData.GameState.READY:
                curveDisplay.ChangeMode(CurveDisplay.Mode.Waiting);
                break;
            case GameData.GameState.LAUNCHING:
                curveDisplay.ChangeMode(CurveDisplay.Mode.Launching);
                break;
            case GameData.GameState.PLAYING_NO_WINNER:
                break;
            default:
                if(GameData.players[id].playerState == PlayerData.PlayerState.FINISHED)
                    curveDisplay.ChangeMode(CurveDisplay.Mode.Finished);
                break;
        }

        //set head rotation;
        if (isMonitor)
        {
            cameraTrans.localRotation = Quaternion.Lerp(cameraTrans.localRotation, GameData.players[id].headsetRot, Time.deltaTime);
            Vector3 standardEuler = new Vector3(-34.95f, -1.862f, 4.79f);
            Quaternion standardLocalRotation = Quaternion.Euler(standardEuler);
            headTrans.localRotation = Quaternion.Lerp(headTrans.localRotation, standardLocalRotation * GameData.players[id].headsetRot, Time.deltaTime);
        }
        else
        {
            if(id == StaticData.playerId)
                headTrans.localScale = Vector3.zero;
            else
            {
                Vector3 standardEuler = new Vector3(-34.95f, -1.862f, 4.79f);
                Quaternion standardLocalRotation = Quaternion.Euler(standardEuler);
                headTrans.localRotation = Quaternion.Lerp(headTrans.localRotation, standardLocalRotation * GameData.players[id].headsetRot, Time.deltaTime);
            }
        }

        //set position
        walkerComp.SetSpeed(0);
        walkerComp.SetProgress(Mathf.Lerp(walkerComp.GetProgress(),GameData.players[id].zPosition, Time.deltaTime));

        //set anim speed
        if (GameData.gameState == GameData.GameState.READY || GameData.gameState == GameData.GameState.LAUNCHING || GameData.players[id].playerState == PlayerData.PlayerState.FINISHED)
            animator.speed = 0;
        else
            animator.speed = Mathf.Lerp(animator.speed,GameData.players[id].zVelocity * 22, Time.deltaTime);
    }
}
