// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using LibreHardwareMonitor.Hardware;

namespace VRCOSC.Modules.HardwareStats;

public enum ComponentType
{
    Unknown = -1,
    Motherboard = 0,
    SuperIO = 1,
    CPU = 2,
    Memory = 3,
    GPU = 4,
    Storage = 7,
    Network = 8,
    Cooler = 9,
    EmbeddedController = 10,
    PSU = 11,
    Battery = 12,
    PowerMonitor = 13
}

public enum SensorType
{
    Unknown = -1,
    Voltage,
    Current,
    Power,
    Clock,
    Temperature,
    Load,
    Frequency,
    Fan,
    Flow,
    Control,
    Level,
    Factor,
    Data,
    Throughput,
    TimeSpan,
    Timing,
    Energy,
    Noise,
    Conductivity,
    Humidity
}

public static class EnumExtensions
{
    extension(HardwareType hardwareType)
    {
        public ComponentType ToGeneric => hardwareType switch
        {
            HardwareType.Motherboard => ComponentType.Motherboard,
            HardwareType.SuperIO => ComponentType.SuperIO,
            HardwareType.Cpu => ComponentType.CPU,
            HardwareType.Memory => ComponentType.Memory,
            HardwareType.GpuNvidia => ComponentType.GPU,
            HardwareType.GpuAmd => ComponentType.GPU,
            HardwareType.GpuIntel => ComponentType.GPU,
            HardwareType.Storage => ComponentType.Storage,
            HardwareType.Network => ComponentType.Network,
            HardwareType.Cooler => ComponentType.Cooler,
            HardwareType.EmbeddedController => ComponentType.EmbeddedController,
            HardwareType.Psu => ComponentType.PSU,
            HardwareType.Battery => ComponentType.Battery,
            HardwareType.PowerMonitor => ComponentType.PowerMonitor,
            _ => ComponentType.Unknown
        };
    }

    extension(LibreHardwareMonitor.Hardware.SensorType sensorType)
    {
        public SensorType ToGeneric => sensorType switch
        {
            LibreHardwareMonitor.Hardware.SensorType.Voltage => SensorType.Voltage,
            LibreHardwareMonitor.Hardware.SensorType.Current => SensorType.Current,
            LibreHardwareMonitor.Hardware.SensorType.Power => SensorType.Power,
            LibreHardwareMonitor.Hardware.SensorType.Clock => SensorType.Clock,
            LibreHardwareMonitor.Hardware.SensorType.Temperature => SensorType.Temperature,
            LibreHardwareMonitor.Hardware.SensorType.Load => SensorType.Load,
            LibreHardwareMonitor.Hardware.SensorType.Frequency => SensorType.Frequency,
            LibreHardwareMonitor.Hardware.SensorType.Fan => SensorType.Fan,
            LibreHardwareMonitor.Hardware.SensorType.Flow => SensorType.Flow,
            LibreHardwareMonitor.Hardware.SensorType.Control => SensorType.Control,
            LibreHardwareMonitor.Hardware.SensorType.Level => SensorType.Level,
            LibreHardwareMonitor.Hardware.SensorType.Factor => SensorType.Factor,
            LibreHardwareMonitor.Hardware.SensorType.Data => SensorType.Data,
            LibreHardwareMonitor.Hardware.SensorType.Throughput => SensorType.Throughput,
            LibreHardwareMonitor.Hardware.SensorType.TimeSpan => SensorType.TimeSpan,
            LibreHardwareMonitor.Hardware.SensorType.Timing => SensorType.Timing,
            LibreHardwareMonitor.Hardware.SensorType.Energy => SensorType.Energy,
            LibreHardwareMonitor.Hardware.SensorType.Noise => SensorType.Noise,
            LibreHardwareMonitor.Hardware.SensorType.Conductivity => SensorType.Conductivity,
            LibreHardwareMonitor.Hardware.SensorType.Humidity => SensorType.Humidity,
            _ => SensorType.Unknown
        };
    }
}
