// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;
using VRCOSC.SDK;
using VRCOSC.SDK.Avatars;
using VRCOSC.SDK.Parameters;

namespace VRCOSC.Modules.Time;

[ModuleTitle("Time")]
[ModuleDescription("Sends your current time to VRChat")]
[ModuleType(ModuleType.Generic)]
public sealed class TimeModule : AvatarModule
{
    protected override void OnLoad()
    {
        CreateToggle(TimeSetting.SmoothSecond, "Smooth Second", "If the Second Normalised parameter should be smoothed", true);
        CreateToggle(TimeSetting.SmoothMinute, "Smooth Minute", "If the Minute Normalised parameter should be smoothed", true);
        CreateToggle(TimeSetting.SmoothHour, "Smooth Hour", "If the Hour Normalised parameter should be smoothed", true);
        CreateToggle(TimeSetting.Mode, "12/24 Mode", "Off for 12 hour. On for 24 hour", false);

        RegisterParameter<bool>(TimeParameter.Period, "VRCOSC/Time/Period", ParameterMode.Write, "Period", "False for AM. True for PM");
        RegisterParameter<bool>(TimeParameter.Mode, "VRCOSC/Time/Mode", ParameterMode.Write, "Mode", "False for 12 hour. True for 24 hour");

        RegisterParameter<int>(TimeParameter.HourValue, "VRCOSC/Time/Hour/Value", ParameterMode.Write, "Hour Value", "The current hour (0-11/23)");
        RegisterParameter<float>(TimeParameter.HourNormalised, "VRCOSC/Time/Hour/Normalised", ParameterMode.Write, "Hour Normalised", "The current hour normalised between 0 and 1");
        RegisterParameter<int>(TimeParameter.MinuteValue, "VRCOSC/Time/Minute/Value", ParameterMode.Write, "Minute Value", "The current minute (0-59)");
        RegisterParameter<float>(TimeParameter.MinuteNormalised, "VRCOSC/Time/Minute/Normalised", ParameterMode.Write, "Minute Normalised", "The current minute normalised between 0 and 1");
        RegisterParameter<int>(TimeParameter.SecondValue, "VRCOSC/Time/Second/Value", ParameterMode.Write, "Second Value", "The current second (0-59)");
        RegisterParameter<float>(TimeParameter.SecondNormalised, "VRCOSC/Time/Second/Normalised", ParameterMode.Write, "Second Normalised", "The current second normalised between 0 and 1");

        RegisterParameter<float>(TimeParameter.LegacyHours, "VRCOSC/Clock/Hours", ParameterMode.Write, "Hours", "The current hour normalised between 0 and 1", true);
        RegisterParameter<float>(TimeParameter.LegacyMinutes, "VRCOSC/Clock/Minutes", ParameterMode.Write, "Minutes", "The current minute normalised between 0 and 1", true);
        RegisterParameter<float>(TimeParameter.LegacySeconds, "VRCOSC/Clock/Seconds", ParameterMode.Write, "Seconds", "The current second normalised between 0 and 1", true);
        RegisterParameter<bool>(TimeParameter.LegacyPeriod, "VRCOSC/Clock/Period", ParameterMode.Write, "Period", "False for AM. True for PM", true);

        CreateGroup("Tweaks", TimeSetting.Mode);
        CreateGroup("Smoothing", TimeSetting.SmoothHour, TimeSetting.SmoothMinute, TimeSetting.SmoothSecond);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateVariables()
    {
        var time = DateTime.Now;

        var hourValue = GetSettingValue<bool>(TimeSetting.Mode) ? time.Hour : time.Hour % 12;
        var minuteValue = GetSettingValue<bool>(TimeSetting.Mode) ? time.Minute : time.Minute % 60;
        var secondValue = GetSettingValue<bool>(TimeSetting.Mode) ? time.Second : time.Second % 60;

        var smoothedHours = GetSettingValue<bool>(TimeSetting.SmoothHour) ? getSmoothedHour(time) : time.Hour;
        var smoothedMinutes = GetSettingValue<bool>(TimeSetting.SmoothMinute) ? getSmoothedMinute(time) : time.Minute;
        var smoothedSeconds = GetSettingValue<bool>(TimeSetting.SmoothSecond) ? getSmoothedSecond(time) : time.Second;

        var hourNormalised = GetSettingValue<bool>(TimeSetting.Mode) ? smoothedHours / 24f : smoothedHours % 12f / 12f;
        var minuteNormalised = smoothedMinutes / 60f;
        var secondNormalised = smoothedSeconds / 60f;

        SendParameter(TimeParameter.Period, string.Equals(time.ToString("tt", CultureInfo.InvariantCulture), "PM", StringComparison.InvariantCultureIgnoreCase));
        SendParameter(TimeParameter.Mode, GetSettingValue<bool>(TimeSetting.Mode));

        SendParameter(TimeParameter.HourValue, hourValue);
        SendParameter(TimeParameter.MinuteValue, minuteValue);
        SendParameter(TimeParameter.SecondValue, secondValue);

        SendParameter(TimeParameter.HourNormalised, hourNormalised);
        SendParameter(TimeParameter.MinuteNormalised, minuteNormalised);
        SendParameter(TimeParameter.SecondNormalised, secondNormalised);

        SendParameter(TimeParameter.LegacyHours, hourNormalised);
        SendParameter(TimeParameter.LegacyMinutes, minuteNormalised);
        SendParameter(TimeParameter.LegacySeconds, secondNormalised);
        SendParameter(TimeParameter.LegacyPeriod, string.Equals(time.ToString("tt", CultureInfo.InvariantCulture), "PM", StringComparison.InvariantCultureIgnoreCase));
    }

    private static float getSmoothedSecond(DateTime time) => time.Second + time.Millisecond / 1000f;
    private static float getSmoothedMinute(DateTime time) => time.Minute + getSmoothedSecond(time) / 60f;
    private static float getSmoothedHour(DateTime time) => time.Hour + getSmoothedMinute(time) / 60f;

    private enum TimeParameter
    {
        Period,
        Mode,
        HourValue,
        MinuteValue,
        SecondValue,
        HourNormalised,
        MinuteNormalised,
        SecondNormalised,
        LegacyHours,
        LegacyMinutes,
        LegacySeconds,
        LegacyPeriod
    }

    private enum TimeSetting
    {
        Mode,
        SmoothSecond,
        SmoothMinute,
        SmoothHour
    }
}
