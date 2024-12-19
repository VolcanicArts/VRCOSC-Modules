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

        CreateGroup("Phrases", VoiceCommandsSetting.Phrases);
    }

    public void OnPartialSpeechResult(string text)
    {
    }

    public async void OnFinalSpeechResult(string text)
    {
        var phrases = GetSettingValue<List<Phrase>>(VoiceCommandsSetting.Phrases);

        foreach (var phrase in phrases.Where(phrase => text.Contains(phrase.Text.Value, StringComparison.InvariantCultureIgnoreCase)))
        {
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
        Phrases
    }
}