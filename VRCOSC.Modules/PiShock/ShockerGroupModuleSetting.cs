﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.PiShock.UI;

namespace VRCOSC.Modules.PiShock;

public class ShockerGroupModuleSetting : ListModuleSetting<ShockerGroup>
{
    public ShockerGroupModuleSetting()
        : base("Groups", "Create, edit, and delete shocker groups", typeof(ShockerGroupModuleSettingView), [])
    {
    }
}

[JsonObject(MemberSerialization.OptIn)]
public class ShockerGroup : ICloneable, IEquatable<ShockerGroup>
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("My New Group");

    [JsonProperty("shockers")]
    public ObservableCollection<Observable<string>> Shockers { get; set; } = new();

    [JsonProperty("max_duration")]
    public Observable<int> MaxDuration { get; set; } = new(15);

    [JsonProperty("max_intensity")]
    public Observable<int> MaxIntensity { get; set; } = new(100);

    [JsonConstructor]
    public ShockerGroup()
    {
    }

    public ShockerGroup(ShockerGroup other)
    {
        Name.Value = other.Name.Value;
        Shockers.AddRange(other.Shockers.Select(shocker => new Observable<string>(shocker.Value)));
        MaxDuration.Value = other.MaxDuration.Value;
        MaxIntensity.Value = other.MaxIntensity.Value;
    }

    public object Clone() => new ShockerGroup(this);

    public bool Equals(ShockerGroup? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Shockers.SequenceEqual(other.Shockers) && MaxDuration.Equals(other.MaxDuration) && MaxIntensity.Equals(other.MaxIntensity);
    }
}
