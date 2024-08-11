// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Utils;

namespace VRCOSC.Modules.Keybinds;

[ModuleTitle("Keybinds")]
[ModuleDescription("Trigger keybinds using avatar parameters")]
[ModuleType(ModuleType.Integrations)]
public class KeybindsModule : Module
{
    protected override void OnPreLoad()
    {
        CreateCustom(KeybindsSetting.Keybinds, new KeybindsModuleSetting());

        CreateGroup("Keybinds", KeybindsSetting.Keybinds);
    }

    protected override void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        if (!parameter.IsValueType<bool>()) return;
        if (!parameter.GetValue<bool>()) return;

        foreach (var keybindsInstance in GetSettingValue<List<KeybindsInstance>>(KeybindsSetting.Keybinds)!)
        {
            if (keybindsInstance.ParameterNames.Select(parameterNames => parameterNames.Value).Contains(parameter.Name))
            {
                foreach (var keybind in keybindsInstance.Keybinds)
                {
                    KeySimulator.ExecuteKeybind(keybind.Value);
                }
            }
        }
    }

    private enum KeybindsSetting
    {
        Keybinds
    }
}
