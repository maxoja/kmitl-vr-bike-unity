﻿using UnityEngine.PostProcessing;
using UnityEngine;

public class PostProcessingController : MonoBehaviour 
{
    public PostProcessingProfile profile;
    public DepthOfFieldModel.Settings waitingPreset;
    public DepthOfFieldModel.Settings playingPreset;

    public bool isPlaying = false;

    [Range(0,1)]
    public float ratioToPlaying = 0;

    void Awake()
    {
        ratioToPlaying = isPlaying ? 1 : 0;
    }

	void Update () 
    {
        UpdateRatio();

        DepthOfFieldModel.Settings tempSetting;
        tempSetting = profile.depthOfField.settings;
        tempSetting.focusDistance = Mathf.Lerp(waitingPreset.focusDistance, playingPreset.focusDistance, ratioToPlaying);
        tempSetting.aperture = Mathf.Lerp(waitingPreset.aperture, playingPreset.aperture, ratioToPlaying);
        tempSetting.focalLength = Mathf.Lerp(waitingPreset.focalLength, playingPreset.focalLength, ratioToPlaying);
        tempSetting.kernelSize = ratioToPlaying < 0.5f? waitingPreset.kernelSize : playingPreset.kernelSize;
        tempSetting.useCameraFov = ratioToPlaying < 0.5f ? waitingPreset.useCameraFov : playingPreset.useCameraFov;
        profile.depthOfField.settings = tempSetting;
    }

    private void UpdateRatio()
    {
        if (isPlaying)
            ratioToPlaying += Time.deltaTime;
        else
            ratioToPlaying -= Time.deltaTime;
        
        ratioToPlaying = Mathf.Clamp(ratioToPlaying, 0, 1);
    }

    void UseReadyPreset()
    {
        isPlaying = false;
    }

    void UseFinishedPreset()
    {
        isPlaying = true;
    }
}