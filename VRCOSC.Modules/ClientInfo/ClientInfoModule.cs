// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.VRChat;

// ReSharper disable MultipleSpaces

namespace VRCOSC.Modules.ClientInfo;

[ModuleTitle("Client Info")]
[ModuleDescription("Takes info from VRChat's log and converts them into parameters")]
[ModuleType(ModuleType.Generic)]
public class ClientInfoModule : Module, IVRCClientEventHandler
{
    private DateTime moduleStartTime;
    private int instanceUserCount;

    protected override void OnPreLoad()
    {
        RegisterParameter<bool>(ClientInfoParameter.Event_Instance_Exit,       "VRCOSC/ClientInfo/Event/Instance/Exit", ParameterMode.Write, "On Exit", "Sends true the moment you exit an instance, allowing you to trigger an exit animation");
        RegisterParameter<bool>(ClientInfoParameter.Event_Instance_Enter,      "VRCOSC/ClientInfo/Event/Instance/Enter", ParameterMode.Write, "On Enter", "Sends true the moment you enter an instance, allowing you to trigger an enter animation");
        RegisterParameter<bool>(ClientInfoParameter.Event_Instance_UserLeft,   "VRCOSC/ClientInfo/Event/Instance/UserLeft", ParameterMode.Write, "On User Left", "Sends true when a user leaves the instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_Instance_UserJoined, "VRCOSC/ClientInfo/Event/Instance/UserJoined", ParameterMode.Write, "On User Joined", "Sends true when a user joins the instance");

        RegisterParameter<int>(ClientInfoParameter.Info_Instance_UserCount,    "VRCOSC/ClientInfo/Info/Instance/UserCount", ParameterMode.Write, "Instance User Count", "The current user count of the instance of the world you're in");

        CreateEvent(ClientInfoEvent.OnInstanceExit, "On World Exit", "Goodbye!");
        CreateEvent(ClientInfoEvent.OnInstanceEnter, "On World Enter", "Hello!");
    }

    protected override Task<bool> OnModuleStart()
    {
        moduleStartTime = DateTime.Now;
        return Task.FromResult(true);
    }

    private async void sendAndReset(ClientInfoParameter parameter, object value, object resetValue)
    {
        await SendParameterAndWait(parameter, value);
        SendParameter(parameter, resetValue);
    }

    public async void OnInstanceEnter(VRChatClientEventInstanceEnter eventArgs)
    {
        instanceUserCount = 0;

        if (eventArgs.DateTime < moduleStartTime) return;

        // delay to make sure avatar is loaded in
        await Task.Delay(500);

        TriggerEvent(ClientInfoEvent.OnInstanceEnter);
        sendAndReset(ClientInfoParameter.Event_Instance_Enter, true, false);
    }

    public void OnInstanceExit(VRChatClientEventInstanceExit eventArgs)
    {
        if (eventArgs.DateTime < moduleStartTime) return;

        TriggerEvent(ClientInfoEvent.OnInstanceExit);
        sendAndReset(ClientInfoParameter.Event_Instance_Exit, true, false);
    }

    public void OnUserJoined(VRChatClientEventUserJoined eventArgs)
    {
        instanceUserCount++;
        SendParameter(ClientInfoParameter.Info_Instance_UserCount, instanceUserCount);
    }

    public void OnUserLeft(VRChatClientEventUserLeft eventArgs)
    {
        instanceUserCount--;
        SendParameter(ClientInfoParameter.Info_Instance_UserCount, instanceUserCount);
    }

    public enum ClientInfoParameter
    {
        Event_Instance_Exit,
        Event_Instance_Enter,
        Event_Instance_UserLeft,
        Event_Instance_UserJoined,
        Info_Instance_UserCount
    }

    public enum ClientInfoEvent
    {
        OnInstanceExit,
        OnInstanceEnter
    }
}