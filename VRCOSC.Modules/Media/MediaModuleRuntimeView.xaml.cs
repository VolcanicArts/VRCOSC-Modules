// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace VRCOSC.Modules.Media;

public partial class MediaModuleRuntimeView
{
    public MediaModule Module { get; }

    public ObservableCollection<SourceSelectionItem> Sessions { get; } = [new("Auto-Switch", null)];

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
        var activeSessions = Module.MediaProvider.SessionStates.Select(pair => new SourceSelectionItem(pair.Key, pair.Key));

        if (!string.IsNullOrWhiteSpace(Module.SourceSelection) && Sessions.All(existingItem => existingItem.Value != Module.SourceSelection))
            Sessions.Add(new SourceSelectionItem(Module.SourceSelection, Module.SourceSelection));

        foreach (var item in activeSessions)
        {
            if (Sessions.All(existingItem => existingItem.Value != item.Value)) Sessions.Add(item);
        }

        SourceComboBox.SelectedValue = Module.SourceSelection;
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