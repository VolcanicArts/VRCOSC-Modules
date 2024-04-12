// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.OVR;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("SteamVR Stats")]
[ModuleDescription("Gathers various stats from SteamVR")]
[ModuleType(ModuleType.Integrations)]
public class SteamVRStatisticsModule : ChatBoxModule
{
    protected override void OnPreLoad()
    {
        RegisterParameter<float>(SteamVRParameter.FPS, "VRCOSC/VR/FPS", ParameterMode.Write, "FPS", "Your measured FPS normalised from 0-240 to 0-1");

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
        CreateVariable<bool>(SteamVRVariable.HMDCharging, "HMD Charging");
        var hmdBatteryReference = CreateVariable<int>(SteamVRVariable.HMDBattery, "HMD Battery (%)")!;
        CreateVariable<bool>(SteamVRVariable.LeftControllerCharging, "Left Controller Charging");
        var lcBatteryReference = CreateVariable<int>(SteamVRVariable.LeftControllerBattery, "Left Controller Battery (%)")!;
        CreateVariable<bool>(SteamVRVariable.RightControllerCharging, "Right Controller Charging");
        var rcBatteryReference = CreateVariable<int>(SteamVRVariable.RightControllerBattery, "Right Controller Battery (%)")!;
        var averageTrackerBatteryReference = CreateVariable<int>(SteamVRVariable.AverageTrackerBattery, "Average Tracker Battery (%)")!;

        CreateState(SteamVRState.Default, "Default", "HMD: {0}\nLC: {1}\nRC: {2}\nTrackers: {3}", new[] { hmdBatteryReference, lcBatteryReference, rcBatteryReference, averageTrackerBatteryReference });
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 5000)]
    private void updateVariablesAndParameters()
    {
        if (OVRClient.HasInitialised)
        {
            SendParameter(SteamVRParameter.FPS, OVRClient.System.FPS / 240.0f);
            updateHmd();
            updateLeftController();
            updateRightController();
            updateTrackers();

            var activeTrackers = OVRClient.Trackers.Where(tracker => tracker.IsConnected).ToList();

            var trackerBatteryAverage = 0f;

            if (activeTrackers.Any())
            {
                trackerBatteryAverage = activeTrackers.Sum(tracker => tracker.BatteryPercentage) / activeTrackers.Count;
            }

            SetVariableValue(SteamVRVariable.FPS, OVRClient.System.FPS);
            SetVariableValue(SteamVRVariable.HMDCharging, OVRClient.HMD.IsCharging);
            SetVariableValue(SteamVRVariable.HMDBattery, (int)(OVRClient.HMD.BatteryPercentage * 100));
            SetVariableValue(SteamVRVariable.LeftControllerCharging, OVRClient.LeftController.IsCharging);
            SetVariableValue(SteamVRVariable.LeftControllerBattery, (int)(OVRClient.LeftController.BatteryPercentage * 100));
            SetVariableValue(SteamVRVariable.RightControllerCharging, OVRClient.RightController.IsCharging);
            SetVariableValue(SteamVRVariable.RightControllerBattery, (int)(OVRClient.RightController.BatteryPercentage * 100));
            SetVariableValue(SteamVRVariable.AverageTrackerBattery, (int)(trackerBatteryAverage * 100));
        }
        else
        {
            SetVariableValue(SteamVRVariable.FPS, 0f);
            SetVariableValue(SteamVRVariable.HMDCharging, false);
            SetVariableValue(SteamVRVariable.HMDBattery, 0);
            SetVariableValue(SteamVRVariable.LeftControllerCharging, false);
            SetVariableValue(SteamVRVariable.LeftControllerBattery, 0);
            SetVariableValue(SteamVRVariable.RightControllerCharging, false);
            SetVariableValue(SteamVRVariable.RightControllerBattery, 0);
            SetVariableValue(SteamVRVariable.AverageTrackerBattery, 0);

            SendParameter(SteamVRParameter.FPS, 0f);

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

    private void updateHmd()
    {
        SendParameter(SteamVRParameter.HMD_Connected, OVRClient.HMD.IsConnected);

        if (OVRClient.HMD.IsConnected && OVRClient.HMD.ProvidesBatteryStatus)
        {
            SendParameter(SteamVRParameter.HMD_Battery, OVRClient.HMD.BatteryPercentage);
            SendParameter(SteamVRParameter.HMD_Charging, OVRClient.HMD.IsCharging);
        }
    }

    private void updateLeftController()
    {
        SendParameter(SteamVRParameter.LC_Connected, OVRClient.LeftController.IsConnected);

        if (OVRClient.LeftController.IsConnected)
        {
            if (OVRClient.LeftController.ProvidesBatteryStatus)
            {
                SendParameter(SteamVRParameter.LC_Battery, OVRClient.LeftController.BatteryPercentage);
                SendParameter(SteamVRParameter.LC_Charging, OVRClient.LeftController.IsCharging);
            }
            else
            {
                SendParameter(SteamVRParameter.LC_Battery, 0f);
                SendParameter(SteamVRParameter.LC_Charging, false);
            }

            var input = OVRClient.LeftController.Input;
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
            SendParameter(SteamVRParameter.LC_Battery, 0f);
            SendParameter(SteamVRParameter.LC_Charging, false);
        }
    }

    private void updateRightController()
    {
        SendParameter(SteamVRParameter.RC_Connected, OVRClient.RightController.IsConnected);

        if (OVRClient.RightController.IsConnected)
        {
            if (OVRClient.RightController.ProvidesBatteryStatus)
            {
                SendParameter(SteamVRParameter.RC_Battery, OVRClient.RightController.BatteryPercentage);
                SendParameter(SteamVRParameter.RC_Charging, OVRClient.RightController.IsCharging);
            }
            else
            {
                SendParameter(SteamVRParameter.RC_Battery, 0f);
                SendParameter(SteamVRParameter.RC_Charging, false);
            }

            if (OVRClient.RightController.IsConnected)
            {
                var input = OVRClient.RightController.Input;
                SendParameter(SteamVRParameter.RightATouch, input.A.Touched);
                SendParameter(SteamVRParameter.RightBTouch, input.B.Touched);
                SendParameter(SteamVRParameter.RightPadTouch, input.PadTouched);
                SendParameter(SteamVRParameter.RightStickTouch, input.StickTouched);
                SendParameter(SteamVRParameter.RightIndex, input.IndexFinger);
                SendParameter(SteamVRParameter.RightMiddle, input.MiddleFinger);
                SendParameter(SteamVRParameter.RightRing, input.RingFinger);
                SendParameter(SteamVRParameter.RightPinky, input.PinkyFinger);
            }
        }
        else
        {
            SendParameter(SteamVRParameter.RC_Battery, 0f);
            SendParameter(SteamVRParameter.RC_Charging, false);
        }
    }

    private void updateTrackers()
    {
        var trackers = OVRClient.Trackers.ToList();

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

    private enum SteamVRParameter
    {
        FPS,
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
        HMDBattery,
        LeftControllerBattery,
        RightControllerBattery,
        AverageTrackerBattery,
        HMDCharging,
        LeftControllerCharging,
        RightControllerCharging
    }
}
