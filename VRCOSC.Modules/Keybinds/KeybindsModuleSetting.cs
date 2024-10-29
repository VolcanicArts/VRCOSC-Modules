// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
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
}

[JsonObject(MemberSerialization.OptIn)]
public class KeybindsInstance : ICloneable, IEquatable<KeybindsInstance>
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Keybind");

    [JsonProperty("mode")]
    public Observable<KeybindInstanceMode> Mode { get; set; } = new(KeybindInstanceMode.Press);

    [JsonProperty("parameter_names")]
    public ObservableCollection<Observable<string>> ParameterNames { get; set; } = [];

    [JsonProperty("keybinds")]
    public ObservableCollection<Observable<Keybind>> Keybinds { get; set; } = [];

    [JsonConstructor]
    public KeybindsInstance()
    {
    }

    public KeybindsInstance(KeybindsInstance other)
    {
        Name.Value = other.Name.Value;
        ParameterNames.AddRange(other.ParameterNames.Select(parameter => new Observable<string>(parameter.Value)));
        Keybinds.AddRange(other.Keybinds.Select(keybind => new Observable<Keybind>(keybind.Value)));
    }

    public object Clone() => new KeybindsInstance(this);

    public bool Equals(KeybindsInstance? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Mode.Equals(other.Mode) && ParameterNames.SequenceEqual(other.ParameterNames) && Keybinds.SequenceEqual(other.Keybinds);
    }
}

public enum KeybindInstanceMode
{
    Press,
    HoldRelease
}