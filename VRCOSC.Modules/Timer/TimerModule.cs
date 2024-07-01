// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;

namespace VRCOSC.Modules.Timer;

[ModuleTitle("Timer")]
[ModuleDescription("Counts to a specified time to be put in the ChatBox")]
[ModuleType(ModuleType.Generic)]
public class TimerModule : Module
{
    protected override void OnPreLoad()
    {
        CreateDateTime(TimerSetting.DateTime, "Date/Time", "The date/time to count to.\nWorks with future and past dates/times", DateTimeOffset.Now);

        var dateTimeReference = CreateVariable<TimeSpan>(TimerVariable.TimeSpan, "Time Span")!;
        CreateState(TimerState.Default, "Default", "{0}", new[] { dateTimeReference });
    }

    protected override Task<bool> OnModuleStart()
    {
        ChangeState(TimerState.Default);

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void onChatBoxUpdate()
    {
        var dateTime = GetSettingValue<DateTimeOffset>(TimerSetting.DateTime);
        var diff = dateTime - DateTimeOffset.Now;
        SetVariableValue(TimerVariable.TimeSpan, diff);
    }

    private enum TimerSetting
    {
        DateTime
    }

    private enum TimerState
    {
        Default
    }

    private enum TimerVariable
    {
        TimeSpan
    }
}
