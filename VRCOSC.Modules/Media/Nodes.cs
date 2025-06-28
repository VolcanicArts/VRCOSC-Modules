// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using Windows.Media;
using Windows.Media.Control;
using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Media;

[Node("Media Info Source")]
public sealed class MediaInfoSourceNode : ModuleNode<MediaModule>, IUpdateNode
{
    public ValueOutput<string> Title = new();
    public ValueOutput<string> Subtitle = new();
    public ValueOutput<string> Genres = new();
    public ValueOutput<string> Artist = new();
    public ValueOutput<int> TrackerNumber = new("Track Number");
    public ValueOutput<string> AlbumTitle = new("Album Title");
    public ValueOutput<string> AlbumArtist = new("Album Artist");
    public ValueOutput<int> AlbumTrackCount = new("Album Track Count");
    public ValueOutput<float> Volume = new();
    public ValueOutput<float> Progress = new();
    public ValueOutput<TimeSpan> Position = new();
    public ValueOutput<TimeSpan> Duration = new();

    protected override Task Process(PulseContext c)
    {
        var s = Module.MediaProvider.CurrentState;

        Title.Write(s.Title, c);
        Subtitle.Write(s.Subtitle, c);
        Genres.Write(string.Join(", ", s.Genres), c);
        Artist.Write(s.Artist, c);
        TrackerNumber.Write(s.TrackNumber, c);
        AlbumTitle.Write(s.AlbumTitle, c);
        AlbumArtist.Write(s.AlbumArtist, c);
        AlbumTrackCount.Write(s.AlbumTrackCount, c);
        Volume.Write(Module.CurrentVolume, c);
        Progress.Write(s.Timeline.Progress, c);
        Position.Write(s.Timeline.Position, c);
        Duration.Write(s.Timeline.End, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}

[Node("Media State Source")]
public sealed class MediaStateSourceNode : ModuleNode<MediaModule>, IUpdateNode
{
    public ValueOutput<bool> Playing = new();
    public ValueOutput<bool> Shuffling = new();
    public ValueOutput<MediaPlaybackAutoRepeatMode> RepeatMode = new("Repeat Mode");

    protected override Task Process(PulseContext c)
    {
        var s = Module.MediaProvider.CurrentState;

        Playing.Write(s.Status == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing, c);
        Shuffling.Write(s.IsShuffle, c);
        RepeatMode.Write(s.RepeatMode, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}

[Node("Media Set Playback")]
public sealed class MediaSetPlaybackNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<bool> Playing = new();

    protected override async Task Process(PulseContext c)
    {
        var playing = Playing.Read(c);

        if (playing)
            Module.MediaProvider.Play();
        else
            Module.MediaProvider.Pause();

        await Next.Execute(c);
    }
}

[Node("Media Skip Next")]
public sealed class MediaSkipNextNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    protected override async Task Process(PulseContext c)
    {
        Module.MediaProvider.SkipNext();
        await Next.Execute(c);
    }
}

[Node("Media Skip Previous")]
public sealed class MediaSkipPreviousNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    protected override async Task Process(PulseContext c)
    {
        Module.MediaProvider.SkipPrevious();
        await Next.Execute(c);
    }
}

[Node("Media Set Shuffle")]
public sealed class MediaSetShuffleNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<bool> Shuffle = new();

    protected override async Task Process(PulseContext c)
    {
        var shuffle = Shuffle.Read(c);

        Module.MediaProvider.ChangeShuffle(shuffle);
        await Next.Execute(c);
    }
}

[Node("Media Set Repeat Mode")]
public sealed class MediaSetRepeatModeNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<MediaPlaybackAutoRepeatMode> Mode = new();

    protected override async Task Process(PulseContext c)
    {
        var mode = Mode.Read(c);

        Module.MediaProvider.ChangeRepeatMode(mode);
        await Next.Execute(c);
    }
}

[Node("Media Set Playback Position")]
public sealed class MediaSetPlaybackPositionNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<TimeSpan> Position = new();

    protected override async Task Process(PulseContext c)
    {
        var position = Position.Read(c);

        Module.MediaProvider.ChangePlaybackPosition(position);
        await Next.Execute(c);
    }
}

[Node("Media Set Volume")]
public sealed class MediaSetVolumeNode : ModuleNode<MediaModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<float> Volume = new();

    protected override async Task Process(PulseContext c)
    {
        var volume = float.Clamp(Volume.Read(c), 0f, 1f);

        Module.MediaProvider.TryChangeVolume(volume);
        await Next.Execute(c);
    }
}