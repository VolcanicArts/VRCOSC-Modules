// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Windows;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.PiShock.UI;

public partial class ShockerGroupEditWindow
{
    private readonly PiShockModule module;
    public ShockerGroup ShockerGroup { get; set; }

    public ObservableCollection<Shocker> ShockerItemsSource => module.ShockersSetting.Attribute;

    public ShockerGroupEditWindow(PiShockModule module, ShockerGroup shockerGroup)
    {
        InitializeComponent();

        this.module = module;
        ShockerGroup = shockerGroup;
        DataContext = this;

        shockerGroup.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    private void AddShockerButton_OnClick(object sender, RoutedEventArgs e)
    {
        ShockerGroup.Shockers.Add(new Observable<string>(string.Empty));
    }

    private void RemoveShockerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var instance = (Observable<string>)element.Tag;

        ShockerGroup.Shockers.Remove(instance);
    }
}
