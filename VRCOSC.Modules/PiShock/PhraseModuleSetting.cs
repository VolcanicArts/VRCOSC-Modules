// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.SDK.Providers.PiShock;
using VRCOSC.App.Utils;
using VRCOSC.Modules.PiShock.UI;

namespace VRCOSC.Modules.PiShock;

public class PhraseModuleSetting : ListModuleSetting<Phrase>
{
    public PhraseModuleSetting()
        : base("Phrases", "Execute groups when a phrase is said", typeof(PhraseModuleSettingView), [])
    {
    }

    protected override Phrase CreateItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class Phrase : IEquatable<Phrase>
{
    [JsonProperty("name")]
    public Observable<string> Name { get; } = new("New Phrase");

    [JsonProperty("text")]
    public Observable<string> Text { get; } = new(string.Empty);

    [JsonProperty("mode")]
    public Observable<PiShockMode> Mode { get; } = new();

    [JsonProperty("duration")]
    public Observable<int> Duration { get; } = new(15);

    [JsonProperty("intensity")]
    public Observable<int> Intensity { get; } = new(100);

    [JsonProperty("shocker_groups")]
    public ObservableCollection<Observable<string>> ShockerGroups { get; } = [];

    [JsonConstructor]
    public Phrase()
    {
    }

    public bool Equals(Phrase? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Text.Equals(other.Text) && Mode.Equals(other.Mode) && ShockerGroups.SequenceEqual(other.ShockerGroups) && Duration.Equals(other.Duration) && Intensity.Equals(other.Intensity);
    }

    public override bool Equals(object? obj) => obj is Phrase phrase && Equals(phrase);
}