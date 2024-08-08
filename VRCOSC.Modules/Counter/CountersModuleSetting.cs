// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.Counter.UI;

namespace VRCOSC.Modules.Counter;

// TODO: Can I change this to ListModuleSetting?
public class CountersModuleSetting : ModuleSetting
{
    public ObservableCollection<Counter> Instances { get; } = new();

    public CountersModuleSetting()
        : base("Counters", "Add, edit, and remove counters", typeof(CountersModuleSettingView))
    {
    }

    private bool postDeserialise;

    public override void PostDeserialise()
    {
        Instances.CollectionChanged += InstancesOnCollectionChanged;

        postDeserialise = true;
        InstancesOnCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Instances));
        postDeserialise = false;
    }

    private void InstancesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (Counter newInstance in e.NewItems)
            {
                newInstance.Name.Subscribe(_ => RequestSerialisation?.Invoke());
                newInstance.ParameterNames.CollectionChanged += ParameterNamesOnCollectionChanged;
                if (postDeserialise) ParameterNamesOnCollectionChanged(null, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newInstance.ParameterNames));
            }
        }

        RequestSerialisation?.Invoke();
    }

    private void ParameterNamesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is not null)
        {
            foreach (Observable<string> newParameterName in e.NewItems)
            {
                newParameterName.Subscribe(_ => RequestSerialisation?.Invoke());
            }
        }

        RequestSerialisation?.Invoke();
    }

    public override void SetDefault() => Instances.Clear();
    public override bool IsDefault() => !Instances.Any();

    public override bool Deserialise(object ingestValue)
    {
        if (ingestValue is not JArray ingestArray) return false;

        Instances.Clear();

        foreach (var item in ingestArray.Select(jObject => jObject.ToObject<Counter>()))
        {
            if (item is null) continue;

            Instances.Add(item);
        }

        return true;
    }

    public override object GetRawValue() => Instances.ToList();
}

[JsonObject(MemberSerialization.OptIn)]
public class Counter
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Counter");

    [JsonProperty("float_threshold")]
    public Observable<float> FloatThreshold { get; set; } = new(0.9f);

    [JsonProperty("parameter_names")]
    public ObservableCollection<Observable<string>> ParameterNames { get; set; } = new();

    [JsonProperty("milestone_parameter")]
    public Observable<string> MilestoneParameter { get; set; } = new(string.Empty);

    [JsonProperty("milestones")]
    public ObservableCollection<Observable<int>> Milestones { get; set; } = new();

    [JsonConstructor]
    public Counter()
    {
    }
}