// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Specialized;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter;

[ModuleTitle("Counter")]
[ModuleDescription("Counts how many times parameters are changed")]
[ModuleType(ModuleType.Generic)]
public class CounterModule : ChatBoxModule
{
    protected override bool ShouldUsePersistence => GetSettingValue<bool>(CounterSetting.SaveCounters);

    [ModulePersistent("counts")]
    private Dictionary<string, CountTracker> counts { get; set; } = new();

    private readonly Dictionary<string, object> parameterValues = new();

    protected override void OnPreLoad()
    {
        CreateToggle(CounterSetting.ResetOnAvatarChange, "Reset On Avatar Change", "Should the counter reset on avatar change?", false);
        CreateToggle(CounterSetting.SaveCounters, "Save Counters", "Should the counters be saved between module restarts?", true);

        CreateSlider(CounterSetting.FloatThreshold, "Float Threshold", "For float parameters, what value needs to be crossed for the count to increase?\nFor example, a value of 0.9 will mean each time the float goes from below 0.9 to above 0.9 the count will increase", 0.9f, 0f, 1f, 0.01f);

        CreateCustom(CounterSetting.CountInstances, new CountInstanceModuleSetting(new ModuleSettingMetadata("Counts", "The count instances", typeof(CountInstanceModuleSettingPage))));

        CreateState(CounterState.Default, "Default");

        CreateGroup("Counters", CounterSetting.CountInstances);
    }

    protected override void OnPostLoad()
    {
        var moduleSetting = GetSetting<CountInstanceModuleSetting>(CounterSetting.CountInstances)!;

        moduleSetting.Instances.CollectionChanged += countInstancesCollectionChanged;
        countInstancesCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, moduleSetting.Instances));
    }

    private void countInstancesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (CountInstance newCountInstance in e.NewItems)
            {
                createDynamicElements(newCountInstance);
            }
        }

        if (e.OldItems is not null)
        {
            foreach (CountInstance oldCountInstance in e.OldItems)
            {
                deleteDynamicElements(oldCountInstance);
            }
        }
    }

    private void createDynamicElements(CountInstance countInstance)
    {
        var valueVariable = CreateVariable<int>($"{countInstance.ID}_value", string.Empty)!;
        var valueTodayVariable = CreateVariable<int>($"{countInstance.ID}_valuetoday", string.Empty)!;

        CreateEvent($"{countInstance.ID}_countchanged", string.Empty, $"{countInstance.Name.Value} - {{0}} ({{1}})", new[] { valueVariable, valueTodayVariable });

        countInstance.Name.Subscribe(newName =>
        {
            GetEvent($"{countInstance.ID}_countchanged")!.DisplayName.Value = $"On '{newName}' Changed";
            GetVariable($"{countInstance.ID}_value")!.DisplayName.Value = $"{newName.Pluralise()} Value";
            GetVariable($"{countInstance.ID}_valuetoday")!.DisplayName.Value = $"{newName.Pluralise()} Value Today";
        }, true);
    }

    private void deleteDynamicElements(CountInstance countInstance)
    {
        DeleteEvent($"{countInstance.ID}_countchanged");
        DeleteVariable($"{countInstance.ID}_value");
        DeleteVariable($"{countInstance.ID}_valuetoday");
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
        var countInstances = GetSettingValue<List<CountInstance>>(CounterSetting.CountInstances)!;

        countInstances.ForEach(countInstance => counts.TryAdd(countInstance.ID, new CountTracker()));

        var countsToRemove = new List<string>();

        counts.ForEach(pair =>
        {
            if (countInstances.All(countInstance => countInstance.ID != pair.Key)) countsToRemove.Add(pair.Key);
        });

        countsToRemove.ForEach(id => counts.Remove(id));
    }

    protected override void OnAvatarChange()
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
        var countInstances = GetSettingValue<List<CountInstance>>(CounterSetting.CountInstances)!.Where(countInstance => countInstance.ParameterNames.Select(instance => instance.Value).Contains(parameter.Name));

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

    private void updateCounter(CountInstance countInstance)
    {
        counts[countInstance.ID].Value++;
        counts[countInstance.ID].ValueToday++;

        SetVariableValue($"{countInstance.ID}_value", counts[countInstance.ID].Value);
        SetVariableValue($"{countInstance.ID}_valuetoday", counts[countInstance.ID].ValueToday);

        TriggerEvent($"{countInstance.ID}_countchanged");
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
