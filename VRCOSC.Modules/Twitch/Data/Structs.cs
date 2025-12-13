// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;

namespace VRCOSC.Modules.Twitch.Data;

[Flags]
public enum TwitchUserRole
{
    Broadcaster = 1 << 0,
    Moderator = 1 << 1,
    Staff = 1 << 2,
    Subscriber = 1 << 3,
    VIP = 1 << 4
}

public enum TwitchMessageType
{
    Text,
    ChannelPointsHighlighted,
    ChannelPointsSubOnly,
    UserIntro,
    PowerUpsMessageEffect,
    PowerUpsGigantifiedEmote
}

public enum TwitchSubscriptionTier
{
    Tier1 = 1000,
    Tier2 = 2000,
    Tier3 = 3000
}

public enum TwitchRewardRedemptionStatus
{
    Unknown,
    Unfulfilled,
    Fulfilled,
    Canceled
}

public enum TwitchGoalType
{
    Followers,
    Subscriptions
}

public static class TwitchFactory
{
    public static TwitchUserRole CreateUserRole(bool isBroadcaster, bool isModerator, bool isStaff, bool isSubscriber, bool isVip)
    {
        TwitchUserRole roles = 0;
        roles = isBroadcaster ? roles | TwitchUserRole.Broadcaster : roles;
        roles = isModerator ? roles | TwitchUserRole.Moderator : roles;
        roles = isStaff ? roles | TwitchUserRole.Staff : roles;
        roles = isSubscriber ? roles | TwitchUserRole.Subscriber : roles;
        roles = isVip ? roles | TwitchUserRole.VIP : roles;
        return roles;
    }

    public static TwitchMessageType CreateMessageType(string messageType) => messageType switch
    {
        "text" => TwitchMessageType.Text,
        "channel_points_highlighted" => TwitchMessageType.ChannelPointsHighlighted,
        "channel_points_sub_only" => TwitchMessageType.ChannelPointsSubOnly,
        "user_intro" => TwitchMessageType.UserIntro,
        "power_ups_message_effect" => TwitchMessageType.PowerUpsMessageEffect,
        "power_ups_gigantified_emote" => TwitchMessageType.PowerUpsGigantifiedEmote,
        _ => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
    };

    public static TwitchSubscriptionTier CreateSubscriptionTier(string tier) => tier switch
    {
        "1000" => TwitchSubscriptionTier.Tier1,
        "2000" => TwitchSubscriptionTier.Tier2,
        "3000" => TwitchSubscriptionTier.Tier3,
        _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
    };

    public static TwitchRewardRedemptionStatus CreateRewardRedemptionStatus(string status) => status switch
    {
        "unknown" => TwitchRewardRedemptionStatus.Unknown,
        "unfulfilled" => TwitchRewardRedemptionStatus.Unfulfilled,
        "fulfilled" => TwitchRewardRedemptionStatus.Fulfilled,
        "canceled" => TwitchRewardRedemptionStatus.Canceled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };

    public static TwitchGoalType CreateGoalType(string type) => type switch
    {
        "followers" => TwitchGoalType.Followers,
        "subscriptions" => TwitchGoalType.Subscriptions,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}

public record TwitchUser
{
    public readonly string Id;
    public readonly string Username;
    public readonly TwitchUserRole Roles;

    public TwitchUser(string id, string username, TwitchUserRole roles)
    {
        Id = id;
        Username = username;
        Roles = roles;
    }

    public virtual bool Equals(TwitchUser? other) => Id == other?.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => $"{Username} ({Id})";
}

public record TwitchMessage
{
    public readonly string Id;
    public readonly TwitchMessageType Type;
    public readonly TwitchUser Chatter;
    public readonly string Text;

    public TwitchMessage(string id, TwitchMessageType type, TwitchUser chatter, string text)
    {
        Id = id;
        Type = type;
        Chatter = chatter;
        Text = text;
    }

    public virtual bool Equals(TwitchMessage? other) => Id == other?.Id;

    public override int GetHashCode() => Id.GetHashCode();
}

public record TwitchFollow
{
    public readonly DateTime Timestamp;
    public readonly TwitchUser User;

    public TwitchFollow(DateTime timestamp, TwitchUser user)
    {
        Timestamp = timestamp;
        User = user;
    }

    public virtual bool Equals(TwitchFollow? other) => User.Id == other?.User.Id;

    public override int GetHashCode() => User.GetHashCode();
}

public record TwitchSubscription
{
    public readonly TwitchUser User;
    public readonly TwitchSubscriptionTier Tier;
    public readonly bool IsGift;

    public TwitchSubscription(TwitchUser user, TwitchSubscriptionTier tier, bool isGift)
    {
        User = user;
        Tier = tier;
        IsGift = isGift;
    }

    public virtual bool Equals(TwitchSubscription? other) => User.Id == other?.User.Id && Tier == other.Tier && IsGift == other.IsGift;

    public override int GetHashCode() => HashCode.Combine(User, (int)Tier, IsGift);
}

public record TwitchReSubscription
{
    public readonly TwitchUser User;
    public readonly TwitchSubscriptionTier Tier;
    public readonly string Message;
    public readonly int CumulativeMonths;
    public readonly int DurationMonths;
    public readonly int StreakMonths;

    public TwitchReSubscription(TwitchUser user, TwitchSubscriptionTier tier, string message, int cumulativeMonths, int durationMonths, int? streakMonths)
    {
        User = user;
        Tier = tier;
        Message = message;
        CumulativeMonths = cumulativeMonths;
        DurationMonths = durationMonths;
        StreakMonths = streakMonths ?? -1;
    }

    public virtual bool Equals(TwitchReSubscription? other) => User.Id == other?.User.Id && Tier == other.Tier;

    public override int GetHashCode() => HashCode.Combine(User, (int)Tier);
}

public record TwitchGiftSubscription
{
    public readonly TwitchUser User;
    public readonly TwitchSubscriptionTier Tier;
    public readonly int Total;
    public readonly int CumulativeTotal;

    public TwitchGiftSubscription(TwitchUser user, TwitchSubscriptionTier tier, int total, int? cumulativeTotal)
    {
        User = user;
        Tier = tier;
        Total = total;
        CumulativeTotal = cumulativeTotal ?? -1;
    }
}

public record TwitchReward
{
    public readonly string Id;
    public readonly string Title;
    public readonly string Prompt;
    public readonly int Cost;

    public TwitchReward(string id, string title, string prompt, int cost)
    {
        Id = id;
        Title = title;
        Prompt = prompt;
        Cost = cost;
    }

    public virtual bool Equals(TwitchReward? other) => Id == other?.Id;

    public override int GetHashCode() => Id.GetHashCode();
}

public record TwitchRewardRedemption
{
    public readonly TwitchReward Reward;
    public readonly DateTime Timestamp;
    public readonly TwitchUser User;
    public readonly TwitchRewardRedemptionStatus Status;

    public TwitchRewardRedemption(TwitchReward reward, DateTime timestamp, TwitchUser user, TwitchRewardRedemptionStatus status)
    {
        Timestamp = timestamp;
        Reward = reward;
        User = user;
        Status = status;
    }
}

public record TwitchBits
{
    public readonly TwitchUser User;
    public readonly int Amount;
    public readonly string Message;

    public TwitchBits(TwitchUser user, int amount, string message)
    {
        User = user;
        Amount = amount;
        Message = message;
    }
}

public record TwitchRaid
{
    public readonly TwitchUser RaidingBroadcaster;
    public readonly int Viewers;

    public TwitchRaid(TwitchUser raidingBroadcaster, int viewers)
    {
        RaidingBroadcaster = raidingBroadcaster;
        Viewers = viewers;
    }
}

public record TwitchGoal
{
    public readonly string Id;
    public readonly TwitchGoalType Type;
    public readonly string Description;
    public readonly DateTime StartTimestamp;
    public readonly int TargetAmount;
    public readonly int CurrentAmount;

    public TwitchGoal(string id, TwitchGoalType type, string description, DateTime startTimestamp, int targetAmount, int currentAmount)
    {
        Id = id;
        Type = type;
        Description = description;
        StartTimestamp = startTimestamp;
        TargetAmount = targetAmount;
        CurrentAmount = currentAmount;
    }

    public virtual bool Equals(TwitchGoal? other) => Id == other?.Id;

    public override int GetHashCode() => Id.GetHashCode();
}

public record TwitchCategory
{
    public readonly string Id;
    public readonly string Name;

    public TwitchCategory(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public virtual bool Equals(TwitchCategory? other) => Id == other?.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Name;
}

public record TwitchChannel
{
    public readonly string Title;
    public readonly CultureInfo Language;
    public readonly TwitchCategory Category;

    public TwitchChannel(string title, CultureInfo language, TwitchCategory category)
    {
        Title = title;
        Language = language;
        Category = category;
    }
}

public record TwitchBan
{
    public readonly TwitchUser Moderator;
    public readonly TwitchUser User;
    public readonly DateTime StartTimestamp;
    public readonly DateTime EndTimestamp;
    public readonly string Reason;

    public TwitchBan(TwitchUser moderator, TwitchUser user, DateTime startTimestamp, DateTime endTimestamp, string reason)
    {
        Moderator = moderator;
        User = user;
        StartTimestamp = startTimestamp;
        EndTimestamp = endTimestamp;
        Reason = reason;
    }
}