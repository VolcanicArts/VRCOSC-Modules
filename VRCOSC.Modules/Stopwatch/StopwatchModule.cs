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
    private TimeSpan currentTime;
    private StopwatchState currentState;

    protected override void OnPreLoad()
    {
        CreateToggle(StopwatchSetting.SmoothSecond, "Smooth Second", "If the Second Normalised parameter should be smoothed", true);
        CreateToggle(StopwatchSetting.SmoothMinute, "Smooth Minute", "If the Minute Normalised parameter should be smoothed", true);

        RegisterParameter<bool>(StopwatchParameter.Start, "VRCOSC/Stopwatch/Start", ParameterMode.Read, "Start", "Starts the stopwatch when this parameter becomes true");
        RegisterParameter<bool>(StopwatchParameter.Pause, "VRCOSC/Stopwatch/Pause", ParameterMode.Read, "Pause", "Pauses the stopwatch when this parameter becomes true");
        RegisterParameter<bool>(StopwatchParameter.Stop, "VRCOSC/Stopwatch/Stop", ParameterMode.Read, "Stop", "Stops and resets the stopwatch when this parameter becomes true");

        RegisterParameter<int>(StopwatchParameter.State, "VRCOSC/Stopwatch/State", ParameterMode.Write, "State", "The state of the stopwatch\n0 - Stopped\n1 - Paused\n2 - Started");

        RegisterParameter<int>(StopwatchParameter.MinuteValue, "VRCOSC/Stopwatch/Minute/Value", ParameterMode.Write, "Minute Value", "The minute value of the stopwatch (0-59)");

        RegisterParameter<float>(StopwatchParameter.MinuteNormalised, "VRCOSC/Stopwatch/Minute/Normalised", ParameterMode.Write, "Minute Normalised",
            "The minute value of the stopwatch normalised between 0 and 1");
        RegisterParameter<int>(StopwatchParameter.SecondValue, "VRCOSC/Stopwatch/Second/Value", ParameterMode.Write, "Second Value", "The second value of the stopwatch (0-59)");

        RegisterParameter<float>(StopwatchParameter.SecondNormalised, "VRCOSC/Stopwatch/Second/Normalised", ParameterMode.Write, "Second Normalised",
            "The second value of the stopwatch normalised between 0 and 1");

        SetRuntimeView(typeof(StopwatchModuleRuntimeView));
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
        changeState(StopwatchState.Stopped);
        return Task.FromResult(true);
    }

    private void changeState(StopwatchState state)
    {
        ChangeState(state);
        SendParameter(StopwatchParameter.State, (int)state);
        currentState = state;

        if (state == StopwatchState.Stopped)
        {
            currentTime = TimeSpan.Zero;

            SendParameter(StopwatchParameter.MinuteValue, 0);
            SendParameter(StopwatchParameter.SecondValue, 0);

            SendParameter(StopwatchParameter.MinuteNormalised, 0f);
            SendParameter(StopwatchParameter.SecondNormalised, 0f);
        }
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateCurrentTime()
    {
        if (currentState != StopwatchState.Started) return;

        currentTime += TimeSpan.FromMilliseconds(50);

        var minutesValue = GetSettingValue<bool>(StopwatchSetting.SmoothMinute) ? getSmoothedMinute(currentTime) : currentTime.Minutes;
        var secondsValue = GetSettingValue<bool>(StopwatchSetting.SmoothSecond) ? getSmoothedSecond(currentTime) : currentTime.Seconds;

        SendParameter(StopwatchParameter.MinuteValue, currentTime.Minutes);
        SendParameter(StopwatchParameter.SecondValue, currentTime.Seconds);

        SendParameter(StopwatchParameter.MinuteNormalised, minutesValue / 60f);
        SendParameter(StopwatchParameter.SecondNormalised, secondsValue / 60f);
    }

    private static float getSmoothedSecond(TimeSpan time) => time.Seconds + time.Milliseconds / 1000f;
    private static float getSmoothedMinute(TimeSpan time) => time.Minutes + getSmoothedSecond(time) / 60f;

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
                changeState(StopwatchState.Started);
                break;

            case StopwatchParameter.Pause when parameter.GetValue<bool>():
                changeState(StopwatchState.Paused);
                break;

            case StopwatchParameter.Stop when parameter.GetValue<bool>():
                changeState(StopwatchState.Stopped);
                break;
        }
    }

    public void StartStopwatch()
    {
        changeState(StopwatchState.Started);
    }

    public void PauseStopwatch()
    {
        changeState(StopwatchState.Paused);
    }

    public void StopStopwatch()
    {
        changeState(StopwatchState.Stopped);
    }

    private enum StopwatchSetting
    {
        SmoothSecond,
        SmoothMinute
    }

    private enum StopwatchParameter
    {
        Start,
        Pause,
        Stop,
        State,
        MinuteValue,
        SecondValue,
        MinuteNormalised,
        SecondNormalised
    }

    private enum StopwatchState
    {
        Stopped = 0,
        Paused = 1,
        Started = 2
    }

    private enum StopwatchVariable
    {
        CurrentTime
    }
}
