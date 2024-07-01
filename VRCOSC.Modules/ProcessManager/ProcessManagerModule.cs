// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.ProcessManager;

[ModuleTitle("Process Manager")]
[ModuleDescription("Allows for starting and stopping processes from avatar parameters")]
[ModuleType(ModuleType.Integrations)]
public class ProcessManagerModule : Module
{
    protected override void OnPreLoad()
    {
        RegisterParameter<bool>(ProcessManagerParameter.Start, "VRCOSC/ProcessManager/Start/*", ParameterMode.Read, "Start", "Becoming true will start the process named in the '*' that you set on your avatar\nFor example, on your avatar you put: VRCOSC/ProcessManager/Start/vrchat");
        RegisterParameter<bool>(ProcessManagerParameter.Stop, "VRCOSC/ProcessManager/Stop/*", ParameterMode.Read, "Stop", "Becoming true will stop the process named in the '*' that you set on your avatar\nFor example, on your avatar you put: VRCOSC/ProcessManager/Stop/vrchat");
    }

    protected override void OnRegisteredParameterReceived(RegisteredParameter parameter)
    {
        var processName = parameter.WildcardAs<string>(0);

        switch (parameter.Lookup)
        {
            case ProcessManagerParameter.Start when parameter.GetValue<bool>():
                startProcess(processName);
                break;

            case ProcessManagerParameter.Stop when parameter.GetValue<bool>():
                stopProcess(processName);
                break;
        }
    }

    private void startProcess(string? processName)
    {
        if (string.IsNullOrEmpty(processName)) return;

        Log($"Attempting to start {processName}");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System),
                FileName = "cmd.exe",
                Arguments = $"/c start {processName}",
                UseShellExecute = true
            }
        };

        process.Start();
    }

    private async void stopProcess(string? processName)
    {
        if (string.IsNullOrEmpty(processName)) return;

        Log($"Attempting to stop {processName}");

        var processes = Process.GetProcessesByName(processName);

        if (!processes.Any())
        {
            Log($"Cannot find any process named {processName}");
            return;
        }

        foreach (var process in processes)
        {
            process.Kill();
            await process.WaitForExitAsync();
            process.Dispose();
        }
    }

    private enum ProcessManagerParameter
    {
        Start,
        Stop
    }
}
