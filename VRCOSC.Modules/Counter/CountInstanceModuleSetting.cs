// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter;

// TODO: Can I change this to ListModuleSetting?
public class CountInstanceModuleSetting : ModuleSetting
{
    #region Required

    public CountInstanceModuleSetting(ModuleSettingMetadata metadata)
        : base(metadata)
    {
        Instances.CollectionChanged += (_, _) => RequestSerialisation?.Invoke();
    }

    public override void Load()
    {
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

    #endregion

    #region Custom

    public ObservableCollection<CountInstance> Instances { get; } = new();

    public CountInstance Create()
    {
        var instance = new CountInstance
        {
            Name = { Value = "New Count" }
        };

        instance.Name.Subscribe(_ => RequestSerialisation?.Invoke());
        instance.ParameterNames.CollectionChanged += (_, _) => RequestSerialisation?.Invoke();

        Instances.Add(instance);
        return instance;
    }

    #endregion
}

[JsonObject(MemberSerialization.OptIn)]
public class CountInstance
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new(string.Empty);

    [JsonProperty("parameter_names")]
    public ObservableCollection<string> ParameterNames { get; set; } = new();

    [JsonConstructor]
    public CountInstance()
    {
    }
}
