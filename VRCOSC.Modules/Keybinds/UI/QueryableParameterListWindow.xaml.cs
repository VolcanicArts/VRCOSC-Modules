// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.Keybinds.UI;

public partial class QueryableParameterListWindow : IManagedWindow
{
    public ObservableCollection<KeybindQueryableParameter> QueryableParameters { get; }
    public IEnumerable<KeybindAction> ActionItemsSource => typeof(KeybindAction).GetEnumValues().Cast<KeybindAction>();

    public QueryableParameterListWindow(ObservableCollection<KeybindQueryableParameter> queryableParameters)
    {
        QueryableParameters = queryableParameters;
        InitializeComponent();
        DataContext = this;
    }

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        QueryableParameters.Add(new KeybindQueryableParameter());
    }

    private void RemoveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var instance = (KeybindQueryableParameter)element.Tag;

        QueryableParameters.Remove(instance);
    }

    public object GetComparer() => QueryableParameters;
}