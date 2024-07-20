// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Windows;

namespace VRCOSC.Modules.Stopwatch;

public partial class StopwatchModuleRuntimePage
{
    private readonly StopwatchModule module;

    public StopwatchModuleRuntimePage(StopwatchModule module)
    {
        this.module = module;

        InitializeComponent();
    }

    private void Start_OnClick(object sender, RoutedEventArgs e)
    {
        module.StartStopwatch();
    }

    private void Stop_OnClick(object sender, RoutedEventArgs e)
    {
        module.PauseStopwatch();
    }

    private void Reset_OnClick(object sender, RoutedEventArgs e)
    {
        module.StopStopwatch();
    }
}
