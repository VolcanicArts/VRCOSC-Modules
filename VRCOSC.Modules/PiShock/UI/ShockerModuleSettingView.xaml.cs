// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;

namespace VRCOSC.Modules.PiShock.UI;

public partial class ShockerModuleSettingView
{
    private readonly ShockerModuleSetting moduleSetting;

    public ShockerModuleSettingView(PiShockModule _, ShockerModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var instance = element.Tag;

        moduleSetting.Remove(instance);
    }

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        moduleSetting.Add();
    }
}
