// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Security.Principal;
using UnitsNet;
using UnitsNet.Units;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.HardwareStats;

[ModuleTitle("Hardware Stats")]
[ModuleDescription("Sends hardware stats as avatar parameters and allows for displaying them in the ChatBox")]
[ModuleType(ModuleType.Generic)]
public sealed class HardwareStatsModule : Module
{
    public static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    private HardwareProvider? provider;
    public Dictionary<string, ComponentData> ComponentData { get; set; } = [];

    private readonly List<SettingsComponent> selectedCpuComponents = [];
    private readonly List<SettingsComponent> selectedGpuComponents = [];
    private readonly List<SettingsComponent> selectedMemoryComponents = [];

    private ComponentData createComponentData(Component component)
    {
        var data = new ComponentData(component)
        {
            Mappings = component.Sensors.Select(pair =>
            {
                var sensors = pair.Value;
                var mappings = new List<ComponentDataMapping>();

                for (var i = 0; i < sensors.Length; i++)
                {
                    var sensor = sensors[i];
                    var map = Mappings.GetSensorMap(component.Type, sensor);

                    mappings.Add(new ComponentDataMapping(sensor.Type, map.Unit, i, map.Name));
                }

                return mappings;
            }).SelectMany(mappings => mappings).ToList()
        };

        return data;
    }

    public Component? GetDefaultComponent(ComponentType type)
    {
        if (provider is null) return null;

        var componentId = ComponentData.SingleOrDefault(pair => pair.Value.Type == type && pair.Value.IsDefault).Key;
        return !string.IsNullOrWhiteSpace(componentId) ? provider.Components[componentId] : null;
    }

    public Component? GetComponentFromId(string id)
    {
        if (provider is null) return null;
        if (string.IsNullOrWhiteSpace(id)) return null;

        return provider.Components[id];
    }

    public Component? GetComponentFromName(string name)
    {
        if (provider is null) return null;
        if (string.IsNullOrWhiteSpace(name)) return null;

        var componentId = ComponentData.SingleOrDefault(pair => string.Equals(pair.Value.Name, name, StringComparison.InvariantCultureIgnoreCase)).Key;
        return !string.IsNullOrWhiteSpace(componentId) ? provider.Components[componentId] : null;
    }

    private (float Value, UnitsNet.UnitInfo UnitInfo)? getCompoundValue(Component component, SensorType type, string name)
    {
        if (component.Type == ComponentType.Memory && type == SensorType.Data && name == "Total")
        {
            var free = getSensorValue(component, type, "Free")!;
            var used = getSensorValue(component, type, "Used")!;

            return (free.Value.Value + used.Value.Value, Quantity.GetUnitInfo(InformationUnit.Byte));
        }

        return null;
    }

    private (float Value, UnitsNet.UnitInfo UnitInfo)? getSensorValue(Component component, SensorType type, string name)
    {
        var mapping = ComponentData[component.Id].Mappings.FirstOrDefault(mapping => mapping.Type == type && mapping.Name == name);
        if (mapping is null) return null;

        var value = component.Sensors[type][mapping.Index].Value;
        return (value, mapping.Unit);
    }

    public IQuantity? GetSensorQuantity(Component component, SensorType type, string name)
    {
        var compoundResult = getCompoundValue(component, type, name);
        if (compoundResult.HasValue) return Quantity.From(compoundResult.Value.Value, compoundResult.Value.UnitInfo.Value);

        var result = getSensorValue(component, type, name);
        return !result.HasValue ? null : Quantity.From(result.Value.Value, result.Value.UnitInfo.Value);
    }

    protected override void OnPreLoad()
    {
        try
        {
            provider = new HardwareProvider();
            provider.Init();

            var seenTypes = new HashSet<ComponentType>();

            foreach (var (id, component) in provider.Components)
            {
                var data = createComponentData(component);
                data.IsDefault = !seenTypes.Contains(data.Type);
                seenTypes.Add(data.Type);
                ComponentData[id] = data;
                LogDebug($"Added new component: {component.Name} ({component.Id}). IsDefault: {data.IsDefault}");

                foreach (var mapping in data.Mappings)
                {
                    LogDebug($"Mapping for {mapping.Type} - {mapping.Index} -> {mapping.Name}");
                }
            }
        }
        catch (Exception e)
        {
            LogDebug($"Unable to init hardware provider - {e.Message}");
            provider = null;
        }

        var defaultCpuComponent = new SettingsComponent("Default", string.Empty);
        selectedCpuComponents.Add(defaultCpuComponent);
        selectedCpuComponents.AddRange(ComponentData.Where(pair => pair.Value.Type == ComponentType.CPU).Select(p => new SettingsComponent(p.Value.Name, p.Key)));
        CreateDropdown(HardwareStatsSetting.SelectedCPU, "Selected CPU", "The selected CPU to use for parameters and variables", selectedCpuComponents, defaultCpuComponent, nameof(SettingsComponent.Name), nameof(SettingsComponent.Id));

        var defaultGpuComponent = new SettingsComponent("Default", string.Empty);
        selectedGpuComponents.Add(defaultGpuComponent);
        selectedGpuComponents.AddRange(ComponentData.Where(pair => pair.Value.Type == ComponentType.GPU).Select(p => new SettingsComponent(p.Value.Name, p.Key)));
        CreateDropdown(HardwareStatsSetting.SelectedGPU, "Selected GPU", "The selected GPU to use for parameters and variables", selectedGpuComponents, defaultGpuComponent, nameof(SettingsComponent.Name), nameof(SettingsComponent.Id));

        var defaultMemoryComponent = new SettingsComponent("Default", string.Empty);
        selectedMemoryComponents.Add(defaultMemoryComponent);
        selectedMemoryComponents.AddRange(ComponentData.Where(pair => pair.Value.Type == ComponentType.Memory).Select(p => new SettingsComponent(p.Value.Name, p.Key)));
        CreateDropdown(HardwareStatsSetting.SelectedMemory, "Selected Memory", "The selected memory to use for parameters and variables", selectedMemoryComponents, defaultMemoryComponent, nameof(SettingsComponent.Name), nameof(SettingsComponent.Id));

        RegisterParameter<float>(HardwareStatsParameter.CPUUsage, "VRCOSC/Hardware/CPU/Usage", ParameterMode.Write, "CPU Usage", "The CPU usage (0-1)");
        RegisterParameter<float>(HardwareStatsParameter.CPUPower, "VRCOSC/Hardware/CPU/Power", ParameterMode.Write, "CPU Power", "The CPU power draw (W)");
        RegisterParameter<float>(HardwareStatsParameter.CPUTemp, "VRCOSC/Hardware/CPU/Temp", ParameterMode.Write, "CPU Temp", "The CPU temperature (°C)");
        RegisterParameter<float>(HardwareStatsParameter.GPUUsage, "VRCOSC/Hardware/GPU/Usage", ParameterMode.Write, "GPU Usage", "The GPU usage (0-1)");
        RegisterParameter<float>(HardwareStatsParameter.GPUPower, "VRCOSC/Hardware/GPU/Power", ParameterMode.Write, "GPU Power", "The GPU power draw (W)");
        RegisterParameter<float>(HardwareStatsParameter.GPUTemp, "VRCOSC/Hardware/GPU/Temp", ParameterMode.Write, "GPU Temp", "The GPU temperature (°C)");
        RegisterParameter<float>(HardwareStatsParameter.RAMUsage, "VRCOSC/Hardware/RAM/Usage", ParameterMode.Write, "RAM Usage", "The RAM usage (0-1)");
        RegisterParameter<float>(HardwareStatsParameter.RAMTotal, "VRCOSC/Hardware/RAM/Total", ParameterMode.Write, "RAM Total", "The total RAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.RAMUsed, "VRCOSC/Hardware/RAM/Used", ParameterMode.Write, "RAM Used", "The used RAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.RAMFree, "VRCOSC/Hardware/RAM/Free", ParameterMode.Write, "RAM Free", "The free RAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.VRAMUsage, "VRCOSC/Hardware/VRAM/Usage", ParameterMode.Write, "VRAM Usage", "The VRAM usage (0-1)");
        RegisterParameter<float>(HardwareStatsParameter.VRAMTotal, "VRCOSC/Hardware/VRAM/Total", ParameterMode.Write, "VRAM Total", "The total VRAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.VRAMUsed, "VRCOSC/Hardware/VRAM/Used", ParameterMode.Write, "VRAM Used", "The used VRAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.VRAMFree, "VRCOSC/Hardware/VRAM/Free", ParameterMode.Write, "VRAM Free", "The free VRAM amount (GB)");
    }

    protected override void OnPostLoad()
    {
        CreateVariable<string>(HardwareStatsVariable.CPUName, "CPU Name");
        var cpuUsageReference = CreateVariable<int>(HardwareStatsVariable.CPUUsage, "CPU Usage (%)")!;
        CreateVariable<int>(HardwareStatsVariable.CPUPower, "CPU Power (W)");
        CreateVariable<int>(HardwareStatsVariable.CPUTemp, "CPU Temp (C)");

        CreateVariable<string>(HardwareStatsVariable.GPUName, "GPU Name");
        var gpuUsageReference = CreateVariable<int>(HardwareStatsVariable.GPUUsage, "GPU Usage (%)")!;
        CreateVariable<int>(HardwareStatsVariable.GPUPower, "GPU Power (W)");
        CreateVariable<int>(HardwareStatsVariable.GPUTemp, "GPU Temp (C)");

        CreateVariable<float>(HardwareStatsVariable.RAMUsage, "RAM Usage (%)");
        var ramTotalReference = CreateVariable<float>(HardwareStatsVariable.RAMTotal, "RAM Total (GB)")!;
        var ramUsedReference = CreateVariable<float>(HardwareStatsVariable.RAMUsed, "RAM Used (GB)")!;
        CreateVariable<float>(HardwareStatsVariable.RAMFree, "RAM Free (GB)");

        CreateVariable<float>(HardwareStatsVariable.VRAMUsage, "VRAM Usage (%)");
        CreateVariable<float>(HardwareStatsVariable.VRAMTotal, "VRAM Total (GB)");
        CreateVariable<float>(HardwareStatsVariable.VRAMUsed, "VRAM Used (GB)");
        CreateVariable<float>(HardwareStatsVariable.VRAMFree, "VRAM Free (GB)");

        CreateState(HardwareStatsState.Default, "Default", "CPU: {0}% | GPU: {1}%\nRAM: {2}GB/{3}GB", new[] { cpuUsageReference, gpuUsageReference, ramUsedReference, ramTotalReference });
    }

    public Component? GetSelectedComponent(ComponentType componentType) => componentType switch
    {
        ComponentType.CPU => getSelectedCPU(),
        ComponentType.Memory => getSelectedMemory(),
        ComponentType.GPU => getSelectedGPU(),
        _ => null
    };

    private Component? getSelectedCPU()
    {
        var selectedCpuId = GetSettingValue<SettingsComponent>(HardwareStatsSetting.SelectedCPU).Id;
        return string.IsNullOrWhiteSpace(selectedCpuId) ? GetDefaultComponent(ComponentType.CPU) : GetComponentFromId(selectedCpuId);
    }

    private Component? getSelectedGPU()
    {
        var selectedGpuId = GetSettingValue<SettingsComponent>(HardwareStatsSetting.SelectedGPU).Id;
        return string.IsNullOrWhiteSpace(selectedGpuId) ? GetDefaultComponent(ComponentType.GPU) : GetComponentFromId(selectedGpuId);
    }

    private Component? getSelectedMemory()
    {
        var selectedMemoryId = GetSettingValue<SettingsComponent>(HardwareStatsSetting.SelectedMemory).Id;
        return string.IsNullOrWhiteSpace(selectedMemoryId) ? GetDefaultComponent(ComponentType.Memory) : GetComponentFromId(selectedMemoryId);
    }

    protected override Task<bool> OnModuleStart()
    {
        if (!IsAdministrator) Log("VRCOSC isn't running as admin so you might not receive power and temp data");

        ChangeState(HardwareStatsState.Default);

        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, false, 250)]
    private void updateParameters()
    {
        if (provider is null) return;

        provider.Update();

        var cpu = getSelectedCPU();
        var gpu = getSelectedGPU();
        var memory = getSelectedMemory();

        if (cpu is not null)
        {
            var usageSensor = GetSensorQuantity(cpu, SensorType.Load, "Total");
            var powerSensor = GetSensorQuantity(cpu, SensorType.Power, "Total");
            var temperatureSensor = GetSensorQuantity(cpu, SensorType.Temperature, "Total");

            var usage = (float)(usageSensor?.As(usageSensor.Unit) ?? 0d);
            var power = (float)(powerSensor?.As(powerSensor.Unit) ?? 0d);
            var temperature = (float)(temperatureSensor?.As(temperatureSensor.Unit) ?? 0d);

            SendParameter(HardwareStatsParameter.CPUUsage, usage / 100f);
            SendParameter(HardwareStatsParameter.CPUPower, power);
            SendParameter(HardwareStatsParameter.CPUTemp, temperature);

            SetVariableValue(HardwareStatsVariable.CPUName, cpu.Name);
            SetVariableValue(HardwareStatsVariable.CPUUsage, (int)usage);
            SetVariableValue(HardwareStatsVariable.CPUPower, (int)power);
            SetVariableValue(HardwareStatsVariable.CPUTemp, (int)temperature);
        }

        if (gpu is not null)
        {
            var usageSensor = GetSensorQuantity(gpu, SensorType.Load, "Total");
            var powerSensor = GetSensorQuantity(gpu, SensorType.Power, "Total");
            var temperatureSensor = GetSensorQuantity(gpu, SensorType.Temperature, "Total");
            var vramFreeSensor = GetSensorQuantity(gpu, SensorType.Data, "Memory Free");
            var vramUsedSensor = GetSensorQuantity(gpu, SensorType.Data, "Memory Used");
            var vramTotalSensor = GetSensorQuantity(gpu, SensorType.Data, "Memory Total");

            var usage = (float)(usageSensor?.As(usageSensor.Unit) ?? 0d);
            var power = (float)(powerSensor?.As(powerSensor.Unit) ?? 0d);
            var temperature = (float)(temperatureSensor?.As(temperatureSensor.Unit) ?? 0d);
            var vramFree = (float)(vramFreeSensor?.As(InformationUnit.Gigabyte) ?? 0d);
            var vramUsed = (float)(vramUsedSensor?.As(InformationUnit.Gigabyte) ?? 0d);
            var vramTotal = (float)(vramTotalSensor?.As(InformationUnit.Gigabyte) ?? 0d);
            var vramUsage = vramTotal == 0f ? 0f : vramUsed / vramTotal;

            SendParameter(HardwareStatsParameter.GPUUsage, usage / 100f);
            SendParameter(HardwareStatsParameter.GPUPower, power);
            SendParameter(HardwareStatsParameter.GPUTemp, temperature);

            SendParameter(HardwareStatsParameter.VRAMUsage, vramUsage);
            SendParameter(HardwareStatsParameter.VRAMFree, vramFree);
            SendParameter(HardwareStatsParameter.VRAMUsed, vramUsed);
            SendParameter(HardwareStatsParameter.VRAMTotal, vramTotal);

            SetVariableValue(HardwareStatsVariable.GPUName, gpu.Name);
            SetVariableValue(HardwareStatsVariable.GPUUsage, (int)usage);
            SetVariableValue(HardwareStatsVariable.GPUPower, (int)power);
            SetVariableValue(HardwareStatsVariable.GPUTemp, (int)temperature);

            SetVariableValue(HardwareStatsVariable.VRAMUsage, vramUsage);
            SetVariableValue(HardwareStatsVariable.VRAMFree, vramFree);
            SetVariableValue(HardwareStatsVariable.VRAMUsed, vramUsed);
            SetVariableValue(HardwareStatsVariable.VRAMTotal, vramTotal);
        }

        if (memory is not null)
        {
            var freeSensor = GetSensorQuantity(memory, SensorType.Data, "Free");
            var usedSensor = GetSensorQuantity(memory, SensorType.Data, "Used");
            var totalSensor = GetSensorQuantity(memory, SensorType.Data, "Total");

            var free = (float)(freeSensor?.As(InformationUnit.Gigabyte) ?? 0d);
            var used = (float)(usedSensor?.As(InformationUnit.Gigabyte) ?? 0d);
            var total = (float)(totalSensor?.As(InformationUnit.Gigabyte) ?? 0d);
            var usage = total == 0f ? 0f : used / total;

            SendParameter(HardwareStatsParameter.RAMUsage, usage);
            SendParameter(HardwareStatsParameter.RAMFree, free);
            SendParameter(HardwareStatsParameter.RAMUsed, used);
            SendParameter(HardwareStatsParameter.RAMTotal, total);

            SetVariableValue(HardwareStatsVariable.RAMUsage, usage);
            SetVariableValue(HardwareStatsVariable.RAMFree, free);
            SetVariableValue(HardwareStatsVariable.RAMUsed, used);
            SetVariableValue(HardwareStatsVariable.RAMTotal, total);
        }
    }

    private enum HardwareStatsSetting
    {
        SelectedCPU,
        SelectedGPU,
        SelectedMemory
    }

    private enum HardwareStatsParameter
    {
        CPUUsage,
        CPUPower,
        CPUTemp,
        GPUUsage,
        GPUPower,
        GPUTemp,
        RAMUsage,
        RAMTotal,
        RAMUsed,
        RAMFree,
        VRAMUsage,
        VRAMFree,
        VRAMUsed,
        VRAMTotal
    }

    private enum HardwareStatsState
    {
        Default
    }

    private enum HardwareStatsVariable
    {
        CPUName,
        CPUUsage,
        CPUPower,
        CPUTemp,
        GPUName,
        GPUUsage,
        GPUPower,
        GPUTemp,
        RAMUsage,
        RAMTotal,
        RAMUsed,
        RAMFree,
        VRAMUsage,
        VRAMFree,
        VRAMUsed,
        VRAMTotal
    }
}

public record SettingsComponent(string Name, string Id);
