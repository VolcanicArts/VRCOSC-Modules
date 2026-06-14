// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;
using VRCOSC.App.SDK.Providers.PiShock;

namespace VRCOSC.Modules.PiShock;

[Node("Execute Group")]
public sealed class PiShockExecuteGroupNode : TryActionAsyncNode, IModuleNode<PiShockModule>
{
    public PiShockModule Module { get; set; } = null!;

    public ValueInput<int> Group = new();
    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<float> Intensity = new();
    public ValueInput<float> Duration = new();

    protected override async Task<bool> TryActionAsync(IPulseContext c)
    {
        var groupIndex = Group.Read(c);

        if (groupIndex < 0 || groupIndex >= Module.GroupsSetting.Attribute.Count)
            return false;

        var group = Module.GroupsSetting.Attribute[groupIndex];

        if (group is null)
            return false;

        var mode = Mode.Read(c);
        var intensity = float.Clamp(Intensity.Read(c), 0f, 1f);
        var duration = float.Clamp(Duration.Read(c), 0f, 1f);

        return await Module.ExecuteGroupAsync(group.ID, mode, intensity, duration);
    }
}

[Node("Execute Sharecode")]
public sealed class PiShockExecuteSharecodeNode : TryActionAsyncNode, IModuleNode<PiShockModule>
{
    public PiShockModule Module { get; set; } = null!;

    public ValueInput<string> Sharecode = new();
    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<int> Intensity = new();
    public ValueInput<int> Duration = new("Duration (ms)");

    protected override async Task<bool> TryActionAsync(IPulseContext c)
    {
        var sharecode = Sharecode.Read(c);

        if (sharecode is null)
            return false;

        var mode = Mode.Read(c);
        var intensity = int.Clamp(Intensity.Read(c), 0, 100);
        var duration = int.Max(Duration.Read(c), 0);

        return await Module.ExecuteSharecode(sharecode, mode, intensity, duration);
    }
}

[Node("Execute Serial")]
public sealed class PiShockExecuteSerialNode : TryActionAsyncNode, IModuleNode<PiShockModule>
{
    public PiShockModule Module { get; set; } = null!;

    public ValueInput<PiShockMode> Mode = new();
    public ValueInput<int> Intensity = new();
    public ValueInput<int> Duration = new("Duration (ms)");
    public ValueInput<int> ShockerId = new(defaultValue: -1);

    protected override Task<bool> TryActionAsync(IPulseContext c)
    {
        var mode = Mode.Read(c);
        var intensity = int.Clamp(Intensity.Read(c), 0, 100);
        var duration = int.Max(Duration.Read(c), 0);
        var shockerId = ShockerId.Read(c);

        return Module.ExecuteSerial(mode, intensity, duration, shockerId == -1 ? null : shockerId);
    }
}