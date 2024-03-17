// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Modules;
using VRCOSC.App.Parameters;
using VRCOSC.OVR;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("OpenVR Statistics")]
[ModuleDescription("Gets statistics from your OpenVR (SteamVR) session")]
[ModuleType(ModuleType.Integrations)]
public class OpenVRStatisticsModule : AvatarModule
{
    protected override void OnPreLoad()
    {
        RegisterParameter<float>(OpenVrParameter.FPS, "VRCOSC/OpenVR/FPS", ParameterMode.Write, "FPS", "The current FPS normalised to 240 FPS");

        RegisterParameter<bool>(OpenVrParameter.HMD_Connected, "VRCOSC/OpenVR/HMD/Connected", ParameterMode.Write, "HMD Connected", "Whether your headset is connected");
        RegisterParameter<float>(OpenVrParameter.HMD_Battery, "VRCOSC/OpenVR/HMD/Battery", ParameterMode.Write, "HMD Battery", "The battery percentage normalised of your headset");
        RegisterParameter<bool>(OpenVrParameter.HMD_Charging, "VRCOSC/OpenVR/HMD/Charging", ParameterMode.Write, "HMD Charging", "The charge state of your headset");

        RegisterParameter<bool>(OpenVrParameter.LeftController_Connected, "VRCOSC/OpenVR/LeftController/Connected", ParameterMode.Write, "Left Controller Connected", "Whether your left controller is connected");
        RegisterParameter<float>(OpenVrParameter.LeftController_Battery, "VRCOSC/OpenVR/LeftController/Battery", ParameterMode.Write, "Left Controller Battery", "The battery percentage normalised of your left controller");
        RegisterParameter<bool>(OpenVrParameter.LeftController_Charging, "VRCOSC/OpenVR/LeftController/Charging", ParameterMode.Write, "Left Controller Charging", "The charge state of your left controller");
        RegisterParameter<bool>(OpenVrParameter.RightController_Connected, "VRCOSC/OpenVR/RightController/Connected", ParameterMode.Write, "Right Controller Connected", "Whether your right controller is connected");
        RegisterParameter<float>(OpenVrParameter.RightController_Battery, "VRCOSC/OpenVR/RightController/Battery", ParameterMode.Write, "Right Controller Battery", "The battery percentage normalised of your right controller");
        RegisterParameter<bool>(OpenVrParameter.RightController_Charging, "VRCOSC/OpenVR/RightController/Charging", ParameterMode.Write, "Right Controller Charging", "The charge state of your right controller");

        for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
        {
            RegisterParameter<bool>(OpenVrParameter.Tracker1_Connected + i, $"VRCOSC/OpenVR/Trackers/{i + 1}/Connected", ParameterMode.Write, $"Tracker {i + 1} Connected", $"Whether tracker {i + 1} is connected");
            RegisterParameter<float>(OpenVrParameter.Tracker1_Battery + i, $"VRCOSC/OpenVR/Trackers/{i + 1}/Battery", ParameterMode.Write, $"Tracker {i + 1} Battery", $"The battery percentage normalised (0-1) of tracker {i + 1}");
            RegisterParameter<bool>(OpenVrParameter.Tracker1_Charging + i, $"VRCOSC/OpenVR/Trackers/{i + 1}/Charging", ParameterMode.Write, $"Tracker {i + 1} Charging", $"Whether tracker {i + 1} is currently charging");
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 5000)]
    private void updateVariablesAndParameters()
    {
        if (OVRClient.HasInitialised)
        {
            SendParameter(OpenVrParameter.FPS, OVRClient.System.FPS / 240.0f);
            updateHmd();
            updateLeftController();
            updateRightController();
            updateTrackers();
        }
        else
        {
            SendParameter(OpenVrParameter.HMD_Connected, false);
            SendParameter(OpenVrParameter.HMD_Battery, 0f);
            SendParameter(OpenVrParameter.HMD_Charging, false);

            SendParameter(OpenVrParameter.LeftController_Connected, false);
            SendParameter(OpenVrParameter.LeftController_Battery, 0f);
            SendParameter(OpenVrParameter.LeftController_Charging, false);

            SendParameter(OpenVrParameter.RightController_Connected, false);
            SendParameter(OpenVrParameter.RightController_Battery, 0f);
            SendParameter(OpenVrParameter.RightController_Charging, false);

            for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
            {
                SendParameter(OpenVrParameter.Tracker1_Connected + i, false);
                SendParameter(OpenVrParameter.Tracker1_Battery + i, 0f);
                SendParameter(OpenVrParameter.Tracker1_Charging + i, false);
            }
        }
    }

    private void updateHmd()
    {
        SendParameter(OpenVrParameter.HMD_Connected, OVRClient.HMD.IsConnected);

        if (OVRClient.HMD.IsConnected && OVRClient.HMD.ProvidesBatteryStatus)
        {
            SendParameter(OpenVrParameter.HMD_Battery, OVRClient.HMD.BatteryPercentage);
            SendParameter(OpenVrParameter.HMD_Charging, OVRClient.HMD.IsCharging);
        }
    }

    private void updateLeftController()
    {
        SendParameter(OpenVrParameter.LeftController_Connected, OVRClient.LeftController.IsConnected);

        if (OVRClient.LeftController.IsConnected && OVRClient.LeftController.ProvidesBatteryStatus)
        {
            SendParameter(OpenVrParameter.LeftController_Battery, OVRClient.LeftController.BatteryPercentage);
            SendParameter(OpenVrParameter.LeftController_Charging, OVRClient.LeftController.IsCharging);
        }
        else
        {
            SendParameter(OpenVrParameter.LeftController_Battery, 0f);
            SendParameter(OpenVrParameter.LeftController_Charging, false);
        }
    }

    private void updateRightController()
    {
        SendParameter(OpenVrParameter.RightController_Connected, OVRClient.RightController.IsConnected);

        if (OVRClient.RightController.IsConnected && OVRClient.RightController.ProvidesBatteryStatus)
        {
            SendParameter(OpenVrParameter.RightController_Battery, OVRClient.RightController.BatteryPercentage);
            SendParameter(OpenVrParameter.RightController_Charging, OVRClient.RightController.IsCharging);
        }
        else
        {
            SendParameter(OpenVrParameter.RightController_Battery, 0f);
            SendParameter(OpenVrParameter.RightController_Charging, false);
        }
    }

    private void updateTrackers()
    {
        var trackers = OVRClient.Trackers.ToList();

        for (int i = 0; i < OVRSystem.MAX_TRACKER_COUNT; i++)
        {
            var tracker = trackers[i];

            SendParameter(OpenVrParameter.Tracker1_Connected + i, tracker.IsConnected);

            if (tracker.IsConnected && tracker.ProvidesBatteryStatus)
            {
                SendParameter(OpenVrParameter.Tracker1_Battery + i, tracker.BatteryPercentage);
                SendParameter(OpenVrParameter.Tracker1_Charging + i, tracker.IsCharging);
            }
            else
            {
                SendParameter(OpenVrParameter.Tracker1_Battery + i, 0f);
                SendParameter(OpenVrParameter.Tracker1_Charging + i, false);
            }
        }
    }

    private enum OpenVrParameter
    {
        FPS,
        HMD_Connected,
        LeftController_Connected,
        RightController_Connected,
        Tracker1_Connected,
        Tracker2_Connected,
        Tracker3_Connected,
        Tracker4_Connected,
        Tracker5_Connected,
        Tracker6_Connected,
        Tracker7_Connected,
        Tracker8_Connected,
        HMD_Battery,
        LeftController_Battery,
        RightController_Battery,
        Tracker1_Battery,
        Tracker2_Battery,
        Tracker3_Battery,
        Tracker4_Battery,
        Tracker5_Battery,
        Tracker6_Battery,
        Tracker7_Battery,
        Tracker8_Battery,
        HMD_Charging,
        LeftController_Charging,
        RightController_Charging,
        Tracker1_Charging,
        Tracker2_Charging,
        Tracker3_Charging,
        Tracker4_Charging,
        Tracker5_Charging,
        Tracker6_Charging,
        Tracker7_Charging,
        Tracker8_Charging
    }

    private enum OpenVrState
    {
        Default
    }

    private enum OpenVrVariable
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
