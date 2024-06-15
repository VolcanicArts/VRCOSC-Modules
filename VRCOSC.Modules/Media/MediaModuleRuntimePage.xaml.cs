// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Windows.Media.Control;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Media;

public partial class MediaModuleRuntimePage
{
    public MediaModule Module { get; }
    public ObservableCollection<GlobalSystemMediaTransportControlsSession> SessionProxy = new();

    public MediaModuleRuntimePage(MediaModule module)
    {
        Module = module;
        InitializeComponent();

        DataContext = this;

        Module.MediaProvider.Sessions.CollectionChanged += (_, e) => Dispatcher.Invoke(() =>
        {
            if (e.NewItems is not null)
            {
                foreach (GlobalSystemMediaTransportControlsSession newSession in e.NewItems)
                {
                    SessionProxy.Add(newSession);
                }
            }

            if (e.OldItems is not null)
            {
                SessionProxy.RemoveIf(session => e.OldItems.Contains(session));
            }
        });
    }

    private void SourceSelection_OnLostMouseCapture(object sender, MouseEventArgs e)
    {
    }

    private void SourceSelection_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var comboBox = (ComboBox)sender;
        var selectedValue = (string)comboBox.SelectedValue;

        Module.MediaProvider.SetAutoSwitch(false);

        var session = Module.MediaProvider.Sessions.FirstOrDefault(session => session.SourceAppUserModelId == selectedValue);
        if (session is not null)
            Module.MediaProvider.SetManualSession(session);
    }
}
