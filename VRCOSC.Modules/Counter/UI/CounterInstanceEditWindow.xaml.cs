// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter.UI;

public partial class CounterInstanceEditWindow
{
    private readonly CounterInstance instance;

    public CounterInstanceEditWindow(CounterInstance instance)
    {
        InitializeComponent();

        this.instance = instance;
        DataContext = instance;
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
}
