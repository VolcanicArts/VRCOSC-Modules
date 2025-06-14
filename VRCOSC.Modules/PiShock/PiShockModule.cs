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
[ModuleDescription("Allows for controlling PiShock shockers with avatar parameters and your voice")]
[ModuleType(ModuleType.NSFW)]
[ModulePrefab("Official Prefabs", "https://vrcosc.com/docs/downloads#prefabs")]
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

    private readonly List<string> executedPhrases = [];

    public ShockerModuleSetting ShockersSetting => GetSetting<ShockerModuleSetting>(PiShockSetting.Shockers);
    public ShockerGroupModuleSetting GroupsSetting => GetSetting<ShockerGroupModuleSetting>(PiShockSetting.Groups);

    protected override void OnPreLoad()
    {
        CreateTextBox(PiShockSetting.Username, "Username", "Your PiShock username", string.Empty);
        CreateCustomSetting(PiShockSetting.APIKey, new StringModuleSetting("API Key", "Your PiShock API key", typeof(PiShockAPIKeyView), string.Empty));

        CreateSlider(PiShockSetting.ButtonDelay, "Button Delay",
            "The amount of time in milliseconds the shock, vibrate, and beep parameters need to be true to execute the action. This is helpful for if you accidentally press buttons on your action menu", 0, 0, 1000, 10);

        CreateCustomSetting(PiShockSetting.Shockers, new ShockerModuleSetting());
        CreateCustomSetting(PiShockSetting.Groups, new ShockerGroupModuleSetting());
        CreateCustomSetting(PiShockSetting.Phrases, new PhraseModuleSetting());

        CreateGroup("Credentials", string.Empty, PiShockSetting.Username, PiShockSetting.APIKey);
        CreateGroup("Management", string.Empty, PiShockSetting.Shockers, PiShockSetting.Groups);
        CreateGroup("Tweaks", string.Empty, PiShockSetting.ButtonDelay);
        CreateGroup("Speech", string.Empty, PiShockSetting.Phrases);

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
        RegisterParameter<bool>(PiShockParameter.Success, "VRCOSC/PiShock/Success", ParameterMode.Write, "Success", "Becomes true for 1 second if the action has succeeded");
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

        executedPhrases.Clear();
    }

    public void OnPartialSpeechResult(string text)
    {
        handleSpeechText(text);
    }

    public void OnFinalSpeechResult(string text)
    {
        handleSpeechText(text);
        executedPhrases.Clear();
    }

    private void handleSpeechText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        foreach (var phrase in GetSettingValue<List<Phrase>>(PiShockSetting.Phrases).Where(phrase => !executedPhrases.Contains(phrase.ID) && text.Contains(phrase.Text.Value, StringComparison.InvariantCultureIgnoreCase)))
        {
            Log($"Found phrase '{phrase.Text.Value}'");
            executedPhrases.Add(phrase.ID);
            phrase.ShockerGroups.DistinctBy(shockerGroupID => shockerGroupID.Value).ForEach(shockerGroupID => _ = ExecuteGroupAsync(shockerGroupID.Value, phrase.Mode.Value, phrase.Duration.Value, phrase.Intensity.Value));
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private async void checkForActions()
    {
        var delay = TimeSpan.FromMilliseconds(GetSettingValue<int>(PiShockSetting.ButtonDelay));

        if (shock is not null)
        {
            if (shock.Value.Item1 + delay <= DateTimeOffset.Now && !shockExecuted)
            {
                await HandleAction(shock.Value.Item2, PiShockMode.Shock);
                shockExecuted = true;
            }
        }
        else
        {
            shockExecuted = false;
        }

        if (vibrate is not null)
        {
            if (vibrate.Value.Item1 + delay <= DateTimeOffset.Now && !vibrateExecuted)
            {
                await HandleAction(vibrate.Value.Item2, PiShockMode.Vibrate);
                vibrateExecuted = true;
            }
        }
        else
        {
            vibrateExecuted = false;
        }

        if (beep is not null)
        {
            if (beep.Value.Item1 + delay <= DateTimeOffset.Now && !beepExecuted)
            {
                await HandleAction(beep.Value.Item2, PiShockMode.Beep);
                beepExecuted = true;
            }
        }
        else
        {
            beepExecuted = false;
        }
    }

    public async Task HandleAction(string? groupId, PiShockMode mode)
    {
        var localDuration = string.IsNullOrEmpty(groupId) ? globalDuration : durations[groupId];
        var localIntensity = string.IsNullOrEmpty(groupId) ? globalIntensity : intensities[groupId];
        var localGroup = string.IsNullOrEmpty(groupId) ? globalGroupID : groupId;

        await ExecuteGroupAsync(localGroup, mode, localDuration, localIntensity);
    }

    public Task<bool> ExecuteGroupAsync(string groupId, PiShockMode mode, float durationPercentage, float intensityPercentage)
    {
        var shockerGroup = GetSettingValue<List<ShockerGroup>>(PiShockSetting.Groups).SingleOrDefault(shockerGroup => shockerGroup.ID == groupId);
        if (shockerGroup is null) return Task.FromResult(false);

        var maxDuration = shockerGroup.MaxDuration.Value;
        var maxIntensity = shockerGroup.MaxIntensity.Value;

        var duration = (int)Math.Round(Interpolation.Map(maxDuration * durationPercentage, 0, maxDuration, 1, maxDuration));
        var intensity = (int)Math.Round(Interpolation.Map(maxIntensity * intensityPercentage, 0, maxIntensity, 1, maxIntensity));

        return ExecuteGroupAsync(groupId, mode, duration, intensity);
    }

    public async Task<bool> ExecuteGroupAsync(string groupId, PiShockMode mode, int duration, int intensity)
    {
        var shockerGroup = GetSettingValue<List<ShockerGroup>>(PiShockSetting.Groups).SingleOrDefault(shockerGroup => shockerGroup.ID == groupId);
        if (shockerGroup is null) return false;

        var localDuration = Math.Min(duration, shockerGroup.MaxDuration.Value);
        var localIntensity = Math.Min(intensity, shockerGroup.MaxIntensity.Value);

        var tasks = shockerGroup.Shockers.DistinctBy(shockerID => shockerID.Value).Select(shockerID => executeShockerAsync(shockerID.Value, mode, localDuration, localIntensity)).ToList();
        await Task.WhenAll(tasks);

        Log($"Group '{shockerGroup.Name.Value}' has been executed");

        if (!tasks.All(task => task.Result)) return false;

        sendSuccessParameter();
        return true;
    }

    private async Task<bool> executeShockerAsync(string shockerId, PiShockMode mode, int duration, int intensity)
    {
        var shockerInstance = GetSettingValue<List<Shocker>>(PiShockSetting.Shockers).SingleOrDefault(shocker => shocker.ID == shockerId);
        if (shockerInstance is null) return false;

        var shockerName = shockerInstance.Name.Value;

        Log($"Executing shocker '{shockerName}'");

        var username = GetSettingValue<string>(PiShockSetting.Username);
        var apiKey = GetSettingValue<string>(PiShockSetting.APIKey);
        var sharecode = shockerInstance.Sharecode.Value;

        var response = await piShockProvider.Execute(username, apiKey, sharecode, mode, duration, intensity);
        Log(response.Success ? $"Shocker '{shockerName}' successfully executed mode {mode} at duration {response.FinalDuration} and intensity {response.FinalIntensity}" : $"Shocker '{shockerName}' failed with: '{response.Message}'");
        return response.Success;
    }

    private async void sendSuccessParameter()
    {
        var wasAcknowledged = await SendParameterAndWait(PiShockParameter.Success, true);

        if (wasAcknowledged)
            SendParameter(PiShockParameter.Success, false);
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
                if (!parameter.IsWildcardType<int>(0))
                {
                    Log("Invalid group for specific group duration parameter");
                    return;
                }

                var groupIndex2 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex2, out var shockerGroup2))
                    durations[shockerGroup2.ID] = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);

                break;

            case PiShockParameter.IntensityGroup:
                if (!parameter.IsWildcardType<int>(0))
                {
                    Log("Invalid group for specific group intensity parameter");
                    return;
                }

                var groupIndex3 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex3, out var shockerGroup3))
                    intensities[shockerGroup3.ID] = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);

                break;

            case PiShockParameter.ShockGroup:
                if (!parameter.IsWildcardType<int>(0))
                {
                    Log("Invalid group for specific group shock parameter");
                    return;
                }

                var groupIndex4 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex4, out var shockerGroup4))
                    shock = parameter.GetValue<bool>() ? (DateTimeOffset.Now, shockerGroup4.ID) : null;

                break;

            case PiShockParameter.VibrateGroup:
                if (!parameter.IsWildcardType<int>(0))
                {
                    Log("Invalid group for specific group vibrate parameter");
                    return;
                }

                var groupIndex5 = parameter.GetWildcard<int>(0);

                if (getShockerGroupFromIndex(groupIndex5, out var shockerGroup5))
                    vibrate = parameter.GetValue<bool>() ? (DateTimeOffset.Now, shockerGroup5.ID) : null;

                break;

            case PiShockParameter.BeepGroup:
                if (!parameter.IsWildcardType<int>(0))
                {
                    Log("Invalid group for specific group beep parameter");
                    return;
                }

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
        BeepGroup,
        Success
    }
}