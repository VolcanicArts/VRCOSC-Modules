// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.OVR.Input;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("Gesture Extensions")]
[ModuleDescription("Detect a range of custom gestures from Index controllers")]
[ModuleType(ModuleType.Integrations)]
public class GestureExtensionsModule : AvatarModule
{
    private float lowerThreshold;
    private float upperThreshold;

    protected override void OnPreLoad()
    {
        CreateSlider(GestureExtensionsSetting.LowerThreshold, "Lower Threshold", "How far down a finger should be until it's not considered up", 0.5f, 0, 1, 0.01f);
        CreateSlider(GestureExtensionsSetting.UpperThreshold, "Upper Threshold", "How far down a finger should be before it's considered down", 0.5f, 0, 1, 0.01f);

        RegisterParameter<int>(GestureExtensionsParameter.GestureLeft, "VRCOSC/Gestures/Left", ParameterMode.Write, "Left Gestures", "Custom left hand gesture value");
        RegisterParameter<int>(GestureExtensionsParameter.GestureRight, "VRCOSC/Gestures/Right", ParameterMode.Write, "Right Gestures", "Custom right hand gesture value");
    }

    protected override Task<bool> OnModuleStart()
    {
        lowerThreshold = GetSettingValue<float>(GestureExtensionsSetting.LowerThreshold);
        upperThreshold = GetSettingValue<float>(GestureExtensionsSetting.UpperThreshold);

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void sendParameters()
    {
        if (!OVRClient.HasInitialised) return;

        if (OVRClient.LeftController.IsConnected) SendParameter(GestureExtensionsParameter.GestureLeft, (int)getControllerGesture(OVRClient.LeftController.Input));
        if (OVRClient.RightController.IsConnected) SendParameter(GestureExtensionsParameter.GestureRight, (int)getControllerGesture(OVRClient.RightController.Input));
    }

    private GestureNames getControllerGesture(InputStates input)
    {
        if (isGestureDoubleGun(input)) return GestureNames.DoubleGun;
        if (isGestureMiddleFinger(input)) return GestureNames.MiddleFinger;
        if (isGesturePinkyFinger(input)) return GestureNames.PinkyFinger;

        return GestureNames.None;
    }

    private bool isGestureDoubleGun(InputStates input) =>
        input.IndexFinger < lowerThreshold
        && input.MiddleFinger < lowerThreshold
        && input.RingFinger > upperThreshold
        && input.PinkyFinger > upperThreshold
        && input.ThumbUp;

    private bool isGestureMiddleFinger(InputStates input) =>
        input.IndexFinger > upperThreshold
        && input.MiddleFinger < lowerThreshold
        && input.RingFinger > upperThreshold
        && input.PinkyFinger > upperThreshold;

    private bool isGesturePinkyFinger(InputStates input) =>
        input.IndexFinger > upperThreshold
        && input.MiddleFinger > upperThreshold
        && input.RingFinger > upperThreshold
        && input.PinkyFinger < lowerThreshold;

    private enum GestureExtensionsSetting
    {
        LowerThreshold,
        UpperThreshold
    }

    private enum GestureNames
    {
        None,
        DoubleGun,
        MiddleFinger,
        PinkyFinger
    }

    private enum GestureExtensionsParameter
    {
        GestureLeft,
        GestureRight
    }
}
