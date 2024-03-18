// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("OpenVR Controller Statistics")]
[ModuleDescription("Gets controller statistics from your OpenVR (SteamVR) session")]
[ModuleType(ModuleType.Integrations)]
public class OpenVRControllerStatisticsModule : AvatarModule
{
    protected override void OnPreLoad()
    {
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.LeftATouch, "VRCOSC/OpenVR/LeftController/Input/A/Touch", ParameterMode.Write, "Left Controller A Touch", "Whether the left a button is currently touched");
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.LeftBTouch, "VRCOSC/OpenVR/LeftController/Input/B/Touch", ParameterMode.Write, "Left Controller B Touch", "Whether the left b button is currently touched");
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.LeftPadTouch, "VRCOSC/OpenVR/LeftController/Input/Pad/Touch", ParameterMode.Write, "Left Controller Pad Touch", "Whether the left pad is currently touched");
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.LeftStickTouch, "VRCOSC/OpenVR/LeftController/Input/Stick/Touch", ParameterMode.Write, "Left Controller Stick Touch", "Whether the left stick is currently touched");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.LeftIndex, "VRCOSC/OpenVR/LeftController/Input/Finger/Index", ParameterMode.Write, "Left Index", "The touch value of your left index finger");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.LeftMiddle, "VRCOSC/OpenVR/LeftController/Input/Finger/Middle", ParameterMode.Write, "Left Middle", "The touch value of your left middle finger");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.LeftRing, "VRCOSC/OpenVR/LeftController/Input/Finger/Ring", ParameterMode.Write, "Left Ring", "The touch value of your left ring finger");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.LeftPinky, "VRCOSC/OpenVR/LeftController/Input/Finger/Pinky", ParameterMode.Write, "Left Pinky", "The touch value of your left pinky finger");

        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.RightATouch, "VRCOSC/OpenVR/RightController/Input/A/Touch", ParameterMode.Write, "Right Controller A Touch", "Whether the right a button is currently touched");
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.RightBTouch, "VRCOSC/OpenVR/RightController/Input/B/Touch", ParameterMode.Write, "Right Controller B Touch", "Whether the right b button is currently touched");
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.RightPadTouch, "VRCOSC/OpenVR/RightController/Input/Pad/Touch", ParameterMode.Write, "Right Controller Pad Touch", "Whether the right pad is currently touched");
        RegisterParameter<bool>(OpenVRControllerStatisticsParameter.RightStickTouch, "VRCOSC/OpenVR/RightController/Input/Stick/Touch", ParameterMode.Write, "Right Controller Stick Touch", "Whether the right stick is currently touched");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.RightIndex, "VRCOSC/OpenVR/RightController/Input/Finger/Index", ParameterMode.Write, "Right Index", "The touch value of your right index finger");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.RightMiddle, "VRCOSC/OpenVR/RightController/Input/Finger/Middle", ParameterMode.Write, "Right Middle", "The touch value of your right middle finger");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.RightRing, "VRCOSC/OpenVR/RightController/Input/Finger/Ring", ParameterMode.Write, "Right Ring", "The touch value of your right ring finger");
        RegisterParameter<float>(OpenVRControllerStatisticsParameter.RightPinky, "VRCOSC/OpenVR/RightController/Input/Finger/Pinky", ParameterMode.Write, "Right Pinky", "The touch value of your right pinky finger");
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateParameters()
    {
        if (!OVRClient.HasInitialised) return;

        if (OVRClient.LeftController.IsConnected)
        {
            var input = OVRClient.LeftController.Input;
            SendParameter(OpenVRControllerStatisticsParameter.LeftATouch, input.A.Touched);
            SendParameter(OpenVRControllerStatisticsParameter.LeftBTouch, input.B.Touched);
            SendParameter(OpenVRControllerStatisticsParameter.LeftPadTouch, input.PadTouched);
            SendParameter(OpenVRControllerStatisticsParameter.LeftStickTouch, input.StickTouched);
            SendParameter(OpenVRControllerStatisticsParameter.LeftIndex, input.IndexFinger);
            SendParameter(OpenVRControllerStatisticsParameter.LeftMiddle, input.MiddleFinger);
            SendParameter(OpenVRControllerStatisticsParameter.LeftRing, input.RingFinger);
            SendParameter(OpenVRControllerStatisticsParameter.LeftPinky, input.PinkyFinger);
        }

        if (OVRClient.RightController.IsConnected)
        {
            var input = OVRClient.RightController.Input;
            SendParameter(OpenVRControllerStatisticsParameter.RightATouch, input.A.Touched);
            SendParameter(OpenVRControllerStatisticsParameter.RightBTouch, input.B.Touched);
            SendParameter(OpenVRControllerStatisticsParameter.RightPadTouch, input.PadTouched);
            SendParameter(OpenVRControllerStatisticsParameter.RightStickTouch, input.StickTouched);
            SendParameter(OpenVRControllerStatisticsParameter.RightIndex, input.IndexFinger);
            SendParameter(OpenVRControllerStatisticsParameter.RightMiddle, input.MiddleFinger);
            SendParameter(OpenVRControllerStatisticsParameter.RightRing, input.RingFinger);
            SendParameter(OpenVRControllerStatisticsParameter.RightPinky, input.PinkyFinger);
        }
    }

    private enum OpenVRControllerStatisticsParameter
    {
        LeftATouch,
        LeftBTouch,
        LeftPadTouch,
        LeftStickTouch,
        LeftIndex,
        LeftMiddle,
        LeftRing,
        LeftPinky,
        RightATouch,
        RightBTouch,
        RightPadTouch,
        RightStickTouch,
        RightIndex,
        RightMiddle,
        RightRing,
        RightPinky
    }
}
