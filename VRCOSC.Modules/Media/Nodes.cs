// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using Windows.Media;
using Windows.Media.Control;
using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Media;

[Node("Media Info Source")]
public sealed class MediaInfoSourceNode : Node, IModuleNode<MediaModule>, IContinuousNode
{
    public int UpdateOffset => 0;

    public MediaModule Module { get; set; } = null!;

    public ValueOutput<string> Title = new();
    public ValueOutput<string> Subtitle = new();
    public ValueOutput<string> Genres = new();
    public ValueOutput<string> Artist = new();
    public ValueOutput<int> TrackerNumber = new();
    public ValueOutput<string> AlbumTitle = new();
    public ValueOutput<string> AlbumArtist = new();
    public ValueOutput<int> AlbumTrackCount = new();
    public ValueOutput<float> Volume = new();
    public ValueOutput<float> Progress = new();
    public ValueOutput<TimeSpan> Position = new();
    public ValueOutput<TimeSpan> Duration = new();

    protected override Task Process(IPulseContext c)
    {
        var s = Module.MediaProvider.GetCurrentState();

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
}

[Node("Media State Source")]
public sealed class MediaStateSourceNode : Node, IModuleNode<MediaModule>, IContinuousNode
{
    public int UpdateOffset => 0;

    public MediaModule Module { get; set; } = null!;

    public ValueOutput<bool> Playing = new();
    public ValueOutput<bool> Shuffling = new();
    public ValueOutput<MediaPlaybackAutoRepeatMode> RepeatMode = new();

    protected override Task Process(IPulseContext c)
    {
        var s = Module.MediaProvider.GetCurrentState();

        Playing.Write(s.Status == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing, c);
        Shuffling.Write(s.IsShuffle, c);
        RepeatMode.Write(s.RepeatMode, c);
        return Task.CompletedTask;
    }
}

[Node("Media Set Playback")]
public sealed class MediaSetPlaybackNode() : ActionValueConsumeNode<bool>("Playing"), IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void ConsumeValue(bool playing, IPulseContext c)
    {
        if (playing)
            Module.MediaProvider.Play();
        else
            Module.MediaProvider.Pause();
    }
}

[Node("Media Skip Next")]
public sealed class MediaSkipNextNode : ActionNode, IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void DoAction(IPulseContext c) => Module.MediaProvider.SkipNext();
}

[Node("Media Skip Previous")]
public sealed class MediaSkipPreviousNode : ActionNode, IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void DoAction(IPulseContext c) => Module.MediaProvider.SkipPrevious();
}

[Node("Media Set Shuffle")]
public sealed class MediaSetShuffleNode() : ActionValueConsumeNode<bool>("Shuffle"), IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void ConsumeValue(bool shuffle, IPulseContext c) => Module.MediaProvider.ChangeShuffle(shuffle);
}

[Node("Media Set Repeat Mode")]
public sealed class MediaSetRepeatModeNode() : ActionValueConsumeNode<MediaPlaybackAutoRepeatMode>("Mode"), IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void ConsumeValue(MediaPlaybackAutoRepeatMode mode, IPulseContext c) => Module.MediaProvider.ChangeRepeatMode(mode);
}

[Node("Media Set Playback Position")]
public sealed class MediaSetPlaybackPositionNode() : ActionValueConsumeNode<TimeSpan>("Position"), IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void ConsumeValue(TimeSpan position, IPulseContext c) => Module.MediaProvider.ChangePlaybackPosition(position);
}

[Node("Media Set Volume")]
public sealed class MediaSetVolumeNode() : ActionValueConsumeNode<float>("Volume"), IModuleNode<MediaModule>
{
    public MediaModule Module { get; set; } = null!;

    protected override void ConsumeValue(float volume, IPulseContext c) => Module.MediaProvider.TryChangeVolume(float.Clamp(volume, 0f, 1f));
}