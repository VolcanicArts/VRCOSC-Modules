// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;

namespace VRCOSC.Modules.Timer;

[ModuleTitle("Timer")]
[ModuleDescription("Counts to a specified time to be put in the ChatBox")]
[ModuleType(ModuleType.Generic)]
public class TimerModule : ChatBoxModule
{
    protected override void OnPreLoad()
    {
        CreateDateTime(TimerSetting.DateTime, "Time", "The date/time to count to.\nWorks with future and past dates/times", DateTime.Today);

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
        var dateTime = GetSettingValue<DateTime>(TimerSetting.DateTime);

        var diff = dateTime - DateTime.Now;
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
