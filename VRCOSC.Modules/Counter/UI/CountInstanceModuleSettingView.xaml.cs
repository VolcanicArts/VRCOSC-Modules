// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;

namespace VRCOSC.Modules.Counter.UI;

public partial class CountInstanceModuleSettingView
{
    private readonly CounterInstanceModuleSetting moduleSetting;

    public CountInstanceModuleSettingView(CounterInstanceModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void AddInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        moduleSetting.Instances.Add(new CounterInstance());
    }

    private void RemoveInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var countInstance = (CounterInstance)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the counter data and remove it from the ChatBox. Are you sure?", "Delete Counter?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Instances.Remove(countInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var countInstance = (CounterInstance)element.Tag;

        new CounterInstanceEditWindow(countInstance).Show();
    }
}
