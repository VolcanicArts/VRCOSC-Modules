// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    protected override Shocker CloneValue(Shocker value) => new(value);

    protected override Shocker ConstructValue(JToken token) => token.ToObject<Shocker>()!;

    protected override Shocker CreateNewItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class Shocker
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
}
