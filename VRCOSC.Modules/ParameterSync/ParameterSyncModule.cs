// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.SDK.Handlers;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.VRChat;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.ParameterSync;

[ModuleTitle("Parameter Sync")]
[ModuleDescription("Sync parameters between avatars on avatar change")]
[ModuleType(ModuleType.Generic)]
public class ParameterSyncModule : Module, IVRCClientEventHandler
{
    private DateTime startTime;
    private bool ignoreParameters;
    private string? currentAvatarId;

    [ModulePersistent("parameter_cache")]
    public Dictionary<Guid, Dictionary<string, object>> Cache { get; set; } = [];

    protected override void OnPreLoad()
    {
        CreateCustomSetting(ParameterSyncSetting.Instances, new ParameterSyncListModuleSetting());

        CreateGroup("Configuration", string.Empty, ParameterSyncSetting.Instances);
    }

    protected override async Task<bool> OnModuleStart()
    {
        startTime = DateTime.Now;
        ignoreParameters = false;
        currentAvatarId = await FindCurrentAvatar();

        if (currentAvatarId is not null)
        {
            var instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances).Where(instance => instance.Avatars.Select(item => item.Value).Contains(currentAvatarId));

            foreach (var instance in instances)
            {
                foreach (var parameter in instance.Parameters)
                {
                    var receivedParameter = await FindParameter(parameter.Value);

                    if (receivedParameter is not null)
                        cacheParameterToCurrentAvatar(receivedParameter);
                }
            }
        }

        var moduleSetting = GetSetting<ParameterSyncListModuleSetting>(ParameterSyncSetting.Instances);
        moduleSetting.Attribute.OnCollectionChanged(instancesCollectionChanged, true);

        return true;
    }

    protected override Task OnModuleStop()
    {
        currentAvatarId = null;
        return Task.CompletedTask;
    }

    private void instancesCollectionChanged(IEnumerable<ParameterSync> newItems, IEnumerable<ParameterSync> oldItems)
    {
        foreach (var newItem in newItems)
        {
            Cache.TryAdd(newItem.Id, new Dictionary<string, object>());
        }

        foreach (var oldItem in oldItems)
        {
            Cache.Remove(oldItem.Id);
        }
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        if (avatarConfig is null) return;

        Log("Avatar has been changed");

        if (currentAvatarId is not null)
            sendSyncedParameters(currentAvatarId, avatarConfig.Id);

        currentAvatarId = avatarConfig.Id;
    }

    protected override void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        if (ignoreParameters || currentAvatarId is null) return;

        cacheParameterToCurrentAvatar(parameter);
    }

    private void cacheParameterToCurrentAvatar(ReceivedParameter parameter)
    {
        var instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances).Where(instance => instance.Avatars.Select(item => item.Value).Contains(currentAvatarId));

        foreach (var instance in instances)
        {
            var parameterValues = Cache[instance.Id];
            parameterValues[parameter.Name] = parameter.Value;
        }
    }

    private void sendSyncedParameters(string previousAvatarId, string newAvatarId)
    {
        ignoreParameters = false;

        var instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances)
                        .Where(instance => instance.Avatars.Select(item => item.Value).Contains(previousAvatarId) && instance.Avatars.Select(item => item.Value).Contains(newAvatarId))
                        .ToList();

        if (instances.Count > 1)
        {
            Log("Multiple syncs have the previous and new avatar assigned. Not syncing due to ambiguity");
            Log($"Ambiguous syncs: {string.Join(", ", instances.Select(instance => instance.Name.Value))}");
            return;
        }

        var instance = instances.FirstOrDefault();
        if (instance is null) return;

        Log($"Sending synced parameters from sync {instance.Name.Value}");

        var parameterValues = Cache[instance.Id];

        foreach (var (name, value) in parameterValues)
        {
            SendParameter(name, value);
        }
    }

    public void OnInstanceJoined(VRChatClientEventInstanceJoined eventArgs)
    {
    }

    public void OnInstanceLeft(VRChatClientEventInstanceLeft eventArgs)
    {
    }

    public void OnUserJoined(VRChatClientEventUserJoined eventArgs)
    {
    }

    public void OnUserLeft(VRChatClientEventUserLeft eventArgs)
    {
    }

    public void OnAvatarPreChange(VRChatClientEventAvatarPreChange eventArgs)
    {
        if (eventArgs.DateTime < startTime) return;

        ignoreParameters = true;
    }

    private enum ParameterSyncSetting
    {
        Instances
    }
}