// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.VoiceCommands;

[JsonObject(MemberSerialization.OptIn)]
public class Parameter : IEquatable<Parameter>
{
    [JsonProperty("parameter_name")]
    public Observable<string> ParameterName { get; set; } = new("MyParameter");

    [JsonProperty("parameter_type")]
    public Observable<ParameterType> ParameterType { get; set; } = new(App.SDK.Parameters.ParameterType.Bool);

    [JsonProperty("bool_mode")]
    public Observable<BoolMode> BoolMode { get; set; } = new(VoiceCommands.BoolMode.Toggle);

    [JsonProperty("int_value")]
    public Observable<int> IntValue { get; set; } = new();

    [JsonProperty("float_value")]
    public Observable<float> FloatValue { get; set; } = new();

    [JsonConstructor]
    public Parameter()
    {
    }

    public bool Equals(Parameter? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return ParameterName.Equals(other.ParameterName) && ParameterType.Equals(other.ParameterType) && BoolMode.Equals(other.BoolMode) && IntValue.Equals(other.IntValue) && FloatValue.Equals(other.FloatValue);
    }
}

public enum BoolMode
{
    True,
    False,
    Toggle
}