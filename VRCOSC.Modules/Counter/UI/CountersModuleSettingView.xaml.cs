// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.Counter.UI;

public partial class CountersModuleSettingView
{
    private readonly CountersModuleSetting moduleSetting;
    private WindowManager windowManager = null!;

    public CountersModuleSettingView(CounterModule _, CountersModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void CountersModuleSettingView_OnLoaded(object sender, RoutedEventArgs e)
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
        var countInstance = (Counter)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the counter data and remove it from the ChatBox. Are you sure?", "Delete Counter?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Remove(countInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var countInstance = (Counter)element.Tag;

        var editWindow = new CounterInstanceEditWindow(countInstance);

        windowManager.TrySpawnChild(editWindow);
    }
}
