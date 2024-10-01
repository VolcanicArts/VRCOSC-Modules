// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Windows;
using VRCOSC.App.SDK.Providers.PiShock;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.PiShock.UI;

public partial class PhraseEditWindow
{
    private readonly PiShockModule module;
    public Phrase Phrase { get; set; }

    public ObservableCollection<ShockerGroup> ShockerGroupsItemsSource => module.GroupsSetting.Attribute;
    public PiShockMode[] ModeItemsSource => Enum.GetValues<PiShockMode>();

    public PhraseEditWindow(PiShockModule module, Phrase phrase)
    {
        InitializeComponent();

        this.module = module;
        Phrase = phrase;
        DataContext = this;

        phrase.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void AddShockerGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        Phrase.ShockerGroups.Add(new Observable<string>(string.Empty));
    }

    private void RemoveShockerGroupButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var instance = (Observable<string>)element.Tag;

        Phrase.ShockerGroups.Remove(instance);
    }
}

