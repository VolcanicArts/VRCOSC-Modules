// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Windows.Media;
using VRCOSC.App.ChatBox.Clips.Variables.Instances;
using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Providers.Media;
using VRCOSC.App.SDK.VRChat;

namespace VRCOSC.Modules.Media;

[ModuleTitle("Media")]
[ModuleDescription("Integration with Windows Media")]
[ModuleType(ModuleType.Integrations)]
[ModulePrefab("VRCOSC-Media", "https://github.com/VolcanicArts/VRCOSC/releases/download/2024.220.1/VRCOSC-Media-2023.629.0.unitypackage")]
public class MediaModule : Module, IVRCClientEventHandler
{
    public WindowsMediaProvider MediaProvider { get; } = new();
    private bool currentlySeeking;
    private TimeSpan targetPosition;
    private bool instanceTransferPlay;

    public MediaModule()
    {
        MediaProvider.OnPlaybackStateChanged += onPlaybackStateChanged;
        MediaProvider.OnTrackChanged += onTrackChanged;
    }

    protected override void OnPreLoad()
    {
        CreateToggle(MediaSetting.PlayOnInstanceTransfer, "Play On Instance Transfer", "Should your media source play, if paused, when transferring between instances?", false);

        RegisterParameter<bool>(MediaParameter.Play, "VRCOSC/Media/Play", ParameterMode.ReadWrite, "Play/Pause", "True for playing. False for paused");
        RegisterParameter<float>(MediaParameter.Volume, "VRCOSC/Media/Volume", ParameterMode.ReadWrite, "Volume", "The volume of the process that is controlling the media");
        RegisterParameter<int>(MediaParameter.Repeat, "VRCOSC/Media/Repeat", ParameterMode.ReadWrite, "Repeat", "0 - Disabled\n1 - Single\n2 - List");
        RegisterParameter<bool>(MediaParameter.Shuffle, "VRCOSC/Media/Shuffle", ParameterMode.ReadWrite, "Shuffle", "True for enabled. False for disabled");
        RegisterParameter<bool>(MediaParameter.Next, "VRCOSC/Media/Next", ParameterMode.Read, "Next", "Becoming true causes the next track to play");
        RegisterParameter<bool>(MediaParameter.Previous, "VRCOSC/Media/Previous", ParameterMode.Read, "Previous", "Becoming true causes the previous track to play");
        RegisterParameter<bool>(MediaParameter.Seeking, "VRCOSC/Media/Seeking", ParameterMode.Read, "Seeking", "Whether the user is currently seeking");
        RegisterParameter<float>(MediaParameter.Position, "VRCOSC/Media/Position", ParameterMode.ReadWrite, "Position", "The position of the song as a percentage");

        SetRuntimeView(typeof(MediaModuleRuntimeView));
    }

    protected override void OnPostLoad()
    {
        var titleReference = CreateVariable<string>(MediaVariable.Title, "Title")!;
        CreateVariable<string>(MediaVariable.Subtitle, "Subtitle");
        CreateVariable<string>(MediaVariable.Genres, "Genres");
        var artistReference = CreateVariable<string>(MediaVariable.Artist, "Artist")!;
        CreateVariable<string>(MediaVariable.ArtistTitle, "Artist + Title");
        var currentTimeReference = CreateVariable<TimeSpan>(MediaVariable.Time, "Current Time")!;
        CreateVariable<TimeSpan>(MediaVariable.TimeRemaining, "Time Remaining");
        var durationReference = CreateVariable<TimeSpan>(MediaVariable.Duration, "Duration")!;
        CreateVariable<int>(MediaVariable.Volume, "Volume");
        CreateVariable<int>(MediaVariable.TrackNumber, "Track Number");
        CreateVariable<string>(MediaVariable.AlbumTitle, "Album Title");
        CreateVariable<string>(MediaVariable.AlbumArtist, "Album Artist");
        CreateVariable<int>(MediaVariable.AlbumTrackCount, "Album Track Count");
        var progressVisualReference = CreateVariable<float>(MediaVariable.ProgressVisual, "Progress Visual", typeof(ProgressClipVariable))!;

        CreateState(MediaState.Playing, "Playing", "[{0}/{1}]\n{2} - {3}\n{4}", new[] { currentTimeReference, durationReference, artistReference, titleReference, progressVisualReference });
        CreateState(MediaState.Paused, "Paused", "[Paused]\n{0} - {1}", new[] { artistReference, titleReference });
        CreateState(MediaState.Stopped, "Stopped", "[No Source]");

        CreateEvent(MediaEvent.OnTrackChange, "On Track Change", "Now Playing\n{0} - {1}", new[] { artistReference, titleReference }, true);
        CreateEvent(MediaEvent.OnPlay, "On Play", "[Playing]\n{0} - {1}", new[] { artistReference, titleReference });
        CreateEvent(MediaEvent.OnPause, "On Pause", "[Paused]\n{0} - {1}", new[] { artistReference, titleReference });
    }

    protected override async Task<bool> OnModuleStart()
    {
        instanceTransferPlay = false;

        var hookResult = await MediaProvider.InitialiseAsync();

        if (!hookResult)
        {
            Log("Could not hook into Windows media\nTry restarting the modules\nIf this persists you will need to restart your PC as Windows has not initialised media correctly");
            return false;
        }

        setState();

        return true;
    }

    protected override Task OnModuleStop()
    {
        MediaProvider.Terminate();
        return Task.CompletedTask;
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        sendUpdatableParameters();
        sendMediaParameters();
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 100)]
    private void fixedUpdate()
    {
        if (MediaProvider.CurrentState.IsPlaying)
        {
            // Hack to allow browsers to have time info
            MediaProvider.Update(TimeSpan.FromMilliseconds(100));
        }

        if (!currentlySeeking)
        {
            SendParameter(MediaParameter.Position, MediaProvider.CurrentState.Timeline.Progress);
        }
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void updateVariables()
    {
        SetVariableValue(MediaVariable.Title, MediaProvider.CurrentState.Title);
        SetVariableValue(MediaVariable.Subtitle, MediaProvider.CurrentState.Subtitle);
        SetVariableValue(MediaVariable.Genres, MediaProvider.CurrentState.Genres.Count != 0 ? string.Join(", ", MediaProvider.CurrentState.Genres) : string.Empty);
        SetVariableValue(MediaVariable.Artist, MediaProvider.CurrentState.Artist);
        SetVariableValue(MediaVariable.ArtistTitle, $"{MediaProvider.CurrentState.Artist} - {MediaProvider.CurrentState.Title}");
        SetVariableValue(MediaVariable.TrackNumber, MediaProvider.CurrentState.TrackNumber);
        SetVariableValue(MediaVariable.AlbumTitle, MediaProvider.CurrentState.AlbumTitle);
        SetVariableValue(MediaVariable.AlbumArtist, MediaProvider.CurrentState.AlbumArtist);
        SetVariableValue(MediaVariable.AlbumTrackCount, MediaProvider.CurrentState.AlbumTrackCount);
        SetVariableValue(MediaVariable.Volume, (int)MathF.Round(MediaProvider.TryGetVolume() * 100));
        SetVariableValue(MediaVariable.ProgressVisual, MediaProvider.CurrentState.Timeline.Progress);
        SetVariableValue(MediaVariable.Time, MediaProvider.CurrentState.Timeline.Position);
        SetVariableValue(MediaVariable.TimeRemaining, MediaProvider.CurrentState.Timeline.End - MediaProvider.CurrentState.Timeline.Position);
        SetVariableValue(MediaVariable.Duration, MediaProvider.CurrentState.Timeline.End);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 1000)]
    private void sendUpdatableParameters()
    {
        SendParameter(MediaParameter.Volume, MediaProvider.TryGetVolume());
    }

    private void onPlaybackStateChanged()
    {
        sendMediaParameters();
        setState();
    }

    private void setState()
    {
        if (MediaProvider.CurrentState.IsPaused)
        {
            ChangeState(MediaState.Paused);
            TriggerEvent(MediaEvent.OnPause);
        }

        if (MediaProvider.CurrentState.IsPlaying)
        {
            ChangeState(MediaState.Playing);
            TriggerEvent(MediaEvent.OnPlay);
        }

        if (MediaProvider.CurrentState.IsStopped)
        {
            ChangeState(MediaState.Stopped);
        }
    }

    private void onTrackChanged()
    {
        TriggerEvent(MediaEvent.OnTrackChange);
    }

    private void sendMediaParameters()
    {
        SendParameter(MediaParameter.Play, MediaProvider.CurrentState.IsPlaying);
        SendParameter(MediaParameter.Shuffle, MediaProvider.CurrentState.IsShuffle);
        SendParameter(MediaParameter.Repeat, (int)MediaProvider.CurrentState.RepeatMode);
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case MediaParameter.Volume:
                MediaProvider.TryChangeVolume(parameter.GetValue<float>());
                break;

            case MediaParameter.Position:
                if (!currentlySeeking) return;

                var position = MediaProvider.CurrentState.Timeline;
                targetPosition = (position.End - position.Start) * parameter.GetValue<float>();
                break;

            case MediaParameter.Repeat:
                MediaProvider.ChangeRepeatMode((MediaPlaybackAutoRepeatMode)parameter.GetValue<int>());
                break;

            case MediaParameter.Play:
                if (parameter.GetValue<bool>())
                    MediaProvider.Play();
                else
                    MediaProvider.Pause();
                break;

            case MediaParameter.Shuffle:
                MediaProvider.ChangeShuffle(parameter.GetValue<bool>());
                break;

            case MediaParameter.Next when parameter.GetValue<bool>():
                MediaProvider.SkipNext();
                break;

            case MediaParameter.Previous when parameter.GetValue<bool>():
                MediaProvider.SkipPrevious();
                break;

            case MediaParameter.Seeking:
                currentlySeeking = parameter.GetValue<bool>();
                if (!currentlySeeking) MediaProvider.ChangePlaybackPosition(targetPosition);
                break;
        }
    }

    public void OnWorldExit()
    {
        if (!GetSettingValue<bool>(MediaSetting.PlayOnInstanceTransfer)) return;

        if (MediaProvider.CurrentState.IsPaused)
        {
            MediaProvider.Play();
            instanceTransferPlay = true;
        }
    }

    public void OnWorldEnter(string worldID)
    {
        if (!instanceTransferPlay) return;

        MediaProvider.Pause();
        instanceTransferPlay = false;
    }

    private enum MediaSetting
    {
        PlayOnInstanceTransfer
    }

    private enum MediaParameter
    {
        Play,
        Next,
        Previous,
        Shuffle,
        Repeat,
        Volume,
        Seeking,
        Position
    }

    private enum MediaState
    {
        Playing,
        Paused,
        Stopped
    }

    private enum MediaEvent
    {
        OnTrackChange,
        OnPlay,
        OnPause
    }

    private enum MediaVariable
    {
        Title,
        Artist,
        ArtistTitle,
        Time,
        TimeRemaining,
        Duration,
        Volume,
        TrackNumber,
        AlbumTitle,
        AlbumArtist,
        AlbumTrackCount,
        ProgressVisual,
        Subtitle,
        Genres
    }
}