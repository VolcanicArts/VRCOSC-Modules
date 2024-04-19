// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter;

// TODO: Can I change this to ListModuleSetting?
public class CountInstanceModuleSetting : ModuleSetting
{
    public ObservableCollection<CountInstance> Instances { get; } = new();

    public CountInstanceModuleSetting(ModuleSettingMetadata metadata)
        : base(metadata)
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
            foreach (CountInstance newInstance in e.NewItems)
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

        foreach (var item in ingestArray.Select(jObject => jObject.ToObject<CountInstance>()))
        {
            if (item is null) continue;

            Instances.Add(item);
        }

        return true;
    }

    public override object GetRawValue() => Instances.ToList();
}

[JsonObject(MemberSerialization.OptIn)]
public class CountInstance
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Count");

    [JsonProperty("parameter_names")]
    public ObservableCollection<Observable<string>> ParameterNames { get; set; } = new();

    [JsonConstructor]
    public CountInstance()
    {
    }
}
