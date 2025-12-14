// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Twitch.UI;

public partial class TwitchAccessTokenView
{
    private readonly TwitchModule module;

    public TwitchAccessTokenView(TwitchModule module, StringModuleSetting moduleSetting)
    {
        this.module = module;
        InitializeComponent();

        DataContext = moduleSetting;

        Loaded += (_, _) =>
        {
            TwitchAccessTokenListener.Start();
        };

        Unloaded += (_, _) =>
        {
            TwitchAccessTokenListener.Stop();
        };
    }

    private void ObtainAccessTokenButton_OnClick(object sender, RoutedEventArgs e)
    {
        new Uri(getUrl()).OpenExternally();
    }

    private string getUrl() => "https://id.twitch.tv/oauth2/authorize?"
                               + "response_type=token"
                               + $"&client_id={TwitchModule.CLIENT_ID}"
                               + "&redirect_uri=http://localhost:5555"
                               + "&scope=user:bot channel:bot user:read:chat moderator:read:followers channel:read:subscriptions channel:read:redemptions bits:read channel:read:goals channel:read:hype_train"
                               + $"&state={Guid.NewGuid().ToString().Replace("-", "")}"
                               + "&force_verify=true";
}