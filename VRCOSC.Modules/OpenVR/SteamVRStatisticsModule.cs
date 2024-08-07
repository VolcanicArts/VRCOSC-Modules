﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.OVR;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("SteamVR Stats")]
[ModuleDescription("Gathers various stats from SteamVR")]
[ModuleType(ModuleType.Integrations)]
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

        RegisterParameter<bool>(SteamVRParameter.LC_Connected, "VRCOSC/VR/LC/Connected", ParameterMode.Write, "Left Controller Connected", "Whether your left controller is connected");
        RegisterParameter<float>(SteamVRParameter.LC_Battery, "VRCOSC/VR/LC/Battery", ParameterMode.Write, "Left Controller Battery", "The battery percentage normalised of your left controller");
        RegisterParameter<bool>(SteamVRParameter.LC_Charging, "VRCOSC/VR/LC/Charging", ParameterMode.Write, "Left Controller Charging", "Weather your left controller is charging");

        RegisterParameter<bool>(SteamVRParameter.LeftATouch, "VRCOSC/VR/LC/Input/A/Touch", ParameterMode.Write, "Left Controller A Touch", "Whether the left a button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.LeftBTouch, "VRCOSC/VR/LC/Input/B/Touch", ParameterMode.Write, "Left Controller B Touch", "Whether the left b button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.LeftPadTouch, "VRCOSC/VR/LC/Input/Pad/Touch", ParameterMode.Write, "Left Controller Pad Touch (Index Only)", "Whether the left pad is currently touched");
        RegisterParameter<bool>(SteamVRParameter.LeftStickTouch, "VRCOSC/VR/LC/Input/Stick/Touch", ParameterMode.Write, "Left Controller Stick Touch", "Whether the left stick is currently touched");
        RegisterParameter<float>(SteamVRParameter.LeftIndex, "VRCOSC/VR/LC/Input/Finger/Index", ParameterMode.Write, "Left Index (Index Only)", "The bend value of your left index finger");
        RegisterParameter<float>(SteamVRParameter.LeftMiddle, "VRCOSC/VR/LC/Input/Finger/Middle", ParameterMode.Write, "Left Middle (Index Only)", "The bend value of your left middle finger");
        RegisterParameter<float>(SteamVRParameter.LeftRing, "VRCOSC/VR/LC/Input/Finger/Ring", ParameterMode.Write, "Left Ring (Index Only)", "The bend value of your left ring finger");
        RegisterParameter<float>(SteamVRParameter.LeftPinky, "VRCOSC/VR/LC/Input/Finger/Pinky", ParameterMode.Write, "Left Pinky (Index Only)", "The bend value of your left pinky finger");

        RegisterParameter<bool>(SteamVRParameter.RC_Connected, "VRCOSC/VR/RC/Connected", ParameterMode.Write, "Right Controller Connected", "Whether your right controller is connected");
        RegisterParameter<float>(SteamVRParameter.RC_Battery, "VRCOSC/VR/RC/Battery", ParameterMode.Write, "Right Controller Battery", "The battery percentage normalised of your right controller");
        RegisterParameter<bool>(SteamVRParameter.RC_Charging, "VRCOSC/VR/RC/Charging", ParameterMode.Write, "Right Controller Charging", "Weather your right controller is charging");

        RegisterParameter<bool>(SteamVRParameter.RightATouch, "VRCOSC/VR/RC/Input/A/Touch", ParameterMode.Write, "Right Controller A Touch", "Whether the right a button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.RightBTouch, "VRCOSC/VR/RC/Input/B/Touch", ParameterMode.Write, "Right Controller B Touch", "Whether the right b button is currently touched");
        RegisterParameter<bool>(SteamVRParameter.RightPadTouch, "VRCOSC/VR/RC/Input/Pad/Touch", ParameterMode.Write, "Right Controller Pad Touch (Index Only)", "Whether the right pad is currently touched");
        RegisterParameter<bool>(SteamVRParameter.RightStickTouch, "VRCOSC/VR/RC/Input/Stick/Touch", ParameterMode.Write, "Right Controller Stick Touch", "Whether the right stick is currently touched");
        RegisterParameter<float>(SteamVRParameter.RightIndex, "VRCOSC/VR/RC/Input/Finger/Index", ParameterMode.Write, "Right Index (Index Only)", "The bend value of your right index finger");
        RegisterParameter<float>(SteamVRParameter.RightMiddle, "VRCOSC/VR/RC/Input/Finger/Middle", ParameterMode.Write, "Right Middle (Index Only)", "The bend value of your right middle finger");
        RegisterParameter<float>(SteamVRParameter.RightRing, "VRCOSC/VR/RC/Input/Finger/Ring", ParameterMode.Write, "Right Ring (Index Only)", "The bend value of your right ring finger");
        RegisterParameter<float>(SteamVRParameter.RightPinky, "VRCOSC/VR/RC/Input/Finger/Pinky", ParameterMode.Write, "Right Pinky (Index Only)", "The bend value of your right pinky finger");

        for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
        {
            var trackerID = i + 1;
            RegisterParameter<bool>(SteamVRParameter.Tracker1_Connected + i, $"VRCOSC/VR/T{trackerID}/Connected", ParameterMode.Write, $"Tracker {trackerID} Connected", $"Whether tracker {trackerID} is connected");
            RegisterParameter<float>(SteamVRParameter.Tracker1_Battery + i, $"VRCOSC/VR/T{trackerID}/Battery", ParameterMode.Write, $"Tracker {trackerID} Battery", $"The battery percentage normalised (0-1) of tracker {trackerID}");
            RegisterParameter<bool>(SteamVRParameter.Tracker1_Charging + i, $"VRCOSC/VR/T{trackerID}/Charging", ParameterMode.Write, $"Tracker {trackerID} Charging", $"Whether tracker {trackerID} is currently charging");
        }
    }

    protected override void OnPostLoad()
    {
        CreateVariable<float>(SteamVRVariable.FPS, "FPS");
        CreateVariable<bool>(SteamVRVariable.DashboardVisible, "Dashboard Visible");
        CreateVariable<bool>(SteamVRVariable.HMDCharging, "HMD Charging");
        var hmdBatteryReference = CreateVariable<int>(SteamVRVariable.HMDBattery, "HMD Battery (%)")!;
        CreateVariable<bool>(SteamVRVariable.LeftControllerCharging, "Left Controller Charging");
        var lcBatteryReference = CreateVariable<int>(SteamVRVariable.LeftControllerBattery, "Left Controller Battery (%)")!;
        CreateVariable<bool>(SteamVRVariable.RightControllerCharging, "Right Controller Charging");
        var rcBatteryReference = CreateVariable<int>(SteamVRVariable.RightControllerBattery, "Right Controller Battery (%)")!;

        for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
        {
            CreateVariable<int>(SteamVRVariable.Tracker1Battery + i, $"Tracker {i + 1} Battery");
        }

        var trackerAverageBattery = CreateVariable<int>(SteamVRVariable.TrackerAverageBattery, "Tracker Average Battery")!;

        CreateState(SteamVRState.Default, "Default", "HMD: {0}\nLC: {1}\nRC: {2}\nTrackers: {3}", new[] { hmdBatteryReference, lcBatteryReference, rcBatteryReference, trackerAverageBattery });
    }

    protected override Task<bool> OnModuleStart()
    {
        ChangeState(SteamVRState.Default);

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void updateVariables()
    {
        if (GetOVRClient().HasInitialised)
        {
            SetVariableValue(SteamVRVariable.FPS, GetOVRClient().System.FPS);
            SetVariableValue(SteamVRVariable.DashboardVisible, GetOVRClient().IsDashboardVisible());
            SetVariableValue(SteamVRVariable.HMDCharging, GetOVRClient().HMD.IsCharging);
            SetVariableValue(SteamVRVariable.HMDBattery, (int)(GetOVRClient().HMD.BatteryPercentage * 100f));
            SetVariableValue(SteamVRVariable.LeftControllerCharging, GetOVRClient().LeftController.IsCharging);
            SetVariableValue(SteamVRVariable.LeftControllerBattery, (int)(GetOVRClient().LeftController.BatteryPercentage * 100f));
            SetVariableValue(SteamVRVariable.RightControllerCharging, GetOVRClient().RightController.IsCharging);
            SetVariableValue(SteamVRVariable.RightControllerBattery, (int)(GetOVRClient().RightController.BatteryPercentage * 100f));

            for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
            {
                SetVariableValue(SteamVRVariable.Tracker1Battery + i, (int)(GetOVRClient().Trackers.ElementAt(i).BatteryPercentage * 100f));
            }

            var activeTrackers = GetOVRClient().Trackers.Where(tracker => tracker.IsConnected).ToList();

            if (activeTrackers.Any())
            {
                var trackerAverageBattery = activeTrackers.Sum(tracker => tracker.BatteryPercentage) / activeTrackers.Count;
                SetVariableValue(SteamVRVariable.TrackerAverageBattery, (int)(trackerAverageBattery * 100f));
            }
            else
            {
                SetVariableValue(SteamVRVariable.TrackerAverageBattery, 0);
            }
        }
        else
        {
            SetVariableValue(SteamVRVariable.FPS, 0f);
            SetVariableValue(SteamVRVariable.DashboardVisible, false);
            SetVariableValue(SteamVRVariable.HMDCharging, false);
            SetVariableValue(SteamVRVariable.HMDBattery, 0);
            SetVariableValue(SteamVRVariable.LeftControllerCharging, false);
            SetVariableValue(SteamVRVariable.LeftControllerBattery, 0);
            SetVariableValue(SteamVRVariable.RightControllerCharging, false);
            SetVariableValue(SteamVRVariable.RightControllerBattery, 0);

            for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
            {
                SetVariableValue(SteamVRVariable.Tracker1Battery + i, 0);
            }

            SetVariableValue(SteamVRVariable.TrackerAverageBattery, 0);
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000)]
    private void updateMetadataParameters()
    {
        if (GetOVRClient().HasInitialised)
        {
            SendParameter(SteamVRParameter.UserPresent, GetOVRClient().IsUserPresent());
            SendParameter(SteamVRParameter.DashboardVisible, GetOVRClient().IsDashboardVisible());

            SendParameter(SteamVRParameter.HMD_Connected, GetOVRClient().HMD.IsConnected);

            if (GetOVRClient().HMD.IsConnected && GetOVRClient().HMD.ProvidesBatteryStatus)
            {
                SendParameter(SteamVRParameter.HMD_Battery, GetOVRClient().HMD.BatteryPercentage);
                SendParameter(SteamVRParameter.HMD_Charging, GetOVRClient().HMD.IsCharging);
            }
            else
            {
                SendParameter(SteamVRParameter.HMD_Battery, 0f);
                SendParameter(SteamVRParameter.HMD_Charging, false);
            }

            SendParameter(SteamVRParameter.LC_Connected, GetOVRClient().LeftController.IsConnected);

            if (GetOVRClient().LeftController.IsConnected && GetOVRClient().LeftController.ProvidesBatteryStatus)
            {
                SendParameter(SteamVRParameter.LC_Battery, GetOVRClient().LeftController.BatteryPercentage);
                SendParameter(SteamVRParameter.LC_Charging, GetOVRClient().LeftController.IsCharging);
            }
            else
            {
                SendParameter(SteamVRParameter.LC_Battery, 0f);
                SendParameter(SteamVRParameter.LC_Charging, false);
            }

            SendParameter(SteamVRParameter.RC_Connected, GetOVRClient().RightController.IsConnected);

            if (GetOVRClient().RightController.IsConnected && GetOVRClient().RightController.ProvidesBatteryStatus)
            {
                SendParameter(SteamVRParameter.RC_Battery, GetOVRClient().RightController.BatteryPercentage);
                SendParameter(SteamVRParameter.RC_Charging, GetOVRClient().RightController.IsCharging);
            }
            else
            {
                SendParameter(SteamVRParameter.RC_Battery, 0f);
                SendParameter(SteamVRParameter.RC_Charging, false);
            }

            var trackers = GetOVRClient().Trackers.ToList();

            for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
            {
                var tracker = trackers[i];

                SendParameter(SteamVRParameter.Tracker1_Connected + i, tracker.IsConnected);

                if (tracker.IsConnected && tracker.ProvidesBatteryStatus)
                {
                    SendParameter(SteamVRParameter.Tracker1_Battery + i, tracker.BatteryPercentage);
                    SendParameter(SteamVRParameter.Tracker1_Charging + i, tracker.IsCharging);
                }
                else
                {
                    SendParameter(SteamVRParameter.Tracker1_Battery + i, 0f);
                    SendParameter(SteamVRParameter.Tracker1_Charging + i, false);
                }
            }
        }
        else
        {
            SendParameter(SteamVRParameter.UserPresent, false);
            SendParameter(SteamVRParameter.DashboardVisible, false);

            SendParameter(SteamVRParameter.HMD_Connected, false);
            SendParameter(SteamVRParameter.HMD_Battery, 0f);
            SendParameter(SteamVRParameter.HMD_Charging, false);

            SendParameter(SteamVRParameter.LC_Connected, false);
            SendParameter(SteamVRParameter.LC_Battery, 0f);
            SendParameter(SteamVRParameter.LC_Charging, false);

            SendParameter(SteamVRParameter.RC_Connected, false);
            SendParameter(SteamVRParameter.RC_Battery, 0f);
            SendParameter(SteamVRParameter.RC_Charging, false);

            for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
            {
                SendParameter(SteamVRParameter.Tracker1_Connected + i, false);
                SendParameter(SteamVRParameter.Tracker1_Battery + i, 0f);
                SendParameter(SteamVRParameter.Tracker1_Charging + i, false);
            }
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000f / 60f)]
    private void updateRealtimeParameters()
    {
        if (GetOVRClient().HasInitialised)
        {
            SendParameter(SteamVRParameter.FPS, (int)MathF.Round(GetOVRClient().System.FPS));
            SendParameter(SteamVRParameter.FPSNormalised, GetOVRClient().System.FPS / 240.0f);
        }
        else
        {
            SendParameter(SteamVRParameter.FPS, 0);
            SendParameter(SteamVRParameter.FPSNormalised, 0f);
        }

        if (GetOVRClient().HasInitialised && GetOVRClient().LeftController.IsConnected)
        {
            var input = GetOVRClient().LeftController.Input;
            SendParameter(SteamVRParameter.LeftATouch, input.A.Touched);
            SendParameter(SteamVRParameter.LeftBTouch, input.B.Touched);
            SendParameter(SteamVRParameter.LeftPadTouch, input.PadTouched);
            SendParameter(SteamVRParameter.LeftStickTouch, input.StickTouched);
            SendParameter(SteamVRParameter.LeftIndex, input.IndexFinger);
            SendParameter(SteamVRParameter.LeftMiddle, input.MiddleFinger);
            SendParameter(SteamVRParameter.LeftRing, input.RingFinger);
            SendParameter(SteamVRParameter.LeftPinky, input.PinkyFinger);
        }
        else
        {
            SendParameter(SteamVRParameter.LeftATouch, false);
            SendParameter(SteamVRParameter.LeftBTouch, false);
            SendParameter(SteamVRParameter.LeftPadTouch, false);
            SendParameter(SteamVRParameter.LeftStickTouch, false);
            SendParameter(SteamVRParameter.LeftIndex, 0f);
            SendParameter(SteamVRParameter.LeftMiddle, 0f);
            SendParameter(SteamVRParameter.LeftRing, 0f);
            SendParameter(SteamVRParameter.LeftPinky, 0f);
        }

        if (GetOVRClient().HasInitialised && GetOVRClient().RightController.IsConnected)
        {
            var input = GetOVRClient().RightController.Input;
            SendParameter(SteamVRParameter.RightATouch, input.A.Touched);
            SendParameter(SteamVRParameter.RightBTouch, input.B.Touched);
            SendParameter(SteamVRParameter.RightPadTouch, input.PadTouched);
            SendParameter(SteamVRParameter.RightStickTouch, input.StickTouched);
            SendParameter(SteamVRParameter.RightIndex, input.IndexFinger);
            SendParameter(SteamVRParameter.RightMiddle, input.MiddleFinger);
            SendParameter(SteamVRParameter.RightRing, input.RingFinger);
            SendParameter(SteamVRParameter.RightPinky, input.PinkyFinger);
        }
        else
        {
            SendParameter(SteamVRParameter.RightATouch, false);
            SendParameter(SteamVRParameter.RightBTouch, false);
            SendParameter(SteamVRParameter.RightPadTouch, false);
            SendParameter(SteamVRParameter.RightStickTouch, false);
            SendParameter(SteamVRParameter.RightIndex, 0f);
            SendParameter(SteamVRParameter.RightMiddle, 0f);
            SendParameter(SteamVRParameter.RightRing, 0f);
            SendParameter(SteamVRParameter.RightPinky, 0f);
        }
    }

    private enum SteamVRParameter
    {
        FPS,
        FPSNormalised,
        UserPresent,
        DashboardVisible,
        HMD_Connected,
        LC_Connected,
        RC_Connected,
        Tracker1_Connected,
        Tracker2_Connected,
        Tracker3_Connected,
        Tracker4_Connected,
        Tracker5_Connected,
        Tracker6_Connected,
        Tracker7_Connected,
        Tracker8_Connected,
        HMD_Battery,
        LC_Battery,
        RC_Battery,
        Tracker1_Battery,
        Tracker2_Battery,
        Tracker3_Battery,
        Tracker4_Battery,
        Tracker5_Battery,
        Tracker6_Battery,
        Tracker7_Battery,
        Tracker8_Battery,
        HMD_Charging,
        LC_Charging,
        RC_Charging,
        Tracker1_Charging,
        Tracker2_Charging,
        Tracker3_Charging,
        Tracker4_Charging,
        Tracker5_Charging,
        Tracker6_Charging,
        Tracker7_Charging,
        Tracker8_Charging,
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
        HMDBattery,
        LeftControllerBattery,
        RightControllerBattery,
        Tracker1Battery,
        Tracker2Battery,
        Tracker3Battery,
        Tracker4Battery,
        Tracker5Battery,
        Tracker6Battery,
        Tracker7Battery,
        Tracker8Battery,
        TrackerAverageBattery,
        HMDCharging,
        LeftControllerCharging,
        RightControllerCharging
    }
}
