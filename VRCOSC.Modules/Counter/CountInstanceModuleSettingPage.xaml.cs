// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;

namespace VRCOSC.Modules.Counter;

public partial class CountInstanceModuleSettingPage
{
    private readonly CountInstanceModuleSetting moduleSetting;

    private CountInstanceWindow? countInstanceWindow;

    public CountInstanceModuleSettingPage(CountInstanceModuleSetting moduleSetting)
    {
        this.moduleSetting = moduleSetting;
        InitializeComponent();
    }

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        if (countInstanceWindow is not null)
        {
            countInstanceWindow.Close();
            countInstanceWindow = null;
        }

        countInstanceWindow ??= new CountInstanceWindow(moduleSetting);
        countInstanceWindow.Closed += (_, _) => countInstanceWindow = null;
        countInstanceWindow.Show();
    }
}
