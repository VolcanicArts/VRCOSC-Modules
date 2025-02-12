// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.Datetime;

[ModuleTitle("DateTime")]
[ModuleDescription("Sends your current date and time to VRChat")]
[ModuleType(ModuleType.Generic)]
[ModulePrefab("Official Prefabs", "https://vrcosc.com/docs/downloads#prefabs")]
public sealed class DateTimeModule : Module
{
    protected override void OnPreLoad()
    {
        List<Timezone> timezoneList = [new("Local", string.Empty)];
        timezoneList.AddRange(TimeZoneInfo.GetSystemTimeZones().Select(info => new Timezone(info.DisplayName, info.Id)));

        CreateToggle(DateTimeSetting.SmoothSecond, "Smooth Second", "If the Second Normalised parameter should be smoothed", true);
        CreateToggle(DateTimeSetting.SmoothMinute, "Smooth Minute", "If the Minute Normalised parameter should be smoothed", true);
        CreateToggle(DateTimeSetting.SmoothHour, "Smooth Hour", "If the Hour Normalised parameter should be smoothed", true);
        CreateToggle(DateTimeSetting.Mode, "12/24 Mode", "Off for 12 hour. On for 24 hour", false);
        CreateDropdown(DateTimeSetting.Timezone, "Timezone", "The timezone that is set for the parameters", timezoneList, timezoneList[0], nameof(Timezone.Title), nameof(Timezone.Id));

        RegisterParameter<bool>(DateTimeParameter.Period, "VRCOSC/Time/Period", ParameterMode.Write, "Period", "False for AM. True for PM");
        RegisterParameter<bool>(DateTimeParameter.Mode, "VRCOSC/Time/Mode", ParameterMode.Write, "Mode", "False for 12 hour. True for 24 hour");

        RegisterParameter<int>(DateTimeParameter.HourValue, "VRCOSC/Time/Hour/Value", ParameterMode.Write, "Hour Value", "The current hour (0-11/23)");
        RegisterParameter<float>(DateTimeParameter.HourNormalised, "VRCOSC/Time/Hour/Normalised", ParameterMode.Write, "Hour Normalised", "The current hour normalised between 0 and 1");
        RegisterParameter<int>(DateTimeParameter.MinuteValue, "VRCOSC/Time/Minute/Value", ParameterMode.Write, "Minute Value", "The current minute (0-59)");
        RegisterParameter<float>(DateTimeParameter.MinuteNormalised, "VRCOSC/Time/Minute/Normalised", ParameterMode.Write, "Minute Normalised", "The current minute normalised between 0 and 1");
        RegisterParameter<int>(DateTimeParameter.SecondValue, "VRCOSC/Time/Second/Value", ParameterMode.Write, "Second Value", "The current second (0-59)");
        RegisterParameter<float>(DateTimeParameter.SecondNormalised, "VRCOSC/Time/Second/Normalised", ParameterMode.Write, "Second Normalised", "The current second normalised between 0 and 1");

        RegisterParameter<int>(DateTimeParameter.Day, "VRCOSC/Date/Day", ParameterMode.Write, "Day", "The current day (1-31)");
        RegisterParameter<int>(DateTimeParameter.Month, "VRCOSC/Date/Month", ParameterMode.Write, "Month", "The current month (1-12)");
        RegisterParameter<int>(DateTimeParameter.Year, "VRCOSC/Date/Year", ParameterMode.Write, "Year", "The current year (2024 = 24)");

        RegisterParameter<int>(DateTimeParameter.Weekday, "VRCOSC/Date/Weekday", ParameterMode.Write, "Weekday", "The current weekday (1-7 Mon-Sun)");

        RegisterParameter<float>(DateTimeParameter.LegacyHours, "VRCOSC/Clock/Hours", ParameterMode.Write, "Hours", "The current hour normalised between 0 and 1", true);
        RegisterParameter<float>(DateTimeParameter.LegacyMinutes, "VRCOSC/Clock/Minutes", ParameterMode.Write, "Minutes", "The current minute normalised between 0 and 1", true);
        RegisterParameter<float>(DateTimeParameter.LegacySeconds, "VRCOSC/Clock/Seconds", ParameterMode.Write, "Seconds", "The current second normalised between 0 and 1", true);
        RegisterParameter<bool>(DateTimeParameter.LegacyPeriod, "VRCOSC/Clock/Period", ParameterMode.Write, "Period", "False for AM. True for PM", true);

        CreateGroup("Tweaks", DateTimeSetting.Mode, DateTimeSetting.Timezone);
        CreateGroup("Smoothing", DateTimeSetting.SmoothSecond, DateTimeSetting.SmoothMinute, DateTimeSetting.SmoothHour);
    }

    protected override void OnPostLoad()
    {
        var timeReference = CreateVariable<DateTimeOffset>(DateTimeVariable.Now, "Now")!;

        CreateState(DateTimeState.Default, "Default", "{0}", [timeReference]);
    }

    protected override Task<bool> OnModuleStart()
    {
        ChangeState(DateTimeState.Default);

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void chatBoxUpdate()
    {
        SetVariableValue(DateTimeVariable.Now, DateTimeOffset.UtcNow);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 100)]
    private void updateVariables()
    {
        var timezoneId = GetSettingValue<Timezone>(DateTimeSetting.Timezone).Id;
        var time = string.IsNullOrEmpty(timezoneId) ? DateTimeOffset.Now : TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timezoneId));

        var hourValue = GetSettingValue<bool>(DateTimeSetting.Mode) ? time.Hour : time.Hour % 12;
        var minuteValue = time.Minute;
        var secondValue = time.Second;

        var smoothedHours = GetSettingValue<bool>(DateTimeSetting.SmoothHour) ? getSmoothedHour(time) : time.Hour;
        var smoothedMinutes = GetSettingValue<bool>(DateTimeSetting.SmoothMinute) ? getSmoothedMinute(time) : time.Minute;
        var smoothedSeconds = GetSettingValue<bool>(DateTimeSetting.SmoothSecond) ? getSmoothedSecond(time) : time.Second;

        var hourNormalised = GetSettingValue<bool>(DateTimeSetting.Mode) ? smoothedHours / 24f : smoothedHours % 12f / 12f;
        var minuteNormalised = smoothedMinutes / 60f;
        var secondNormalised = smoothedSeconds / 60f;

        SendParameter(DateTimeParameter.Period, string.Equals(time.ToString("tt", CultureInfo.InvariantCulture), "PM", StringComparison.InvariantCultureIgnoreCase));
        SendParameter(DateTimeParameter.Mode, GetSettingValue<bool>(DateTimeSetting.Mode));

        SendParameter(DateTimeParameter.HourValue, hourValue);
        SendParameter(DateTimeParameter.MinuteValue, minuteValue);
        SendParameter(DateTimeParameter.SecondValue, secondValue);

        SendParameter(DateTimeParameter.HourNormalised, hourNormalised);
        SendParameter(DateTimeParameter.MinuteNormalised, minuteNormalised);
        SendParameter(DateTimeParameter.SecondNormalised, secondNormalised);

        SendParameter(DateTimeParameter.LegacyHours, hourNormalised);
        SendParameter(DateTimeParameter.LegacyMinutes, minuteNormalised);
        SendParameter(DateTimeParameter.LegacySeconds, secondNormalised);
        SendParameter(DateTimeParameter.LegacyPeriod, string.Equals(time.ToString("tt", CultureInfo.InvariantCulture), "PM", StringComparison.InvariantCultureIgnoreCase));

        SendParameter(DateTimeParameter.Day, time.Day);
        SendParameter(DateTimeParameter.Month, time.Month);
        SendParameter(DateTimeParameter.Year, time.Year % 100);

        SendParameter(DateTimeParameter.Weekday, ((int)time.DayOfWeek + 6) % 7 + 1);
    }

    private static float getSmoothedSecond(DateTimeOffset time) => time.Second + time.Millisecond / 1000f;
    private static float getSmoothedMinute(DateTimeOffset time) => time.Minute + getSmoothedSecond(time) / 60f;
    private static float getSmoothedHour(DateTimeOffset time) => time.Hour + getSmoothedMinute(time) / 60f;

    private enum DateTimeParameter
    {
        Period,
        Mode,
        HourValue,
        MinuteValue,
        SecondValue,
        HourNormalised,
        MinuteNormalised,
        SecondNormalised,
        Day,
        Month,
        Year,
        Weekday,
        LegacyHours,
        LegacyMinutes,
        LegacySeconds,
        LegacyPeriod
    }

    private enum DateTimeSetting
    {
        Mode,
        Timezone,
        SmoothSecond,
        SmoothMinute,
        SmoothHour
    }

    private enum DateTimeState
    {
        Default
    }

    private enum DateTimeVariable
    {
        Now
    }

    public record Timezone(string Title, string Id);
}