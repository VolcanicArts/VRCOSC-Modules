// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.Stopwatch;

[ModuleTitle("Stopwatch")]
[ModuleDescription("A simple stopwatch")]
[ModuleType(ModuleType.Generic)]
public class StopwatchModule : Module
{
    private StopwatchModuleRuntimePage runtimePage;

    private TimeSpan currentTime;
    private bool shouldAddTime;

    protected override void OnPreLoad()
    {
        RegisterParameter<bool>(StopwatchParameter.Start, "VRCOSC/Stopwatch/Start", ParameterMode.Read, "Start", "Starts the stopwatch when this parameter becomes true");
        RegisterParameter<bool>(StopwatchParameter.Pause, "VRCOSC/Stopwatch/Pause", ParameterMode.Read, "Pause", "Pauses the stopwatch when this parameter becomes true");
        RegisterParameter<bool>(StopwatchParameter.Stop, "VRCOSC/Stopwatch/Stop", ParameterMode.Read, "Stop", "Stops and resets the stopwatch when this parameter becomes true");
    }

    protected override void OnPostLoad()
    {
        var currentTimeReference = CreateVariable<TimeSpan>(StopwatchVariable.CurrentTime, "Current Time")!;

        CreateState(StopwatchState.Started, "Started", "{0}", [currentTimeReference]);
        CreateState(StopwatchState.Paused, "Paused", "{0}", [currentTimeReference]);
        CreateState(StopwatchState.Stopped, "Stopped");
    }

    protected override Task<bool> OnModuleStart()
    {
        currentTime = TimeSpan.Zero;
        shouldAddTime = false;

        ChangeState(StopwatchState.Stopped);

        SetRuntimePage(runtimePage = new StopwatchModuleRuntimePage(this));

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateCurrentTime()
    {
        if (!shouldAddTime) return;

        currentTime += TimeSpan.FromMilliseconds(50);
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void updateVariables()
    {
        SetVariableValue(StopwatchVariable.CurrentTime, currentTime);
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case StopwatchParameter.Start when parameter.GetValue<bool>():
                shouldAddTime = true;
                ChangeState(StopwatchState.Started);
                break;

            case StopwatchParameter.Pause when parameter.GetValue<bool>():
                shouldAddTime = false;
                ChangeState(StopwatchState.Paused);
                break;

            case StopwatchParameter.Stop when parameter.GetValue<bool>():
                shouldAddTime = false;
                currentTime = TimeSpan.Zero;
                ChangeState(StopwatchState.Stopped);
                break;
        }
    }

    public void StartStopwatch()
    {
        shouldAddTime = true;
        ChangeState(StopwatchState.Started);
    }

    public void PauseStopwatch()
    {
        shouldAddTime = false;
        ChangeState(StopwatchState.Paused);
    }

    public void StopStopwatch()
    {
        shouldAddTime = false;
        currentTime = TimeSpan.Zero;
        ChangeState(StopwatchState.Stopped);
    }

    private enum StopwatchParameter
    {
        Start,
        Pause,
        Stop
    }

    private enum StopwatchState
    {
        Started,
        Paused,
        Stopped,
    }

    private enum StopwatchVariable
    {
        CurrentTime
    }
}
