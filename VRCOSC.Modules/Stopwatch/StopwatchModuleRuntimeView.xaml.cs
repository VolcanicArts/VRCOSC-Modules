using System.Windows;

namespace VRCOSC.Modules.Stopwatch;

public partial class StopwatchModuleRuntimeView
{
    private readonly StopwatchModule module;

    public StopwatchModuleRuntimeView(StopwatchModule module)
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
