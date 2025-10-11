// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Media;

public partial class MediaModuleRuntimeView : IDisposable
{
    public MediaModule Module { get; }
    public ObservableCollection<string> Sessions { get; } = new();

    private readonly IDisposable sessionsChangedDisposable;

    public MediaModuleRuntimeView(MediaModule module)
    {
        Module = module;
        InitializeComponent();

        DataContext = this;

        sessionsChangedDisposable = Module.MediaProvider.Sessions.OnCollectionChanged((newItems, oldItems) => Dispatcher.Invoke(() =>
        {
            foreach (string newSession in newItems)
            {
                Sessions.Add(newSession);
            }

            foreach (string oldSession in oldItems)
            {
                Sessions.RemoveIf(session => session == oldSession);
            }
        }), true);

        SourceComboBox.SelectedValue = module.SourceSelection;
    }

    private void SourceSelection_OnLostMouseCapture(object sender, MouseEventArgs e)
    {
    }

    private void SourceSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var selectedValue = (string)comboBox.SelectedValue;

        Module.SourceSelection = selectedValue;
        Module.MediaProvider.SetFocusedSession(selectedValue);
    }

    public void Dispose()
    {
        sessionsChangedDisposable.Dispose();
    }
}