// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.PiShock.UI;

public partial class ShockerGroupModuleSettingView
{
    private readonly PiShockModule module;
    private readonly ShockerGroupModuleSetting moduleSetting;
    private WindowManager windowManager = null!;

    public ShockerGroupModuleSettingView(PiShockModule module, ShockerGroupModuleSetting moduleSetting)
    {
        this.module = module;
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void shockerGroupModuleSettingView_OnLoaded(object sender, RoutedEventArgs e)
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
        var countInstance = (ShockerGroup)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the shocker group data. Are you sure?", "Delete Shocker Group?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Remove(countInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var shockerGroup = (ShockerGroup)element.Tag;

        var editWindow = new ShockerGroupEditWindow(module, shockerGroup);

        windowManager.TrySpawnChild(editWindow);
    }
}
