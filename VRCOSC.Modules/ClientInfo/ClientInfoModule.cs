// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.VRChat;
using VRCOSC.App.SDK.VRChat.Logs;
using VRCOSC.App.SDK.VRChat.Logs.Handlers;

// ReSharper disable MultipleSpaces

namespace VRCOSC.Modules.ClientInfo;

[ModuleTitle("Client Info")]
[ModuleDescription("Takes info from VRChat's log and converts them into parameters")]
[ModuleType(ModuleType.Generic)]
public class ClientInfoModule : Module, IVRCClientEventHandler
{
    private DateTime moduleStartTime;

    protected override void OnPreLoad()
    {
        RegisterParameter<bool>(ClientInfoParameter.Event_InstanceLeft,    "VRCOSC/ClientInfo/Event/InstanceLeft", ParameterMode.Write, "Instance Left", "Sends true when you have left an instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_InstanceJoined,  "VRCOSC/ClientInfo/Event/InstanceJoined", ParameterMode.Write, "Instance Joined", "Sends true when you have joined an instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_UserLeft,        "VRCOSC/ClientInfo/Event/UserLeft", ParameterMode.Write, "User Left", "Sends true when a user has left your instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_UserJoined,      "VRCOSC/ClientInfo/Event/UserJoined", ParameterMode.Write, "User Joined", "Sends true when a user has joined your instance");

        RegisterParameter<int>(ClientInfoParameter.Info_InstanceUserCount, "VRCOSC/ClientInfo/Info/InstanceUserCount", ParameterMode.Write, "Instance User Count", "The current user count of the instance you're in");
        RegisterParameter<int>(ClientInfoParameter.Info_FPS,               "VRCOSC/ClientInfo/Info/FPS", ParameterMode.Write, "FPS", "The current FPS of VRChat");
    }

    protected override void OnPostLoad()
    {
        CreateVariable<int>(ClientInfoVariable.InstanceCount, "Instance Count");
        var fpsVariable = CreateVariable<int>(ClientInfoVariable.FPS, "FPS")!;

        CreateState(ClientInfoState.Default, "Default", "FPS: {0}", [fpsVariable]);
    }

    protected override Task<bool> OnModuleStart()
    {
        ChangeState(ClientInfoState.Default);
        moduleStartTime = DateTime.Now;
        return Task.FromResult(true);
    }

    [ModuleUpdate(ModuleUpdateMode.Custom)]
    private void fastUpdate()
    {
        var fps = (int)double.Round(GetClient().FPS, MidpointRounding.AwayFromZero);
        SendParameter(ClientInfoParameter.Info_FPS, fps);
        SetVariableValue(ClientInfoVariable.FPS, fps);
    }

    private async void sendAndReset<T>(ClientInfoParameter parameter, T value, T resetValue)
    {
        await SendParameterAndWait(parameter, value);
        SendParameter(parameter, resetValue);
    }

    public async void HandleClientEvent(IVRChatClientEvent @event)
    {
        switch (@event)
        {
            case InstanceJoinedClientEvent instanceJoinedClientEvent:
            {
                if (instanceJoinedClientEvent.Timestamp < moduleStartTime) return;

                // delay to make sure avatar is loaded in
                await Task.Delay(500);

                sendAndReset(ClientInfoParameter.Event_InstanceJoined, true, false);
                break;
            }

            case InstanceLeftClientEvent instanceLeftClientEvent:
            {
                if (instanceLeftClientEvent.Timestamp < moduleStartTime) return;

                SendParameter(ClientInfoParameter.Info_InstanceUserCount, 0);
                sendAndReset(ClientInfoParameter.Event_InstanceLeft, true, false);
                break;
            }

            case UserLeftClientEvent:
            {
                var userCount = GetClient().Instance.Users.Count;
                SendParameter(ClientInfoParameter.Info_InstanceUserCount, userCount);
                SetVariableValue(ClientInfoVariable.InstanceCount, userCount);
                break;
            }

            case UserJoinedClientEvent:
            {
                var userCount = GetClient().Instance!.Users.Count;
                SendParameter(ClientInfoParameter.Info_InstanceUserCount, userCount);
                SetVariableValue(ClientInfoVariable.InstanceCount, userCount);
                break;
            }
        }
    }

    protected override void OnAvatarChange(Avatar? avatar)
    {
        var userCount = GetClient().Instance!.Users.Count;
        SendParameter(ClientInfoParameter.Info_InstanceUserCount, userCount);
    }

    public enum ClientInfoState
    {
        Default
    }

    public enum ClientInfoVariable
    {
        InstanceCount,
        FPS
    }

    public enum ClientInfoParameter
    {
        Event_InstanceLeft,
        Event_InstanceJoined,
        Event_UserLeft,
        Event_UserJoined,
        Info_InstanceUserCount,
        Info_FPS
    }
}