// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

// ReSharper disable once InconsistentNaming

namespace VRCOSC.Modules.AFKDetection;

[ModuleTitle("AFK Detection")]
[ModuleDescription("Tracks if you're AFK in VRChat or SteamVR")]
[ModuleType(ModuleType.Integrations)]
public class AFKDetectionModule : ChatBoxModule
{
    private bool manualAFK;
    private DateTimeOffset? afkBegan;

    private bool previousAFKState;

    protected override void OnPreLoad()
    {
        CreateDropdown(AFKDetectionSetting.Source, "Source", "What source should be queried for if you're AFK?", AFKDetectionSource.VRChat);

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

    protected override void OnRegisteredParameterReceived(AvatarParameter avatarParameter)
    {
        switch (avatarParameter.Lookup)
        {
            case AFKDetectionParameter.ManualAFK:
                manualAFK = avatarParameter.GetValue<bool>();
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
            previousAFKState = true;
        }

        if (!isUserAFK && previousAFKState)
        {
            Log("User is no longer AFK");
            afkBegan = null;
            previousAFKState = false;
        }
    }

    private bool checkForAFK()
    {
        if (manualAFK) return true;

        switch (GetSettingValue<AFKDetectionSource>(AFKDetectionSetting.Source))
        {
            case AFKDetectionSource.SteamVR when !OVRClient.HasInitialised:
            case AFKDetectionSource.VRChat:
                return isVRChatAFK();

            case AFKDetectionSource.SteamVR when OVRClient.HasInitialised:
                return isSteamVRAFK();

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool isVRChatAFK() => VRChatClient.Player.AFK is not null && VRChatClient.Player.AFK.Value;
    private bool isSteamVRAFK() => !OVRClient.IsUserPresent();

    private enum AFKDetectionSetting
    {
        Source
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
