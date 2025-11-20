// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.ParameterSync.UI;

public partial class ParameterSyncInstanceEditWindow : IManagedWindow
{
    public ParameterSync ParameterSync { get; }

    public ParameterSyncInstanceEditWindow(ParameterSync parameterSync)
    {
        ParameterSync = parameterSync;
        DataContext = parameterSync;
        InitializeComponent();

        parameterSync.Name.Subscribe(newName => Title = $"{newName.Pluralise()} Settings", true);
    }

    public object GetComparer() => ParameterSync;

    private void RemoveAvatar_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var avatarEntry = (Observable<string>)element.Tag;

        ParameterSync.Avatars.Remove(avatarEntry);
    }

    private void AddAvatar_OnClick(object sender, RoutedEventArgs e)
    {
        ParameterSync.Avatars.Add(new Observable<string>(string.Empty));
    }

    private void RemoveParameter_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var parameterEntry = (Observable<string>)element.Tag;

        ParameterSync.Parameters.Remove(parameterEntry);
    }

    private void AddParameter_OnClick(object sender, RoutedEventArgs e)
    {
        ParameterSync.Parameters.Add(new Observable<string>(string.Empty));
    }
}