// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.Game.SDK;
using VRCOSC.Game.SDK.Parameters;
using VRCOSC.Game.SDK.Providers.Hardware;

namespace VRCOSC.Modules.HardwareStats;

[ModuleTitle("Hardware Stats")]
[ModuleDescription("Sends hardware stats as avatar parameters and allows for displaying them in the ChatBox")]
[ModuleType(ModuleType.Generic)]
public sealed class HardwareStatsModule : Module
{
    private HardwareStatsProvider? hardwareStatsProvider;

    protected override void OnLoad()
    {
        CreateTextBox(HardwareStatsSetting.SelectedCPU, "Selected CPU", "Enter the (0th based) index of the CPU you want to track", 0);
        CreateTextBox(HardwareStatsSetting.SelectedGPU, "Selected GPU", "Enter the (0th based) index of the GPU you want to track", 0);

        RegisterParameter<float>(HardwareStatsParameter.CpuUsage, "VRCOSC/Hardware/CPUUsage", ParameterMode.Write, "CPU Usage", "The CPU usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.CpuPower, "VRCOSC/Hardware/CPUPower", ParameterMode.Write, "CPU Power", "The CPU power draw (W)");
        RegisterParameter<int>(HardwareStatsParameter.CpuTemp, "VRCOSC/Hardware/CPUTemp", ParameterMode.Write, "CPU Temp", "The CPU temperature (C)");
        RegisterParameter<float>(HardwareStatsParameter.GpuUsage, "VRCOSC/Hardware/GPUUsage", ParameterMode.Write, "GPU Usage", "The GPU usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.GpuPower, "VRCOSC/Hardware/GPUPower", ParameterMode.Write, "GPU Power", "The GPU power draw (W)");
        RegisterParameter<int>(HardwareStatsParameter.GpuTemp, "VRCOSC/Hardware/GPUTemp", ParameterMode.Write, "GPU Temp", "The GPU temperature (C)");
        RegisterParameter<float>(HardwareStatsParameter.RamUsage, "VRCOSC/Hardware/RAMUsage", ParameterMode.Write, "RAM Usage", "The RAM usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.RamTotal, "VRCOSC/Hardware/RAMTotal", ParameterMode.Write, "RAM Total", "The total RAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.RamUsed, "VRCOSC/Hardware/RAMUsed", ParameterMode.Write, "RAM Used", "The used RAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.RamFree, "VRCOSC/Hardware/RAMFree", ParameterMode.Write, "RAM Free", "The free RAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.VRamUsage, "VRCOSC/Hardware/VRamUsage", ParameterMode.Write, "VRAM Usage", "The VRAM usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.VRamTotal, "VRCOSC/Hardware/VRamTotal", ParameterMode.Write, "VRAM Total", "The total VRAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.VRamUsed, "VRCOSC/Hardware/VRamUsed", ParameterMode.Write, "VRAM Used", "The used VRAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.VRamFree, "VRCOSC/Hardware/VRamFree", ParameterMode.Write, "VRAM Free", "The free VRAM amount (GB)");
    }

    protected override async Task<bool> OnModuleStart()
    {
        Log("Hooking into hardware monitor...");
        hardwareStatsProvider ??= new HardwareStatsProvider();
        hardwareStatsProvider.Init();
        hardwareStatsProvider.Log += LogDebug;

        await Task.Run(() =>
        {
            while (!hardwareStatsProvider.CanAcceptQueries)
            {
            }

            return true;
        });

        await hardwareStatsProvider.Update();

        var cpu = hardwareStatsProvider.GetCPU(GetSettingValue<int>(HardwareStatsSetting.SelectedCPU));

        if (cpu is null)
        {
            Log("Warning. Could not connect to CPU");
            return false;
        }

        var gpu = hardwareStatsProvider.GetGPU(GetSettingValue<int>(HardwareStatsSetting.SelectedGPU));

        if (gpu is null)
        {
            Log("Warning. Could not connect to GPU");
            return false;
        }

        return true;
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 500)]
    private async void updateParameters()
    {
        await hardwareStatsProvider!.Update();

        var cpu = hardwareStatsProvider.GetCPU(GetSettingValue<int>(HardwareStatsSetting.SelectedCPU));
        var gpu = hardwareStatsProvider.GetGPU(GetSettingValue<int>(HardwareStatsSetting.SelectedGPU));
        var ram = hardwareStatsProvider.GetRam();

        if (cpu is null)
        {
            Log("Warning. Could not connect to CPU");
            return;
        }

        if (gpu is null)
        {
            Log("Warning. Could not connect to GPU");
            return;
        }

        if (ram is null)
        {
            Log("Warning. Could not connect to RAM");
            return;
        }

        SendParameter(HardwareStatsParameter.CpuUsage, cpu.Usage / 100f);
        SendParameter(HardwareStatsParameter.CpuPower, cpu.Power);
        SendParameter(HardwareStatsParameter.CpuTemp, cpu.Temperature);

        SendParameter(HardwareStatsParameter.GpuUsage, gpu.Usage / 100f);
        SendParameter(HardwareStatsParameter.GpuPower, gpu.Power);
        SendParameter(HardwareStatsParameter.GpuTemp, gpu.Temperature);

        SendParameter(HardwareStatsParameter.RamUsage, ram.Usage / 100f);
        SendParameter(HardwareStatsParameter.RamTotal, ram.Total);
        SendParameter(HardwareStatsParameter.RamUsed, ram.Used);
        SendParameter(HardwareStatsParameter.RamFree, ram.Available);

        SendParameter(HardwareStatsParameter.VRamUsage, gpu.MemoryUsage);
        SendParameter(HardwareStatsParameter.VRamTotal, gpu.MemoryTotal);
        SendParameter(HardwareStatsParameter.VRamUsed, gpu.MemoryUsed);
        SendParameter(HardwareStatsParameter.VRamFree, gpu.MemoryFree);
    }

    protected override Task OnModuleStop()
    {
        hardwareStatsProvider!.Shutdown();
        return Task.CompletedTask;
    }

    private enum HardwareStatsSetting
    {
        SelectedCPU,
        SelectedGPU
    }

    private enum HardwareStatsParameter
    {
        CpuUsage,
        CpuPower,
        CpuTemp,
        GpuUsage,
        GpuPower,
        GpuTemp,
        RamUsage,
        RamTotal,
        RamUsed,
        RamFree,
        VRamUsage,
        VRamFree,
        VRamUsed,
        VRamTotal
    }
}
