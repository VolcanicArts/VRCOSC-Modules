// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.SDK.Utils;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Keybinds.UI;

public partial class KeybindsInstanceEditWindow
{
    private readonly KeybindsInstance instance;

    public KeybindsInstanceEditWindow(KeybindsInstance instance)
    {
        InitializeComponent();

        this.instance = instance;
        DataContext = instance;

        instance.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void AddParameter_OnClick(object sender, RoutedEventArgs e)
    {
        instance.ParameterNames.Add(new Observable<string>(string.Empty));
    }

    private void RemoveParameter_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterName = (Observable<string>)element.Tag;

        instance.ParameterNames.Remove(parameterName);
    }

    private void AddKeybind_OnClick(object sender, RoutedEventArgs e)
    {
        instance.Keybinds.Add(new Observable<Keybind>(new Keybind()));
    }

    private void RemoveKeybind_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var keybind = (Observable<Keybind>)element.Tag;

        instance.Keybinds.Remove(keybind);
    }
}
