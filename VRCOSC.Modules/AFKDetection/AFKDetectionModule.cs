// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.Utils;

// ReSharper disable once InconsistentNaming

namespace VRCOSC.Modules.AFKDetection;

[ModuleTitle("AFK Detection")]
[ModuleDescription("Tracks if you're AFK in VRChat or SteamVR")]
[ModuleType(ModuleType.Integrations)]
public class AFKDetectionModule : Module
{
    private bool manualAFK;
    private DateTimeOffset? afkBegan;

    private bool previousAFKState;

    protected override void OnPreLoad()
    {
        CreateDropdown(AFKDetectionSetting.Source, "Source", "What source should be queried for if you're AFK?\nIf SteamVR is selected but unavailable, it will fallback to using VRChat", AFKDetectionSource.VRChat);
        CreateToggle(AFKDetectionSetting.ManageVRChatWindow, "Manage VRChat Window", "Maximise the VRChat window when you're AFK, and minimise the VRChat window when in-game. This is only useful when using a VR headset", false);

        RegisterParameter<bool>(AFKDetectionParameter.ManualAFK, "VRCOSC/AFKDetection/AFK", ParameterMode.Read, "Manual AFK", "Setting this to true will override the AFK source to allow you to manually set that you're AFK");

        var durationReference = CreateVariable<TimeSpan>(AFKDetectionVariable.Duration, "Duration")!;
        CreateVariable<DateTimeOffset>(AFKDetectionVariable.StartTime, "Start Time");

        CreateState(AFKDetectionState.NotAFK, "Not AFK");
        CreateState(AFKDetectionState.AFK, "AFK", "AFK for {0}", new[] { durationReference });

        CreateEvent(AFKDetectionEvent.AFKStopped, "AFK Stopped", "AFK has ended");
        CreateEvent(AFKDetectionEvent.AFKStarted, "AFK Started", "AFK has begun");
    }

    protected override Task<bool> OnModuleStart()
    {
        manualAFK = false;
        afkBegan = null;
        previousAFKState = false;

        return Task.FromResult(true);
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case AFKDetectionParameter.ManualAFK:
                manualAFK = parameter.GetValue<bool>();
                break;
        }
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void moduleChatBoxUpdate()
    {
        SetVariableValue(AFKDetectionVariable.Duration, afkBegan is null ? TimeSpan.Zero : DateTimeOffset.UtcNow - afkBegan.Value);
        SetVariableValue(AFKDetectionVariable.StartTime, afkBegan ?? DateTimeOffset.UnixEpoch);
        ChangeState(afkBegan is null ? AFKDetectionState.NotAFK : AFKDetectionState.AFK);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000)]
    private void moduleUpdate()
    {
        var isUserAFK = checkForAFK();

        if (isUserAFK && !previousAFKState)
        {
            Log("User is now AFK");
            afkBegan = DateTimeOffset.UtcNow;
            manageVRChatWindow(true);
            previousAFKState = true;
        }

        if (!isUserAFK && previousAFKState)
        {
            Log("User is no longer AFK");
            afkBegan = null;
            manageVRChatWindow(false);
            previousAFKState = false;
        }
    }

    private bool checkForAFK()
    {
        if (manualAFK) return true;

        switch (GetSettingValue<AFKDetectionSource>(AFKDetectionSetting.Source))
        {
            case AFKDetectionSource.SteamVR when !GetOVRClient().HasInitialised:
            case AFKDetectionSource.VRChat:
                return isVRChatAFK();

            case AFKDetectionSource.SteamVR when GetOVRClient().HasInitialised:
                return isSteamVRAFK();

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void manageVRChatWindow(bool showWindow)
    {
        if (!GetSettingValue<bool>(AFKDetectionSetting.ManageVRChatWindow)) return;

        var vrchatProcesses = Process.GetProcessesByName("vrchat");
        if (vrchatProcesses.Length != 1) return;

        vrchatProcesses[0].SetWindowVisibility(showWindow);
    }

    private bool isVRChatAFK() => GetPlayer().AFK;
    private bool isSteamVRAFK() => !GetOVRClient().IsUserPresent();

    private enum AFKDetectionSetting
    {
        Source,
        ManageVRChatWindow
    }

    private enum AFKDetectionParameter
    {
        ManualAFK
    }

    private enum AFKDetectionVariable
    {
        Duration,
        StartTime
    }

    private enum AFKDetectionState
    {
        NotAFK,
        AFK
    }

    private enum AFKDetectionEvent
    {
        AFKStopped,
        AFKStarted
    }

    private enum AFKDetectionSource
    {
        VRChat,
        SteamVR
    }
}