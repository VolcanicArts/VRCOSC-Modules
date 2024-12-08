// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Utils;

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

        CreateGroup("Keybinds", KeybindsSetting.Keybinds);
    }

    protected override async void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        if (!parameter.IsValueType<bool>()) return;

        foreach (var keybindsInstance in GetSettingValue<List<KeybindsInstance>>(KeybindsSetting.Keybinds)!)
        {
            KeybindMode localMode;

            if (keybindsInstance.Mode.Value == KeybindInstanceMode.HoldRelease)
            {
                localMode = parameter.GetValue<bool>() ? KeybindMode.Hold : KeybindMode.Release;
            }
            else
            {
                localMode = KeybindMode.Press;
            }

            if (localMode == KeybindMode.Press && !parameter.GetValue<bool>()) continue;
            if (!keybindsInstance.ParameterNames.Select(parameterNames => parameterNames.Value).Contains(parameter.Name)) continue;

            foreach (var keybind in keybindsInstance.Keybinds)
            {
                await KeySimulator.ExecuteKeybind(keybind.Value, localMode);
            }
        }
    }

    private enum KeybindsSetting
    {
        Keybinds
    }
}