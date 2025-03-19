// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Utils;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Keybinds;

[ModuleTitle("Keybinds")]
[ModuleDescription("Trigger keybinds using avatar parameters")]
[ModuleType(ModuleType.Integrations)]
[ModulePrefab("Official Prefabs", "https://vrcosc.com/docs/downloads#prefabs")]
public class KeybindsModule : Module
{
    protected override void OnPreLoad()
    {
        CreateCustomSetting(KeybindsSetting.Keybinds, new KeybindsModuleSetting());

        CreateGroup("Configuration", string.Empty, KeybindsSetting.Keybinds);
    }

    protected override async Task<bool> OnModuleStart()
    {
        foreach (var queryableParameter in GetSettingValue<List<KeybindsInstance>>(KeybindsSetting.Keybinds).SelectMany(instance => instance.Parameters))
        {
            await queryableParameter.Init();
        }

        return true;
    }

    protected override async void OnAnyParameterReceived(ReceivedParameter receivedParameter)
    {
        var keybinds = GetSettingValue<List<KeybindsInstance>>(KeybindsSetting.Keybinds);

        foreach (var keybind in keybinds)
        {
            foreach (KeybindQueryableParameter queryableParameter in keybind.Parameters.Where(queryableParameter => queryableParameter.Name.Value == receivedParameter.Name))
            {
                var result = queryableParameter.Evaluate(receivedParameter);

                // TODO Do this filter in the UI
                if (queryableParameter.Comparison.Value == ComparisonOperation.Changed)
                {
                    switch (queryableParameter.Action.Value)
                    {
                        case KeybindAction.Hold:
                        {
                            Log($"{KeybindAction.Hold} action is not compatible with the {ComparisonOperation.Changed} comparison");
                            break;
                        }

                        case KeybindAction.Press:
                        {
                            await executeKeybinds(keybind.Keybinds, KeybindMode.Press);
                            break;
                        }
                    }
                }

                if (result.JustBecameValid)
                {
                    switch (queryableParameter.Action.Value)
                    {
                        case KeybindAction.Hold:
                        {
                            await executeKeybinds(keybind.Keybinds, KeybindMode.Hold);
                            break;
                        }

                        case KeybindAction.Press:
                        {
                            await executeKeybinds(keybind.Keybinds, KeybindMode.Press);
                            break;
                        }
                    }
                }

                if (result.JustBecameInvalid)
                {
                    if (queryableParameter.Action.Value == KeybindAction.Hold)
                    {
                        await executeKeybinds(keybind.Keybinds, KeybindMode.Release);
                    }
                }
            }
        }
    }

    private async Task executeKeybinds(ObservableCollection<Observable<Keybind>> keybinds, KeybindMode mode)
    {
        foreach (var keybindKeybind in keybinds)
        {
            await KeySimulator.ExecuteKeybind(keybindKeybind.Value, mode);
        }
    }

    private enum KeybindsSetting
    {
        Keybinds
    }
}

public enum KeybindAction
{
    Press,
    Hold
}