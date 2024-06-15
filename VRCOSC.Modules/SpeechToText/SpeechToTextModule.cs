// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Globalization;
using VRCOSC.App.Audio;
using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.SpeechToText;

[ModuleTitle("Speech To Text")]
[ModuleDescription("Uses VRCOSC's speech engine for STT to the ChatBox")]
[ModuleType(ModuleType.Generic)]
public class SpeechToTextModule : ChatBoxModule, ISpeechHandler
{
    private bool listening;
    private bool playerMuted;

    protected override void OnPreLoad()
    {
        CreateDropdown(SpeechToTextSetting.ListenCriteria, "Listen Criteria", "When should STT be listening?", SpeechToTextListenCriteria.Anytime);
        CreateSlider(SpeechToTextSetting.Confidence, "Confidence", "How confident should STT be to put a result in the ChatBox? (%)", 75, 0, 100);

        RegisterParameter<bool>(SpeechToTextParameter.Listen, "VRCOSC/SpeechToText/Listen", ParameterMode.ReadWrite, "Listen", "Whether Speech To Text is currently listening");
    }

    protected override void OnPostLoad()
    {
        var textVariable = CreateVariable<string>(SpeechToTextVariable.Text, "Text")!;

        CreateEvent(SpeechToTextEvent.PartialResult, "Generating", "{0}", new[] { textVariable }, true);
        CreateEvent(SpeechToTextEvent.FinalResult, "Generated", "{0}", new[] { textVariable }, false, 20);
    }

    protected override Task<bool> OnModuleStart()
    {
        listening = true;
        reset();
        SendParameter(SpeechToTextParameter.Listen, listening);

        return Task.FromResult(true);
    }

    protected override void OnPlayerUpdate()
    {
        var isPlayerMuted = VRChatClient.Player.IsMuted.GetValueOrDefault();
        if (playerMuted == isPlayerMuted) return;

        playerMuted = isPlayerMuted;

        if (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.OnlyWhenUnMuted && playerMuted) reset();
        if (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.OnlyWhenMuted && !playerMuted) reset();
    }

    protected override void OnRegisteredParameterReceived(AvatarParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case SpeechToTextParameter.Listen:
                listening = parameter.GetValue<bool>();
                break;
        }
    }

    public void OnPartialResult(string text)
    {
        if (!shouldHandleResult()) return;

        SetVariableValue(SpeechToTextVariable.Text, formatText(text));
        TriggerEvent(SpeechToTextEvent.PartialResult);
    }

    public void OnFinalResult(SpeechResult result)
    {
        if (!shouldHandleResult()) return;
        if (result.Confidence < GetSettingValue<int>(SpeechToTextSetting.Confidence) / 100f) return;

        if (result.Success)
        {
            SetVariableValue(SpeechToTextVariable.Text, formatText(result.Text));
            TriggerEvent(SpeechToTextEvent.FinalResult);
        }
        else
        {
            reset();
        }
    }

    private bool shouldHandleResult() => listening &&
                                         (
                                             (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.OnlyWhenUnMuted && !playerMuted) ||
                                             (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.OnlyWhenMuted && playerMuted) ||
                                             (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.Anytime)
                                         );

    private void reset()
    {
        SetVariableValue(SpeechToTextVariable.Text, string.Empty);
    }

    private static string formatText(string text) => text.Length > 1 ? text[..1].ToUpper(CultureInfo.CurrentCulture) + text[1..] : text.ToUpper(CultureInfo.CurrentCulture);

    private enum SpeechToTextSetting
    {
        ListenCriteria,
        Confidence
    }

    private enum SpeechToTextParameter
    {
        Listen
    }

    private enum SpeechToTextEvent
    {
        PartialResult,
        FinalResult
    }

    private enum SpeechToTextVariable
    {
        Text
    }

    private enum SpeechToTextListenCriteria
    {
        Anytime,
        OnlyWhenMuted,
        OnlyWhenUnMuted
    }
}
