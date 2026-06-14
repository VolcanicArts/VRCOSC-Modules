// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.Modules.Twitch.Data;

namespace VRCOSC.Modules.Twitch.Nodes;

[Node("Twitch Send Chat Message", "Actions")]
public sealed class TwitchSendChatMessageNode : TryActionAsyncNode, IModuleNode<TwitchModule>
{
    public TwitchModule Module { get; set; } = null!;

    public ValueInput<string> Text = new();
    public ValueInput<TwitchUser?> Broadcaster = new();
    public ValueInput<TwitchMessage?> ReplyParentMessage = new();

    protected override Task<bool> TryActionAsync(IPulseContext c)
    {
        var text = Text.Read(c);
        var broadcaster = Broadcaster.Read(c);
        var replyParentMessage = ReplyParentMessage.Read(c);

        return Module.SendChatMessage(text, broadcaster, replyParentMessage);
    }
}