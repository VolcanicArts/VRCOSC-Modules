// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.OVR.Input;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("Index Gesture Extensions")]
[ModuleDescription("Detect a range of custom gestures from Index controllers")]
[ModuleType(ModuleType.Integrations)]
public class GestureExtensionsModule : Module
{
    protected override void OnPreLoad()
    {
        CreateSlider(GestureExtensionsSetting.Threshold, "Threshold", "How far down a finger should be to be considered down\n0 being fully up. 1 being full down", 0.5f, 0, 1, 0.01f);

        RegisterParameter<int>(GestureExtensionsParameter.GestureLeft, "VRCOSC/VR/Gestures/Left", ParameterMode.Write, "Left Gestures", "Custom left hand gesture value");
        RegisterParameter<int>(GestureExtensionsParameter.GestureRight, "VRCOSC/VR/Gestures/Right", ParameterMode.Write, "Right Gestures", "Custom right hand gesture value");
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

    private float getThreshold() => GetSettingValue<float>(GestureExtensionsSetting.Threshold);

    private bool isGestureDoubleGun(InputStates input) =>
        input.IndexFinger <= getThreshold()
        && input.MiddleFinger <= getThreshold()
        && input.RingFinger > getThreshold()
        && input.PinkyFinger > getThreshold()
        && input.ThumbUp;

    private bool isGestureMiddleFinger(InputStates input) =>
        input.IndexFinger > getThreshold()
        && input.MiddleFinger <= getThreshold()
        && input.RingFinger > getThreshold()
        && input.PinkyFinger > getThreshold();

    private bool isGesturePinkyFinger(InputStates input) =>
        input.IndexFinger > getThreshold()
        && input.MiddleFinger > getThreshold()
        && input.RingFinger > getThreshold()
        && input.PinkyFinger <= getThreshold();

    private enum GestureExtensionsSetting
    {
        Threshold
    }

    private enum GestureExtensionsParameter
    {
        GestureLeft,
        GestureRight
    }

    private enum GestureNames
    {
        None,
        DoubleGun,
        MiddleFinger,
        PinkyFinger
    }
}
