// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Specialized;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
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
        CreateToggle(CounterSetting.ResetOnAvatarChange, "Reset On Avatar Change", "Should the counter reset on avatar change?", false);
        CreateSlider(CounterSetting.FloatThreshold, "Float Threshold", "For float parameters, what value needs to be crossed for the count to increase?\nFor example, a value of 0.9 will mean each time the float goes from below 0.9 to above 0.9 the count will increase", 0.9f, 0f, 1f, 0.01f);
        CreateCustom(CounterSetting.CountInstances, new CounterInstanceModuleSetting(new ModuleSettingMetadata("Counters", "The counter instances", typeof(CountInstanceModuleSettingView))));

        CreateState(CounterState.Default, "Default");

        CreateGroup("Counters", CounterSetting.CountInstances);
    }

    protected override void OnPostLoad()
    {
        var moduleSetting = GetSetting<CounterInstanceModuleSetting>(CounterSetting.CountInstances)!;

        moduleSetting.Instances.CollectionChanged += countInstancesCollectionChanged;
        countInstancesCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, moduleSetting.Instances));
    }

    private void countInstancesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (CounterInstance newCountInstance in e.NewItems)
            {
                createDynamicElements(newCountInstance);
            }
        }

        if (e.OldItems is not null)
        {
            foreach (CounterInstance oldCountInstance in e.OldItems)
            {
                deleteDynamicElements(oldCountInstance);
            }
        }
    }

    private void createDynamicElements(CounterInstance counterInstance)
    {
        var valueVariable = CreateVariable<int>($"{counterInstance.ID}_value", string.Empty)!;
        var valueTodayVariable = CreateVariable<int>($"{counterInstance.ID}_valuetoday", string.Empty)!;

        CreateEvent($"{counterInstance.ID}_countchanged", string.Empty, $"{counterInstance.Name.Value} - {{0}} ({{1}})", new[] { valueVariable, valueTodayVariable });

        counterInstance.Name.Subscribe(newName =>
        {
            GetEvent($"{counterInstance.ID}_countchanged")!.DisplayName.Value = $"On '{newName}' Changed";
            GetVariable($"{counterInstance.ID}_value")!.DisplayName.Value = $"{newName.Pluralise()} Value";
            GetVariable($"{counterInstance.ID}_valuetoday")!.DisplayName.Value = $"{newName.Pluralise()} Value Today";
        }, true);
    }

    private void deleteDynamicElements(CounterInstance counterInstance)
    {
        DeleteEvent($"{counterInstance.ID}_countchanged");
        DeleteVariable($"{counterInstance.ID}_value");
        DeleteVariable($"{counterInstance.ID}_valuetoday");
    }

    protected override Task<bool> OnModuleStart()
    {
        parameterValues.Clear();

        ChangeState(CounterState.Default);

        auditCounts();

        return Task.FromResult(true);
    }

    private void auditCounts()
    {
        var counterInstances = GetSettingValue<List<CounterInstance>>(CounterSetting.CountInstances)!;
        counterInstances.ForEach(countInstance => counts.TryAdd(countInstance.ID, new CountTracker()));
        counts.RemoveIf(pair => counterInstances.All(instance => instance.ID != pair.Key));
    }

    protected override void OnAvatarChange(AvatarConfig? avatarConfig)
    {
        if (GetSettingValue<bool>(CounterSetting.ResetOnAvatarChange))
        {
            counts.ForEach(pair =>
            {
                counts[pair.Key].Value = 0;
                counts[pair.Key].ValueToday = 0;
            });
        }
    }

    protected override void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        var countInstances = GetSettingValue<List<CounterInstance>>(CounterSetting.CountInstances)!.Where(countInstance => countInstance.ParameterNames.Select(instance => instance.Value).Contains(parameter.Name));

        countInstances.ForEach(countInstance =>
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

                if (currentValue == 0 && newValue != 0)
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

                if (currentValue < GetSettingValue<float>(CounterSetting.FloatThreshold) && newValue >= GetSettingValue<float>(CounterSetting.FloatThreshold))
                {
                    updateCounter(countInstance);
                }

                parameterValues[parameter.Name] = newValue;
            }
        });
    }

    private void updateCounter(CounterInstance counterInstance)
    {
        counts[counterInstance.ID].Value++;
        counts[counterInstance.ID].ValueToday++;

        SetVariableValue($"{counterInstance.ID}_value", counts[counterInstance.ID].Value);
        SetVariableValue($"{counterInstance.ID}_valuetoday", counts[counterInstance.ID].ValueToday);

        TriggerEvent($"{counterInstance.ID}_countchanged");
    }

    private enum CounterSetting
    {
        ResetOnAvatarChange,
        SaveCounters,
        CountInstances,
        FloatThreshold
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
