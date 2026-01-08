// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter.UI;

public partial class CounterInstanceEditWindow : IManagedWindow
{
    private readonly Counter instance;

    public CounterValueTodayMode[] ValueTodayModeItemsSource => Enum.GetValues<CounterValueTodayMode>();

    public CounterInstanceEditWindow(Counter instance)
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
        var parameterInstance = (Observable<string>)element.Tag;

        instance.ParameterNames.Remove(parameterInstance);
    }

    private void AddMilestone_OnClick(object sender, RoutedEventArgs e)
    {
        instance.Milestones.Add(new Observable<int>());
    }

    private void RemoveMilestone_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var milestoneInstance = (Observable<int>)element.Tag;

        instance.Milestones.Remove(milestoneInstance);
    }

    public object GetComparer() => instance;
}