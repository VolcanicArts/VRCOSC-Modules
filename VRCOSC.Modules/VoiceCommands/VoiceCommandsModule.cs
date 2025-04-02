// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.VoiceCommands;

[ModuleTitle("Voice Commands")]
[ModuleDescription("Uses VRCOSC's speech engine to change avatar parameters using your voice")]
[ModuleType(ModuleType.Generic)]
public class VoiceCommandsModule : Module, ISpeechHandler
{
    protected override void OnPreLoad()
    {
        CreateCustomSetting(VoiceCommandsSetting.Phrases, new PhraseModuleSetting());
        CreateToggle(VoiceCommandsSetting.SpeechLog, "Speech Log", "Log all the final speech results to debug what the speech engine heard", false);

        CreateGroup("Configuration", string.Empty, VoiceCommandsSetting.Phrases);
        CreateGroup("Debug", string.Empty, VoiceCommandsSetting.SpeechLog);
    }

    public void OnPartialSpeechResult(string text)
    {
    }

    public async void OnFinalSpeechResult(string text)
    {
        if (GetSettingValue<bool>(VoiceCommandsSetting.SpeechLog))
        {
            Log(text);
        }

        var phrases = GetSettingValue<List<Phrase>>(VoiceCommandsSetting.Phrases);

        foreach (var phrase in phrases.Where(phrase => !string.IsNullOrEmpty(phrase.Text.Value) && text.Contains(phrase.Text.Value, StringComparison.InvariantCultureIgnoreCase)))
        {
            Log($"Found '{phrase.Text.Value}' from phrase '{phrase.Name.Value}'");

            foreach (var parameter in phrase.Parameters)
            {
                switch (parameter.ParameterType.Value)
                {
                    case ParameterType.Bool:
                    {
                        if (parameter.BoolMode.Value == BoolMode.True)
                        {
                            SendParameter(parameter.ParameterName.Value, true);
                        }
                        else if (parameter.BoolMode.Value == BoolMode.False)
                        {
                            SendParameter(parameter.ParameterName.Value, false);
                        }
                        else
                        {
                            var receivedParameter = await FindParameter(parameter.ParameterName.Value);

                            if (receivedParameter is null)
                            {
                                SendParameter(parameter.ParameterName.Value, true);
                            }
                            else
                            {
                                if (receivedParameter.Type == ParameterType.Bool)
                                {
                                    var parameterValue = receivedParameter.GetValue<bool>();
                                    SendParameter(parameter.ParameterName.Value, !parameterValue);
                                }
                            }
                        }

                        break;
                    }

                    case ParameterType.Int:
                        SendParameter(parameter.ParameterName.Value, parameter.IntValue.Value);
                        break;

                    case ParameterType.Float:
                        SendParameter(parameter.ParameterName.Value, parameter.FloatValue.Value);
                        break;
                }
            }
        }
    }

    public enum VoiceCommandsSetting
    {
        Phrases,
        SpeechLog
    }
}