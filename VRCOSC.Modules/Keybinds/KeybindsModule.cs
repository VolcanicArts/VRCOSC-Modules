// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

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
    }

    private enum KeybindsSetting
    {
        Keybinds,
        Test
    }
}

public enum KeybindAction
{
    Press,
    Hold
}