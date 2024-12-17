// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.Counter.UI;

namespace VRCOSC.Modules.Counter;

public class CountersModuleSetting : ListModuleSetting<Counter>
{
    public CountersModuleSetting()
        : base("Counters", "Add, edit, and remove counters", typeof(CountersModuleSettingView), [])
    {
    }

    protected override Counter CreateItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class Counter : IEquatable<Counter>
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Counter");

    [JsonProperty("int_threshold")]
    public Observable<int> IntThreshold { get; set; } = new(1);

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

    public bool Equals(Counter? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && FloatThreshold.Equals(other.FloatThreshold) && ParameterNames.SequenceEqual(other.ParameterNames) && MilestoneParameter.Equals(other.MilestoneParameter) && Milestones.SequenceEqual(other.Milestones);
    }
}