// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.Maths.UI;

namespace VRCOSC.Modules.Maths;

public class EquationModuleSetting : ListModuleSetting<Equation>
{
    public EquationModuleSetting()
        : base("Equations", "Here you can write equations to be ran on trigger parameters and output to a parameter\nValues will be automatically converted to best fit the output parameter\nYou can access any parameter value by writing its name\nChanges to this setting requires a module restart", typeof(EquationModuleSettingView), [])
    {
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class Equation : ICloneable, IEquatable<Equation>
{
    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("My Equation");

    [JsonProperty("trigger_parameters")]
    public ObservableCollection<Observable<string>> TriggerParameters { get; set; } = new();

    [JsonProperty("equation")]
    public Observable<string> EquationString { get; set; } = new(string.Empty);

    [JsonProperty("output_parameter")]
    public Observable<string> OutputParameter { get; set; } = new(string.Empty);

    [JsonConstructor]
    public Equation()
    {
    }

    public Equation(Equation other)
    {
        TriggerParameters.AddRange(other.TriggerParameters.Select(triggerParameter => new Observable<string>(triggerParameter.Value)));
        EquationString.Value = other.EquationString.Value;
        OutputParameter.Value = other.OutputParameter.Value;
    }

    public object Clone() => new Equation(this);

    public bool Equals(Equation? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return TriggerParameters.SequenceEqual(other.TriggerParameters) && EquationString.Equals(other.EquationString) && OutputParameter.Equals(other.OutputParameter);
    }
}
