// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.ParameterSync.UI;

public partial class ParameterSyncListModuleSettingView
{
    public ParameterSyncModule Module { get; }
    private readonly ParameterSyncListModuleSetting moduleSetting;
    private WindowManager windowManager = null!;

    public ParameterSyncListModuleSettingView(ParameterSyncModule module, ParameterSyncListModuleSetting moduleSetting)
    {
        Module = module;
        this.moduleSetting = moduleSetting;

        InitializeComponent();
        Loaded += parameterSyncListModuleSettingView_OnLoaded;

        DataContext = moduleSetting;
    }

    private void parameterSyncListModuleSettingView_OnLoaded(object sender, RoutedEventArgs e)
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
        var syncInstance = (ParameterSync)element.Tag;

        var result = MessageBox.Show("Warning. This will remove the sync data. Are you sure?", "Delete Sync?", MessageBoxButton.YesNo);
        if (result != MessageBoxResult.Yes) return;

        moduleSetting.Remove(syncInstance);
    }

    private void EditInstanceButton_OnClick(object sender, RoutedEventArgs e)
    {
        var element = (FrameworkElement)sender;
        var syncInstance = (ParameterSync)element.Tag;

        var editWindow = new ParameterSyncInstanceEditWindow(syncInstance);

        windowManager.TrySpawnChild(editWindow);
    }
}