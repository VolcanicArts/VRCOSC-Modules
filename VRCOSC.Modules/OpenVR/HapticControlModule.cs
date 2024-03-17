// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Modules;
using VRCOSC.App.Parameters;

namespace VRCOSC.Modules.OpenVR;

[ModuleTitle("Haptic Control")]
[ModuleDescription("Lets you set haptic parameters and trigger them for OpenVR controllers")]
[ModuleType(ModuleType.Integrations)]
public class HapticControlModule : AvatarModule
{
    private float duration;
    private float frequency;
    private float amplitude;

    protected override void OnPreLoad()
    {
        RegisterParameter<float>(HapticControlParameter.Duration, "VRCOSC/Haptics/Duration", ParameterMode.Read, "Duration", "The duration of the haptic trigger in seconds");
        RegisterParameter<float>(HapticControlParameter.Frequency, "VRCOSC/Haptics/Frequency", ParameterMode.Read, "Frequency", "The frequency of the haptic trigger");
        RegisterParameter<float>(HapticControlParameter.Amplitude, "VRCOSC/Haptics/Amplitude", ParameterMode.Read, "Amplitude", "The amplitude of the haptic trigger");
        RegisterParameter<bool>(HapticControlParameter.Trigger, "VRCOSC/Haptics/Trigger", ParameterMode.Read, "Trigger", "Becoming true causes a haptic trigger in both controllers");
        RegisterParameter<bool>(HapticControlParameter.TriggerLeft, "VRCOSC/Haptics/TriggerLeft", ParameterMode.Read, "Trigger Left", "Becoming true causes a haptic trigger in the left controller");
        RegisterParameter<bool>(HapticControlParameter.TriggerRight, "VRCOSC/Haptics/TriggerRight", ParameterMode.Read, "Trigger Right", "Becoming true causes a haptic trigger in the right controller");
    }

    protected override Task<bool> OnModuleStart()
    {
        duration = 0;
        frequency = 0;
        amplitude = 0;

        return Task.FromResult(true);
    }

    protected override void OnRegisteredParameterReceived(AvatarParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case HapticControlParameter.Duration:
                duration = parameter.GetValue<float>();
                break;

            case HapticControlParameter.Frequency:
                frequency = Math.Clamp(parameter.GetValue<float>(), 0, 1) * 100f;
                break;

            case HapticControlParameter.Amplitude:
                amplitude = Math.Clamp(parameter.GetValue<float>(), 0, 1);
                break;

            case HapticControlParameter.Trigger when parameter.GetValue<bool>():
                triggerHaptic(true, true);
                break;

            case HapticControlParameter.TriggerLeft when parameter.GetValue<bool>():
                triggerHaptic(true, false);
                break;

            case HapticControlParameter.TriggerRight when parameter.GetValue<bool>():
                triggerHaptic(false, true);
                break;
        }
    }

    private async void triggerHaptic(bool left, bool right)
    {
        if (!OVRClient.HasInitialised) return;

        if (left) OVRClient.TriggerLeftControllerHaptic(duration, frequency, amplitude);
        await Task.Delay(10);
        if (right) OVRClient.TriggerRightControllerHaptic(duration, frequency, amplitude);
    }

    private enum HapticControlParameter
    {
        Duration,
        Frequency,
        Amplitude,
        TriggerLeft,
        TriggerRight,
        Trigger
    }
}
