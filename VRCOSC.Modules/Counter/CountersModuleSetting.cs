// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    protected override Counter CloneValue(Counter value) => new(value);

    protected override Counter ConstructValue(JToken token) => token.ToObject<Counter>()!;

    protected override Counter CreateNewItem() => new();
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

    public Counter(Counter other)
    {
        Name.Value = other.Name.Value;
        FloatThreshold.Value = other.FloatThreshold.Value;
        ParameterNames.AddRange(other.ParameterNames.Select(parameterName => new Observable<string>(parameterName.Value)));
        MilestoneParameter.Value = other.MilestoneParameter.Value;
        Milestones.AddRange(other.Milestones.Select(milestone => new Observable<int>(milestone.Value)));
    }
}
