// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.OVR;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("SteamVR Stats")]
[ModuleDescription("Gathers various stats from SteamVR")]
[ModuleType(ModuleType.SteamVR)]
public class SteamVRStatisticsModule : Module
{
    protected override void OnPreLoad()
    {
        RegisterParameter<int>(SteamVRParameter.FPS, "VRCOSC/VR/FPS/Value", ParameterMode.Write, "FPS", "Your measured FPS");
        RegisterParameter<float>(SteamVRParameter.FPSNormalised, "VRCOSC/VR/FPS/Normalised", ParameterMode.Write, "FPS", "Your measured FPS normalised from 0-240 to 0-1");

        RegisterParameter<bool>(SteamVRParameter.UserPresent, "VRCOSC/VR/UserPresent", ParameterMode.Write, "User Present", "Whether you are currently wearing your headset");
        RegisterParameter<bool>(SteamVRParameter.DashboardVisible, "VRCOSC/VR/DashboardVisible", ParameterMode.Write, "Dashboard Visible", "Whether the dashboard is currently visible");

        RegisterParameter<bool>(SteamVRParameter.HMD_Connected, "VRCOSC/VR/HMD/Connected", ParameterMode.Write, "HMD Connected", "Whether your headset is connected");
        RegisterParameter<float>(SteamVRParameter.HMD_Battery, "VRCOSC/VR/HMD/Battery", ParameterMode.Write, "HMD Battery", "The battery percentage normalised of your headset");
        RegisterParameter<bool>(SteamVRParameter.HMD_Charging, "VRCOSC/VR/HMD/Charging", ParameterMode.Write, "HMD Charging", "Weather your headset is charging");

        RegisterParameter<bool>(SteamVRParameter.LHand_Connected, "VRCOSC/VR/LHand/Connected", ParameterMode.Write, "Left Hand Connected", "Whether your left hand is connected");
        RegisterParameter<float>(SteamVRParameter.LHand_Battery, "VRCOSC/VR/LHand/Battery", ParameterMode.Write, "Left Hand Battery", "The battery percentage normalised of your left hand");
        RegisterParameter<bool>(SteamVRParameter.LHand_Charging, "VRCOSC/VR/LHand/Charging", ParameterMode.Write, "Left Hand Charging", "Weather your left hand is charging");

        RegisterParameter<bool>(SteamVRParameter.LeftATouch, "VRCOSC/VR/LHand/Input/A/Touch", ParameterMode.Write, "Left Hand A Touch", "Whether the left a button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.LeftBTouch, "VRCOSC/VR/LHand/Input/B/Touch", ParameterMode.Write, "Left Hand B Touch", "Whether the left b button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.LeftPadTouch, "VRCOSC/VR/LHand/Input/Pad/Touch", ParameterMode.Write, "Left Hand Pad Touch (Index Only)", "Whether the left pad is currently touched");
        RegisterParameter<bool>(SteamVRParameter.LeftStickTouch, "VRCOSC/VR/LHand/Input/Stick/Touch", ParameterMode.Write, "Left Hand Stick Touch", "Whether the left stick is currently touched");
        RegisterParameter<float>(SteamVRParameter.LeftIndex, "VRCOSC/VR/LHand/Input/Finger/Index", ParameterMode.Write, "Left Index (Index Only)", "The bend value of your left index finger");
        RegisterParameter<float>(SteamVRParameter.LeftMiddle, "VRCOSC/VR/LHand/Input/Finger/Middle", ParameterMode.Write, "Left Middle (Index Only)", "The bend value of your left middle finger");
        RegisterParameter<float>(SteamVRParameter.LeftRing, "VRCOSC/VR/LHand/Input/Finger/Ring", ParameterMode.Write, "Left Ring (Index Only)", "The bend value of your left ring finger");
        RegisterParameter<float>(SteamVRParameter.LeftPinky, "VRCOSC/VR/LHand/Input/Finger/Pinky", ParameterMode.Write, "Left Pinky (Index Only)", "The bend value of your left pinky finger");

        RegisterParameter<bool>(SteamVRParameter.RHand_Connected, "VRCOSC/VR/RHand/Connected", ParameterMode.Write, "Right Hand Connected", "Whether your right hand is connected");
        RegisterParameter<float>(SteamVRParameter.RHand_Battery, "VRCOSC/VR/RHand/Battery", ParameterMode.Write, "Right Hand Battery", "The battery percentage normalised of your right hand");
        RegisterParameter<bool>(SteamVRParameter.RHand_Charging, "VRCOSC/VR/RHand/Charging", ParameterMode.Write, "Right Hand Charging", "Weather your right hand is charging");

        RegisterParameter<bool>(SteamVRParameter.RightATouch, "VRCOSC/VR/RHand/Input/A/Touch", ParameterMode.Write, "Right Hand A Touch", "Whether the right a button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.RightBTouch, "VRCOSC/VR/RHand/Input/B/Touch", ParameterMode.Write, "Right Hand B Touch", "Whether the right b button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.RightPadTouch, "VRCOSC/VR/RHand/Input/Pad/Touch", ParameterMode.Write, "Right Hand Pad Touch (Index Only)", "Whether the right pad is currently touched");
        RegisterParameter<bool>(SteamVRParameter.RightStickTouch, "VRCOSC/VR/RHand/Input/Stick/Touch", ParameterMode.Write, "Right Hand Stick Touch", "Whether the right stick is currently touched");
        RegisterParameter<float>(SteamVRParameter.RightIndex, "VRCOSC/VR/RHand/Input/Finger/Index", ParameterMode.Write, "Right Index (Index Only)", "The bend value of your right index finger");
        RegisterParameter<float>(SteamVRParameter.RightMiddle, "VRCOSC/VR/RHand/Input/Finger/Middle", ParameterMode.Write, "Right Middle (Index Only)", "The bend value of your right middle finger");
        RegisterParameter<float>(SteamVRParameter.RightRing, "VRCOSC/VR/RHand/Input/Finger/Ring", ParameterMode.Write, "Right Ring (Index Only)", "The bend value of your right ring finger");
        RegisterParameter<float>(SteamVRParameter.RightPinky, "VRCOSC/VR/RHand/Input/Finger/Pinky", ParameterMode.Write, "Right Pinky (Index Only)", "The bend value of your right pinky finger");

        RegisterParameter<bool>(SteamVRParameter.LElbow_Connected, "VRCOSC/VR/LElbow/Connected", ParameterMode.Write, "Left Elbow Connected", "Whether any tracker marked as Left Elbow is connected");
        RegisterParameter<float>(SteamVRParameter.LElbow_Battery, "VRCOSC/VR/LElbow/Battery", ParameterMode.Write, "Left Elbow Battery", "Any tracker marked as Left Elbow's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.LElbow_Charging, "VRCOSC/VR/LElbow/Charging", ParameterMode.Write, "Left Elbow Charging", "Whether any tracker marked as Left Elbow is currently charging");
        RegisterParameter<bool>(SteamVRParameter.RElbow_Connected, "VRCOSC/VR/RElbow/Connected", ParameterMode.Write, "Right Elbow Connected", "Whether any tracker marked as Right Elbow is connected");
        RegisterParameter<float>(SteamVRParameter.RElbow_Battery, "VRCOSC/VR/RElbow/Battery", ParameterMode.Write, "Right Elbow Battery", "Any tracker marked as Right Elbow's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.RElbow_Charging, "VRCOSC/VR/RElbow/Charging", ParameterMode.Write, "Right Elbow Charging", "Whether any tracker marked as Right Elbow is currently charging");

        RegisterParameter<bool>(SteamVRParameter.LFoot_Connected, "VRCOSC/VR/LFoot/Connected", ParameterMode.Write, "Left Foot Connected", "Whether any tracker marked as Left Foot is connected");
        RegisterParameter<float>(SteamVRParameter.LFoot_Battery, "VRCOSC/VR/LFoot/Battery", ParameterMode.Write, "Left Foot Battery", "Any tracker marked as Left Foot's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.LFoot_Charging, "VRCOSC/VR/LFoot/Charging", ParameterMode.Write, "Left Foot Charging", "Whether any tracker marked as Left Foot is currently charging");
        RegisterParameter<bool>(SteamVRParameter.RFoot_Connected, "VRCOSC/VR/RFoot/Connected", ParameterMode.Write, "Right Foot Connected", "Whether any tracker marked as Right Foot is connected");
        RegisterParameter<float>(SteamVRParameter.RFoot_Battery, "VRCOSC/VR/RFoot/Battery", ParameterMode.Write, "Right Foot Battery", "Any tracker marked as Right Foot's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.RFoot_Charging, "VRCOSC/VR/RFoot/Charging", ParameterMode.Write, "Right Foot Charging", "Whether any tracker marked as Right Foot is currently charging");

        RegisterParameter<bool>(SteamVRParameter.LKnee_Connected, "VRCOSC/VR/LKnee/Connected", ParameterMode.Write, "Left Knee Connected", "Whether any tracker marked as Left Knee is connected");
        RegisterParameter<float>(SteamVRParameter.LKnee_Battery, "VRCOSC/VR/LKnee/Battery", ParameterMode.Write, "Left Knee Battery", "Any tracker marked as Left Knee's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.LKnee_Charging, "VRCOSC/VR/LKnee/Charging", ParameterMode.Write, "Left Knee Charging", "Whether any tracker marked as Left Knee is currently charging");
        RegisterParameter<bool>(SteamVRParameter.RKnee_Connected, "VRCOSC/VR/RKnee/Connected", ParameterMode.Write, "Right Knee Connected", "Whether any tracker marked as Right Knee is connected");
        RegisterParameter<float>(SteamVRParameter.RKnee_Battery, "VRCOSC/VR/RKnee/Battery", ParameterMode.Write, "Right Knee Battery", "Any tracker marked as Right Knee's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.RKnee_Charging, "VRCOSC/VR/RKnee/Charging", ParameterMode.Write, "Right Knee Charging", "Whether any tracker marked as Right Knee is currently charging");

        RegisterParameter<bool>(SteamVRParameter.Waist_Connected, "VRCOSC/VR/Waist/Connected", ParameterMode.Write, "Waist Connected", "Whether any tracker marked as Waist is connected");
        RegisterParameter<float>(SteamVRParameter.Waist_Battery, "VRCOSC/VR/Waist/Battery", ParameterMode.Write, "Waist Battery", "Any tracker marked as Waist's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.Waist_Charging, "VRCOSC/VR/Waist/Charging", ParameterMode.Write, "Waist Charging", "Whether any tracker marked as Waist is currently charging");

        RegisterParameter<bool>(SteamVRParameter.Chest_Connected, "VRCOSC/VR/Chest/Connected", ParameterMode.Write, "Chest Connected", "Whether any tracker marked as Chest is connected");
        RegisterParameter<float>(SteamVRParameter.Chest_Battery, "VRCOSC/VR/Chest/Battery", ParameterMode.Write, "Chest Battery", "Any tracker marked as Chest's battery percentage normalised (0-1)");
        RegisterParameter<bool>(SteamVRParameter.Chest_Charging, "VRCOSC/VR/Chest/Charging", ParameterMode.Write, "Chest Charging", "Whether any tracker marked as Chest is currently charging");
    }

    protected override void OnPostLoad()
    {
        CreateVariable<float>(SteamVRVariable.FPS, "FPS");
        CreateVariable<bool>(SteamVRVariable.DashboardVisible, "Dashboard Visible");
        CreateVariable<bool>(SteamVRVariable.HMD_Charging, "HMD Charging");
        var hmdBatteryReference = CreateVariable<int>(SteamVRVariable.HMD_Battery, "HMD Battery (%)")!;
        CreateVariable<bool>(SteamVRVariable.LHand_Charging, "Left Hand Charging");
        var lcBatteryReference = CreateVariable<int>(SteamVRVariable.LHand_Battery, "Left Hand Battery (%)")!;
        CreateVariable<bool>(SteamVRVariable.RHand_Charging, "Right Hand Charging");
        var rcBatteryReference = CreateVariable<int>(SteamVRVariable.RHand_Battery, "Right Hand Battery (%)")!;

        CreateVariable<bool>(SteamVRVariable.LElbow_Charging, "Left Elbow Charging");
        CreateVariable<int>(SteamVRVariable.LElbow_Battery, "Left Elbow Battery (%)");
        CreateVariable<bool>(SteamVRVariable.RElbow_Charging, "Right Elbow Charging");
        CreateVariable<int>(SteamVRVariable.RElbow_Battery, "Right Elbow Battery (%)");
        CreateVariable<bool>(SteamVRVariable.LFoot_Charging, "Left Foot Charging");
        CreateVariable<int>(SteamVRVariable.LFoot_Battery, "Left Foot Battery (%)");
        CreateVariable<bool>(SteamVRVariable.RFoot_Charging, "Right Foot Charging");
        CreateVariable<int>(SteamVRVariable.RFoot_Battery, "Right Foot Battery (%)");
        CreateVariable<bool>(SteamVRVariable.LKnee_Charging, "Left Knee Charging");
        CreateVariable<int>(SteamVRVariable.LKnee_Battery, "Left Knee Battery (%)");
        CreateVariable<bool>(SteamVRVariable.RKnee_Charging, "Right Knee Charging");
        CreateVariable<int>(SteamVRVariable.RKnee_Battery, "Right Knee Battery (%)");
        CreateVariable<bool>(SteamVRVariable.Waist_Charging, "Waist Charging");
        CreateVariable<int>(SteamVRVariable.Waist_Battery, "Waist Battery (%)");
        CreateVariable<bool>(SteamVRVariable.Chest_Charging, "Chest Charging");
        CreateVariable<int>(SteamVRVariable.Chest_Battery, "Chest Battery (%)");

        var trackerAverageBattery = CreateVariable<int>(SteamVRVariable.TrackerAverageBattery, "Tracker Average Battery (%)")!;

        CreateState(SteamVRState.Default, "Default", "HMD: {0}\nLHand: {1}\nRHand: {2}\nTrackers: {3}", new[] { hmdBatteryReference, lcBatteryReference, rcBatteryReference, trackerAverageBattery });
    }

    protected override Task<bool> OnModuleStart()
    {
        ChangeState(SteamVRState.Default);

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void updateVariables()
    {
        SetVariableValue(SteamVRVariable.FPS, GetOVRClient().FPS);
        SetVariableValue(SteamVRVariable.DashboardVisible, GetOVRClient().IsDashboardVisible());
        SetVariableValue(SteamVRVariable.HMD_Battery, (int)(GetOVRClient().GetHMD().BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.HMD_Charging, GetOVRClient().GetHMD().IsCharging);
        SetVariableValue(SteamVRVariable.LHand_Battery, (int)(GetOVRClient().GetLeftController().BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.LHand_Charging, GetOVRClient().GetLeftController().IsCharging);
        SetVariableValue(SteamVRVariable.RHand_Battery, (int)(GetOVRClient().GetRightController().BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.RHand_Charging, GetOVRClient().GetRightController().IsCharging);
        SetVariableValue(SteamVRVariable.Chest_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.Chest).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.Chest_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.Chest).IsCharging);
        SetVariableValue(SteamVRVariable.Waist_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.Waist).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.Waist_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.Waist).IsCharging);
        SetVariableValue(SteamVRVariable.LElbow_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.LeftElbow).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.LElbow_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.LeftElbow).IsCharging);
        SetVariableValue(SteamVRVariable.RElbow_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.RightElbow).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.RElbow_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.RightElbow).IsCharging);
        SetVariableValue(SteamVRVariable.LKnee_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.LeftKnee).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.LKnee_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.LeftKnee).IsCharging);
        SetVariableValue(SteamVRVariable.RKnee_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.RightKnee).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.RKnee_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.RightKnee).IsCharging);
        SetVariableValue(SteamVRVariable.LFoot_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.LeftFoot).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.LFoot_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.LeftFoot).IsCharging);
        SetVariableValue(SteamVRVariable.RFoot_Battery, (int)(GetOVRClient().GetTrackedDevice(DeviceRole.RightFoot).BatteryPercentage * 100f));
        SetVariableValue(SteamVRVariable.RFoot_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.RightFoot).IsCharging);

        var totalBatteryPercentage = 0f;
        var totalTrackers = 0;

        foreach (var deviceRole in Enum.GetValues<DeviceRole>())
        {
            if (deviceRole is DeviceRole.Head or DeviceRole.LeftHand or DeviceRole.RightHand or DeviceRole.Unset) continue;

            var device = GetOVRClient().GetTrackedDevice(deviceRole);

            if (!device.IsConnected) continue;

            totalTrackers++;
            totalBatteryPercentage += device.BatteryPercentage;
        }

        var trackerAverageBattery = totalBatteryPercentage / totalTrackers;
        SetVariableValue(SteamVRVariable.TrackerAverageBattery, (int)(trackerAverageBattery * 100f));
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000)]
    private void updateMetadataParameters()
    {
        SendParameter(SteamVRParameter.UserPresent, GetOVRClient().IsUserPresent());
        SendParameter(SteamVRParameter.DashboardVisible, GetOVRClient().IsDashboardVisible());

        SendParameter(SteamVRParameter.HMD_Connected, GetOVRClient().GetHMD().IsConnected);
        SendParameter(SteamVRParameter.HMD_Battery, GetOVRClient().GetHMD().BatteryPercentage);
        SendParameter(SteamVRParameter.HMD_Charging, GetOVRClient().GetHMD().IsCharging);

        SendParameter(SteamVRParameter.LHand_Connected, GetOVRClient().GetLeftController().IsConnected);
        SendParameter(SteamVRParameter.LHand_Battery, GetOVRClient().GetLeftController().BatteryPercentage);
        SendParameter(SteamVRParameter.LHand_Charging, GetOVRClient().GetLeftController().IsCharging);

        SendParameter(SteamVRParameter.RHand_Connected, GetOVRClient().GetRightController().IsConnected);
        SendParameter(SteamVRParameter.RHand_Battery, GetOVRClient().GetRightController().BatteryPercentage);
        SendParameter(SteamVRParameter.RHand_Charging, GetOVRClient().GetRightController().IsCharging);

        SendParameter(SteamVRParameter.Chest_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.Chest).IsConnected);
        SendParameter(SteamVRParameter.Chest_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.Chest).BatteryPercentage);
        SendParameter(SteamVRParameter.Chest_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.Chest).IsConnected);

        SendParameter(SteamVRParameter.Waist_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.Waist).IsConnected);
        SendParameter(SteamVRParameter.Waist_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.Waist).BatteryPercentage);
        SendParameter(SteamVRParameter.Waist_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.Waist).IsConnected);

        SendParameter(SteamVRParameter.LElbow_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.LeftElbow).IsConnected);
        SendParameter(SteamVRParameter.LElbow_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.LeftElbow).BatteryPercentage);
        SendParameter(SteamVRParameter.LElbow_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.LeftElbow).IsConnected);

        SendParameter(SteamVRParameter.RElbow_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.RightElbow).IsConnected);
        SendParameter(SteamVRParameter.RElbow_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.RightElbow).BatteryPercentage);
        SendParameter(SteamVRParameter.RElbow_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.RightElbow).IsConnected);

        SendParameter(SteamVRParameter.LKnee_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.LeftKnee).IsConnected);
        SendParameter(SteamVRParameter.LKnee_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.LeftKnee).BatteryPercentage);
        SendParameter(SteamVRParameter.LKnee_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.LeftKnee).IsConnected);

        SendParameter(SteamVRParameter.RKnee_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.RightKnee).IsConnected);
        SendParameter(SteamVRParameter.RKnee_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.RightKnee).BatteryPercentage);
        SendParameter(SteamVRParameter.RKnee_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.RightKnee).IsConnected);

        SendParameter(SteamVRParameter.LFoot_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.LeftFoot).IsConnected);
        SendParameter(SteamVRParameter.LFoot_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.LeftFoot).BatteryPercentage);
        SendParameter(SteamVRParameter.LFoot_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.LeftFoot).IsConnected);

        SendParameter(SteamVRParameter.RFoot_Connected, GetOVRClient().GetTrackedDevice(DeviceRole.RightFoot).IsConnected);
        SendParameter(SteamVRParameter.RFoot_Battery, GetOVRClient().GetTrackedDevice(DeviceRole.RightFoot).BatteryPercentage);
        SendParameter(SteamVRParameter.RFoot_Charging, GetOVRClient().GetTrackedDevice(DeviceRole.RightFoot).IsConnected);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000f / 60f)]
    private void updateRealtimeParameters()
    {
        SendParameter(SteamVRParameter.FPS, (int)MathF.Round(GetOVRClient().FPS));
        SendParameter(SteamVRParameter.FPSNormalised, GetOVRClient().FPS / 240.0f);

        var leftInput = GetOVRClient().GetLeftController().Input;
        SendParameter(SteamVRParameter.LeftATouch, leftInput.A.Touched);
        SendParameter(SteamVRParameter.LeftBTouch, leftInput.B.Touched);
        SendParameter(SteamVRParameter.LeftPadTouch, leftInput.PadTouched);
        SendParameter(SteamVRParameter.LeftStickTouch, leftInput.StickTouched);
        SendParameter(SteamVRParameter.LeftIndex, leftInput.IndexFinger);
        SendParameter(SteamVRParameter.LeftMiddle, leftInput.MiddleFinger);
        SendParameter(SteamVRParameter.LeftRing, leftInput.RingFinger);
        SendParameter(SteamVRParameter.LeftPinky, leftInput.PinkyFinger);

        var rightInput = GetOVRClient().GetRightController().Input;
        SendParameter(SteamVRParameter.RightATouch, rightInput.A.Touched);
        SendParameter(SteamVRParameter.RightBTouch, rightInput.B.Touched);
        SendParameter(SteamVRParameter.RightPadTouch, rightInput.PadTouched);
        SendParameter(SteamVRParameter.RightStickTouch, rightInput.StickTouched);
        SendParameter(SteamVRParameter.RightIndex, rightInput.IndexFinger);
        SendParameter(SteamVRParameter.RightMiddle, rightInput.MiddleFinger);
        SendParameter(SteamVRParameter.RightRing, rightInput.RingFinger);
        SendParameter(SteamVRParameter.RightPinky, rightInput.PinkyFinger);
    }

    private enum SteamVRParameter
    {
        FPS,
        FPSNormalised,
        UserPresent,
        DashboardVisible,
        HMD_Connected,
        LHand_Connected,
        RHand_Connected,
        LElbow_Connected,
        RElbow_Connected,
        LFoot_Connected,
        RFoot_Connected,
        LKnee_Connected,
        RKnee_Connected,
        Waist_Connected,
        Chest_Connected,
        HMD_Battery,
        LHand_Battery,
        RHand_Battery,
        LElbow_Battery,
        RElbow_Battery,
        LFoot_Battery,
        RFoot_Battery,
        LKnee_Battery,
        RKnee_Battery,
        Waist_Battery,
        Chest_Battery,
        HMD_Charging,
        LHand_Charging,
        RHand_Charging,
        LElbow_Charging,
        RElbow_Charging,
        LFoot_Charging,
        RFoot_Charging,
        LKnee_Charging,
        RKnee_Charging,
        Waist_Charging,
        Chest_Charging,
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

    private enum SteamVRState
    {
        Default
    }

    private enum SteamVRVariable
    {
        FPS,
        DashboardVisible,
        HMD_Battery,
        LHand_Battery,
        RHand_Battery,
        LElbow_Battery,
        RElbow_Battery,
        LFoot_Battery,
        RFoot_Battery,
        LKnee_Battery,
        RKnee_Battery,
        Waist_Battery,
        Chest_Battery,
        TrackerAverageBattery,
        HMD_Charging,
        LHand_Charging,
        RHand_Charging,
        LElbow_Charging,
        RElbow_Charging,
        LFoot_Charging,
        RFoot_Charging,
        LKnee_Charging,
        RKnee_Charging,
        Waist_Charging,
        Chest_Charging
    }
}
