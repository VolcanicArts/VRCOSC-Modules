// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;

namespace VRCOSC.Modules.Hyperate;

public sealed class EventModel
{
    [JsonProperty("event")]
    public string Event = null!;
}

public sealed class HeartBeatModel
{
    [JsonProperty("event")]
    public string Event = "heartbeat";

    [JsonProperty("payload")]
    public WebSocketHeartBeatPayload Payload = new();

    [JsonProperty("ref")]
    public int Ref;

    [JsonProperty("topic")]
    public string Topic = "phoenix";
}

public sealed class WebSocketHeartBeatPayload
{
}

public sealed class HeartRateUpdateModel
{
    [JsonProperty("payload")]
    public HeartRateUpdatePayload Payload = null!;
}

public class HeartRateUpdatePayload
{
    [JsonProperty("hr")]
    public int HeartRate;
}

public sealed class JoinChannelModel
{
    [JsonProperty("event")]
    public string Event = "phx_join";

    [JsonProperty("payload")]
    public JoinChannelPayload Payload = new();

    [JsonProperty("ref")]
    public int Ref;

    [JsonProperty("topic")]
    public string Topic = null!;

    [JsonIgnore]
    public string Id
    {
        set => Topic = "hr:" + value;
    }
}

public sealed class JoinChannelPayload
{
}

public sealed class PhxReplyModel
{
    [JsonProperty("payload")]
    public PhxReplyPayload Payload = null!;
}

public sealed class PhxReplyPayload
{
    [JsonProperty("status")]
    public string Status = null!;
}
