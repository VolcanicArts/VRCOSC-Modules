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
        RegisterParameter<bool>(ClientInfoParameter.Event_InstanceLeft,    "VRCOSC/ClientInfo/Event/InstanceLeft", ParameterMode.Write, "Instance Left", "Sends true when you have left an instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_InstanceJoined,  "VRCOSC/ClientInfo/Event/InstanceJoined", ParameterMode.Write, "Instance Joined", "Sends true when you have joined an instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_UserLeft,        "VRCOSC/ClientInfo/Event/UserLeft", ParameterMode.Write, "User Left", "Sends true when a user has left your instance");
        RegisterParameter<bool>(ClientInfoParameter.Event_UserJoined,      "VRCOSC/ClientInfo/Event/UserJoined", ParameterMode.Write, "User Joined", "Sends true when a user has joined your instance");

        RegisterParameter<int>(ClientInfoParameter.Info_InstanceUserCount, "VRCOSC/ClientInfo/Info/InstanceUserCount", ParameterMode.Write, "Instance User Count", "The current user count of the instance you're in");
        RegisterParameter<int>(ClientInfoParameter.Info_FPS,               "VRCOSC/ClientInfo/Info/FPS", ParameterMode.Write, "FPS", "The current FPS of VRChat");
    }

    protected override void OnPostLoad()
    {
        var instanceCountVariable = CreateVariable<int>(ClientInfoVariable.InstanceCount, "Instance Count")!;
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

    private async void sendAndReset(ClientInfoParameter parameter, object value, object resetValue)
    {
        await SendParameterAndWait(parameter, value);
        SendParameter(parameter, resetValue);
    }

    public async void OnInstanceJoined(VRChatClientEventInstanceJoined eventArgs)
    {
        instanceUserCount = 0;

        if (eventArgs.DateTime < moduleStartTime) return;

        // delay to make sure avatar is loaded in
        await Task.Delay(500);

        sendAndReset(ClientInfoParameter.Event_InstanceJoined, true, false);
    }

    public void OnInstanceLeft(VRChatClientEventInstanceLeft eventArgs)
    {
        if (eventArgs.DateTime < moduleStartTime) return;

        sendAndReset(ClientInfoParameter.Event_InstanceLeft, true, false);
    }

    public void OnUserJoined(VRChatClientEventUserJoined eventArgs)
    {
        instanceUserCount++;
        SendParameter(ClientInfoParameter.Info_InstanceUserCount, instanceUserCount);
        SetVariableValue(ClientInfoVariable.InstanceCount, instanceUserCount);
    }

    public void OnUserLeft(VRChatClientEventUserLeft eventArgs)
    {
        instanceUserCount--;
        SendParameter(ClientInfoParameter.Info_InstanceUserCount, instanceUserCount);
        SetVariableValue(ClientInfoVariable.InstanceCount, instanceUserCount);
    }

    public void OnAvatarPreChange(VRChatClientEventAvatarPreChange eventArgs)
    {
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