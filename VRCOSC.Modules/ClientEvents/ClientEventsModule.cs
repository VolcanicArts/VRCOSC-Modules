// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;

namespace VRCOSC.Modules.ClientEvents;

[ModuleTitle("Client Events")]
[ModuleDescription("Sets parameters when the VRChat client does something")]
[ModuleType(ModuleType.Generic)]
public class ClientEventsModule : ChatBoxModule
{
    protected override void OnPreLoad()
    {
        RegisterParameter<bool>(ClientEventsParameter.OnWorldExit, "VRCOSC/ClientEvents/World/Exit", ParameterMode.Write, "On World Exit", "Sends true the moment you exit a world, allowing you to trigger an exit animation");

        CreateEvent(ClientEventsEvent.OnWorldExit, "On World Exit", "Goodbye!");
    }

    protected override void Client_OnWorldExit()
    {
        SendParameter(ClientEventsParameter.OnWorldExit, true);
        TriggerEvent(ClientEventsEvent.OnWorldExit);
    }

    public enum ClientEventsParameter
    {
        OnWorldExit
    }

    public enum ClientEventsEvent
    {
        OnWorldExit
    }
}
