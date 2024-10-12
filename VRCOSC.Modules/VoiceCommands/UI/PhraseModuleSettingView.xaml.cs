// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.VoiceCommands.UI;

public partial class PhraseModuleSettingView
{
    private readonly PhraseModuleSetting moduleSetting;
    private WindowManager windowManager = null!;

    public PhraseModuleSettingView(VoiceCommandsModule _, PhraseModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;

        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void PhraseModuleSettingView_OnLoaded(object sender, RoutedEventArgs e)
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
        var phraseInstance = (Phrase)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the phrase data. Are you sure?", "Delete Phrase?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Remove(phraseInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var phraseInstance = (Phrase)element.Tag;

        var editWindow = new PhraseInstanceEditWindow(phraseInstance);

        windowManager.TrySpawnChild(editWindow);
    }
}
