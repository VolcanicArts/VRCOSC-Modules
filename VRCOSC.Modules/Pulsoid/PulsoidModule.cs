// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Net.Http;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.SDK.Modules.Heartrate;

namespace VRCOSC.Modules.Pulsoid;

[ModuleTitle("Pulsoid")]
[ModuleDescription("Connects to Pulsoid and sends your heartrate to VRChat")]
[ModuleType(ModuleType.Health)]
[ModulePrefab("Official Prefabs", "https://vrcosc.com/docs/downloads#prefabs")]
public sealed class PulsoidModule : HeartrateModule<PulsoidProvider>
{
    private static readonly HttpClient client = new();
    private static readonly Uri token_validate_uri = new("https://dev.pulsoid.net/api/v1/token/validate");

    protected override PulsoidProvider CreateProvider() => new(GetSettingValue<string>(PulsoidSetting.AccessToken));

    protected override void OnPreLoad()
    {
        CreateCustomSetting(PulsoidSetting.AccessToken, new StringModuleSetting("Access Token", "Your Pulsoid access token", typeof(PulsoidAccessTokenView), string.Empty));

        CreateGroup("Access", string.Empty, PulsoidSetting.AccessToken);

        base.OnPreLoad();
    }

    protected override async Task<bool> OnModuleStart()
    {
        var token = GetSettingValue<string>(PulsoidSetting.AccessToken);

        if (string.IsNullOrEmpty(token))
        {
            Log("Please enter an access token in module's settings");
            return false;
        }

        if (!await validateToken(token))
        {
            Log("Your access token has expired. Please obtain a new access token");
            return false;
        }

        return await base.OnModuleStart();
    }

    private static async Task<bool> validateToken(string token)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, token_validate_uri);
        httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");

        var response = await client.SendAsync(httpRequestMessage);
        if (!response.IsSuccessStatusCode) return false;

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PulsoidValidateResponse>(content);
        if (result is null) return false;

        return result.Scopes.Contains("data:heart_rate:read") && result.Scopes.Contains("data:battery_level:read");
    }

    private enum PulsoidSetting
    {
        AccessToken
    }
}

public sealed class PulsoidValidateResponse
{
    [JsonProperty("scopes")]
    public string[] Scopes = [];
}