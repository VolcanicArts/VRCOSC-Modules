// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.PiShock.UI;

namespace VRCOSC.Modules.PiShock;

public class ShockerModuleSetting : ListModuleSetting<Shocker>
{
    public ShockerModuleSetting()
        : base("Shockers", "Individual shockers. Name them something recognisable", typeof(ShockerModuleSettingView), [])
    {
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class Shocker : ICloneable, IEquatable<Shocker>
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Shocker");

    [JsonProperty("sharecode")]
    public Observable<string> Sharecode { get; set; } = new(string.Empty);

    [JsonConstructor]
    public Shocker()
    {
    }

    public Shocker(Shocker other)
    {
        Name.Value = other.Name.Value;
        Sharecode.Value = other.Sharecode.Value;
    }

    public object Clone() => new Shocker(this);

    public bool Equals(Shocker? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Sharecode.Equals(other.Sharecode);
    }
}
