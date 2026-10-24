// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UnitsNet;
using UnitsNet.Units;

// ReSharper disable RedundantVerbatimStringPrefix

namespace VRCOSC.Modules.HardwareStats;

[SuppressMessage("Performance", @"SYSLIB1045:Convert to 'GeneratedRegexAttribute'.")]
public static class Mappings
{
    private static readonly List<SensorTypeMapping> cpu_mappings = [];
    private static readonly List<SensorTypeMapping> gpu_mappings = [];
    private static readonly List<SensorTypeMapping> memory_mappings = [];

    static Mappings()
    {
        cpu_mappings.AddRange(getCpuMappings());
        gpu_mappings.AddRange(getGpuMappings());
        memory_mappings.AddRange(getMemoryMappings());
    }

    private static (string Name, UnitInfo Unit) getSensorMap(List<SensorTypeMapping> mappings, Sensor sensor)
    {
        var mapping = mappings.FirstOrDefault(mapping => mapping.Regex.IsMatch(sensor.Name) && mapping.Type == sensor.Type);
        if (mapping is null) return (string.Empty, null!);

        var name = string.Format(mapping.Name, mapping.Regex.Match(sensor.Name).Groups.Values.Skip(1).Select(group => group.Value).ToArray());
        return (name, mapping.Unit);
    }

    public static (string Name, UnitInfo Unit) GetSensorMap(ComponentType type, Sensor sensor) => type switch
    {
        ComponentType.CPU => getSensorMap(cpu_mappings, sensor),
        ComponentType.GPU => getSensorMap(gpu_mappings, sensor),
        ComponentType.Memory => getSensorMap(memory_mappings, sensor),
        _ => (string.Empty, null!)
    };

    private static IEnumerable<SensorTypeMapping> getCpuMappings()
    {
        yield return new SensorTypeMapping(SensorType.Load, "Total", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:CPU Total|Total CPU)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Core {0} Thread {1}", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^CPU Core #(\d+) Thread #(\d+)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Core {0}", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^CPU Core #(\d+)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Core Max", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:CPU Core Max)$"));

        yield return new SensorTypeMapping(SensorType.Clock, "Bus Speed", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:Bus Speed)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "Core {0}", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^CPU Core #(\d+)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "Average", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:CPU Average Clock)$"));

        yield return new SensorTypeMapping(SensorType.Temperature, "Total", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:CPU Package|CPU Package Temperature|Core \(Tdie\)|Core \(Tctl/Tdie\)|CPU Cores)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Core {0}", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^CPU Core #(\d+)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Core {0} Distance to TjMax", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^CPU Core #(\d+) Distance to TjMax$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Distance to TjMax", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^CPU Distance to TjMax$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Core Average", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:Core Average)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Core Max", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:Core Max)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "CCD {0}", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^CPU CCD(\d+) \(Tdie\)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "IO Die", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:CPU IOD|IO-Die)$"));

        yield return new SensorTypeMapping(SensorType.Power, "Total", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:CPU Package|Package|CPU Package Power)$"));
        yield return new SensorTypeMapping(SensorType.Power, "Cores", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:CPU Cores)$"));
        yield return new SensorTypeMapping(SensorType.Power, "Graphics", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:CPU Graphics)$"));
        yield return new SensorTypeMapping(SensorType.Power, "Memory", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:CPU Memory)$"));
        yield return new SensorTypeMapping(SensorType.Power, "PPT", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:CPU PPT)$"));
        yield return new SensorTypeMapping(SensorType.Power, "Core", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^CPU Core \(SVI2 TFN\)$"));
        yield return new SensorTypeMapping(SensorType.Power, "SoC", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^CPU SoC \(SVI2 TFN\)$"));

        yield return new SensorTypeMapping(SensorType.Voltage, "Core {0}", Quantity.GetUnitInfo(ElectricPotentialUnit.Volt), new Regex(@"^CPU Core #(\d+)$"));
        yield return new SensorTypeMapping(SensorType.Voltage, "Core", Quantity.GetUnitInfo(ElectricPotentialUnit.Volt), new Regex(@"^(?:CPU Core)$"));
        yield return new SensorTypeMapping(SensorType.Voltage, "VID", Quantity.GetUnitInfo(ElectricPotentialUnit.Volt), new Regex(@"^(?:CPU VID|VID)$"));
        yield return new SensorTypeMapping(SensorType.Voltage, "SoC", Quantity.GetUnitInfo(ElectricPotentialUnit.Volt), new Regex(@"^CPU SoC \(SVI2 TFN\)$"));
    }

    private static IEnumerable<SensorTypeMapping> getGpuMappings()
    {
        yield return new SensorTypeMapping(SensorType.Load, "Total", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:GPU Core|GPU Compute)$"));
        yield return new SensorTypeMapping(SensorType.Load, "3D", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:GPU D3D Usage)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Memory Controller", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:GPU Memory Controller)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Video Engine", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:GPU Video Engine)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Bus Interface", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:GPU Bus Interface)$"));
        yield return new SensorTypeMapping(SensorType.Load, "3D", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:D3D 3D)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Video Decode", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:D3D Video Decode)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Video Encode", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:D3D Video Encode)$"));
        yield return new SensorTypeMapping(SensorType.Load, "Copy", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:D3D Copy)$"));

        yield return new SensorTypeMapping(SensorType.Clock, "Core", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:GPU Core)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "Memory", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:GPU Memory)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "Shader", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:GPU Shader)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "Video", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:GPU Video Clock)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "Fabric", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:GPU Fabric)$"));
        yield return new SensorTypeMapping(SensorType.Clock, "SOC", Quantity.GetUnitInfo(FrequencyUnit.Megahertz), new Regex(@"^(?:GPU SOC)$"));

        yield return new SensorTypeMapping(SensorType.Temperature, "Total", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:GPU Core)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Hot Spot", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:GPU Hot Spot)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Memory", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:GPU Memory Junction|GPU Memory)$"));
        yield return new SensorTypeMapping(SensorType.Temperature, "Edge", Quantity.GetUnitInfo(TemperatureUnit.DegreeCelsius), new Regex(@"^(?:GPU Edge Temperature)$"));

        yield return new SensorTypeMapping(SensorType.Power, "Total", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:GPU Package|GPU Power|GPU Board Power Draw)$"));
        yield return new SensorTypeMapping(SensorType.Power, "PPT", Quantity.GetUnitInfo(PowerUnit.Watt), new Regex(@"^(?:GPU PPT|GPU ASIC)$"));

        yield return new SensorTypeMapping(SensorType.Voltage, "Core", Quantity.GetUnitInfo(ElectricPotentialUnit.Volt), new Regex(@"^(?:GPU Core)$"));

        yield return new SensorTypeMapping(SensorType.Control, "Fan", Quantity.GetUnitInfo(RatioUnit.Percent), new Regex(@"^(?:GPU Fan)$"));
        yield return new SensorTypeMapping(SensorType.Fan, "Fan", Quantity.GetUnitInfo(RotationalSpeedUnit.RevolutionPerMinute), new Regex(@"^(?:GPU Fan)$"));

        yield return new SensorTypeMapping(SensorType.Data, "Memory Free", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:GPU Memory Free)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Memory Used", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:GPU Memory Used)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Memory Total", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:GPU Memory Total)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Dedicated Memory Used", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:D3D Dedicated Memory Used)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Shared Memory Used", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:D3D Shared Memory Used)$"));

        yield return new SensorTypeMapping(SensorType.Throughput, "Receive", Quantity.GetUnitInfo(BitRateUnit.BytePerSecond), new Regex(@"^(?:GPU PCIe Rx)$"));
        yield return new SensorTypeMapping(SensorType.Throughput, "Transmit", Quantity.GetUnitInfo(BitRateUnit.BytePerSecond), new Regex(@"^(?:GPU PCIe Tx)$"));
        yield return new SensorTypeMapping(SensorType.Throughput, "Bandwidth", Quantity.GetUnitInfo(BitRateUnit.BytePerSecond), new Regex(@"^(?:GPU Memory Bandwidth)$"));
    }

    private static IEnumerable<SensorTypeMapping> getMemoryMappings()
    {
        yield return new SensorTypeMapping(SensorType.Data, "Used", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:Memory Used)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Free", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:Memory Available)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Virtual Used", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:Virtual Memory Used)$"));
        yield return new SensorTypeMapping(SensorType.Data, "Virtual Free", Quantity.GetUnitInfo(InformationUnit.Byte), new Regex(@"^(?:Virtual Memory Available)$"));
    }
}

public record SensorTypeMapping(SensorType Type, string Name, UnitInfo Unit, Regex Regex);