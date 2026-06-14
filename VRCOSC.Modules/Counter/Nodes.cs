// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Counter;

[Node("Counter Source")]
[NodeForceReprocess]
public sealed class CounterSourceNode : Node, IModuleNode<CounterModule>, IContinuousNode
{
    public int UpdateOffset => 0;

    public CounterModule Module { get; set; } = null!;

    [InputMode(InputModes.Inline)]
    public ValueInput<string> Name = new();

    public ValueOutput<int> Value = new();
    public ValueOutput<int> ValueToday = new();

    protected override Task Process(IPulseContext c)
    {
        var counter = Module.GetSettingValue<List<Counter>>(CounterModule.CounterSetting.CountInstances).SingleOrDefault(counter => counter.Name.Value == Name.Read(c));
        if (counter is null) return Task.CompletedTask;

        var countTracker = Module.Counts[counter.ID];
        Value.Write(countTracker.Value, c);
        ValueToday.Write(countTracker.ValueToday, c);
        return Task.CompletedTask;
    }
}

[Node("Write Counter")]
public sealed class WriteCounterNode : ActionNode, IModuleNode<CounterModule>
{
    public CounterModule Module { get; set; } = null!;

    public ValueInput<string> Name = new();
    public ValueInput<int> Value = new(defaultValue: -1);
    public ValueInput<int> ValueToday = new(defaultValue: -1);

    protected override void DoAction(IPulseContext c)
    {
        var counter = Module.GetSettingValue<List<Counter>>(CounterModule.CounterSetting.CountInstances).SingleOrDefault(counter => counter.Name.Value == Name.Read(c));
        if (counter is null) return;

        var value = Value.Read(c);
        var valueToday = ValueToday.Read(c);

        var countTracker = Module.Counts[counter.ID];
        if (value != -1) countTracker.Value = value;
        if (valueToday != -1) countTracker.ValueToday = valueToday;

        Module.HandleChatBox(counter);
    }
}