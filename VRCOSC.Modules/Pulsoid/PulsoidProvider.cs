// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Heartrate;

namespace VRCOSC.Modules.Pulsoid;

public sealed class PulsoidProvider : WebSocketHeartrateProvider
{
    private readonly string accessToken;

    protected override Uri WebsocketUri => new($"wss://dev.pulsoid.net/api/v2/data/real_time?access_token={accessToken}&types=heart_rate,battery_level");

    public PulsoidProvider(string accessToken)
    {
        this.accessToken = accessToken;
    }

    protected override void OnWebSocketConnected()
    {
        Log("Connected to the Pulsoid websocket");
    }

    protected override void OnWebSocketDisconnected()
    {
        Log("Disconnected from the Pulsoid websocket");
    }

    protected override void OnWebSocketMessage(string message)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<PulsoidResponse>(message);
            if (data is null) return;

            switch (data.Type)
            {
                case "heart_rate":
                {
                    var heartrateData = JsonConvert.DeserializeObject<PulsoidHeartrateResponse>(message)!;
                    OnHeartrateUpdate?.Invoke(heartrateData.Data.HeartRate);
                    break;
                }

                case "battery_level":
                {
                    var batteryData = JsonConvert.DeserializeObject<PulsoidBatteryResponse>(message)!;
                    OnBatteryUpdate?.Invoke(batteryData.Data.BatteryPercentage / 100f);
                    break;
                }
            }
        }
        catch (Exception)
        {
            Log("Error deserialising Pulsoid message");
        }
    }
}

public class PulsoidResponse
{
    [JsonProperty("type")]
    public string Type = null!;

    [JsonProperty("timestamp")]
    public DateTime Timestamp;
}

public sealed class PulsoidHeartrateResponse : PulsoidResponse
{
    [JsonProperty("heartRate")]
    public PulsoidHeartrateResponseData Data = null!;
}

public sealed class PulsoidHeartrateResponseData
{
    [JsonProperty("bpm")]
    public int HeartRate;
}

public sealed class PulsoidBatteryResponse : PulsoidResponse
{
    [JsonProperty("batteryLevel")]
    public PulsoidBatteryResponseData Data = null!;
}

public sealed class PulsoidBatteryResponseData
{
    [JsonProperty("percent")]
    public int BatteryPercentage;
}
