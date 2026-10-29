// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using UnitsNet;
using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.HardwareStats;

[Node("Selected Component Source", "Components")]
public sealed class SelectedComponentSourceNode : ValueSourceNode<Component?>, IModuleNode<HardwareStatsModule>
{
    public HardwareStatsModule Module { get; set; } = null!;

    [InputMode(InputModes.Inline)]
    public ValueInput<ComponentType> Type = new();

    protected override Component? ComputeValue(IPulseContext c) => Module.GetSelectedComponent(Type.Read(c));
}

[Node("Component Source", "Components")]
public sealed class ComponentSourceNode : ValueSourceNode<Component?>, IModuleNode<HardwareStatsModule>
{
    public HardwareStatsModule Module { get; set; } = null!;

    [InputMode(InputModes.Inline)]
    public ValueInput<string> Name = new();

    protected override Component? ComputeValue(IPulseContext c) => Module.GetComponentFromName(Name.Read(c));
}

[Node("Sensor Data", "Components/Data")]
public sealed class SensorDataNode : Node, IModuleNode<HardwareStatsModule>, IContinuousNode
{
    public int UpdateOffset => 0;
    public HardwareStatsModule Module { get; set; } = null!;

    public ValueInput<Component?> Component = new();

    public ValueInput<SensorType> Type = new();
    public ValueInput<string> Name = new();
    public ValueOutput<IQuantity?> Value = new();

    protected override Task Process(IPulseContext c)
    {
        var component = Component.Read(c);
        if (component is null) return Task.CompletedTask;

        Value.Write(Module.GetSensorValue(component, Type.Read(c), Name.Read(c)), c);
        return Task.CompletedTask;
    }
}