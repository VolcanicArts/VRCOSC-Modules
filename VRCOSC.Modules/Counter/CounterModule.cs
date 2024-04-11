// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.Specialized;
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
    private Dictionary<string, int> counts { get; set; } = new();

    private readonly Dictionary<string, object> parameterValues = new();

    protected override void OnPreLoad()
    {
        CreateToggle(CounterSetting.ResetOnAvatarChange, "Reset On Avatar Change", "Should the counter reset on avatar change?", false);
        CreateToggle(CounterSetting.SaveCounters, "Save Counters", "Should the counters be saved between module restarts?", true);

        CreateCustom(CounterSetting.CountInstances, new CountInstanceModuleSetting(new ModuleSettingMetadata("Counts", "The count instances", typeof(CountInstancePage))));

        CreateState(CounterState.Default, "Default");
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
        CreateEvent($"{countInstance.ID}_countchanged", string.Empty);
        CreateVariable<int>($"{countInstance.ID}_value", string.Empty);

        countInstance.Name.Subscribe(_ =>
        {
            GetEvent($"{countInstance.ID}_countchanged")!.DisplayName.Value = $"On '{countInstance.Name.Value}' Changed";
            GetVariable($"{countInstance.ID}_value")!.DisplayName.Value = $"{countInstance.Name.Value.Pluralise()} Value";
        }, true);
    }

    private void deleteDynamicElements(CountInstance countInstance)
    {
        DeleteEvent($"{countInstance.ID}_countchanged");
        DeleteVariable($"{countInstance.ID}_value");
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

        countInstances.ForEach(countInstance => { counts.TryAdd(countInstance.ID, 0); });

        var countsToRemove = new List<string>();

        counts.ForEach(pair =>
        {
            if (countInstances.All(countInstance => countInstance.ID != pair.Key)) countsToRemove.Add(pair.Key);
        });

        countsToRemove.ForEach(id => counts.Remove(id));
    }

    [ModuleUpdate(ModuleUpdateMode.ChatBox)]
    private void chatBoxUpdate()
    {
        foreach (var countInstance in GetSettingValue<List<CountInstance>>(CounterSetting.CountInstances)!)
        {
            GetVariable($"{countInstance.ID}_value")!.SetValue(counts[countInstance.ID]);
        }
    }

    protected override void OnAvatarChange()
    {
        if (GetSettingValue<bool>(CounterSetting.ResetOnAvatarChange))
        {
            counts.ForEach(pair => counts[pair.Key] = 0);
        }
    }

    protected override void OnAnyParameterReceived(ReceivedParameter parameter)
    {
        var countInstances = GetSettingValue<List<CountInstance>>(CounterSetting.CountInstances)!.Where(countInstance => countInstance.ParameterNames.Contains(parameter.Name));

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

                if (currentValue < 0.9f && newValue >= 0.9f)
                {
                    updateCounter(countInstance);
                }

                parameterValues[parameter.Name] = newValue;
            }
        });
    }

    private void updateCounter(CountInstance countInstance)
    {
        counts[countInstance.ID]++;
        TriggerEvent($"{countInstance.ID}_countchanged");
    }

    private enum CounterSetting
    {
        ResetOnAvatarChange,
        SaveCounters,
        CountInstances
    }

    private enum CounterState
    {
        Default
    }
}
