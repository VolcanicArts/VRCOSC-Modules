// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.ParameterSync.UI;

namespace VRCOSC.Modules.ParameterSync;

public class ParameterSyncListModuleSetting : ListModuleSetting<ParameterSync>
{
    public ParameterSyncListModuleSetting()
        : base("Sync Instances", "Add, edit, and remove sync instances", typeof(ParameterSyncListModuleSettingView), [])
    {
    }

    protected override ParameterSync CreateItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class ParameterSync : IEquatable<ParameterSync>
{
    [JsonProperty("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Sync Instance");

    [JsonProperty("avatars")]
    public ObservableCollection<Observable<string>> Avatars { get; set; } = [];

    [JsonProperty("parameters")]
    public ObservableCollection<Observable<string>> Parameters { get; set; } = [];

    public bool Equals(ParameterSync? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Avatars.Equals(other.Avatars) && Parameters.Equals(other.Parameters);
    }
}