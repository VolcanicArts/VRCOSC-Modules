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
    private readonly List<IDisposable> disposables = new();
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
        disposables.Clear();
        currentAvatarId = await FindCurrentAvatar();

        var moduleSetting = GetSetting<ParameterSyncListModuleSetting>(ParameterSyncSetting.Instances);
        disposables.Add(moduleSetting.Attribute.OnCollectionChanged(instancesCollectionChanged, true));

        sendSyncedParameters(null, currentAvatarId);
        return true;
    }

    protected override Task OnModuleStop()
    {
        currentAvatarId = null;

        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        return Task.CompletedTask;
    }

    private void updateCacheLayout()
    {
        var instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances);

        foreach (var parameterSync in instances)
        {
            if (!Cache.ContainsKey(parameterSync.Id))
            {
                Cache.Add(parameterSync.Id, new Dictionary<string, object>());
            }
        }

        Cache.RemoveIf(pair => instances.All(instance => instance.Id != pair.Key));

        foreach (var (id, store) in Cache)
        {
            var instance = instances.Single(instance => instance.Id == id);
            store.RemoveIf(d => !instance.Parameters.Select(p => p.Value).Contains(d.Key));
        }
    }

    private void instancesCollectionChanged(IEnumerable<ParameterSync> newItems, IEnumerable<ParameterSync> oldItems)
    {
        foreach (var newItem in newItems)
        {
            disposables.Add(newItem.Parameters.OnCollectionChanged((_, _) => updateCacheLayout()));
        }

        updateCacheLayout();
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        ignoreParameters = false;

        if (avatarConfig is null)
        {
            Log("Local avatar detected. Not syncing.");
        }
        else
        {
            Log("Avatar has been changed.");
            sendSyncedParameters(currentAvatarId, avatarConfig.Id);
        }

        currentAvatarId = avatarConfig?.Id;
    }

    protected override void OnAnyParameterReceived(VRChatParameter parameter)
    {
        if (ignoreParameters) return;

        cacheParameter(parameter);
    }

    private void cacheParameter(VRChatParameter parameter)
    {
        if (string.IsNullOrEmpty(currentAvatarId)) return;

        var instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances)
                        .Where(instance => instance.Avatars.Select(item => item.Value).Contains(currentAvatarId))
                        .ToList();

        if (instances.Count == 0) return;

        if (instances.Count > 1)
        {
            Log($"Multiple syncs have the same avatar assigned with ID {currentAvatarId}. Unable to cache parameter value.");
            Log($"Ambiguous syncs: {string.Join(", ", instances.Select(instance => instance.Name.Value))}.");
            return;
        }

        var instance = instances[0];

        if (!instance.Parameters.Select(p => p.Value).Contains(parameter.Name)) return;

        var store = Cache[instance.Id];
        store[parameter.Name] = parameter.Value;
    }

    private void sendSyncedParameters(string? previousAvatarId, string? newAvatarId)
    {
        if (string.IsNullOrEmpty(newAvatarId)) return;

        List<ParameterSync> instances;

        if (string.IsNullOrEmpty(previousAvatarId))
        {
            instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances)
                        .Where(instance => instance.Avatars.Select(item => item.Value).Contains(newAvatarId))
                        .ToList();
        }
        else
        {
            instances = GetSettingValue<List<ParameterSync>>(ParameterSyncSetting.Instances)
                        .Where(instance => instance.Avatars.Select(item => item.Value).Contains(previousAvatarId) && instance.Avatars.Select(item => item.Value).Contains(newAvatarId))
                        .ToList();
        }

        if (instances.Count == 0) return;

        if (instances.Count > 1)
        {
            Log("Multiple syncs have the previous and new avatar assigned. Not syncing due to ambiguity.");
            Log($"Ambiguous syncs: {string.Join(", ", instances.Select(instance => instance.Name.Value))}.");
            return;
        }

        var instance = instances[0];

        Log($"Sending parameters from sync {instance.Name.Value}.");

        var store = Cache[instance.Id];

        foreach (var (name, value) in store)
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
        Log("Avatar pre-change occurred.");
    }

    private enum ParameterSyncSetting
    {
        Instances
    }
}