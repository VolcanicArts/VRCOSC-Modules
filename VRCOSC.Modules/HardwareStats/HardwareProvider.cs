// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using LibreHardwareMonitor.Hardware;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.HardwareStats;

public sealed class HardwareProvider
{
    private readonly Computer computer = new()
    {
        IsCpuEnabled = true,
        IsGpuEnabled = true,
        IsMemoryEnabled = true
    };

    public readonly Dictionary<string, Component> Components = new();

    public void Init()
    {
        computer.Open();
        computer.Hardware.ForEach(auditHardware);
    }

    public async Task Shutdown() => await Task.Run(() =>
    {
        Components.Clear();
        computer.Close();
    });

    public void Update()
    {
        computer.Hardware.ForEach(hardware =>
        {
            var hardwareId = hardware.Identifier.ToString()!;
            updateHardware(hardware);

            if (!Components.TryGetValue(hardwareId, out var component))
                return;

            component.Update(hardware);
        });
    }

    private void auditHardware(IHardware hardware)
    {
        var id = hardware.Identifier.ToString()!;
        var name = hardware.Name;
        var type = hardware.HardwareType.ToGeneric;
        var component = new Component(id, name, type);

        component.Init(hardware);
        Components[id] = component;
    }

    private static void updateHardware(IHardware hardware)
    {
        hardware.Update();
        hardware.SubHardware.ForEach(updateHardware);
    }
}
