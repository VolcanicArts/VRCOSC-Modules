// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter;

public partial class CountInstanceModuleSettingView
{
    private readonly CounterInstanceModuleSetting moduleSetting;

    public CountInstanceModuleSettingView(CounterInstanceModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var countInstance = (CounterInstance)element.Tag;

        countInstance.ParameterNames.Add(new Observable<string>(string.Empty));
    }

    private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterInstance = (Observable<string>)element.Tag;

        foreach (var countInstance in moduleSetting.Instances)
        {
            if (countInstance.ParameterNames.Contains(parameterInstance))
                countInstance.ParameterNames.Remove(parameterInstance);
        }
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
}
