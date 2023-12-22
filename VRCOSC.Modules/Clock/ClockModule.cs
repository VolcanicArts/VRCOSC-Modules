// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;
using VRCOSC.SDK;
using VRCOSC.SDK.Avatars;
using VRCOSC.SDK.Parameters;

namespace VRCOSC.Modules.Clock;

[ModuleTitle("Clock")]
[ModuleDescription("Sends a chosen timezone as hours, minutes, and seconds")]
[ModuleType(ModuleType.Generic)]
public sealed class ClockModule : AvatarModule
{
    protected override void OnLoad()
    {
        CreateToggle(ClockSetting.SmoothSecond, "Smooth Second", "If the Second Normalised parameter should be smoothed", true);
        CreateToggle(ClockSetting.SmoothMinute, "Smooth Minute", "If the Minute Normalised parameter should be smoothed", true);
        CreateToggle(ClockSetting.SmoothHour, "Smooth Hour", "If the Hour Normalised parameter should be smoothed", true);
        CreateToggle(ClockSetting.Mode, "12/24 Mode", "Off for 12 hour. On for 24 hour", false);

        RegisterParameter<bool>(ClockParameter.Period, "VRCOSC/Clock/Period", ParameterMode.Write, "Period", "False for AM. True for PM");
        RegisterParameter<bool>(ClockParameter.Mode, "VRCOSC/Clock/Mode", ParameterMode.Write, "Mode", "False for 12 hour. True for 24 hour");

        RegisterParameter<int>(ClockParameter.HourValue, "VRCOSC/Clock/Hour/Value", ParameterMode.Write, "Hour Value", "The current hour (0-11/23)");
        RegisterParameter<float>(ClockParameter.HourNormalised, "VRCOSC/Clock/Hour/Normalised", ParameterMode.Write, "Hour Normalised", "The current hour normalised between 0 and 1");
        RegisterParameter<int>(ClockParameter.MinuteValue, "VRCOSC/Clock/Minute/Value", ParameterMode.Write, "Minute Value", "The current minute (0-59)");
        RegisterParameter<float>(ClockParameter.MinuteNormalised, "VRCOSC/Clock/Minute/Normalised", ParameterMode.Write, "Minute Normalised", "The current minute normalised between 0 and 1");
        RegisterParameter<int>(ClockParameter.SecondValue, "VRCOSC/Clock/Second/Value", ParameterMode.Write, "Second Value", "The current second (0-59)");
        RegisterParameter<float>(ClockParameter.SecondNormalised, "VRCOSC/Clock/Second/Normalised", ParameterMode.Write, "Second Normalised", "The current second normalised between 0 and 1");

        RegisterParameter<float>(LegacyClockParameter.Hours, "VRCOSC/Clock/Hours", ParameterMode.Write, "Legacy: Hours", "The current hour normalised between 0 and 1");
        RegisterParameter<float>(LegacyClockParameter.Minutes, "VRCOSC/Clock/Minutes", ParameterMode.Write, "Legacy: Minutes", "The current minute normalised between 0 and 1");
        RegisterParameter<float>(LegacyClockParameter.Seconds, "VRCOSC/Clock/Seconds", ParameterMode.Write, "Legacy: Seconds", "The current second normalised between 0 and 1");

        CreateGroup("Tweaks", ClockSetting.Mode);
        CreateGroup("Smoothing", ClockSetting.SmoothHour, ClockSetting.SmoothMinute, ClockSetting.SmoothSecond);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateVariables()
    {
        var time = DateTime.Now;

        var hourValue = GetSettingValue<bool>(ClockSetting.Mode) ? time.Hour : time.Hour % 12;
        var minuteValue = GetSettingValue<bool>(ClockSetting.Mode) ? time.Minute : time.Minute % 60;
        var secondValue = GetSettingValue<bool>(ClockSetting.Mode) ? time.Second : time.Second % 60;

        var smoothedHours = GetSettingValue<bool>(ClockSetting.SmoothHour) ? getSmoothedHour(time) : time.Hour;
        var smoothedMinutes = GetSettingValue<bool>(ClockSetting.SmoothMinute) ? getSmoothedMinute(time) : time.Minute;
        var smoothedSeconds = GetSettingValue<bool>(ClockSetting.SmoothSecond) ? getSmoothedSecond(time) : time.Second;

        var hourNormalised = GetSettingValue<bool>(ClockSetting.Mode) ? smoothedHours / 24f : smoothedHours % 12f / 12f;
        var minuteNormalised = smoothedMinutes / 60f;
        var secondNormalised = smoothedSeconds / 60f;

        SendParameter(ClockParameter.Period, string.Equals(time.ToString("tt", CultureInfo.InvariantCulture), "PM", StringComparison.InvariantCultureIgnoreCase));
        SendParameter(ClockParameter.Mode, GetSettingValue<bool>(ClockSetting.Mode));

        SendParameter(ClockParameter.HourValue, hourValue);
        SendParameter(ClockParameter.MinuteValue, minuteValue);
        SendParameter(ClockParameter.SecondValue, secondValue);

        SendParameter(ClockParameter.HourNormalised, hourNormalised);
        SendParameter(ClockParameter.MinuteNormalised, minuteNormalised);
        SendParameter(ClockParameter.SecondNormalised, secondNormalised);

        SendParameter(LegacyClockParameter.Hours, hourNormalised);
        SendParameter(LegacyClockParameter.Minutes, minuteNormalised);
        SendParameter(LegacyClockParameter.Seconds, secondNormalised);
    }

    private static float getSmoothedSecond(DateTime time) => time.Second + time.Millisecond / 1000f;
    private static float getSmoothedMinute(DateTime time) => time.Minute + getSmoothedSecond(time) / 60f;
    private static float getSmoothedHour(DateTime time) => time.Hour + getSmoothedMinute(time) / 60f;

    private enum ClockParameter
    {
        Period,
        Mode,
        HourValue,
        MinuteValue,
        SecondValue,
        HourNormalised,
        MinuteNormalised,
        SecondNormalised
    }

    private enum LegacyClockParameter
    {
        Hours,
        Minutes,
        Seconds
    }

    private enum ClockSetting
    {
        Mode,
        SmoothSecond,
        SmoothMinute,
        SmoothHour
    }
}
