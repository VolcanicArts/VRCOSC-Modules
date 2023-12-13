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
[ModulePrefab("VRCOSC-Watch", "https://github.com/VolcanicArts/VRCOSC/releases/download/latest/VRCOSC-Watch.unitypackage")]
public sealed class ClockModule : AvatarModule
{
    protected override void OnLoad()
    {
        CreateToggle(ClockSetting.SmoothSecond, "Smooth Second", "If the seconds value should be smoothed", false);
        CreateToggle(ClockSetting.SmoothMinute, "Smooth Minute", "If the minutes value should be smoothed", true);
        CreateToggle(ClockSetting.SmoothHour, "Smooth Hour", "If the hours value should be smoothed", true);
        CreateToggle(ClockSetting.Mode, "12/24 Mode", "Off for 12 hour. On for 24 hour", false);
        CreateDropdown(ClockSetting.Timezone, "Timezone", "The timezone the clock should follow", ClockTimeZone.Local);

        RegisterParameter<bool>(ClockParameter.Period, "VRCOSC/Clock/Period", ParameterMode.Write, "Period", "False for AM. True for PM");
        RegisterParameter<bool>(ClockParameter.Mode, "VRCOSC/Clock/Mode", ParameterMode.Write, "Mode", "False for 12 hour. True for 24 hour");
        RegisterParameter<int>(ClockParameter.HoursI, "VRCOSC/Clock/HoursI", ParameterMode.Write, "Hours Int", "The current hour (0-11/23)");
        RegisterParameter<int>(ClockParameter.MinutesI, "VRCOSC/Clock/MinutesI", ParameterMode.Write, "Minutes Int", "The current minute (0-11/23)");
        RegisterParameter<int>(ClockParameter.SecondsI, "VRCOSC/Clock/SecondsI", ParameterMode.Write, "Seconds Int", "The current second (0-11/23)");
        RegisterParameter<float>(ClockParameter.HoursF, "VRCOSC/Clock/HoursF", ParameterMode.Write, "Hours Float", "The current hour normalised between 0 and 1");
        RegisterParameter<float>(ClockParameter.MinutesF, "VRCOSC/Clock/MinutesF", ParameterMode.Write, "Minutes Float", "The current minute normalised between 0 and 1");
        RegisterParameter<float>(ClockParameter.SecondsF, "VRCOSC/Clock/SecondsF", ParameterMode.Write, "Seconds Float", "The current second normalised between 0 and 1");

        CreateGroup("Tweaks", ClockSetting.Mode, ClockSetting.Timezone);
        CreateGroup("Smoothing", ClockSetting.SmoothHour, ClockSetting.SmoothMinute, ClockSetting.SmoothSecond);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void updateVariables()
    {
        var time = DateTime.Now;

        var hoursInt = GetSettingValue<bool>(ClockSetting.Mode) ? time.Hour : time.Hour % 12;
        var minutesInt = GetSettingValue<bool>(ClockSetting.Mode) ? time.Minute : time.Minute % 60;
        var secondsInt = GetSettingValue<bool>(ClockSetting.Mode) ? time.Second : time.Second % 60;

        var hours = GetSettingValue<bool>(ClockSetting.SmoothHour) ? getSmoothedHours(time) : time.Hour;
        var minutes = GetSettingValue<bool>(ClockSetting.SmoothMinute) ? getSmoothedMinutes(time) : time.Minute;
        var seconds = GetSettingValue<bool>(ClockSetting.SmoothSecond) ? getSmoothedSeconds(time) : time.Second;

        var hourNormalised = GetSettingValue<bool>(ClockSetting.Mode) ? hours / 24f : hours % 12f / 12f;
        var minuteNormalised = minutes / 60f;
        var secondNormalised = seconds / 60f;

        SendParameter(ClockParameter.Period, string.Equals(time.ToString("tt", CultureInfo.InvariantCulture), "PM", StringComparison.InvariantCultureIgnoreCase));
        SendParameter(ClockParameter.Mode, GetSettingValue<bool>(ClockSetting.Mode));

        SendParameter(ClockParameter.HoursI, hoursInt);
        SendParameter(ClockParameter.MinutesI, minutesInt);
        SendParameter(ClockParameter.SecondsI, secondsInt);

        SendParameter(ClockParameter.HoursF, hourNormalised);
        SendParameter(ClockParameter.MinutesF, minuteNormalised);
        SendParameter(ClockParameter.SecondsF, secondNormalised);
    }

    private static float getSmoothedSeconds(DateTime time) => time.Second + time.Millisecond / 1000f;
    private static float getSmoothedMinutes(DateTime time) => time.Minute + getSmoothedSeconds(time) / 60f;
    private static float getSmoothedHours(DateTime time) => time.Hour + getSmoothedMinutes(time) / 60f;

    private enum ClockParameter
    {
        Period,
        Mode,
        HoursI,
        MinutesI,
        SecondsI,
        HoursF,
        MinutesF,
        SecondsF
    }

    private enum ClockSetting
    {
        Timezone,
        Mode,
        SmoothSecond,
        SmoothMinute,
        SmoothHour
    }

    private enum ClockTimeZone
    {
        Local,
        UTC,
        GMT,
        EST,
        CST,
        MNT,
        PST
    }
}
