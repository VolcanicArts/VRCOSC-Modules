// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.OpenVR.Device;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("Index Gesture Extensions")]
[ModuleDescription("Detect a range of custom gestures from Index controllers")]
[ModuleType(ModuleType.SteamVR)]
[ModuleInfo("https://vrcosc.com/docs/v2/modules/gesture-extensions")]
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
        var openvrManager = GetOpenVRManager();
        var threshold = GetSettingValue<float>(GestureExtensionsSetting.Threshold);

        var lc = openvrManager.GetLeftController();
        var rc = openvrManager.GetRightController();

        if (lc is not null) SendParameter(GestureExtensionsParameter.GestureLeft, (int)getControllerGesture(lc.Input, threshold));
        if (rc is not null) SendParameter(GestureExtensionsParameter.GestureRight, (int)getControllerGesture(rc.Input, threshold));
    }

    private static GestureNames getControllerGesture(InputState input, float threshold)
    {
        if (isGestureDoubleGun(input, threshold)) return GestureNames.DoubleGun;
        if (isGestureMiddleFinger(input, threshold)) return GestureNames.MiddleFinger;
        if (isGesturePinkyFinger(input, threshold)) return GestureNames.PinkyFinger;

        return GestureNames.None;
    }

    private static bool isGestureDoubleGun(InputState input, float threshold) =>
        input.Skeleton.Index <= threshold
        && input.Skeleton.Middle <= threshold
        && input.Skeleton.Ring > threshold
        && input.Skeleton.Pinky > threshold
        && !input.Primary.Touch && !input.Secondary.Touch && !input.Stick.Touch && !input.Pad.Touch;

    private static bool isGestureMiddleFinger(InputState input, float threshold) =>
        input.Skeleton.Index > threshold
        && input.Skeleton.Middle <= threshold
        && input.Skeleton.Ring > threshold
        && input.Skeleton.Pinky > threshold;

    private static bool isGesturePinkyFinger(InputState input, float threshold) =>
        input.Skeleton.Index > threshold
        && input.Skeleton.Middle > threshold
        && input.Skeleton.Ring > threshold
        && input.Skeleton.Pinky <= threshold;

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
        None = 0,
        DoubleGun = 1,
        MiddleFinger = 2,
        PinkyFinger = 3
    }
}