﻿// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.SDK.Parameters.Queryable;
using VRCOSC.App.SDK.Utils;
using VRCOSC.App.Utils;
using VRCOSC.Modules.Keybinds.UI;

namespace VRCOSC.Modules.Keybinds;

public class KeybindsModuleSetting : ListModuleSetting<KeybindsInstance>
{
    public KeybindsModuleSetting()
        : base("Keybinds", "Create, edit, and delete keybinds", typeof(KeybindsModuleSettingView), [])
    {
    }

    protected override KeybindsInstance CreateItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class KeybindsInstance : IEquatable<KeybindsInstance>
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Keybind");

    [JsonProperty("parameters")]
    public QueryableParameterList Parameters { get; set; } = new(typeof(ActionableQueryableParameter<KeybindAction>));

    [JsonProperty("keybinds")]
    public ObservableCollection<Observable<Keybind>> Keybinds { get; set; } = [];

    [JsonConstructor]
    public KeybindsInstance()
    {
    }

    public bool Equals(KeybindsInstance? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Parameters.Parameters.SequenceEqual(other.Parameters.Parameters) && Keybinds.SequenceEqual(other.Keybinds);
    }
}