// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.OVR;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("SteamVR Haptic Control")]
[ModuleDescription("Lets you trigger haptics for SteamVR controllers")]
[ModuleType(ModuleType.SteamVR)]
public class HapticControlModule : Module
{
    private float duration;
    private float frequency;
    private float amplitude;

    protected override void OnPreLoad()
    {
        // TODO: Add settings for each device for triggering, default duration, frequency, and amplitude, and whether false -> true should trigger once or repeat

        RegisterParameter<float>(HapticControlParameter.Duration, "VRCOSC/VR/Haptics/Duration", ParameterMode.Read, "Duration", "The duration of the haptic trigger in seconds");
        RegisterParameter<float>(HapticControlParameter.Frequency, "VRCOSC/VR/Haptics/Frequency", ParameterMode.Read, "Frequency", "The frequency of the haptic trigger");
        RegisterParameter<float>(HapticControlParameter.Amplitude, "VRCOSC/VR/Haptics/Amplitude", ParameterMode.Read, "Amplitude", "The amplitude of the haptic trigger");
        RegisterParameter<bool>(HapticControlParameter.TriggerLeft, "VRCOSC/VR/Haptics/TriggerLeft", ParameterMode.Read, "Trigger Left", "Becoming true causes a haptic trigger in the left controller using the above parameters");
        RegisterParameter<bool>(HapticControlParameter.TriggerRight, "VRCOSC/VR/Haptics/TriggerRight", ParameterMode.Read, "Trigger Right", "Becoming true causes a haptic trigger in the right controller using the above parameters");

        RegisterParameter<bool>(HapticControlParameter.TriggerLeftDirect, "VRCOSC/VR/Haptics/TriggerLeft/*/*/*", ParameterMode.Read, "Trigger Left Direct",
            "Becoming true causes a haptic trigger in the left controller using the wildcards\nFor example:\n Writing 'VRCOSC/VR/Haptics/TriggerLeft/2/0.5/0.75' will trigger haptics in the left controller with a 2 second duration, 0.5 frequency, and 0.75 amplitude");

        RegisterParameter<bool>(HapticControlParameter.TriggerRightDirect, "VRCOSC/VR/Haptics/TriggerRight/*/*/*", ParameterMode.Read, "Trigger Right Direct",
            "Becoming true causes a haptic trigger in the right controller using the wildcards\nFor example:\n Writing 'VRCOSC/VR/Haptics/TriggerRight/2/0.5/0.75' will trigger haptics in the right controller with a 2 second duration, 0.5 frequency, and 0.75 amplitude");
    }

    protected override Task<bool> OnModuleStart()
    {
        duration = 0;
        frequency = 0;
        amplitude = 0;

        return Task.FromResult(true);
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case HapticControlParameter.Duration:
                duration = parameter.GetValue<float>();
                break;

            case HapticControlParameter.Frequency:
                frequency = convertFrequency(parameter.GetValue<float>());
                break;

            case HapticControlParameter.Amplitude:
                amplitude = convertAmplitude(parameter.GetValue<float>());
                break;

            case HapticControlParameter.TriggerLeft when parameter.GetValue<bool>():
                triggerHaptic(true, false);
                break;

            case HapticControlParameter.TriggerRight when parameter.GetValue<bool>():
                triggerHaptic(false, true);
                break;

            case HapticControlParameter.TriggerLeftDirect when parameter.GetValue<bool>():
                triggerHaptic(true, false, parameter.GetWildcard<float>(0), convertFrequency(parameter.GetWildcard<float>(1)), convertAmplitude(parameter.GetWildcard<float>(2)));
                break;

            case HapticControlParameter.TriggerRightDirect when parameter.GetValue<bool>():
                triggerHaptic(false, true, parameter.GetWildcard<float>(0), convertFrequency(parameter.GetWildcard<float>(1)), convertAmplitude(parameter.GetWildcard<float>(2)));
                break;
        }
    }

    private float convertFrequency(float frequency) => Math.Clamp(frequency, 0, 1) * 100f;
    private float convertAmplitude(float amplitude) => Math.Clamp(amplitude, 0, 1);

    private async void triggerHaptic(bool left, bool right, float? localDuration = null, float? localFrequency = null, float? localAmplitude = null)
    {
        if (left) GetOVRClient().TriggerHaptic(DeviceRole.LeftHand, localDuration ?? duration, localFrequency ?? frequency, localAmplitude ?? amplitude);
        await Task.Delay(10);
        if (right) GetOVRClient().TriggerHaptic(DeviceRole.RightHand, localDuration ?? duration, localFrequency ?? frequency, localAmplitude ?? amplitude);
    }

    private enum HapticControlParameter
    {
        Duration,
        Frequency,
        Amplitude,
        TriggerLeft,
        TriggerRight,
        TriggerLeftDirect,
        TriggerRightDirect
    }
}