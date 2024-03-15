// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRCOSC.App.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.PiShock;

public class Shocker : IEquatable<Shocker>
{
    // TODO: Find a way to get custom things triggering serialisation

    [JsonProperty("name")]
    public Observable<string> Name = new(string.Empty);

    [JsonProperty("sharecode")]
    public Observable<string> Sharecode = new(string.Empty);

    [JsonConstructor]
    public Shocker()
    {
    }

    public Shocker(Shocker other)
    {
        Name.Value = other.Name.Value;
        Sharecode.Value = other.Sharecode.Value;
    }

    public bool Equals(Shocker? other)
    {
        if (ReferenceEquals(null, other)) return false;

        return Name.Value.Equals(other.Name.Value) && Sharecode.Value.Equals(other.Sharecode.Value);
    }
}

public class ShockerListModuleSetting : ListModuleSetting<Shocker>
{
    public ShockerListModuleSetting(ModuleSettingMetadata metadata, IEnumerable<Shocker> defaultValues)
        : base(metadata, defaultValues, false)
    {
    }

    protected override Shocker CloneValue(Shocker value) => new(value);
    protected override Shocker ConstructValue(JToken token) => token.ToObject<Shocker>()!;
    protected override Shocker CreateNewItem() => new();
}
