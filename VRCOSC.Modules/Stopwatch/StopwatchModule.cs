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
        RegisterParameter<bool>(StopwatchParameter.Stop, "VRCOSC/Stopwatch/Stop", ParameterMode.Read, "Stop", "Stops the stopwatch when this parameter becomes true");
        RegisterParameter<bool>(StopwatchParameter.Reset, "VRCOSC/Stopwatch/Reset", ParameterMode.Read, "Reset", "Resets the stopwatch when this parameter becomes true");
    }

    protected override void OnPostLoad()
    {
        var currentTimeReference = CreateVariable<TimeSpan>(StopwatchVariable.CurrentTime, "Current Time")!;

        CreateState(StopwatchState.Default, "Default", "{0}", [currentTimeReference]);
    }

    protected override Task<bool> OnModuleStart()
    {
        currentTime = TimeSpan.Zero;
        shouldAddTime = false;

        ChangeState(StopwatchState.Default);

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
                break;

            case StopwatchParameter.Stop when parameter.GetValue<bool>():
                shouldAddTime = false;
                break;

            case StopwatchParameter.Reset when parameter.GetValue<bool>():
                currentTime = TimeSpan.Zero;
                break;
        }
    }

    public void StartStopwatch()
    {
        shouldAddTime = true;
    }

    public void StopStopwatch()
    {
        shouldAddTime = false;
    }

    public void ResetStopwatch()
    {
        currentTime = TimeSpan.Zero;
    }

    private enum StopwatchParameter
    {
        Start,
        Stop,
        Reset
    }

    private enum StopwatchState
    {
        Default
    }

    private enum StopwatchVariable
    {
        CurrentTime
    }
}
