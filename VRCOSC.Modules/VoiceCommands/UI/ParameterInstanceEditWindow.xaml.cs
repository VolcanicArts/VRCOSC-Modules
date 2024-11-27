// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;
using VRCOSC.App.UI.Core;

namespace VRCOSC.Modules.VoiceCommands.UI;

public partial class ParameterInstanceEditWindow : IManagedWindow
{
    public Parameter Instance { get; set; }

    public ParameterType[] ParameterTypeSource => Enum.GetValues<ParameterType>();
    public BoolMode[] BoolModeSource => Enum.GetValues<BoolMode>();

    public ParameterInstanceEditWindow(Parameter instance)
    {
        Instance = instance;

        InitializeComponent();

        DataContext = this;

        instance.ParameterType.Subscribe(newType =>
        {
            switch (newType)
            {
                case ParameterType.Bool:
                    BoolModeContainer.Visibility = Visibility.Visible;
                    IntContainer.Visibility = Visibility.Collapsed;
                    FloatContainer.Visibility = Visibility.Collapsed;
                    break;

                case ParameterType.Int:
                    BoolModeContainer.Visibility = Visibility.Collapsed;
                    IntContainer.Visibility = Visibility.Visible;
                    FloatContainer.Visibility = Visibility.Collapsed;
                    break;

                case ParameterType.Float:
                    BoolModeContainer.Visibility = Visibility.Collapsed;
                    IntContainer.Visibility = Visibility.Collapsed;
                    FloatContainer.Visibility = Visibility.Visible;
                    break;
            }
        }, true);
    }

    public object GetComparer() => Instance;
}