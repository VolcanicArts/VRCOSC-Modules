// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;
using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.Modules.Twitch.Data;

namespace VRCOSC.Modules.Twitch.Nodes;

[Node("Twitch Message Unpack", "Structs")]
public sealed class TwitchMessageUnpackNode() : ValueConsumeNode<TwitchMessage?>("Message"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<string> MessageId = new("Id");
    public ValueOutput<TwitchMessageType> Type = new();
    public ValueOutput<TwitchUser> Chatter = new();
    public ValueOutput<string> Text = new();

    protected override void ConsumeValue(TwitchMessage? message, IPulseContext c)
    {
        if (message is null) return;

        MessageId.Write(message.Id, c);
        Type.Write(message.Type, c);
        Chatter.Write(message.Chatter, c);
        Text.Write(message.Text, c);
    }
}

[Node("Twitch User Unpack", "Structs")]
public sealed class TwitchUserUnpackNode() : ValueConsumeNode<TwitchUser?>("User"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<string> UserId = new("Id");
    public ValueOutput<string> Username = new();
    public ValueOutput<TwitchUserRole> Roles = new();

    protected override void ConsumeValue(TwitchUser? user, IPulseContext c)
    {
        if (user is null) return;

        UserId.Write(user.Id, c);
        Username.Write(user.Username, c);
        Roles.Write(user.Roles, c);
    }
}

[Node("Twitch Follow Unpack", "Structs")]
public sealed class TwitchFollowUnpackNode() : ValueConsumeNode<TwitchFollow?>("Follow"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<DateTime> Timestamp = new();
    public ValueOutput<TwitchUser> User = new();

    protected override void ConsumeValue(TwitchFollow? follow, IPulseContext c)
    {
        if (follow is null) return;

        Timestamp.Write(follow.Timestamp, c);
        User.Write(follow.User, c);
    }
}

[Node("Twitch Subscription Unpack", "Structs")]
public sealed class TwitchSubscriptionUnpackNode() : ValueConsumeNode<TwitchSubscription?>("Subscription"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchSubscriptionTier> Tier = new();
    public ValueOutput<bool> IsGift = new();

    protected override void ConsumeValue(TwitchSubscription? subscription, IPulseContext c)
    {
        if (subscription is null) return;

        User.Write(subscription.User, c);
        Tier.Write(subscription.Tier, c);
        IsGift.Write(subscription.IsGift, c);
    }
}

[Node("Twitch ReSubscription Unpack", "Structs")]
public sealed class TwitchReSubscriptionUnpackNode : ValueConsumeNode<TwitchReSubscription?>, IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchSubscriptionTier> Tier = new();
    public ValueOutput<string> Message = new();
    public ValueOutput<int> CumulativeMonths = new();
    public ValueOutput<int> DurationMonths = new();
    public ValueOutput<int> StreakMonths = new();

    protected override void ConsumeValue(TwitchReSubscription? reSubscription, IPulseContext c)
    {
        if (reSubscription is null) return;

        User.Write(reSubscription.User, c);
        Tier.Write(reSubscription.Tier, c);
        Message.Write(reSubscription.Message, c);
        CumulativeMonths.Write(reSubscription.CumulativeMonths, c);
        DurationMonths.Write(reSubscription.DurationMonths, c);
        StreakMonths.Write(reSubscription.StreakMonths, c);
    }
}

[Node("Twitch Reward Unpack", "Structs")]
public sealed class TwitchRewardUnpackNode() : ValueConsumeNode<TwitchReward?>("Reward"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<string> RewardId = new("Id");
    public ValueOutput<string> Title = new();
    public ValueOutput<string> Prompt = new();
    public ValueOutput<int> Cost = new();

    protected override void ConsumeValue(TwitchReward? reward, IPulseContext c)
    {
        if (reward is null) return;

        RewardId.Write(reward.Id, c);
        Title.Write(reward.Title, c);
        Prompt.Write(reward.Prompt, c);
        Cost.Write(reward.Cost, c);
    }
}

[Node("Twitch Reward Redemption Unpack", "Structs")]
public sealed class TwitchRewardRedemptionUnpackNode() : ValueConsumeNode<TwitchRewardRedemption?>("Reward Redemption"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchReward> Reward = new();
    public ValueOutput<DateTime> Timestamp = new();
    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchRewardRedemptionStatus> Status = new();

    protected override void ConsumeValue(TwitchRewardRedemption? rewardRedemption, IPulseContext c)
    {
        if (rewardRedemption is null) return;

        Reward.Write(rewardRedemption.Reward, c);
        Timestamp.Write(rewardRedemption.Timestamp, c);
        User.Write(rewardRedemption.User, c);
        Status.Write(rewardRedemption.Status, c);
    }
}

[Node("Twitch Gift Subscription Unpack", "Structs")]
public sealed class TwitchGiftSubscriptionUnpackNode() : ValueConsumeNode<TwitchGiftSubscription?>("Gift Subscription"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<TwitchSubscriptionTier> Tier = new();
    public ValueOutput<int> Total = new();
    public ValueOutput<int> CumulativeTotal = new();

    protected override void ConsumeValue(TwitchGiftSubscription? giftSubscription, IPulseContext c)
    {
        if (giftSubscription is null) return;

        User.Write(giftSubscription.User, c);
        Tier.Write(giftSubscription.Tier, c);
        Total.Write(giftSubscription.Total, c);
        CumulativeTotal.Write(giftSubscription.CumulativeTotal, c);
    }
}

[Node("Twitch Bits Unpack", "Structs")]
public sealed class TwitchBitsUnpackNode() : ValueConsumeNode<TwitchBits?>("Bits"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<int> Amount = new();
    public ValueOutput<string> Message = new();

    protected override void ConsumeValue(TwitchBits? bits, IPulseContext c)
    {
        if (bits is null) return;

        User.Write(bits.User, c);
        Amount.Write(bits.Amount, c);
        Message.Write(bits.Message, c);
    }
}

[Node("Twitch Raid Unpack", "Structs")]
public sealed class TwitchRaidUnpackNode() : ValueConsumeNode<TwitchRaid?>("Raid"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchUser> RaidingBroadcaster = new();
    public ValueOutput<int> Viewers = new();

    protected override void ConsumeValue(TwitchRaid? raid, IPulseContext c)
    {
        if (raid is null) return;

        RaidingBroadcaster.Write(raid.RaidingBroadcaster, c);
        Viewers.Write(raid.Viewers, c);
    }
}

[Node("Twitch Goal Unpack", "Structs")]
public sealed class TwitchGoalUnpackNode() : ValueConsumeNode<TwitchGoal?>("Goal"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<string> GoalId = new("Id");
    public ValueOutput<TwitchGoalType> Type = new();
    public ValueOutput<string> Description = new();
    public ValueOutput<DateTime> StartTimestamp = new();
    public ValueOutput<int> TargetAmount = new();
    public ValueOutput<int> CurrentAmount = new();

    protected override void ConsumeValue(TwitchGoal? goal, IPulseContext c)
    {
        if (goal is null) return;

        GoalId.Write(goal.Id, c);
        Type.Write(goal.Type, c);
        Description.Write(goal.Description, c);
        StartTimestamp.Write(goal.StartTimestamp, c);
        TargetAmount.Write(goal.TargetAmount, c);
        CurrentAmount.Write(goal.CurrentAmount, c);
    }
}

[Node("Twitch Category Unpack", "Structs")]
public sealed class TwitchCategoryUnpackNode() : ValueConsumeNode<TwitchCategory?>("Category"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<string> CategoryId = new("Id");
    public ValueOutput<string> Name = new();

    protected override void ConsumeValue(TwitchCategory? category, IPulseContext c)
    {
        if (category is null) return;

        CategoryId.Write(category.Id, c);
        Name.Write(category.Name, c);
    }
}

[Node("Twitch Channel Unpack", "Structs")]
public sealed class TwitchChannelUnpackNode() : ValueConsumeNode<TwitchChannel?>("Channel"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<string> Title = new();
    public ValueOutput<CultureInfo> Language = new();
    public ValueOutput<TwitchCategory> Category = new();

    protected override void ConsumeValue(TwitchChannel? channel, IPulseContext c)
    {
        if (channel is null) return;

        Title.Write(channel.Title, c);
        Language.Write(channel.Language, c);
        Category.Write(channel.Category, c);
    }
}

[Node("Twitch Ban Unpack", "Structs")]
public sealed class TwitchBanUnpackNode() : ValueConsumeNode<TwitchBan?>("Ban"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<TwitchUser> Moderator = new();
    public ValueOutput<TwitchUser> User = new();
    public ValueOutput<DateTime> StartTimestamp = new();
    public ValueOutput<DateTime> EndTimestamp = new();
    public ValueOutput<string> Reason = new();

    protected override void ConsumeValue(TwitchBan? ban, IPulseContext c)
    {
        if (ban is null) return;

        Moderator.Write(ban.Moderator, c);
        User.Write(ban.User, c);
        StartTimestamp.Write(ban.StartTimestamp, c);
        EndTimestamp.Write(ban.EndTimestamp, c);
        Reason.Write(ban.Reason, c);
    }
}

[Node("Twitch HypeTrain Unpack", "Structs")]
public sealed class TwitchHypeTrainUnpackNode() : ValueConsumeNode<TwitchHypeTrain?>("HypeTrain"), IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueOutput<DateTime> StartTimestamp = new();
    public ValueOutput<DateTime> ExpireTimestamp = new();
    public ValueOutput<TwitchHypeTrainType> Type = new();
    public ValueOutput<int> Goal = new();
    public ValueOutput<int> Progress = new();
    public ValueOutput<int> Total = new();

    protected override void ConsumeValue(TwitchHypeTrain? hypeTrain, IPulseContext c)
    {
        if (hypeTrain is null) return;

        StartTimestamp.Write(hypeTrain.StartTimestamp, c);
        ExpireTimestamp.Write(hypeTrain.ExpireTimestamp, c);
        Type.Write(hypeTrain.Type, c);
        Goal.Write(hypeTrain.Goal, c);
        Progress.Write(hypeTrain.Progress, c);
        Total.Write(hypeTrain.Total, c);
    }
}