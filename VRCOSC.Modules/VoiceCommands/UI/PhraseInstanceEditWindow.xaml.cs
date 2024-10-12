// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.VoiceCommands.UI;

public partial class PhraseInstanceEditWindow
{
    private readonly Phrase instance;
    private WindowManager windowManager = null!;

    public PhraseInstanceEditWindow(Phrase instance)
    {
        this.instance = instance;

        InitializeComponent();

        DataContext = instance;

        instance.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void PhraseInstanceEditWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        windowManager = new WindowManager(this);
    }

    private void AddInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        instance.Parameters.Add(new Parameter());
    }

    private void RemoveInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterInstance = (Parameter)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the parameter data. Are you sure?", "Delete Parameter?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        instance.Parameters.Remove(parameterInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterInstance = (Parameter)element.Tag;

        var editWindow = new ParameterInstanceEditWindow(parameterInstance);

        windowManager.TrySpawnChild(editWindow);
    }
}
