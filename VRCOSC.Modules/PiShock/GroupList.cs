// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRCOSC.App.Modules.Attributes.Settings;
using VRCOSC.App.Utils;

namespace VRCOSC.Modules.PiShock;

public class Group : IEquatable<Group>
{
    [JsonProperty("keys")]
    public Observable<string> Names = new(string.Empty);

    [JsonConstructor]
    public Group()
    {
    }

    public Group(Group other)
    {
        Names.Value = other.Names.Value;
    }

    public bool Equals(Group? other)
    {
        if (ReferenceEquals(null, other)) return false;

        return Names.Value.Equals(other.Names.Value);
    }
}

public class GroupListModuleSetting : ListModuleSetting<Group>
{
    public GroupListModuleSetting(ModuleSettingMetadata metadata, IEnumerable<Group> defaultValues)
        : base(metadata, defaultValues, true)
    {
    }

    protected override Group CloneValue(Group value) => new(value);
    protected override Group ConstructValue(JToken token) => token.ToObject<Group>()!;
    protected override Group CreateNewItem() => new();
}
