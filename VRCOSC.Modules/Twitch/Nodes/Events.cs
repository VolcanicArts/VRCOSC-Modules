// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.Modules.Twitch.Data;

namespace VRCOSC.Modules.Twitch.Nodes;

[Node("On Twitch Channel Chat Message", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelChatMessageNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchMessage> Message = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Message.Write((TwitchMessage)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Follow", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelFollowNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchFollow> Follow = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Follow.Write((TwitchFollow)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Subscription", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelSubscriptionNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchSubscription> Subscription = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Subscription.Write((TwitchSubscription)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel ReSubscription", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelReSubscriptionNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchReSubscription> ReSubscription = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        ReSubscription.Write((TwitchReSubscription)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Reward Redemption", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelRewardRedemptionNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchRewardRedemption> RewardRedemption = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        RewardRedemption.Write((TwitchRewardRedemption)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Gift Subscription", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelGiftSubscriptionNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGiftSubscription> GiftSubscription = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        GiftSubscription.Write((TwitchGiftSubscription)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Bits", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelBitsNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchBits> Bits = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Bits.Write((TwitchBits)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Raid", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelRaidNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchRaid> Raid = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Raid.Write((TwitchRaid)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Goal Begin", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelGoalBeginNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGoal> Goal = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Goal.Write((TwitchGoal)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Goal Progress", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelGoalProgressNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGoal> Goal = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Goal.Write((TwitchGoal)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Goal End", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelGoalEndNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGoal> Goal = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Goal.Write((TwitchGoal)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Update", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelUpdateNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchChannel> Channel = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Channel.Write((TwitchChannel)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Ban", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelBanNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchBan> Ban = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Ban.Write((TwitchBan)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel HypeTrain Begin", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelHypeTrainBeginNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchHypeTrain> HypeTrain = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        HypeTrain.Write((TwitchHypeTrain)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel HypeTrain Progress", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelHypeTrainProgressNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchHypeTrain> HypeTrain = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        HypeTrain.Write((TwitchHypeTrain)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel HypeTrain End", "Events")]
[NodeNoCancel]
public sealed class TwitchChannelHypeTrainEndNode : Node, IModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public TwitchModule Module { get; set; } = null!;

    public FlowOutput Next = new();

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchHypeTrain> HypeTrain = new();

    protected override Task Process(IPulseContext c) => Next.Execute(c);

    public Task Write(object[] args, IPulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        HypeTrain.Write((TwitchHypeTrain)args[1], c);
        return Task.CompletedTask;
    }
}