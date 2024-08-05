// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Pulsoid;

public partial class PulsoidAccessTokenView
{
    private static readonly Uri pulsoid_access_token_url = new("https://pulsoid.net/oauth2/authorize?response_type=token&client_id=a31caa68-b6ac-4680-976a-9761b915a1e3&redirect_uri=&scope=data:heart_rate:read&state=a52beaeb-c491-4cd3-b915-16fed71e17a8&response_mode=web_page");

    public PulsoidAccessTokenView(StringModuleSetting moduleSetting)
    {
        InitializeComponent();

        DataContext = moduleSetting;
    }

    private void ObtainAccessTokenButton_OnClick(object sender, RoutedEventArgs e)
    {
        pulsoid_access_token_url.OpenExternally();
    }
}
