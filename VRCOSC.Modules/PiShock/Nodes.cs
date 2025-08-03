// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.App.SDK.Providers.PiShock;

namespace VRCOSC.Modules.PiShock;

[Node("PiShock Execute Group")]
public sealed class PiShockExecuteGroupNode : ModuleNode<PiShockModule>, IFlowInput
{
    public FlowContinuation OnSuccess = new("On Success");
    public FlowContinuation OnFail = new("On Fail");

    public ValueInput<int> Group = new();
    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<float> Intensity = new();
    public ValueInput<float> Duration = new();

    protected override async Task Process(PulseContext c)
    {
        var groupIndex = Group.Read(c);

        if (groupIndex < 0 || groupIndex >= Module.GroupsSetting.Attribute.Count)
        {
            await OnFail.Execute(c);
            return;
        }

        var group = Module.GroupsSetting.Attribute[groupIndex];

        if (group is null)
        {
            await OnFail.Execute(c);
            return;
        }

        var mode = Mode.Read(c);
        var intensity = float.Clamp(Intensity.Read(c), 0f, 1f);
        var duration = float.Clamp(Duration.Read(c), 0f, 1f);

        var result = await Module.ExecuteGroupAsync(group.ID, mode, intensity, duration);

        if (!result)
        {
            await OnFail.Execute(c);
            return;
        }

        await OnSuccess.Execute(c);
    }
}

[Node("PiShock Execute Sharecode")]
public sealed class PiShockExecuteSharecodeNode : ModuleNode<PiShockModule>, IFlowInput
{
    public FlowContinuation OnSuccess = new("On Success");
    public FlowContinuation OnFail = new("On Fail");

    public ValueInput<string> Sharecode = new("Sharecode");
    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<int> Intensity = new();
    public ValueInput<int> Duration = new("Duration Milli");

    protected override async Task Process(PulseContext c)
    {
        var sharecode = Sharecode.Read(c);

        if (sharecode is null)
        {
            await OnFail.Execute(c);
            return;
        }

        var mode = Mode.Read(c);
        var intensity = int.Clamp(Intensity.Read(c), 0, 100);
        var duration = int.Max(Duration.Read(c), 0);

        var result = await Module.ExecuteSharecode(sharecode, mode, intensity, duration);

        if (!result)
        {
            await OnFail.Execute(c);
            return;
        }

        await OnSuccess.Execute(c);
    }
}

[Node("PiShock Execute Serial")]
public sealed class PiShockExecuteSerial : ModuleNode<PiShockModule>, IFlowInput
{
    public FlowContinuation OnSuccess = new("On Success");
    public FlowContinuation OnFail = new("On Fail");

    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<int> Intensity = new();
    public ValueInput<int> Duration = new("Duration Milli");
    public ValueInput<int?> ShockerId = new("Shocker Id");

    protected override async Task Process(PulseContext c)
    {
        var mode = Mode.Read(c);
        var intensity = int.Clamp(Intensity.Read(c), 0, 100);
        var duration = int.Max(Duration.Read(c), 0);
        var shockerId = ShockerId.Read(c);

        var success = await Module.ExecuteSerial(mode, intensity, duration, shockerId);

        if (success)
            await OnSuccess.Execute(c);
        else
            await OnFail.Execute(c);
    }
}