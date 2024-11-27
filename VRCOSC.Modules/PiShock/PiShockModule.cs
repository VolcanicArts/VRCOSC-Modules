// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Providers.PiShock;
using VRCOSC.App.SDK.VRChat;
using VRCOSC.App.Utils;
using VRCOSC.Modules.PiShock.UI;

namespace VRCOSC.Modules.PiShock;

[ModuleTitle("PiShock")]
[ModuleDescription("Allows for controlling PiShock shockers")]
[ModuleType(ModuleType.NSFW)]
[ModulePrefab("VRCOSC-PiShock", "https://github.com/VolcanicArts/VRCOSC/releases/download/latest/VRCOSC-PiShock.unitypackage")]
public class PiShockModule : Module, ISpeechHandler
{
    private PiShockProvider piShockProvider = null!;

    private string globalGroupID = null!;
    private float globalDuration;
    private float globalIntensity;

    private readonly Dictionary<string, float> durations = new();
    private readonly Dictionary<string, float> intensities = new();

    private (DateTimeOffset, string?)? shock;
    private (DateTimeOffset, string?)? vibrate;
    private (DateTimeOffset, string?)? beep;
    private bool shockExecuted;
    private bool vibrateExecuted;
    private bool beepExecuted;

    public ShockerModuleSetting ShockersSetting => GetSetting<ShockerModuleSetting>(PiShockSetting.Shockers);
    public ShockerGroupModuleSetting GroupsSetting => GetSetting<ShockerGroupModuleSetting>(PiShockSetting.Groups);

    protected override void OnPreLoad()
    {
        CreateTextBox(PiShockSetting.Username, "Username", "Your PiShock username", string.Empty);
        CreateCustomSetting(PiShockSetting.APIKey, new StringModuleSetting("API Key", "Your PiShock API key", typeof(PiShockAPIKeyView), string.Empty));

        CreateTextBox(PiShockSetting.ButtonDelay, "Button Delay", "The amount of time in milliseconds the shock, vibrate, and beep parameters need to be true to execute the action. This is helpful for if you accidentally press buttons on your action menu", 0);

        CreateCustomSetting(PiShockSetting.Shockers, new ShockerModuleSetting());
        CreateCustomSetting(PiShockSetting.Groups, new ShockerGroupModuleSetting());
        CreateCustomSetting(PiShockSetting.Phrases, new PhraseModuleSetting());

        CreateGroup("Credentials", PiShockSetting.Username, PiShockSetting.APIKey);
        CreateGroup("Management", PiShockSetting.Shockers, PiShockSetting.Groups);
        CreateGroup("Tweaks", PiShockSetting.ButtonDelay);
        CreateGroup("Speech", PiShockSetting.Phrases);

        RegisterParameter<int>(PiShockParameter.Group, "VRCOSC/PiShock/Group", ParameterMode.Read, "Group", "Sets the specific group to use when using the non-specific action parameters");
        RegisterParameter<float>(PiShockParameter.Duration, "VRCOSC/PiShock/Duration", ParameterMode.Read, "Duration", "The duration of the action as a 0-1 float for the group set by the Group parameter");
        RegisterParameter<float>(PiShockParameter.Intensity, "VRCOSC/PiShock/Intensity", ParameterMode.Read, "Intensity", "The intensity of the action as a 0-1 float for the group set by the Group parameter");
        RegisterParameter<bool>(PiShockParameter.Shock, "VRCOSC/PiShock/Shock", ParameterMode.Read, "Shock", "Shock the group set by the Group parameter");
        RegisterParameter<bool>(PiShockParameter.Vibrate, "VRCOSC/PiShock/Vibrate", ParameterMode.Read, "Vibrate", "Vibrate the group set by the Group parameter");
        RegisterParameter<bool>(PiShockParameter.Beep, "VRCOSC/PiShock/Beep", ParameterMode.Read, "Beep", "Beep the group set by the Group parameter");
        RegisterParameter<float>(PiShockParameter.DurationGroup, "VRCOSC/PiShock/Duration/*", ParameterMode.Read, "Duration Group", "The duration of the action as a 0-1 float for a specific group\nE.G. VRCOSC/PiShock/Duration/0");
        RegisterParameter<float>(PiShockParameter.IntensityGroup, "VRCOSC/PiShock/Intensity/*", ParameterMode.Read, "Intensity Group", "The intensity of the action as a 0-1 float for a specific group\nE.G. VRCOSC/PiShock/Intensity/0");
        RegisterParameter<bool>(PiShockParameter.ShockGroup, "VRCOSC/PiShock/Shock/*", ParameterMode.Read, "Shock Group", "Shock a specific group\nE.G. VRCOSC/PiShock/Shock/0");
        RegisterParameter<bool>(PiShockParameter.VibrateGroup, "VRCOSC/PiShock/Vibrate/*", ParameterMode.Read, "Vibrate Group", "Vibrate a specific group\nE.G. VRCOSC/PiShock/Vibrate/0");
        RegisterParameter<bool>(PiShockParameter.BeepGroup, "VRCOSC/PiShock/Beep/*", ParameterMode.Read, "Beep Group", "Beep a specific group\nE.G. VRCOSC/PiShock/Beep/0");
    }

    protected override void OnPostLoad()
    {
        GetSetting<ShockerModuleSetting>(PiShockSetting.Shockers).Attribute.OnCollectionChanged((_, _) =>
        {
            var groupSetting = GetSetting<ShockerGroupModuleSetting>(PiShockSetting.Groups);

            foreach (var shockerGroup in groupSetting.Attribute)
            {
                shockerGroup.Shockers.RemoveIf(shockerID => string.IsNullOrEmpty(shockerID.Value));
            }
        });
    }

    protected override Task<bool> OnModuleStart()
    {
        reset();

        piShockProvider = new PiShockProvider();

        return Task.FromResult(true);
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        reset();
    }

    private void reset()
    {
        var groups = GetSettingValue<List<ShockerGroup>>(PiShockSetting.Groups);

        if (groups.Count != 0)
            globalGroupID = groups[0].ID;

        durations.Clear();
        intensities.Clear();

        foreach (var shockerGroup in groups)
        {
            durations[shockerGroup.ID] = 0f;
            intensities[shockerGroup.ID] = 0f;
        }

        globalDuration = 0f;
        globalIntensity = 0f;
        shock = null;
        vibrate = null;
        beep = null;
        shockExecuted = false;
        vibrateExecuted = false;
        beepExecuted = false;
    }

    public void OnPartialSpeechResult(string text)
    {
    }

    public async void OnFinalSpeechResult(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        foreach (var phrase in GetSettingValue<List<Phrase>>(PiShockSetting.Phrases).Where(phrase => text.Contains(phrase.Text.Value, StringComparison.InvariantCultureIgnoreCase)))
        {
            Log($"Found phrase '{phrase.Text.Value}'");

            var tasks = phrase.ShockerGroups.DistinctBy(shockerGroupID => shockerGroupID.Value).Select(shockerGroupID =>
            {
                var shockerGroup = GetSettingValue<IEnumerable<ShockerGroup>>(PiShockSetting.Groups).SingleOrDefault(shockerGroup => shockerGroup.ID == shockerGroupID.Value);
                if (shockerGroup is null) return Task.CompletedTask;

                var localDuration = (float)Interpolation.Map(phrase.Duration.Value, 1, 15, 0, 1);
                var localIntensity = (float)Interpolation.Map(phrase.Intensity.Value, 1, 100, 0, 1);

                return Task.Run(async () => await executeGroupAsync(shockerGroupID.Value, phrase.Mode.Value, localDuration, localIntensity));
            });

            await Task.WhenAll(tasks);
            Log($"Phrase '{phrase.Text.Value}' complete");
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void checkForExecutions()
    {
        var delay = TimeSpan.FromMilliseconds(GetSettingValue<int>(PiShockSetting.ButtonDelay));

        if (shock is not null && shock.Value.Item1 + delay <= DateTimeOffset.Now && !shockExecuted)
        {
            var groupID = shock.Value.Item2;
            var localDuration = string.IsNullOrEmpty(groupID) ? globalDuration : durations[groupID];
            var localIntensity = string.IsNullOrEmpty(groupID) ? globalIntensity : intensities[groupID];
            var localGroup = string.IsNullOrEmpty(groupID) ? globalGroupID : groupID;

            shockExecuted = true;
            _ = executeGroupAsync(localGroup, PiShockMode.Shock, localDuration, localIntensity);
        }

        if (shock is null) shockExecuted = false;

        if (vibrate is not null && vibrate.Value.Item1 + delay <= DateTimeOffset.Now && !vibrateExecuted)
        {
            var groupID = vibrate.Value.Item2;
            var localDuration = string.IsNullOrEmpty(groupID) ? globalDuration : durations[groupID];
            var localIntensity = string.IsNullOrEmpty(groupID) ? globalIntensity : intensities[groupID];
            var localGroup = string.IsNullOrEmpty(groupID) ? globalGroupID : groupID;

            vibrateExecuted = true;
            _ = executeGroupAsync(localGroup, PiShockMode.Vibrate, localDuration, localIntensity);
        }

        if (vibrate is null) vibrateExecuted = false;

        if (beep is not null && beep.Value.Item1 + delay <= DateTimeOffset.Now && !beepExecuted)
        {
            var groupID = beep.Value.Item2;
            var localDuration = string.IsNullOrEmpty(groupID) ? globalDuration : durations[groupID];
            var localIntensity = string.IsNullOrEmpty(groupID) ? globalIntensity : intensities[groupID];
            var localGroup = string.IsNullOrEmpty(groupID) ? globalGroupID : groupID;

            beepExecuted = true;
            _ = executeGroupAsync(localGroup, PiShockMode.Beep, localDuration, localIntensity);
        }

        if (beep is null) beepExecuted = false;
    }

    private async Task executeGroupAsync(string groupID, PiShockMode mode, float durationPercentage, float intensityPercentage)
    {
        var shockerGroup = GetSettingValue<IEnumerable<ShockerGroup>>(PiShockSetting.Groups).SingleOrDefault(shockerGroup => shockerGroup.ID == groupID);
        if (shockerGroup is null) return;

        var convertedDuration = Math.Min((int)Math.Round(Interpolation.Map(durationPercentage, 0, 1, 1, 15)), shockerGroup.MaxDuration.Value);
        var convertedIntensity = Math.Min((int)Math.Round(Interpolation.Map(intensityPercentage, 0, 1, 1, 100)), shockerGroup.MaxIntensity.Value);

        var tasks = shockerGroup.Shockers.DistinctBy(shockerID => shockerID.Value).Select(shockerID =>
        {
            return Task.Run(async () => await executeShockerAsync(shockerID.Value, mode, convertedDuration, convertedIntensity));
        });

        await Task.WhenAll(tasks);
        Log($"Group '{shockerGroup.Name.Value}' has been executed");
    }

    private async Task executeShockerAsync(string shockerID, PiShockMode mode, int duration, int intensity)
    {
        var shockerInstance = GetSettingValue<IEnumerable<Shocker>>(PiShockSetting.Shockers).SingleOrDefault(shocker => shocker.ID == shockerID);
        if (shockerInstance is null) return;

        var shockerName = shockerInstance.Name.Value;

        Log($"Executing shocker '{shockerName}'");

        var username = GetSettingValue<string>(PiShockSetting.Username);
        var apiKey = GetSettingValue<string>(PiShockSetting.APIKey);
        var sharecode = shockerInstance.Sharecode.Value;

        var response = await piShockProvider.Execute(username, apiKey, sharecode, mode, duration, intensity);
        Log(response.Success ? $"Shocker '{shockerName}' successfully executed at duration {response.FinalDuration} and intensity {response.FinalIntensity}" : $"Shocker '{shockerName}' failed with: '{response.Message}'");
    }

    private bool getShockerGroupFromIndex(int index, [NotNullWhen(true)] out ShockerGroup? shockerGroup)
    {
        var groups = GetSettingValue<List<ShockerGroup>>(PiShockSetting.Groups);

        if (index < 0 || index >= groups.Count)
        {
            shockerGroup = null;
            return false;
        }

        shockerGroup = groups[index];
        return true;
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case PiShockParameter.Group:
                var groupIndex = parameter.GetValue<int>();

                if (getShockerGroupFromIndex(groupIndex, out var shockerGroup))
                    globalGroupID = shockerGroup.ID;

                break;

            case PiShockParameter.Duration:
                globalDuration = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);
                break;

            case PiShockParameter.Intensity:
                globalIntensity = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);
                break;

            case PiShockParameter.Shock:
                shock = parameter.GetValue<bool>() ? (DateTimeOffset.Now, null) : null;
                break;

            case PiShockParameter.Vibrate:
                vibrate = parameter.GetValue<bool>() ? (DateTimeOffset.Now, null) : null;
                break;

            case PiShockParameter.Beep:
                beep = parameter.GetValue<bool>() ? (DateTimeOffset.Now, null) : null;
                break;

            case PiShockParameter.DurationGroup:
                var groupIndex2 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex2, out var shockerGroup2))
                    durations[shockerGroup2.ID] = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);

                break;

            case PiShockParameter.IntensityGroup:
                var groupIndex3 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex3, out var shockerGroup3))
                    intensities[shockerGroup3.ID] = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);

                break;

            case PiShockParameter.ShockGroup:
                var groupIndex4 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex4, out var shockerGroup4))
                    shock = parameter.GetValue<bool>() ? (DateTimeOffset.Now, shockerGroup4.ID) : null;

                break;

            case PiShockParameter.VibrateGroup:
                var groupIndex5 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex5, out var shockerGroup5))
                    vibrate = parameter.GetValue<bool>() ? (DateTimeOffset.Now, shockerGroup5.ID) : null;

                break;

            case PiShockParameter.BeepGroup:
                var groupIndex6 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex6, out var shockerGroup6))
                    beep = parameter.GetValue<bool>() ? (DateTimeOffset.Now, shockerGroup6.ID) : null;

                break;
        }
    }

    private enum PiShockSetting
    {
        Username,
        APIKey,
        ButtonDelay,
        Shockers,
        Groups,
        Phrases
    }

    private enum PiShockParameter
    {
        Group,
        Duration,
        Intensity,
        Shock,
        Vibrate,
        Beep,
        DurationGroup,
        IntensityGroup,
        ShockGroup,
        VibrateGroup,
        BeepGroup
    }
}