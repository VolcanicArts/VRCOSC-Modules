// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.Leash;

[ModuleTitle("Leash")]
[ModuleDescription("Allows for dragging you around using a Physbone")]
[ModuleType(ModuleType.Generic)]
public class LeashModule : Module
{
    private bool isGrabbed;
    private float zPositive;
    private float zNegative;
    private float xPositive;
    private float xNegative;

    protected override void OnPreLoad()
    {
        CreateSlider(LeashSetting.WalkThreshold, "Walk Threshold", "How far should the Physbone be stretched before you start walking?", 0.15f, 0.0f, 1.0f, 0.01f);
        CreateSlider(LeashSetting.RunThreshold, "Run Threshold", "How far should the Physbone be stretched before you start running?", 0.7f, 0.0f, 1.0f, 0.01f);

        RegisterParameter<bool>(LeashParameter.PhysboneGrabbed, "VRCOSC/Leash/Physbone_IsGrabbed", ParameterMode.Read, "Physbone Grabbed", "The grabbed parameter of the Physbone");
        RegisterParameter<float>(LeashParameter.PhysboneStretch, "VRCOSC/Leash/Physbone_Stretch", ParameterMode.Read, "Physbone Stretch", "The stretch parameter of the Physbone");
        RegisterParameter<float>(LeashParameter.ZPositive, "VRCOSC/Leash/Z+", ParameterMode.Read, "Z Positive", "The Z positive collider's parameter's name");
        RegisterParameter<float>(LeashParameter.ZNegative, "VRCOSC/Leash/Z-", ParameterMode.Read, "Z Negative", "The Z negative collider's parameter's name");
        RegisterParameter<float>(LeashParameter.XPositive, "VRCOSC/Leash/X+", ParameterMode.Read, "X Positive", "The X positive collider's parameter's name");
        RegisterParameter<float>(LeashParameter.XNegative, "VRCOSC/Leash/X-", ParameterMode.Read, "X Negative", "The X negative collider's parameter's name");
    }

    protected override Task<bool> OnModuleStart()
    {
        isGrabbed = false;
        zPositive = 0f;
        zNegative = 0f;
        xPositive = 0f;
        xNegative = 0f;

        return Task.FromResult(true);
    }

    protected override Task OnModuleStop()
    {
        if (isGrabbed)
            executeLeash(0f);

        return Task.CompletedTask;
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        switch (parameter.Lookup)
        {
            case LeashParameter.PhysboneGrabbed:
                var isGrabbedNew = parameter.GetValue<bool>();

                if (isGrabbed && !isGrabbedNew)
                    executeLeash(0f);

                isGrabbed = isGrabbedNew;
                break;

            case LeashParameter.PhysboneStretch:
                executeLeash(parameter.GetValue<float>());
                break;

            case LeashParameter.ZPositive:
                zPositive = parameter.GetValue<float>();
                break;

            case LeashParameter.ZNegative:
                zNegative = parameter.GetValue<float>();
                break;

            case LeashParameter.XPositive:
                xPositive = parameter.GetValue<float>();
                break;

            case LeashParameter.XNegative:
                xNegative = parameter.GetValue<float>();
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    private void executeLeash(float stretch)
    {
        if (!isGrabbed) return;

        if (stretch == 0f || stretch < GetSettingValue<float>(LeashSetting.WalkThreshold))
        {
            GetPlayer().MoveVertical(0);
            GetPlayer().MoveHorizontal(0);
            return;
        }

        if (stretch >= GetSettingValue<float>(LeashSetting.RunThreshold))
        {
            GetPlayer().Run();
        }

        if (stretch < GetSettingValue<float>(LeashSetting.RunThreshold))
        {
            GetPlayer().StopRun();
        }

        var verticalMovement = (zPositive + -zNegative) / 2f;
        var horizontalMovement = (xPositive + -xNegative) / 2f;

        GetPlayer().MoveVertical(verticalMovement);
        GetPlayer().MoveHorizontal(horizontalMovement);
    }

    private enum LeashSetting
    {
        WalkThreshold,
        RunThreshold
    }

    private enum LeashParameter
    {
        PhysboneGrabbed,
        PhysboneStretch,
        ZPositive,
        ZNegative,
        XPositive,
        XNegative
    }
}
