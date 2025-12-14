// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using System.Globalization;
using TwitchLib.Api;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Channels.SendChatMessage;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.EventSub.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.Modules.Twitch.Data;
using VRCOSC.Modules.Twitch.Nodes;
using VRCOSC.Modules.Twitch.UI;

namespace VRCOSC.Modules.Twitch;

[ModuleTitle("Twitch")]
[ModuleDescription("Twitch integration for Pulse")]
[ModuleType(ModuleType.Integrations)]
public class TwitchModule : Module
{
    public const string CLIENT_ID = "6y51jdzkdtlwv56akwerab47wwov1w";

    private TwitchAPI? twitchApi;
    private User? twitchUser;
    private EventSubWebsocketClient? websocket;

    protected override void OnPreLoad()
    {
        CreateCustomSetting(TwitchSetting.AccessToken, new StringModuleSetting("Access Token", "Your Twitch access token", typeof(TwitchAccessTokenView), string.Empty));
    }

    protected override async Task<bool> OnModuleStart()
    {
        var accessToken = GetSettingValue<string>(TwitchSetting.AccessToken);

        twitchApi = new TwitchAPI();

        var result = await twitchApi.Auth.ValidateAccessTokenAsync(accessToken);

        if (result is null)
        {
            Log("Please re-obtain an access token");
            return false;
        }

        Log("Successfully logged in");

        twitchApi.Settings.ClientId = CLIENT_ID;
        twitchApi.Settings.AccessToken = accessToken;

        var users = await twitchApi.Helix.Users.GetUsersAsync(accessToken: accessToken);
        twitchUser = users.Users[0];

        websocket = new EventSubWebsocketClient();
        websocket.WebsocketConnected += onWebsocketConnected;
        websocket.ChannelChatMessage += onChannelChatMessage;
        websocket.ChannelFollow += onChannelFollow;
        websocket.ChannelSubscribe += onChannelSubscribe; // new subs
        websocket.ChannelSubscriptionMessage += onChannelSubscriptionMessage; // resubs
        websocket.ChannelPointsCustomRewardRedemptionAdd += onChannelPointsCustomRewardRedemptionAdd; // channel point redeem
        websocket.ChannelSubscriptionGift += onChannelSubscriptionGift; // gift subs
        websocket.ChannelBitsUse += onChannelBitsUse; // cheers & powerups
        websocket.ChannelRaid += onChannelRaid;
        websocket.ChannelGoalBegin += onChannelGoalBegin;
        websocket.ChannelGoalProgress += onChannelGoalProgress;
        websocket.ChannelGoalEnd += onChannelGoalEnd;
        websocket.ChannelUpdate += onChannelUpdate;
        websocket.ChannelBan += onChannelBan;
        websocket.ChannelHypeTrainBeginV2 += onHypeTrainBegin;
        websocket.ChannelHypeTrainProgressV2 += onHypeTrainProgress;
        websocket.ChannelHypeTrainEndV2 += onHypeTrainEnd;

        await websocket.ConnectAsync();
        return true;
    }

    protected override async Task OnModuleStop()
    {
        await websocket!.DisconnectAsync();
    }

    private async Task onWebsocketConnected(object? sender, WebsocketConnectedArgs e)
    {
        Debug.Assert(twitchApi is not null);
        Debug.Assert(websocket is not null);
        Debug.Assert(twitchUser is not null);

        LogDebug("Websocket connected");

        if (e.IsRequestedReconnect)
        {
            LogDebug("Requested reconnect");
            return;
        }

        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.chat.message", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id }, { "user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.follow", "2", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id }, { "moderator_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.subscribe", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.subscription.message", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.channel_points_custom_reward_redemption.add", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.subscription.gift", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.bits.use", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.raid", "1", new Dictionary<string, string> { { "to_broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.goal.begin", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.goal.progress", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.goal.end", "1", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.update", "2", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.hype_train.begin", "2", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.hype_train.progress", "2", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        await twitchApi.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.hype_train.end", "2", new Dictionary<string, string> { { "broadcaster_user_id", twitchUser.Id } }, EventSubTransportMethod.Websocket, websocket.SessionId);
        LogDebug("Finished creating events");
    }

    private async Task onChannelChatMessage(object? sender, ChannelChatMessageArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var chatter = new TwitchUser(ev.ChatterUserId, ev.ChatterUserName, TwitchFactory.CreateUserRole(ev.IsBroadcaster, ev.IsModerator, ev.IsStaff, ev.IsSubscriber, ev.IsVip));
        var message = new TwitchMessage(ev.MessageId, TwitchFactory.CreateMessageType(ev.MessageType), chatter, ev.Message.Text);

        await TriggerModuleNode(typeof(TwitchChannelChatMessageNode), [broadcaster, message]);
    }

    private async Task onChannelFollow(object? sender, ChannelFollowArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var user = new TwitchUser(ev.UserId, ev.UserName, 0);
        var follow = new TwitchFollow(ev.FollowedAt.DateTime, user);

        await TriggerModuleNode(typeof(TwitchChannelFollowNode), [broadcaster, follow]);
    }

    private async Task onChannelSubscribe(object? sender, ChannelSubscribeArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var user = new TwitchUser(ev.UserId, ev.UserName, 0);
        var subscription = new TwitchSubscription(user, TwitchFactory.CreateSubscriptionTier(ev.Tier), ev.IsGift);

        await TriggerModuleNode(typeof(TwitchChannelSubscriptionNode), [broadcaster, subscription]);
    }

    private async Task onChannelSubscriptionMessage(object? sender, ChannelSubscriptionMessageArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var user = new TwitchUser(ev.UserId, ev.UserName, 0);
        var resubscription = new TwitchReSubscription(user, TwitchFactory.CreateSubscriptionTier(ev.Tier), ev.Message.Text, ev.CumulativeMonths, ev.DurationMonths, ev.StreakMonths);

        await TriggerModuleNode(typeof(TwitchChannelReSubscriptionNode), [broadcaster, resubscription]);
    }

    private async Task onChannelPointsCustomRewardRedemptionAdd(object? sender, ChannelPointsCustomRewardRedemptionArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var user = new TwitchUser(ev.UserId, ev.UserName, 0);
        var reward = new TwitchReward(ev.Reward.Id, ev.Reward.Title, ev.Reward.Prompt, ev.Reward.Cost);
        var rewardRedemption = new TwitchRewardRedemption(reward, ev.RedeemedAt.DateTime, user, TwitchFactory.CreateRewardRedemptionStatus(ev.Status));

        await TriggerModuleNode(typeof(TwitchChannelRewardRedemptionNode), [broadcaster, rewardRedemption]);
    }

    private async Task onChannelSubscriptionGift(object? sender, ChannelSubscriptionGiftArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var user = ev.IsAnonymous ? new TwitchUser(string.Empty, string.Empty, 0) : new TwitchUser(ev.UserId, ev.UserName, 0);
        var giftSubscription = new TwitchGiftSubscription(user, TwitchFactory.CreateSubscriptionTier(ev.Tier), ev.Total, ev.CumulativeTotal);

        await TriggerModuleNode(typeof(TwitchChannelGiftSubscriptionNode), [broadcaster, giftSubscription]);
    }

    private async Task onChannelBitsUse(object? sender, ChannelBitsUseArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var user = new TwitchUser(ev.UserId, ev.UserName, 0);
        var bits = new TwitchBits(user, ev.Bits, ev.Message?.Text ?? string.Empty);

        await TriggerModuleNode(typeof(TwitchChannelBitsNode), [broadcaster, bits]);
    }

    private async Task onChannelRaid(object? sender, ChannelRaidArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.ToBroadcasterUserId, ev.ToBroadcasterUserName, TwitchUserRole.Broadcaster);
        var raidingBroadcaster = new TwitchUser(ev.FromBroadcasterUserId, ev.FromBroadcasterUserName, 0);
        var raid = new TwitchRaid(raidingBroadcaster, ev.Viewers);

        await TriggerModuleNode(typeof(TwitchChannelRaidNode), [broadcaster, raid]);
    }

    private async Task onChannelGoalBegin(object? sender, ChannelGoalBeginArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var goal = new TwitchGoal(ev.Id, TwitchFactory.CreateGoalType(ev.Type), ev.Description, ev.StartedAt.DateTime, ev.TargetAmount, ev.CurrentAmount);

        await TriggerModuleNode(typeof(TwitchChannelGoalBeginNode), [broadcaster, goal]);
    }

    private async Task onChannelGoalProgress(object? sender, ChannelGoalProgressArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var goal = new TwitchGoal(ev.Id, TwitchFactory.CreateGoalType(ev.Type), ev.Description, ev.StartedAt.DateTime, ev.TargetAmount, ev.CurrentAmount);

        await TriggerModuleNode(typeof(TwitchChannelGoalProgressNode), [broadcaster, goal]);
    }

    private async Task onChannelGoalEnd(object? sender, ChannelGoalEndArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var goal = new TwitchGoal(ev.Id, TwitchFactory.CreateGoalType(ev.Type), ev.Description, ev.StartedAt.DateTime, ev.TargetAmount, ev.CurrentAmount);

        await TriggerModuleNode(typeof(TwitchChannelGoalEndNode), [broadcaster, goal]);
    }

    private async Task onChannelUpdate(object? sender, ChannelUpdateArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var category = new TwitchCategory(ev.CategoryId, ev.CategoryName);
        var channel = new TwitchChannel(ev.Title, CultureInfo.GetCultureInfo(ev.Language), category);

        await TriggerModuleNode(typeof(TwitchChannelUpdateNode), [broadcaster, channel]);
    }

    private async Task onChannelBan(object? sender, ChannelBanArgs e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var moderator = new TwitchUser(ev.ModeratorUserId, ev.ModeratorUserName, TwitchUserRole.Moderator);
        var user = new TwitchUser(ev.UserId, ev.UserName, 0);
        var ban = new TwitchBan(moderator, user, ev.BannedAt.DateTime, ev.EndsAt?.DateTime ?? DateTime.UnixEpoch, ev.Reason);

        await TriggerModuleNode(typeof(TwitchChannelBanNode), [broadcaster, ban]);
    }

    private async Task onHypeTrainBegin(object? sender, ChannelHypeTrainBeginV2Args e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var hypeTrain = new TwitchHypeTrain(ev.StartedAt.DateTime, ev.ExpiresAt.DateTime, TwitchFactory.CreateHypeTrainType(ev.Type), ev.Goal, ev.Progress, ev.Total);

        await TriggerModuleNode(typeof(TwitchChannelHypeTrainBeginNode), [broadcaster, hypeTrain]);
    }

    private async Task onHypeTrainProgress(object? sender, ChannelHypeTrainProgressV2Args e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var hypeTrain = new TwitchHypeTrain(ev.StartedAt.DateTime, ev.ExpiresAt.DateTime, TwitchFactory.CreateHypeTrainType(ev.Type), ev.Goal, ev.Progress, ev.Total);

        await TriggerModuleNode(typeof(TwitchChannelHypeTrainProgressNode), [broadcaster, hypeTrain]);
    }

    private async Task onHypeTrainEnd(object? sender, ChannelHypeTrainEndV2Args e)
    {
        var ev = e.Payload.Event;

        var broadcaster = new TwitchUser(ev.BroadcasterUserId, ev.BroadcasterUserName, TwitchUserRole.Broadcaster);
        var hypeTrain = new TwitchHypeTrain(ev.StartedAt.DateTime, ev.EndedAt.DateTime, TwitchFactory.CreateHypeTrainType(ev.Type), 0, 0, ev.Total);

        await TriggerModuleNode(typeof(TwitchChannelHypeTrainEndNode), [broadcaster, hypeTrain]);
    }

    public async Task<bool> SendChatMessage(string text, TwitchUser? broadcaster = null, TwitchMessage? replyParentMessage = null)
    {
        try
        {
            if (twitchApi is null || twitchUser is null) return false;
            if (string.IsNullOrWhiteSpace(text) || text.Length > 500) return false;

            await twitchApi.Helix.Chat.SendChatMessage(new SendChatMessageRequest
            {
                BroadcasterId = broadcaster?.Id ?? twitchUser.Id,
                Message = text,
                SenderId = twitchUser.Id,
                ReplyParentMessageId = replyParentMessage?.Id
            });

            return true;
        }
        catch (Exception e)
        {
            LogDebug(e.Message);
            return false;
        }
    }
}

public enum TwitchSetting
{
    AccessToken
}