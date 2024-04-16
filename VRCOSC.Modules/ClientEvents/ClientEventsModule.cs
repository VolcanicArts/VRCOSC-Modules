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
        RegisterParameter<bool>(ClientEventsParameter.OnWorldExit, "VRCOSC/ClientEvents/WorldExit", ParameterMode.Write, "On World Exit", "Sends true the moment you exit a world, allowing you to trigger an exit animation");
        RegisterParameter<bool>(ClientEventsParameter.OnWorldEnter, "VRCOSC/ClientEvents/WorldEnter", ParameterMode.Write, "On World Enter", "Sends true the moment you enter a world, allowing you to trigger an enter animation");

        CreateEvent(ClientEventsEvent.OnWorldExit, "On World Exit", "Goodbye!");
        CreateEvent(ClientEventsEvent.OnWorldEnter, "On World Enter", "Hello!");
    }

    protected override void Client_OnWorldExit()
    {
        SendParameter(ClientEventsParameter.OnWorldExit, true);
        TriggerEvent(ClientEventsEvent.OnWorldExit);
    }

    protected override async void Client_OnWorldEnter(string worldID)
    {
        // delay to sure avatar is loaded in
        await Task.Delay(500);
        SendParameter(ClientEventsParameter.OnWorldEnter, true);
        TriggerEvent(ClientEventsEvent.OnWorldEnter);
    }

    public enum ClientEventsParameter
    {
        OnWorldExit,
        OnWorldEnter
    }

    public enum ClientEventsEvent
    {
        OnWorldExit,
        OnWorldEnter
    }
}
