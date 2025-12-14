// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.Modules.Twitch.Data;

namespace VRCOSC.Modules.Twitch.Nodes;

[Node("Twitch Send Chat Message", "Actions")]
public sealed class TwitchSendChatMessageNode : ModuleNode<TwitchModule>, IFlowInput
{
    public FlowContinuation OnSuccess = new("On Success");
    public FlowContinuation OnFail = new("On Fail");

    public ValueInput<string> Text = new();
    public ValueInput<TwitchUser> Broadcaster = new();
    public ValueInput<TwitchMessage> ReplyParentMessage = new("Reply Parent Message");

    protected override async Task Process(PulseContext c)
    {
        var text = Text.Read(c);
        var broadcaster = Broadcaster.Read(c);
        var replyParentMessage = ReplyParentMessage.Read(c);

        var result = await Module.SendChatMessage(text, broadcaster, replyParentMessage);

        if (result)
            await OnSuccess.Execute(c);
        else
            await OnFail.Execute(c);
    }
}