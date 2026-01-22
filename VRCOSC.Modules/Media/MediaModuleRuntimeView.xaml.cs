// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows.Controls;

namespace VRCOSC.Modules.Media;

public partial class MediaModuleRuntimeView
{
    public MediaModule Module { get; }

    public MediaModuleRuntimeView(MediaModule module)
    {
        Module = module;
        InitializeComponent();
        DataContext = this;

        Module.MediaProvider.OnSessionsChanged += updateSessionComboBox;
        updateSessionComboBox();
    }

    private void updateSessionComboBox() => Dispatcher.Invoke(() =>
    {
        var sessions = new List<SourceSelectionItem> { new("Auto-Switch", string.Empty) };
        sessions.AddRange(Module.MediaProvider.SessionStates.Select(pair => new SourceSelectionItem(pair.Key, pair.Key)));
        SourceComboBox.ItemsSource = sessions;
        SourceComboBox.SelectedValue = Module.SourceSelection ?? string.Empty;
    });

    private void SourceSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var selectedValue = (string)comboBox.SelectedValue;

        Module.SourceSelection = selectedValue;
        Module.MediaProvider.SetFocusedSession(selectedValue);
    }
}

public record SourceSelectionItem(string Name, string? Value);