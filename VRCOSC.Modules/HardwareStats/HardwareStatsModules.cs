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
public sealed class HardwareStatsModule : AvatarModule
{
    private HardwareStatsProvider? hardwareStatsProvider;

    public static bool IsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    protected override void OnPreLoad()
    {
        CreateTextBox(HardwareStatsSetting.SelectedCPU, "Selected CPU", "Enter the (0th based) index of the CPU you want to track", 0);
        CreateTextBox(HardwareStatsSetting.SelectedGPU, "Selected GPU", "Enter the (0th based) index of the GPU you want to track", 0);

        RegisterParameter<float>(HardwareStatsParameter.CpuUsage, "VRCOSC/Hardware/CPU/Usage", ParameterMode.Write, "CPU Usage", "The CPU usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.CpuPower, "VRCOSC/Hardware/CPU/Power", ParameterMode.Write, "CPU Power", "The CPU power draw (W)");
        RegisterParameter<int>(HardwareStatsParameter.CpuTemp, "VRCOSC/Hardware/CPU/Temp", ParameterMode.Write, "CPU Temp", "The CPU temperature (C)");
        RegisterParameter<float>(HardwareStatsParameter.GpuUsage, "VRCOSC/Hardware/GPU/Usage", ParameterMode.Write, "GPU Usage", "The GPU usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.GpuPower, "VRCOSC/Hardware/GPU/Power", ParameterMode.Write, "GPU Power", "The GPU power draw (W)");
        RegisterParameter<int>(HardwareStatsParameter.GpuTemp, "VRCOSC/Hardware/GPU/Temp", ParameterMode.Write, "GPU Temp", "The GPU temperature (C)");
        RegisterParameter<float>(HardwareStatsParameter.RamUsage, "VRCOSC/Hardware/RAM/Usage", ParameterMode.Write, "RAM Usage", "The RAM usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.RamTotal, "VRCOSC/Hardware/RAM/Total", ParameterMode.Write, "RAM Total", "The total RAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.RamUsed, "VRCOSC/Hardware/RAM/Used", ParameterMode.Write, "RAM Used", "The used RAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.RamFree, "VRCOSC/Hardware/RAM/Free", ParameterMode.Write, "RAM Free", "The free RAM amount (GB)");
        RegisterParameter<float>(HardwareStatsParameter.VRamUsage, "VRCOSC/Hardware/VRAM/Usage", ParameterMode.Write, "VRAM Usage", "The VRAM usage (0-1)");
        RegisterParameter<int>(HardwareStatsParameter.VRamTotal, "VRCOSC/Hardware/VRAM/Total", ParameterMode.Write, "VRAM Total", "The total VRAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.VRamUsed, "VRCOSC/Hardware/VRAM/Used", ParameterMode.Write, "VRAM Used", "The used VRAM amount (GB)");
        RegisterParameter<int>(HardwareStatsParameter.VRamFree, "VRCOSC/Hardware/VRAM/Free", ParameterMode.Write, "VRAM Free", "The free VRAM amount (GB)");
    }

    protected override async Task<bool> OnModuleStart()
    {
        Log("Hooking into hardware monitor...");

        if (!IsAdministrator) Log("VRCOSC isn't running as admin so you might not receive power and temp data");

        hardwareStatsProvider ??= new HardwareStatsProvider();
        hardwareStatsProvider.Init();

        await Task.Run(() =>
        {
            while (!hardwareStatsProvider.CanAcceptQueries)
            {
            }

            return true;
        });

        await hardwareStatsProvider.Update();

        if (!hardwareStatsProvider.CPUs.ContainsKey(GetSettingValue<int>(HardwareStatsSetting.SelectedCPU)))
        {
            Log($"CPU of id {GetSettingValue<int>(HardwareStatsSetting.SelectedCPU)} isn't available. If you have multiple, try changing the index");
            Log("Available CPU indexes:");
            hardwareStatsProvider.CPUs.ForEach(pair => Log("Index: " + pair.Key));
            return false;
        }

        if (!hardwareStatsProvider.GPUs.ContainsKey(GetSettingValue<int>(HardwareStatsSetting.SelectedGPU)))
        {
            Log($"GPU of id {GetSettingValue<int>(HardwareStatsSetting.SelectedGPU)} isn't available. If you have multiple, try changing the index");
            Log("Available GPU indexes:");
            hardwareStatsProvider.CPUs.ForEach(pair => Log("Index: " + pair.Key));
            return false;
        }

        var ram = hardwareStatsProvider.RAM;

        if (ram is null)
        {
            Log("Could not connect to RAM. This is impossible, so well done!");
            return false;
        }

        return true;
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 500)]
    private async void updateParameters()
    {
        if (!hardwareStatsProvider!.CanAcceptQueries) return;

        await hardwareStatsProvider.Update();

        if (!hardwareStatsProvider!.CanAcceptQueries) return;

        if (!hardwareStatsProvider.CPUs.TryGetValue(GetSettingValue<int>(HardwareStatsSetting.SelectedCPU), out var cpu)) return;
        if (!hardwareStatsProvider.GPUs.TryGetValue(GetSettingValue<int>(HardwareStatsSetting.SelectedGPU), out var gpu)) return;

        var ram = hardwareStatsProvider.RAM!;

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
