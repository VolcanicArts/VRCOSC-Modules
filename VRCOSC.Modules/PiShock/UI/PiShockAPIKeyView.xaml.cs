// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.PiShock.UI;

public partial class PiShockAPIKeyView
{
    private static readonly Uri pishock_account_url = new("https://login.pishock.com/account");

    public PiShockAPIKeyView(PiShockModule _, StringModuleSetting moduleSetting)
    {
        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void GenerateAPIKey_OnClick(object sender, RoutedEventArgs e)
    {
        pishock_account_url.OpenExternally();
    }
}