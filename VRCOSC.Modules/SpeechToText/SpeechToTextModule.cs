// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.SpeechToText;

[ModuleTitle("Speech To Text")]
[ModuleDescription("Uses VRCOSC's speech engine for transcribing your voice to the ChatBox")]
[ModuleType(ModuleType.Generic)]
public class SpeechToTextModule : Module, ISpeechHandler
{
    private bool listening;
    private bool playerMuted;

    protected override void OnPreLoad()
    {
        CreateDropdown(SpeechToTextSetting.ListenCriteria, "Listen Criteria", "When should STT be listening?", SpeechToTextListenCriteria.Anytime);

        RegisterParameter<bool>(SpeechToTextParameter.Listen, "VRCOSC/SpeechToText/Listen", ParameterMode.ReadWrite, "Listen", "Whether Speech To Text is currently listening");
    }

    protected override void OnPostLoad()
    {
        var textVariable = CreateVariable<string>(SpeechToTextVariable.Text, "Text")!;

        CreateState(SpeechToTextState.Default, "Default", string.Empty);
        CreateEvent(SpeechToTextEvent.Result, "On Speech Result", "{0}", [textVariable], true, 10f);
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
        var isPlayerMuted = GetPlayer().IsMuted;
        if (playerMuted == isPlayerMuted) return;

        playerMuted = isPlayerMuted;

        if (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.OnlyWhenUnMuted && playerMuted) reset();
        if (GetSettingValue<SpeechToTextListenCriteria>(SpeechToTextSetting.ListenCriteria) == SpeechToTextListenCriteria.OnlyWhenMuted && !playerMuted) reset();
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case SpeechToTextParameter.Listen:
                listening = parameter.GetValue<bool>();
                break;
        }
    }

    public void OnPartialSpeechResult(string text)
    {
        if (!shouldHandleResult()) return;

        SetVariableValue(SpeechToTextVariable.Text, text);
        TriggerEvent(SpeechToTextEvent.Result);
    }

    public void OnFinalSpeechResult(string text)
    {
        if (!shouldHandleResult()) return;

        SetVariableValue(SpeechToTextVariable.Text, text);
        TriggerEvent(SpeechToTextEvent.Result);
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

    private enum SpeechToTextSetting
    {
        ListenCriteria
    }

    private enum SpeechToTextParameter
    {
        Listen
    }

    private enum SpeechToTextState
    {
        Default,
    }

    private enum SpeechToTextEvent
    {
        Result
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