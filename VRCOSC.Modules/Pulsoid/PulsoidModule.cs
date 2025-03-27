// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

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
    protected override PulsoidProvider CreateProvider() => new(GetSettingValue<string>(PulsoidSetting.AccessToken)!);

    protected override void OnPreLoad()
    {
        CreateCustomSetting(PulsoidSetting.AccessToken, new StringModuleSetting("Access Token", "Your Pulsoid access token", typeof(PulsoidAccessTokenView), string.Empty));

        CreateGroup("Access", PulsoidSetting.AccessToken);

        base.OnPreLoad();
    }

    protected override Task<bool> OnModuleStart()
    {
        if (string.IsNullOrEmpty(GetSettingValue<string>(PulsoidSetting.AccessToken)))
        {
            Log("Please enter a valid access token in the settings");
            return Task.FromResult(false);
        }

        return base.OnModuleStart();
    }

    private enum PulsoidSetting
    {
        AccessToken
    }
}