// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Security.Principal;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Providers.Hardware;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.HardwareStats;

[ModuleTitle("Hardware Stats")]
[ModuleDescription("Sends hardware stats as avatar parameters and allows for displaying them in the ChatBox")]
[ModuleType(ModuleType.Generic)]
public sealed class HardwareStatsModule : Module
{
    public const float BYTES_PER_KB = 1024f;
    public const float BYTES_PER_MB = 1024f * BYTES_PER_KB;
    public const float BYTES_PER_GB = 1024f * BYTES_PER_MB;

    private HardwareStatsProvider? hardwareStatsProvider;

    public static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    protected override void OnPreLoad()
    {
        CreateTextBox(HardwareStatsSetting.SelectedCPU, "Selected CPU", "Enter the (0th based) index of the CPU you want to track", 0);
        CreateTextBox(HardwareStatsSetting.SelectedGPU, "Selected GPU", "Enter the (0th based) index of the GPU you want to track", 0);

        RegisterParameter<float>(HardwareStatsParameter.CPUUsage, "VRCOSC/Hardware/CPU/Usage", ParameterMode.Write, "CPU Usage", "The CPU usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.CPUPower, "VRCOSC/Hardware/CPU/Power", ParameterMode.Write, "CPU Power", "The CPU power draw (W)");
        RegisterParameter<int>(HardwareStatsParameter.CPUTemp, "VRCOSC/Hardware/CPU/Temp", ParameterMode.Write, "CPU Temp", "The CPU temperature (C)");
        RegisterParameter<float>(HardwareStatsParameter.GPUUsage, "VRCOSC/Hardware/GPU/Usage", ParameterMode.Write, "GPU Usage", "The GPU usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.GPUPower, "VRCOSC/Hardware/GPU/Power", ParameterMode.Write, "GPU Power", "The GPU power draw (W)");
        RegisterParameter<int>(HardwareStatsParameter.GPUTemp, "VRCOSC/Hardware/GPU/Temp", ParameterMode.Write, "GPU Temp", "The GPU temperature (C)");
        RegisterParameter<float>(HardwareStatsParameter.RAMUsage, "VRCOSC/Hardware/RAM/Usage", ParameterMode.Write, "RAM Usage", "The RAM usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.RAMTotal, "VRCOSC/Hardware/RAM/Total", ParameterMode.Write, "RAM Total", "The total RAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.RAMUsed, "VRCOSC/Hardware/RAM/Used", ParameterMode.Write, "RAM Used", "The used RAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.RAMFree, "VRCOSC/Hardware/RAM/Free", ParameterMode.Write, "RAM Free", "The free RAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.VRAMUsage, "VRCOSC/Hardware/VRAM/Usage", ParameterMode.Write, "VRAM Usage", "The VRAM usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.VRAMTotal, "VRCOSC/Hardware/VRAM/Total", ParameterMode.Write, "VRAM Total", "The total VRAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.VRAMUsed, "VRCOSC/Hardware/VRAM/Used", ParameterMode.Write, "VRAM Used", "The used VRAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.VRAMFree, "VRCOSC/Hardware/VRAM/Free", ParameterMode.Write, "VRAM Free", "The free VRAM amount (GB)");
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

    protected override async Task<bool> OnModuleStart()
    {
        Log("Hooking into hardware monitor...");

        if (!IsAdministrator) Log("VRCOSC isn't running as admin so you might not receive power and temp data");

        hardwareStatsProvider ??= new HardwareStatsProvider();
        hardwareStatsProvider.Init();

        await Task.Run(async () =>
        {
            while (!hardwareStatsProvider.CanAcceptQueries)
            {
                await Task.Delay(1);
            }

            return true;
        });

        await hardwareStatsProvider.Update();

        var selectedCpu = GetSettingValue<int>(HardwareStatsSetting.SelectedCPU);
        var selectedGpu = GetSettingValue<int>(HardwareStatsSetting.SelectedGPU);

        if (!hardwareStatsProvider.CPUs.ContainsKey(selectedCpu))
        {
            Log($"CPU of id {selectedCpu} isn't available. If you have multiple, try changing the index");
            Log("Available CPU indexes:");
            hardwareStatsProvider.CPUs.ForEach(pair => Log($"Index: {pair.Key}"));
            return false;
        }

        if (!hardwareStatsProvider.GPUs.ContainsKey(selectedGpu))
        {
            Log($"GPU of id {selectedGpu} isn't available. If you have multiple, try changing the index");
            Log("Available GPU indexes:");
            hardwareStatsProvider.CPUs.ForEach(pair => Log($"Index: {pair.Key}"));
            return false;
        }

        var ram = hardwareStatsProvider.RAM;

        if (ram is null)
        {
            Log("Could not connect to RAM. This is impossible, so well done!");
            return false;
        }

        ChangeState(HardwareStatsState.Default);

        return true;
    }

    public CPU? GetCPU() => hardwareStatsProvider?.CPUs.GetValueOrDefault(GetSettingValue<int>(HardwareStatsSetting.SelectedCPU));
    public GPU? GetGPU() => hardwareStatsProvider?.GPUs.GetValueOrDefault(GetSettingValue<int>(HardwareStatsSetting.SelectedGPU));
    public RAM? GetRAM() => hardwareStatsProvider?.RAM;

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 500)]
    private async void updateParameters()
    {
        if (hardwareStatsProvider is not { CanAcceptQueries: true }) return;

        await hardwareStatsProvider.Update();

        var cpu = GetCPU();
        var gpu = GetGPU();
        var ram = GetRAM();

        if (cpu is not null)
        {
            SendParameter(HardwareStatsParameter.CPUUsage, cpu.Usage / 100f);
            SendParameter(HardwareStatsParameter.CPUPower, cpu.Power);
            SendParameter(HardwareStatsParameter.CPUTemp, cpu.Temperature);

            SetVariableValue(HardwareStatsVariable.CPUName, cpu.Name);
            SetVariableValue(HardwareStatsVariable.CPUUsage, (int)cpu.Usage);
            SetVariableValue(HardwareStatsVariable.CPUPower, cpu.Power);
            SetVariableValue(HardwareStatsVariable.CPUTemp, cpu.Temperature);
        }

        if (gpu is not null)
        {
            SendParameter(HardwareStatsParameter.GPUUsage, gpu.Usage / 100f);
            SendParameter(HardwareStatsParameter.GPUPower, gpu.Power);
            SendParameter(HardwareStatsParameter.GPUTemp, gpu.Temperature);

            SendParameter(HardwareStatsParameter.VRAMUsage, gpu.MemoryUsage);
            SendParameter(HardwareStatsParameter.VRAMTotal, gpu.MemoryTotal / BYTES_PER_GB);
            SendParameter(HardwareStatsParameter.VRAMUsed, gpu.MemoryUsed / BYTES_PER_GB);
            SendParameter(HardwareStatsParameter.VRAMFree, gpu.MemoryFree / BYTES_PER_GB);

            SetVariableValue(HardwareStatsVariable.GPUName, gpu.Name);
            SetVariableValue(HardwareStatsVariable.GPUUsage, (int)gpu.Usage);
            SetVariableValue(HardwareStatsVariable.GPUPower, gpu.Power);
            SetVariableValue(HardwareStatsVariable.GPUTemp, gpu.Temperature);

            SetVariableValue(HardwareStatsVariable.VRAMUsage, gpu.MemoryUsage * 100f);
            SetVariableValue(HardwareStatsVariable.VRAMTotal, gpu.MemoryTotal / BYTES_PER_GB);
            SetVariableValue(HardwareStatsVariable.VRAMUsed, gpu.MemoryUsed / BYTES_PER_GB);
            SetVariableValue(HardwareStatsVariable.VRAMFree, gpu.MemoryFree / BYTES_PER_GB);
        }

        if (ram is not null)
        {
            SendParameter(HardwareStatsParameter.RAMUsage, ram.Usage / 100f);
            SendParameter(HardwareStatsParameter.RAMTotal, ram.Total / BYTES_PER_GB);
            SendParameter(HardwareStatsParameter.RAMUsed, ram.Used / BYTES_PER_GB);
            SendParameter(HardwareStatsParameter.RAMFree, ram.Available / BYTES_PER_GB);

            SetVariableValue(HardwareStatsVariable.RAMUsage, ram.Usage);
            SetVariableValue(HardwareStatsVariable.RAMTotal, ram.Total / BYTES_PER_GB);
            SetVariableValue(HardwareStatsVariable.RAMUsed, ram.Used / BYTES_PER_GB);
            SetVariableValue(HardwareStatsVariable.RAMFree, ram.Available / BYTES_PER_GB);
        }
    }

    protected override Task OnModuleStop()
    {
        hardwareStatsProvider?.Shutdown();
        return Task.CompletedTask;
    }

    private enum HardwareStatsSetting
    {
        SelectedCPU,
        SelectedGPU
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