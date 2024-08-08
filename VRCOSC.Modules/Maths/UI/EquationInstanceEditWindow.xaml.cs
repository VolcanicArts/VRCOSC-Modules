// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Maths.UI;

public partial class EquationInstanceEditWindow
{
    private readonly Equation equation;

    public EquationInstanceEditWindow(Equation equation)
    {
        this.equation = equation;

        InitializeComponent();

        DataContext = equation;

        equation.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void AddTriggerParameter_OnClick(object sender, RoutedEventArgs e)
    {
        equation.TriggerParameters.Add(new Observable<string>(string.Empty));
    }

    private void RemoveTriggerParameter_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterInstance = (Observable<string>)element.Tag;

        equation.TriggerParameters.Remove(parameterInstance);
    }
}
