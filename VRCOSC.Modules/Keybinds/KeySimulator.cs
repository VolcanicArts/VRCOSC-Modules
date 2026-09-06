// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Windows.Input;
using VRCOSC.App.SDK.Utils;
using WindowsInput;
using WindowsInput.Events;

namespace VRCOSC.Modules.Keybinds;

public static class KeySimulator
{
    public static async Task ExecuteKeybind(Keybind keybind, KeybindMode mode = KeybindMode.Press)
    {
        var keys = keybind.Modifiers.Concat(keybind.Keys).Select(key => (KeyCode)KeyInterop.VirtualKeyFromKey(key));

        switch (mode)
        {
            case KeybindMode.Press:
                await Simulate.Events().ClickChord(keys).Wait(50).Invoke();
                break;

            case KeybindMode.Hold:
                await Simulate.Events().Hold(keys).Invoke();
                break;

            case KeybindMode.Release:
                await Simulate.Events().Release(keys).Invoke();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}

public enum KeybindMode
{
    Press,
    Hold,
    Release
}