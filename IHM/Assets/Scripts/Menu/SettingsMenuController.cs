using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    public Toggle togglePlayerJumpAnimationActivated; 
    public Toggle togglePlayerClimbAnimationActivated;
    public Toggle togglePlayerTrailAnimationActivated;
    public Toggle toggleCrumblingPlatformShakeAnimation;
    public Toggle toggleCrumblingPlatformControllerShakeAnimation;

    void Start()
    {
        // Initialize the toggles based on the current state of the static booleans
        togglePlayerJumpAnimationActivated.isOn = FeedbackAnimationParameters.playerJumpAnimationActivated;
        togglePlayerClimbAnimationActivated.isOn = FeedbackAnimationParameters.playerClimbAnimationActivated;
        togglePlayerTrailAnimationActivated.isOn = FeedbackAnimationParameters.playerTrailAnimationActivated;
        toggleCrumblingPlatformShakeAnimation.isOn = FeedbackAnimationParameters.crumblingPlatformShakeAnimation;
        toggleCrumblingPlatformControllerShakeAnimation.isOn = FeedbackAnimationParameters.crumblingPlatformControllerShakeAnimation;
        // Add listeners to update the static booleans when the toggle changes
        togglePlayerJumpAnimationActivated.onValueChanged.AddListener(SetPlayerJumpAnimationActivated);
        togglePlayerClimbAnimationActivated.onValueChanged.AddListener(SetPlayerClimbAnimationActivated);
        togglePlayerTrailAnimationActivated.onValueChanged.AddListener(SetPlayerTrailAnimationActivated);
        toggleCrumblingPlatformShakeAnimation.onValueChanged.AddListener(SetCrumblingPlatformShakeAnimation);
        toggleCrumblingPlatformControllerShakeAnimation.onValueChanged.AddListener(SetCrumblingPlatformControllerShakeAnimation);
    }

    void SetPlayerJumpAnimationActivated(bool isOn)
    {
        FeedbackAnimationParameters.playerJumpAnimationActivated = isOn;
    }

    void SetPlayerClimbAnimationActivated(bool isOn)
    {
        FeedbackAnimationParameters.playerClimbAnimationActivated = isOn;
    }

    void SetPlayerTrailAnimationActivated(bool isOn)
    {
        FeedbackAnimationParameters.playerTrailAnimationActivated = isOn;
    }

    void SetCrumblingPlatformShakeAnimation(bool isOn)
    {
        FeedbackAnimationParameters.crumblingPlatformShakeAnimation = isOn;
    }

    void SetCrumblingPlatformControllerShakeAnimation(bool isOn)
    {
        FeedbackAnimationParameters.crumblingPlatformControllerShakeAnimation = isOn;
    }
}