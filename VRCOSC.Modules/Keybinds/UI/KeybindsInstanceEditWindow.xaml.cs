// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.SDK.Utils;
using VRCOSC.App.UI.Core;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Keybinds.UI;

public partial class KeybindsInstanceEditWindow : IManagedWindow
{
    private WindowManager windowManager = null!;
    public KeybindsInstance Instance { get; }

    public KeybindsInstanceEditWindow(KeybindsInstance instance)
    {
        InitializeComponent();

        Instance = instance;
        DataContext = this;
        SourceInitialized += OnSourceInitialized;

        instance.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        windowManager = new WindowManager(this);
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

    private void EditParametersButton_OnClick(object sender, RoutedEventArgs e)
    {
        windowManager.TrySpawnChild(new QueryableParameterListWindow(Instance.Parameters));
    }
}