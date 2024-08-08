// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

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
public class PiShockModule : Module
{
    private PiShockProvider? piShockProvider;

    private int group;
    private float duration;
    private float intensity;

    private (DateTimeOffset, int?)? shock;
    private (DateTimeOffset, int?)? vibrate;
    private (DateTimeOffset, int?)? beep;
    private bool shockExecuted;
    private bool vibrateExecuted;
    private bool beepExecuted;

    public List<Shocker> ShockersSetting => GetSettingValue<List<Shocker>>(PiShockSetting.Shockers)!;

    protected override void OnPreLoad()
    {
        CreateTextBox(PiShockSetting.Username, "Username", "Your PiShock username", string.Empty);
        CreateCustom(PiShockSetting.APIKey, new StringModuleSetting("API Key", "Your PiShock API key", typeof(PiShockAPIKeyView), string.Empty));

        CreateTextBox(PiShockSetting.ButtonDelay, "Button Delay", "The amount of time in milliseconds the shock, vibrate, and beep parameters need to be true to execute the action. This is helpful for if you accidentally press buttons on your action menu", 0);

        CreateCustom(PiShockSetting.Shockers, new ShockerModuleSetting());
        CreateCustom(PiShockSetting.Groups, new ShockerGroupModuleSetting());

        CreateGroup("Credentials", PiShockSetting.Username, PiShockSetting.APIKey);
        CreateGroup("Management", PiShockSetting.Shockers, PiShockSetting.Groups);
        CreateGroup("Tweaks", PiShockSetting.ButtonDelay);

        RegisterParameter<float>(PiShockParameter.Duration, "VRCOSC/PiShock/Duration", ParameterMode.Read, "Duration", "The duration of the action as a 0-1 float mapped between 1 and Max Duration");
        RegisterParameter<float>(PiShockParameter.Intensity, "VRCOSC/PiShock/Intensity", ParameterMode.Read, "Intensity", "The intensity of the action as a 0-1 float mapped between 1 and Max Intensity");
        RegisterParameter<int>(PiShockParameter.Group, "VRCOSC/PiShock/Group", ParameterMode.Read, "Group", "Sets the specific group to use when using the non-specific action parameters");
        RegisterParameter<bool>(PiShockParameter.Shock, "VRCOSC/PiShock/Shock", ParameterMode.Read, "Shock", "Shock the group set by the Group parameter");
        RegisterParameter<bool>(PiShockParameter.Vibrate, "VRCOSC/PiShock/Vibrate", ParameterMode.Read, "Vibrate", "Vibrate the group set by the Group parameter");
        RegisterParameter<bool>(PiShockParameter.Beep, "VRCOSC/PiShock/Beep", ParameterMode.Read, "Beep", "Beep the group set by the Group parameter");
        RegisterParameter<bool>(PiShockParameter.ShockGroup, "VRCOSC/PiShock/Shock/*", ParameterMode.Read, "Shock Group", "Shock a specific group\nE.G. VRCOSC/PiShock/Shock/0");
        RegisterParameter<bool>(PiShockParameter.VibrateGroup, "VRCOSC/PiShock/Vibrate/*", ParameterMode.Read, "Vibrate Group", "Vibrate a specific group\nE.G. VRCOSC/PiShock/Vibrate/0");
        RegisterParameter<bool>(PiShockParameter.BeepGroup, "VRCOSC/PiShock/Beep/*", ParameterMode.Read, "Beep Group", "Beep a specific group\nE.G. VRCOSC/PiShock/Beep/0");
    }

    protected override void OnPostLoad()
    {
        GetSetting<ShockerModuleSetting>(PiShockSetting.Shockers)!.Attribute.CollectionChanged += (_, e) =>
        {
            if (e.OldItems is not null)
            {
                var groupSetting = GetSetting<ShockerGroupModuleSetting>(PiShockSetting.Groups)!;

                foreach (Shocker oldShocker in e.OldItems)
                {
                    foreach (var shockerGroup in groupSetting.Attribute)
                    {
                        shockerGroup.Shockers.RemoveIf(shockerID => shockerID.Value == oldShocker.ID);
                    }
                }
            }
        };
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
        group = 0;
        duration = 0f;
        intensity = 0f;
        shock = null;
        vibrate = null;
        beep = null;
        shockExecuted = false;
        vibrateExecuted = false;
        beepExecuted = false;
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void checkForExecutions()
    {
        var delay = TimeSpan.FromMilliseconds(GetSettingValue<int>(PiShockSetting.ButtonDelay));

        if (shock is not null && shock.Value.Item1 + delay <= DateTimeOffset.Now && !shockExecuted)
        {
            executePiShockMode(PiShockMode.Shock, shock.Value.Item2 ?? group);
            shockExecuted = true;
        }

        if (shock is null) shockExecuted = false;

        if (vibrate is not null && vibrate.Value.Item1 + delay <= DateTimeOffset.Now && !vibrateExecuted)
        {
            executePiShockMode(PiShockMode.Vibrate, vibrate.Value.Item2 ?? group);
            vibrateExecuted = true;
        }

        if (vibrate is null) vibrateExecuted = false;

        if (beep is not null && beep.Value.Item1 + delay <= DateTimeOffset.Now && !beepExecuted)
        {
            executePiShockMode(PiShockMode.Beep, beep.Value.Item2 ?? group);
            beepExecuted = true;
        }

        if (beep is null) beepExecuted = false;
    }

    private void executePiShockMode(PiShockMode mode, int group)
    {
        var shockerGroup = GetSettingValue<List<ShockerGroup>>(PiShockSetting.Groups)?.ElementAtOrDefault(group);

        if (shockerGroup is null)
        {
            Log($"No group with ID {group}");
            return;
        }

        Log($"Executing {mode} on group '{shockerGroup.Name}'");

        foreach (var shockerID in shockerGroup.Shockers)
        {
            var shockerInstance = getShockerFromID(shockerID.Value);
            if (shockerInstance is null) continue;

            sendPiShockData(mode, shockerGroup, shockerInstance);
        }
    }

    private async void sendPiShockData(PiShockMode mode, ShockerGroup group, Shocker instance)
    {
        var convertedDuration = (int)Math.Round(Interpolation.Map(duration, 0, 1, 1, group.MaxDuration.Value));
        var convertedIntensity = (int)Math.Round(Interpolation.Map(intensity, 0, 1, 1, group.MaxIntensity.Value));

        var response = await piShockProvider!.Execute(GetSettingValue<string>(PiShockSetting.Username)!, GetSettingValue<string>(PiShockSetting.APIKey)!, instance.Sharecode.Value, mode, convertedDuration, convertedIntensity);
        Log(response.Success ? $"{instance.Name.Value} succeeded" : $"{instance.Name.Value} failed - {response.Message}");
    }

    private Shocker? getShockerFromID(string id) => GetSettingValue<List<Shocker>>(PiShockSetting.Shockers)!.SingleOrDefault(shockerInstance => shockerInstance.ID == id);

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case PiShockParameter.Group:
                group = parameter.GetValue<int>();
                break;

            case PiShockParameter.Duration:
                duration = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);
                break;

            case PiShockParameter.Intensity:
                intensity = Math.Clamp(parameter.GetValue<float>(), 0f, 1f);
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

            case PiShockParameter.ShockGroup:
                shock = parameter.GetValue<bool>() ? (DateTimeOffset.Now, parameter.WildcardAs<int>(0)) : null;
                break;

            case PiShockParameter.VibrateGroup:
                vibrate = parameter.GetValue<bool>() ? (DateTimeOffset.Now, parameter.WildcardAs<int>(0)) : null;
                break;

            case PiShockParameter.BeepGroup:
                beep = parameter.GetValue<bool>() ? (DateTimeOffset.Now, parameter.WildcardAs<int>(0)) : null;
                break;
        }
    }

    private enum PiShockSetting
    {
        Username,
        APIKey,
        ButtonDelay,
        Shockers,
        Groups
    }

    private enum PiShockParameter
    {
        Group,
        Duration,
        Intensity,
        Shock,
        Vibrate,
        Beep,
        ShockGroup,
        VibrateGroup,
        BeepGroup
    }
}
