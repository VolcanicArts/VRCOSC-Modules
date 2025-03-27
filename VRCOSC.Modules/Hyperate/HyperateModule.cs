// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Heartrate;

namespace VRCOSC.Modules.Hyperate;

[ModuleTitle("HypeRate")]
[ModuleDescription("Connects to HypeRate.io and sends your heartrate to VRChat")]
[ModuleType(ModuleType.Health)]
[ModulePrefab("Official Prefabs", "https://vrcosc.com/docs/downloads#prefabs")]
public sealed class HypeRateModule : HeartrateModule<HypeRateProvider>
{
    protected override HypeRateProvider CreateProvider() => new(GetSettingValue<string>(HypeRateSetting.Id)!, OfficialModuleSecrets.GetSecret(OfficialModuleSecretsKeys.Hyperate));

    protected override void OnPreLoad()
    {
        CreateTextBox(HypeRateSetting.Id, "HypeRate ID", "Your HypeRate ID is the characters at the end of the link on PC or the ID given on your screen", string.Empty);

        CreateGroup("Access", string.Empty, HypeRateSetting.Id);

        base.OnPreLoad();
    }

    protected override Task<bool> OnModuleStart()
    {
        if (string.IsNullOrEmpty(GetSettingValue<string>(HypeRateSetting.Id)))
        {
            Log("Cannot connect to HypeRate. Please enter an Id");
            return Task.FromResult(false);
        }

        return base.OnModuleStart();
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 10000)]
    private void sendWsHeartbeat()
    {
        HeartrateProvider?.SendWsHeartBeat();
    }

    private enum HypeRateSetting
    {
        Id
    }
}