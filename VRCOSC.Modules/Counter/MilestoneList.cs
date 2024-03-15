// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRCOSC.App.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.Counter;

public class Milestone : IEquatable<Milestone>
{
    [JsonProperty("counter_key")]
    public Observable<string> CounterKey = new(string.Empty);

    [JsonProperty("required_count")]
    public Observable<int> RequiredCount = new();

    [JsonProperty("parameter_name")]
    public Observable<string> ParameterName = new(string.Empty);

    [JsonConstructor]
    public Milestone()
    {
    }

    public Milestone(Milestone other)
    {
        CounterKey.Value = other.CounterKey.Value;
        RequiredCount.Value = other.RequiredCount.Value;
        ParameterName.Value = other.ParameterName.Value;
    }

    public bool Equals(Milestone? other)
    {
        if (ReferenceEquals(null, other)) return false;

        return CounterKey.Value.Equals(other.CounterKey.Value) && RequiredCount.Value.Equals(other.RequiredCount.Value) && ParameterName.Value.Equals(other.ParameterName.Value);
    }
}

public class MilestoneListModuleSetting : ListModuleSetting<Milestone>
{
    public MilestoneListModuleSetting(ModuleSettingMetadata metadata, IEnumerable<Milestone> defaultValues)
        : base(metadata, defaultValues, false)
    {
    }

    protected override Milestone CloneValue(Milestone value) => new(value);
    protected override Milestone ConstructValue(JToken token) => token.ToObject<Milestone>()!;
    protected override Milestone CreateNewItem() => new();
}
