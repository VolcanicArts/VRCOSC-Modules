// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Counter;

[Node("Read Counter")]
public sealed class ReadCounterNode : ModuleNode<CounterModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<string> Name = new("Counter Name");
    public ValueOutput<int> Value = new("Value");
    public ValueOutput<int> ValueToday = new("Value Today");

    protected override async Task Process(PulseContext c)
    {
        var name = Name.Read(c);

        var counter = Module.GetSettingValue<List<Counter>>(CounterModule.CounterSetting.CountInstances).SingleOrDefault(counter => counter.Name.Value == name);
        if (counter is null) return;

        Value.Write(Module.Counts[counter.ID].Value, c);
        ValueToday.Write(Module.Counts[counter.ID].ValueToday, c);

        await Next.Execute(c);
    }
}

[Node("Counter Source")]
[NodeForceReprocess]
public sealed class CounterSourceNode : ModuleNode<CounterModule>, IHasTextProperty, IActiveUpdateNode
{
    [NodeProperty("text")]
    public string Text { get; set; } = string.Empty;

    public ValueOutput<int> Value = new("Value");
    public ValueOutput<int> ValueToday = new("Value Today");

    protected override Task Process(PulseContext c)
    {
        var counter = Module.GetSettingValue<List<Counter>>(CounterModule.CounterSetting.CountInstances).SingleOrDefault(counter => counter.Name.Value == Text);
        if (counter is null) return Task.CompletedTask;

        var countTracker = Module.Counts[counter.ID];
        Value.Write(countTracker.Value, c);
        ValueToday.Write(countTracker.ValueToday, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}

[Node("Direct Write Counter")]
public sealed class DirectWriteCounterNode : ModuleNode<CounterModule>, IFlowInput, IHasTextProperty
{
    [NodeProperty("text")]
    public string Text { get; set; } = string.Empty;

    public FlowContinuation OnWrite = new("On Write");

    public ValueInput<int> Value = new("Value", -1);
    public ValueInput<int> ValueToday = new("Value Today", -1);

    protected override async Task Process(PulseContext c)
    {
        var counter = Module.GetSettingValue<List<Counter>>(CounterModule.CounterSetting.CountInstances).SingleOrDefault(counter => counter.Name.Value == Text);
        if (counter is null) return;

        var value = Value.Read(c);
        var valueToday = ValueToday.Read(c);

        var countTracker = Module.Counts[counter.ID];
        if (value != -1) countTracker.Value = value;
        if (valueToday != -1) countTracker.ValueToday = valueToday;

        Module.HandleChatBox(counter);
        await OnWrite.Execute(c);
    }
}

[Node("Indirect Write Counter")]
public sealed class IndirectWriteCounterNode : ModuleNode<CounterModule>, IFlowInput
{
    public FlowContinuation OnWrite = new("On Write");

    public ValueInput<string> Name = new("Counter Name");
    public ValueInput<int> Value = new("Value", -1);
    public ValueInput<int> ValueToday = new("Value Today", -1);

    protected override async Task Process(PulseContext c)
    {
        var name = Name.Read(c);

        var counter = Module.GetSettingValue<List<Counter>>(CounterModule.CounterSetting.CountInstances).SingleOrDefault(counter => counter.Name.Value == name);
        if (counter is null) return;

        var value = Value.Read(c);
        var valueToday = ValueToday.Read(c);

        var countTracker = Module.Counts[counter.ID];
        if (value != -1) countTracker.Value = value;
        if (valueToday != -1) countTracker.ValueToday = valueToday;

        Module.HandleChatBox(counter);
        await OnWrite.Execute(c);
    }
}