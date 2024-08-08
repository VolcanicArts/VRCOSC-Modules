// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Maths.UI;

public partial class EquationModuleSettingView
{
    private static readonly Uri equation_docs_url = new("https://mathparser.org/mxparser-math-collection");

    private readonly EquationModuleSetting moduleSetting;
    private WindowManager windowManager = null!;

    public EquationModuleSettingView(MathsModule _, EquationModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void EquationModuleSettingView_OnLoaded(object sender, RoutedEventArgs e)
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
        var countInstance = (Equation)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the counter data and remove it from the ChatBox. Are you sure?", "Delete Counter?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Remove(countInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var equationInstance = (Equation)element.Tag;

        var editWindow = new EquationInstanceEditWindow(equationInstance);

        windowManager.TrySpawnChild(editWindow);
    }

    private void OpenEquationDocs_OnClick(object sender, RoutedEventArgs e)
    {
        equation_docs_url.OpenExternally();
    }
}
