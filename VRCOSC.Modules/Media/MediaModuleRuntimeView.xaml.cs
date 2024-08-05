using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Media;

public partial class MediaModuleRuntimeView
{
    public MediaModule Module { get; }
    public ObservableCollection<string> Sessions { get; } = new();

    public MediaModuleRuntimeView(MediaModule module)
    {
        Module = module;
        InitializeComponent();

        DataContext = this;

        Module.MediaProvider.Sessions.CollectionChanged += (_, e) => Dispatcher.Invoke(() =>
        {
            if (e.NewItems is not null)
            {
                foreach (string newSession in e.NewItems)
                {
                    Sessions.Add(newSession);
                }
            }

            if (e.OldItems is not null)
            {
                foreach (string oldSession in e.OldItems)
                {
                    Sessions.RemoveIf(session => session == oldSession);
                }
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

        Module.MediaProvider.SetFocusedSession(selectedValue);
    }
}
