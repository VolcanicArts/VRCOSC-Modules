// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

// ReSharper disable once InconsistentNaming

namespace VRCOSC.Modules.AFKDetection;

[ModuleTitle("AFK Detection")]
[ModuleDescription("Tracks if you're AFK in VRChat or SteamVR")]
[ModuleType(ModuleType.Integrations)]
public class AFKTrackerModule : ChatBoxModule
{
    private bool manualAFK;
    private DateTime? afkBegan;

    private bool previousAFKState;

    protected override void OnPreLoad()
    {
        CreateDropdown(AFKTrackerSetting.Source, "Source", "What source should the AFK tracker track?", AFKTrackerSource.SteamVR);

        RegisterParameter<bool>(AFKTrackerParameter.ManualAFK, "VRCOSC/AFKTracker/AFK", ParameterMode.Read, "Manual AFK", "Setting this to true will override any AFK detection to allow you to manually set that you're AFK");

        CreateVariable<TimeSpan>(AFKTrackerVariable.Duration, "Duration");
        CreateVariable<DateTime>(AFKTrackerVariable.StartTime, "Start Time");

        CreateState(AFKTrackerState.NotAFK, "Not AFK");
        CreateState(AFKTrackerState.AFK, "AFK");

        CreateEvent(AFKTrackerEvent.AFKStopped, "AFK Stopped", "AFK has ended");
        CreateEvent(AFKTrackerEvent.AFKStarted, "AFK Started", "AFK has begun");
    }

    protected override Task<bool> OnModuleStart()
    {
        manualAFK = false;
        afkBegan = null;
        previousAFKState = false;

        Log(GetSettingValue<AFKTrackerSource>(AFKTrackerSetting.Source).ToString());

        return Task.FromResult(true);
    }

    protected override void OnRegisteredParameterReceived(AvatarParameter avatarParameter)
    {
        switch (avatarParameter.Lookup)
        {
            case AFKTrackerParameter.ManualAFK:
                manualAFK = avatarParameter.GetValue<bool>();
                break;
        }
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void moduleChatBoxUpdate()
    {
        GetVariable(AFKTrackerVariable.Duration)!.SetValue(afkBegan is null ? TimeSpan.Zero : DateTime.Now - afkBegan.Value);
        GetVariable(AFKTrackerVariable.StartTime)!.SetValue(afkBegan ?? DateTime.UnixEpoch);
        ChangeState(afkBegan is null ? AFKTrackerState.NotAFK : AFKTrackerState.AFK);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000)]
    private void moduleUpdate()
    {
        var isUserAFK = checkForAFK();

        if (isUserAFK && !previousAFKState)
        {
            Log("AFK detected");
            afkBegan = DateTime.Now;
            previousAFKState = true;
        }

        if (!isUserAFK && previousAFKState)
        {
            Log("Not AFK detected");
            afkBegan = null;
            previousAFKState = false;
        }
    }

    private bool checkForAFK()
    {
        if (manualAFK) return true;

        switch (GetSettingValue<AFKTrackerSource>(AFKTrackerSetting.Source))
        {
            case AFKTrackerSource.SteamVR when !OVRClient.HasInitialised:
            case AFKTrackerSource.VRChat:
                return isVRChatAFK();

            case AFKTrackerSource.SteamVR when OVRClient.HasInitialised:
                return OVRClient.System.IsUserPresent();

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool isVRChatAFK() => Player.AFK is not null && Player.AFK.Value;
    private bool isSteamVRAFK() => !OVRClient.System.IsUserPresent();

    private enum AFKTrackerSetting
    {
        Source
    }

    private enum AFKTrackerParameter
    {
        ManualAFK
    }

    private enum AFKTrackerVariable
    {
        Duration,
        StartTime
    }

    private enum AFKTrackerState
    {
        NotAFK,
        AFK
    }

    private enum AFKTrackerEvent
    {
        AFKStopped,
        AFKStarted
    }

    private enum AFKTrackerSource
    {
        VRChat,
        SteamVR
    }
}
