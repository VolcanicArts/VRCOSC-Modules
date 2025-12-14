// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;
using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.Modules.Twitch.Data;

namespace VRCOSC.Modules.Twitch.Nodes;

[Node("Twitch Message Unpack", "Structs")]
public sealed class TwitchMessageUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchMessage> Message = new();

    public ValueOutput<string> Id = new();
    public ValueOutput<TwitchMessageType> Type = new();
    public ValueOutput<TwitchUser> Chatter = new();
    public ValueOutput<string> Text = new();

    protected override Task Process(PulseContext c)
    {
        var message = Message.Read(c);
        if (message is null) return Task.CompletedTask;

        Id.Write(message.Id, c);
        Type.Write(message.Type, c);
        Chatter.Write(message.Chatter, c);
        Text.Write(message.Text, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch User Unpack", "Structs")]
public sealed class TwitchUserUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchUser> User = new();

    public ValueOutput<string> Id = new();
    public ValueOutput<string> Username = new();
    public ValueOutput<TwitchUserRole> Roles = new();

    protected override Task Process(PulseContext c)
    {
        var user = User.Read(c);
        if (user is null) return Task.CompletedTask;

        Id.Write(user.Id, c);
        Username.Write(user.Username, c);
        Roles.Write(user.Roles, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Follow Unpack", "Structs")]
public sealed class TwitchFollowUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchFollow> Follow = new();

    public ValueOutput<DateTime> Timestamp = new();
    public ValueOutput<TwitchUser> User = new();

    protected override Task Process(PulseContext c)
    {
        var follow = Follow.Read(c);
        if (follow is null) return Task.CompletedTask;

        Timestamp.Write(follow.Timestamp, c);
        User.Write(follow.User, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Subscription Unpack", "Structs")]
public sealed class TwitchSubscriptionUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchSubscription> Subscription = new();

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchSubscriptionTier> Tier = new();
    public ValueOutput<bool> IsGift = new("Is Gift");

    protected override Task Process(PulseContext c)
    {
        var subscription = Subscription.Read(c);
        if (subscription is null) return Task.CompletedTask;

        User.Write(subscription.User, c);
        Tier.Write(subscription.Tier, c);
        IsGift.Write(subscription.IsGift, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch ReSubscription Unpack", "Structs")]
public sealed class TwitchReSubscriptionUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchReSubscription> ReSubscription = new();

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchSubscriptionTier> Tier = new();
    public ValueOutput<string> Message = new();
    public ValueOutput<int> CumulativeMonths = new("Cumulative Months");
    public ValueOutput<int> DurationMonths = new("Duration Months");
    public ValueOutput<int> StreakMonths = new("Streak Months");

    protected override Task Process(PulseContext c)
    {
        var reSubscription = ReSubscription.Read(c);
        if (reSubscription is null) return Task.CompletedTask;

        User.Write(reSubscription.User, c);
        Tier.Write(reSubscription.Tier, c);
        Message.Write(reSubscription.Message, c);
        CumulativeMonths.Write(reSubscription.CumulativeMonths, c);
        DurationMonths.Write(reSubscription.DurationMonths, c);
        StreakMonths.Write(reSubscription.StreakMonths, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Reward Unpack", "Structs")]
public sealed class TwitchRewardUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchReward> Reward = new();

    public ValueOutput<string> Id = new();
    public ValueOutput<string> Title = new();
    public ValueOutput<string> Prompt = new();
    public ValueOutput<int> Cost = new();

    protected override Task Process(PulseContext c)
    {
        var reward = Reward.Read(c);
        if (reward is null) return Task.CompletedTask;

        Id.Write(reward.Id, c);
        Title.Write(reward.Title, c);
        Prompt.Write(reward.Prompt, c);
        Cost.Write(reward.Cost, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Reward Redemption Unpack", "Structs")]
public sealed class TwitchRewardRedemptionUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchRewardRedemption> RewardRedemption = new("Reward Redemption");

    public ValueOutput<TwitchReward> Reward = new();
    public ValueOutput<DateTime> Timestamp = new();
    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchRewardRedemptionStatus> Status = new();

    protected override Task Process(PulseContext c)
    {
        var rewardRedemption = RewardRedemption.Read(c);
        if (rewardRedemption is null) return Task.CompletedTask;

        Reward.Write(rewardRedemption.Reward, c);
        Timestamp.Write(rewardRedemption.Timestamp, c);
        User.Write(rewardRedemption.User, c);
        Status.Write(rewardRedemption.Status, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Gift Subscription Unpack", "Structs")]
public sealed class TwitchGiftSubscriptionUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchGiftSubscription> GiftSubscription = new("Gift Subscription");

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchSubscriptionTier> Tier = new();
    public ValueOutput<int> Total = new();
    public ValueOutput<int> CumulativeTotal = new();

    protected override Task Process(PulseContext c)
    {
        var giftSubscription = GiftSubscription.Read(c);
        if (giftSubscription is null) return Task.CompletedTask;

        User.Write(giftSubscription.User, c);
        Tier.Write(giftSubscription.Tier, c);
        Total.Write(giftSubscription.Total, c);
        CumulativeTotal.Write(giftSubscription.CumulativeTotal, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Bits Unpack", "Structs")]
public sealed class TwitchBitsUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchBits> Bits = new();

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<int> Amount = new();
    public ValueOutput<string> Message = new();

    protected override Task Process(PulseContext c)
    {
        var bits = Bits.Read(c);
        if (bits is null) return Task.CompletedTask;

        User.Write(bits.User, c);
        Amount.Write(bits.Amount, c);
        Message.Write(bits.Message, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Raid Unpack", "Structs")]
public sealed class TwitchRaidUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchRaid> Raid = new();

    public ValueOutput<TwitchUser> RaidingBroadcaster = new("Raiding Broadcaster");
    public ValueOutput<int> Viewers = new();

    protected override Task Process(PulseContext c)
    {
        var raid = Raid.Read(c);
        if (raid is null) return Task.CompletedTask;

        RaidingBroadcaster.Write(raid.RaidingBroadcaster, c);
        Viewers.Write(raid.Viewers, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Goal Unpack", "Structs")]
public sealed class TwitchGoalUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchGoal> Goal = new();

    public ValueOutput<string> Id = new();
    public ValueOutput<TwitchGoalType> Type = new();
    public ValueOutput<string> Description = new();
    public ValueOutput<DateTime> StartTimestamp = new("Start Timestamp");
    public ValueOutput<int> TargetAmount = new("Target Amount");
    public ValueOutput<int> CurrentAmount = new("Current Amount");

    protected override Task Process(PulseContext c)
    {
        var goal = Goal.Read(c);
        if (goal is null) return Task.CompletedTask;

        Id.Write(goal.Id, c);
        Type.Write(goal.Type, c);
        Description.Write(goal.Description, c);
        StartTimestamp.Write(goal.StartTimestamp, c);
        TargetAmount.Write(goal.TargetAmount, c);
        CurrentAmount.Write(goal.CurrentAmount, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Category Unpack", "Structs")]
public sealed class TwitchCategoryUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchCategory> Category = new();

    public ValueOutput<string> Id = new();
    public ValueOutput<string> Name = new();

    protected override Task Process(PulseContext c)
    {
        var category = Category.Read(c);
        if (category is null) return Task.CompletedTask;

        Id.Write(category.Id, c);
        Name.Write(category.Name, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Channel Unpack", "Structs")]
public sealed class TwitchChannelUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchChannel> Channel = new();

    public ValueOutput<string> Title = new();
    public ValueOutput<CultureInfo> Language = new();
    public ValueOutput<TwitchCategory> Category = new();

    protected override Task Process(PulseContext c)
    {
        var channel = Channel.Read(c);
        if (channel is null) return Task.CompletedTask;

        Title.Write(channel.Title, c);
        Language.Write(channel.Language, c);
        Category.Write(channel.Category, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch Ban Unpack", "Structs")]
public sealed class TwitchBanUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchBan> Ban = new();

    public ValueOutput<TwitchUser> Moderator = new();
    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<DateTime> StartTimestamp = new("Start Timestamp");
    public ValueOutput<DateTime> EndTimestamp = new("End Timestamp");
    public ValueOutput<string> Reason = new();

    protected override Task Process(PulseContext c)
    {
        var ban = Ban.Read(c);
        if (ban is null) return Task.CompletedTask;

        Moderator.Write(ban.Moderator, c);
        User.Write(ban.User, c);
        StartTimestamp.Write(ban.StartTimestamp, c);
        EndTimestamp.Write(ban.EndTimestamp, c);
        Reason.Write(ban.Reason, c);
        return Task.CompletedTask;
    }
}

[Node("Twitch HypeTrain Unpack", "Structs")]
public sealed class TwitchHypeTrainUnpackNode : ModuleNode<TwitchModule>
{
    public ValueInput<TwitchHypeTrain> HypeTrain = new();

    public ValueOutput<DateTime> StartTimestamp = new("Start Timestamp");
    public ValueOutput<DateTime> ExpireTimestamp = new("Expire Timestamp");
    public ValueOutput<TwitchHypeTrainType> Type = new();
    public ValueOutput<int> Goal = new();
    public ValueOutput<int> Progress = new();
    public ValueOutput<int> Total = new();

    protected override Task Process(PulseContext c)
    {
        var hypeTrain = HypeTrain.Read(c);
        if (hypeTrain is null) return Task.CompletedTask;

        StartTimestamp.Write(hypeTrain.StartTimestamp, c);
        ExpireTimestamp.Write(hypeTrain.ExpireTimestamp, c);
        Type.Write(hypeTrain.Type, c);
        Goal.Write(hypeTrain.Goal, c);
        Progress.Write(hypeTrain.Progress, c);
        Total.Write(hypeTrain.Total, c);
        return Task.CompletedTask;
    }
}