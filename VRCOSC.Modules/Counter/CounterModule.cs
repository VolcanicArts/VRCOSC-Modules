// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using osu.Framework.Extensions.IEnumerableExtensions;
using VRCOSC.SDK;
using VRCOSC.SDK.Attributes.Settings;
using VRCOSC.SDK.Attributes.Settings.Types;
using VRCOSC.SDK.Avatars;
using VRCOSC.SDK.Parameters;

namespace VRCOSC.Modules.Counter;

[ModuleTitle("Counter")]
[ModuleDescription("Counts how many times parameters are triggered based on parameter change events")]
[ModuleType(ModuleType.Generic)]
public class CounterModule : AvatarModule
{
    protected override bool ShouldUsePersistence => GetSettingValue<bool>(CounterSetting.SaveCounters);

    [ModulePersistent("counts")]
    private Dictionary<string, CountInstance> counts { get; set; } = new();

    protected override void OnLoad()
    {
        CreateToggle(CounterSetting.ResetOnAvatarChange, "Reset On Avatar Change", "Should the counter reset on avatar change?", false);
        CreateToggle(CounterSetting.SaveCounters, "Save Counters", "Should the counters be saved between module restarts?", true);

        CreateKeyValuePairList(CounterSetting.Parameters, "Parameter List", "What parameters should be monitored for changes?\nCounter names can be reused to allow multiple parameters to add to the same counter", new List<MutableKeyValuePair> { new() { Key = { Value = "Example" }, Value = { Value = "ExampleParameterName" } } }, "Counter Name",
            "Parameter Name");

        CreateCustom(CounterSetting.Milestones, new MilestoneListModuleSetting(new ListModuleSettingMetadata("Milestones",
            "Set Parameter Name to true when Counter Name reaches Required Count\nThese will be set when the module starts if a counter has already reached the milestone",
            typeof(DrawableMilestoneList), typeof(DrawableMilestone)), new List<Milestone>()));
    }

    protected override Task<bool> OnModuleStart()
    {
        auditParameters();
        counts.ForEach(checkMilestones);

        return Task.FromResult(true);
    }

    protected override void OnAvatarChange()
    {
        if (GetSettingValue<bool>(CounterSetting.ResetOnAvatarChange))
        {
            counts.ForEach(pair =>
            {
                pair.Value.Count = 0;
                pair.Value.CountToday = 0;
            });
        }
    }

    private void auditParameters()
    {
        GetSettingValue<List<MutableKeyValuePair>>(CounterSetting.Parameters)!.ForEach(pair =>
        {
            if (string.IsNullOrEmpty(pair.Key.Value) || string.IsNullOrEmpty(pair.Value.Value)) return;

            counts.TryAdd(pair.Key.Value, new CountInstance());
            counts[pair.Key.Value].ParameterNames.Add(pair.Value.Value);
        });

        counts.ForEach(pair =>
        {
            if (GetSettingValue<List<MutableKeyValuePair>>(CounterSetting.Parameters)!.All(instance => instance.Key.Value != pair.Key))
            {
                counts.Remove(pair.Key);
            }
        });
    }

    protected override void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        var candidates = counts.Where(pair => pair.Value.ParameterNames.Contains(parameter.Name)).ToList();
        if (!candidates.Any()) return;

        var pair = candidates[0];

        if (parameter.IsValueType<float>() && (float)(pair.Value.PreviousValue ?? 0f) < 0.9f && parameter.GetValue<float>() >= 0.9f)
        {
            counterChanged(pair);
            pair.Value.PreviousValue = parameter.GetValue<float>();
        }

        if (parameter.IsValueType<int>() && parameter.GetValue<int>() != (int)(pair.Value.PreviousValue ?? 0))
        {
            counterChanged(pair);
            pair.Value.PreviousValue = parameter.GetValue<int>();
        }

        if (parameter.IsValueType<bool>() && parameter.GetValue<bool>())
        {
            counterChanged(pair);
            pair.Value.PreviousValue = parameter.GetValue<bool>();
        }
    }

    private void counterChanged(KeyValuePair<string, CountInstance> pair)
    {
        pair.Value.Count++;
        pair.Value.CountToday++;

        checkMilestones(pair);
    }

    private void checkMilestones(KeyValuePair<string, CountInstance> pair)
    {
        var milestones = GetSettingValue<List<Milestone>>(CounterSetting.Milestones)!.Where(instance => instance.CounterKey.Value == pair.Key).ToList();
        if (!milestones.Any()) return;

        var instances = milestones.Where(instance => pair.Value.Count >= instance.RequiredCount.Value && !string.IsNullOrEmpty(instance.ParameterName.Value));
        instances.ForEach(instance => SendParameter(instance.ParameterName.Value, true));
    }

    private enum CounterSetting
    {
        ResetOnAvatarChange,
        SaveCounters,
        Parameters,
        Milestones
    }
}

public class CountInstance
{
    [JsonProperty("count")]
    public int Count;

    [JsonIgnore]
    public object? PreviousValue;

    [JsonIgnore]
    public readonly List<string> ParameterNames = new();

    [JsonIgnore]
    public int CountToday;

    [JsonConstructor]
    public CountInstance()
    {
    }
}
