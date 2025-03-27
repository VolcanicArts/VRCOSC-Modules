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
        : base("Shockers", "These are your individual shockers. Name them something you'll understand as these names are used in groups and phrases", typeof(ShockerModuleSettingView), [])
    {
    }

    protected override Shocker CreateItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class Shocker : IEquatable<Shocker>
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

    public bool Equals(Shocker? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Sharecode.Equals(other.Sharecode);
    }
}