// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.SDK.Utils;
using VRCOSC.App.UI.Core;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Keybinds.UI;

public partial class KeybindsInstanceEditWindow : IManagedWindow
{
    public KeybindsInstance Instance { get; }
    public IEnumerable<KeybindInstanceMode> ModeItemsSource => Enum.GetValues<KeybindInstanceMode>();

    public KeybindsInstanceEditWindow(KeybindsInstance instance)
    {
        InitializeComponent();

        Instance = instance;
        DataContext = this;

        instance.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void AddParameter_OnClick(object sender, RoutedEventArgs e)
    {
        Instance.ParameterNames.Add(new Observable<string>(string.Empty));
    }

    private void RemoveParameter_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterName = (Observable<string>)element.Tag;

        Instance.ParameterNames.Remove(parameterName);
    }

    private void AddKeybind_OnClick(object sender, RoutedEventArgs e)
    {
        Instance.Keybinds.Add(new Observable<Keybind>(new Keybind()));
    }

    private void RemoveKeybind_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var keybind = (Observable<Keybind>)element.Tag;

        Instance.Keybinds.Remove(keybind);
    }

    public object GetComparer() => Instance;
}