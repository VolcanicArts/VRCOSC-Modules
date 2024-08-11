// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.Keybinds.UI;

public partial class KeybindsModuleSettingView
{
    private readonly KeybindsModuleSetting moduleSetting;
    private WindowManager windowManager = null!;

    public KeybindsModuleSettingView(KeybindsModule _, KeybindsModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void KeybindsModuleSettingView_OnLoaded(object sender, RoutedEventArgs e)
    {
        windowManager = new WindowManager(this);
    }

    private void AddInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        moduleSetting.Add();
    }

    private void RemoveInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var countInstance = (KeybindsInstance)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the keybind data. Are you sure?", "Delete Keybind?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Remove(countInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var keybindsInstance = (KeybindsInstance)element.Tag;

        windowManager.TrySpawnChild(new KeybindsInstanceEditWindow(keybindsInstance));
    }
}
