// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.HardwareStats;

[Node("CPU Info Source")]
[NodeForceReprocess]
public sealed class CPUInfoSourceNode : ModuleNode<HardwareStatsModule>, IActiveUpdateNode
{
    public ValueOutput<float> Usage = new();
    public ValueOutput<int> Power = new();
    public ValueOutput<int> Temperature = new();

    protected override Task Process(PulseContext c)
    {
        var cpu = Module.GetCPU();
        if (cpu is null) return Task.CompletedTask;

        Usage.Write(cpu.Usage / 100f, c);
        Power.Write(cpu.Power, c);
        Temperature.Write(cpu.Temperature, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}

[Node("GPU Info Source")]
public sealed class GPUInfoSourceNode : ModuleNode<HardwareStatsModule>, IActiveUpdateNode
{
    public ValueOutput<float> Usage = new();
    public ValueOutput<int> Power = new();
    public ValueOutput<int> Temperature = new();

    protected override Task Process(PulseContext c)
    {
        var gpu = Module.GetGPU();
        if (gpu is null) return Task.CompletedTask;

        Usage.Write(gpu.Usage / 100f, c);
        Power.Write(gpu.Power, c);
        Temperature.Write(gpu.Temperature, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}

[Node("RAM Info Source")]
public sealed class RAMInfoSourceNode : ModuleNode<HardwareStatsModule>, IActiveUpdateNode
{
    public ValueOutput<float> Usage = new();
    public ValueOutput<float> Total = new();
    public ValueOutput<float> Used = new();
    public ValueOutput<float> Free = new();

    protected override Task Process(PulseContext c)
    {
        var ram = Module.GetRAM();
        if (ram is null) return Task.CompletedTask;

        Usage.Write(ram.Usage / 100f, c);
        Total.Write(ram.Total, c);
        Used.Write(ram.Used, c);
        Free.Write(ram.Available, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}

[Node("VRAM Info Source")]
public sealed class VRAMInfoSourceNode : ModuleNode<HardwareStatsModule>, IActiveUpdateNode
{
    public ValueOutput<float> Usage = new();
    public ValueOutput<float> Total = new();
    public ValueOutput<float> Used = new();
    public ValueOutput<float> Free = new();

    protected override Task Process(PulseContext c)
    {
        var gpu = Module.GetGPU();
        if (gpu is null) return Task.CompletedTask;

        Usage.Write(gpu.MemoryUsage, c);
        Total.Write(gpu.MemoryTotal / 1000f, c);
        Used.Write(gpu.MemoryUsed / 1000f, c);
        Free.Write(gpu.MemoryFree / 1000f, c);
        return Task.CompletedTask;
    }

    public bool OnUpdate(PulseContext c) => true;
}