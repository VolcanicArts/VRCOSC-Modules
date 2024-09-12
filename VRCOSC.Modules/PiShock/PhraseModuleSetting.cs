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
}

[JsonObject(MemberSerialization.OptIn)]
public class Phrase : ICloneable, IEquatable<Phrase>
{
    [JsonProperty("name")]
    public Observable<string> Name { get; } = new("New Phrase");

    [JsonProperty("text")]
    public Observable<string> Text { get; } = new(string.Empty);

    [JsonProperty("mode")]
    public Observable<PiShockMode> Mode { get; } = new();

    [JsonProperty("duration")]
    public Observable<int> Duration { get; } = new();

    [JsonProperty("intensity")]
    public Observable<int> Intensity { get; } = new();

    [JsonProperty("shocker_groups")]
    public ObservableCollection<string> ShockerGroups { get; } = new();

    public Phrase()
    {
    }

    private Phrase(Phrase other)
    {
        Text.Value = other.Text.Value;
    }

    public object Clone() => new Phrase(this);

    public bool Equals(Phrase? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Text.Equals(other.Text) && ShockerGroups.SequenceEqual(other.ShockerGroups);
    }

    public override bool Equals(object? obj) => obj is Phrase phrase && Equals(phrase);
}
