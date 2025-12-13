// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.Modules.Twitch.Data;

namespace VRCOSC.Modules.Twitch.Nodes;

[Node("On Twitch Channel Chat Message")]
public sealed class TwitchChannelChatMessageNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Message");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchMessage> Message = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Message.Write((TwitchMessage)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Follow")]
public sealed class TwitchChannelFollowNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Follow");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchFollow> Follow = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Follow.Write((TwitchFollow)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Subscription")]
public sealed class TwitchChannelSubscriptionNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Subscription");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchSubscription> Subscription = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Subscription.Write((TwitchSubscription)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel ReSubscription")]
public sealed class TwitchChannelReSubscriptionNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On ReSubscription");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchReSubscription> ReSubscription = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        ReSubscription.Write((TwitchReSubscription)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Reward Redemption")]
public sealed class TwitchChannelRewardRedemptionNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Reward Redemption");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchRewardRedemption> RewardRedemption = new("Reward Redemption");

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        RewardRedemption.Write((TwitchRewardRedemption)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Gift Subscription")]
public sealed class TwitchChannelGiftSubscriptionNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Gift Subscription");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGiftSubscription> GiftSubscription = new("Gift Subscription");

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        GiftSubscription.Write((TwitchGiftSubscription)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Bits")]
public sealed class TwitchChannelBitsNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Bits");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchBits> Bits = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Bits.Write((TwitchBits)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Raid")]
public sealed class TwitchChannelRaidNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Raid");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchRaid> Raid = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Raid.Write((TwitchRaid)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Goal Begin")]
public sealed class TwitchChannelGoalBeginNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Goal Begin");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGoal> Goal = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Goal.Write((TwitchGoal)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Goal Progress")]
public sealed class TwitchChannelGoalProgressNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Goal Progress");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGoal> Goal = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Goal.Write((TwitchGoal)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Goal End")]
public sealed class TwitchChannelGoalEndNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Goal End");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchGoal> Goal = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Goal.Write((TwitchGoal)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Update")]
public sealed class TwitchChannelUpdateNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Update");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchChannel> Channel = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Channel.Write((TwitchChannel)args[1], c);
        return Task.CompletedTask;
    }
}

[Node("On Twitch Channel Ban")]
public sealed class TwitchChannelBanNode : ModuleNode<TwitchModule>, IModuleNodeEventHandler
{
    public FlowContinuation Next = new("On Ban");

    public ValueOutput<TwitchUser> Broadcaster = new();
    public ValueOutput<TwitchBan> Ban = new();

    protected override async Task Process(PulseContext c)
    {
        await Next.Execute(c);
    }

    public Task Write(object[] args, PulseContext c)
    {
        Broadcaster.Write((TwitchUser)args[0], c);
        Ban.Write((TwitchBan)args[1], c);
        return Task.CompletedTask;
    }
}