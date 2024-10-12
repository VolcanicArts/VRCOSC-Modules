// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using VRCOSC.App.ChatBox.Clips.Variables.Instances;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.VRChat;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter;

[ModuleTitle("Counter")]
[ModuleDescription("Counts how many times parameters are changed")]
[ModuleType(ModuleType.Generic)]
public class CounterModule : Module
{
    [ModulePersistent("counts")]
    private Dictionary<string, CountTracker> counts { get; set; } = new();

    private readonly Dictionary<string, object> parameterValues = new();

    protected override void OnPreLoad()
    {
        CreateCustomSetting(CounterSetting.CountInstances, new CountersModuleSetting());

        CreateState(CounterState.Default, "Default");

        CreateGroup("Counters", CounterSetting.CountInstances);
    }

    protected override void OnPostLoad()
    {
        var moduleSetting = GetSetting<CountersModuleSetting>(CounterSetting.CountInstances)!;

        moduleSetting.Attribute.OnCollectionChanged(countersCollectionChanged, true);
    }

    private void countersCollectionChanged(IEnumerable<Counter> newItems, IEnumerable<Counter> oldItems)
    {
        foreach (Counter newCountInstance in newItems)
        {
            createDynamicCounters(newCountInstance);
        }

        foreach (Counter oldCountInstance in oldItems)
        {
            deleteDynamicCounters(oldCountInstance);
        }
    }

    private void createDynamicCounters(Counter counter)
    {
        var valueVariable = CreateVariable<int>($"{counter.ID}_value", string.Empty)!;
        var valueTodayVariable = CreateVariable<int>($"{counter.ID}_valuetoday", string.Empty)!;
        CreateVariable<int>($"{counter.ID}_milestoneprevious", string.Empty);
        CreateVariable<int>($"{counter.ID}_milestonenext", string.Empty);
        CreateVariable<float>($"{counter.ID}_milestoneprogress", string.Empty, typeof(ProgressClipVariable));

        CreateEvent($"{counter.ID}_countchanged", string.Empty, $"{counter.Name.Value} - {{0}} ({{1}})", new[] { valueVariable, valueTodayVariable });
        CreateEvent($"{counter.ID}_milestoneachieved", string.Empty);

        counter.Name.Subscribe(newName =>
        {
            GetEvent($"{counter.ID}_countchanged")!.DisplayName.Value = $"On '{newName}' Count Changed";
            GetEvent($"{counter.ID}_milestoneachieved")!.DisplayName.Value = $"On '{newName}' Milestone Achieved";
            GetVariable($"{counter.ID}_value")!.DisplayName.Value = $"{newName.Pluralise()} Value";
            GetVariable($"{counter.ID}_valuetoday")!.DisplayName.Value = $"{newName.Pluralise()} Value Today";
            GetVariable($"{counter.ID}_milestoneprevious")!.DisplayName.Value = $"{newName.Pluralise()} Previous Milestone";
            GetVariable($"{counter.ID}_milestonenext")!.DisplayName.Value = $"{newName.Pluralise()} Next Milestone";
            GetVariable($"{counter.ID}_milestoneprogress")!.DisplayName.Value = $"{newName.Pluralise()} Milestone Progress";
        }, true);
    }

    private void deleteDynamicCounters(Counter counter)
    {
        DeleteEvent($"{counter.ID}_countchanged");
        DeleteEvent($"{counter.ID}_milestoneachieved");
        DeleteVariable($"{counter.ID}_value");
        DeleteVariable($"{counter.ID}_valuetoday");
        DeleteVariable($"{counter.ID}_milestoneprevious");
        DeleteVariable($"{counter.ID}_milestonenext");
        DeleteVariable($"{counter.ID}_milestoneprogress");
    }

    protected override Task<bool> OnModuleStart()
    {
        parameterValues.Clear();

        ChangeState(CounterState.Default);

        auditCounts();
        auditMilestones();

        return Task.FromResult(true);
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        parameterValues.Clear();
    }

    private void auditCounts()
    {
        var counterInstances = GetSettingValue<List<Counter>>(CounterSetting.CountInstances)!;
        counterInstances.ForEach(countInstance => counts.TryAdd(countInstance.ID, new CountTracker()));
        counts.RemoveIf(pair => counterInstances.All(instance => instance.ID != pair.Key));

        foreach (var counter in counterInstances)
        {
            var countTracker = counts[counter.ID];
            SetVariableValue($"{counter.ID}_value", countTracker.Value);
            SetVariableValue($"{counter.ID}_valuetoday", countTracker.ValueToday);
        }
    }

    private void auditMilestones()
    {
        var counters = GetSettingValue<List<Counter>>(CounterSetting.CountInstances)!;

        foreach (var counter in counters)
        {
            checkMilestone(counter);
        }
    }

    private bool checkMilestone(Counter counter)
    {
        if (counter.Milestones.Count == 0) return false;

        var countValue = counts[counter.ID].Value;

        var previousMilestone = counter.Milestones.LastOrDefault(milestone => milestone.Value <= countValue);
        var nextMilestone = counter.Milestones.FirstOrDefault(milestone => milestone.Value > countValue);

        var previousMilestoneValue = previousMilestone?.Value ?? 0;
        var nextMilestoneValue = nextMilestone?.Value ?? int.MaxValue;

        var milestoneProgress = (float)countValue / (nextMilestoneValue - previousMilestoneValue);

        SetVariableValue($"{counter.ID}_milestoneprevious", previousMilestone);
        SetVariableValue($"{counter.ID}_milestonenext", nextMilestone);
        SetVariableValue($"{counter.ID}_milestoneprogress", milestoneProgress);

        if (!string.IsNullOrEmpty(counter.MilestoneParameter.Value))
        {
            var milestonesPassed = counter.Milestones.Count(milestone => milestone.Value <= countValue);
            SendParameter(counter.MilestoneParameter.Value, milestonesPassed);
        }

        return countValue == previousMilestoneValue;
    }

    protected override void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        var countInstances = GetSettingValue<List<Counter>>(CounterSetting.CountInstances)!.Where(countInstance => countInstance.ParameterNames.Select(instance => instance.Value).Contains(parameter.Name));

        foreach (var countInstance in countInstances)
        {
            if (parameter.IsValueType<bool>())
            {
                bool currentValue;

                if (parameterValues.TryGetValue(parameter.Name, out var cachedValue))
                    currentValue = (bool)cachedValue;
                else
                    currentValue = false;

                var newValue = parameter.GetValue<bool>();

                if (!currentValue && newValue)
                {
                    updateCounter(countInstance);
                }

                parameterValues[parameter.Name] = newValue;
            }

            if (parameter.IsValueType<int>())
            {
                int currentValue;

                if (parameterValues.TryGetValue(parameter.Name, out var cachedValue))
                    currentValue = (int)cachedValue;
                else
                    currentValue = 0;

                var newValue = parameter.GetValue<int>();
                var intThreshold = countInstance.IntThreshold.Value;

                if (currentValue < intThreshold && newValue >= intThreshold)
                {
                    updateCounter(countInstance);
                }

                parameterValues[parameter.Name] = newValue;
            }

            if (parameter.IsValueType<float>())
            {
                float currentValue;

                if (parameterValues.TryGetValue(parameter.Name, out var cachedValue))
                    currentValue = (float)cachedValue;
                else
                    currentValue = 0f;

                var newValue = parameter.GetValue<float>();
                var floatThreshold = countInstance.FloatThreshold.Value;

                if (currentValue < floatThreshold && newValue >= floatThreshold)
                {
                    updateCounter(countInstance);
                }

                parameterValues[parameter.Name] = newValue;
            }
        }
    }

    private void updateCounter(Counter counter)
    {
        counts[counter.ID].Value++;
        counts[counter.ID].ValueToday++;

        SetVariableValue($"{counter.ID}_value", counts[counter.ID].Value);
        SetVariableValue($"{counter.ID}_valuetoday", counts[counter.ID].ValueToday);

        TriggerEvent($"{counter.ID}_countchanged");

        if (checkMilestone(counter)) TriggerEvent($"{counter.ID}_milestoneachieved");
    }

    private enum CounterSetting
    {
        CountInstances
    }

    private enum CounterState
    {
        Default
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class CountTracker
{
    [JsonProperty("value")]
    public int Value { get; set; }

    public int ValueToday { get; set; }

    [JsonConstructor]
    public CountTracker()
    {
    }
}
