// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.App.SDK.Providers.PiShock;

namespace VRCOSC.Modules.PiShock;

[Node("PiShock Execute")]
public sealed class PiShockExecuteNode : ModuleNode<PiShockModule>, IFlowInput
{
    public FlowContinuation OnSuccess = new("On Success");
    public FlowContinuation OnFail = new("On Fail");

    public ValueInput<string> GroupName = new("Group Name");
    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<float> Intensity = new();
    public ValueInput<float> Duration = new();

    protected override async Task Process(PulseContext c)
    {
        var groupName = GroupName.Read(c);
        var group = Module.GroupsSetting.Attribute.SingleOrDefault(group => group.Name.Value == groupName);

        if (group is null)
        {
            await OnFail.Execute(c);
            return;
        }

        var mode = Mode.Read(c);
        var intensity = float.Clamp(Intensity.Read(c), 0f, 1f);
        var duration = float.Clamp(Duration.Read(c), 0f, 1f);

        await Module.ExecuteGroupAsync(group.ID, mode, intensity, duration);
        await OnSuccess.Execute(c);
    }
}