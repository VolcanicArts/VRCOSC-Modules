// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Collections.ObjectModel;
using Newtonsoft.Json;
using VRCOSC.App.SDK.Modules.Attributes.Settings;
using VRCOSC.App.Utils;
using VRCOSC.Modules.VoiceCommands.UI;

namespace VRCOSC.Modules.VoiceCommands;

public class PhraseModuleSetting : ListModuleSetting<Phrase>
{
    public PhraseModuleSetting()
        : base("Phrases", "The phrases to recognise and drive avatar parameters", typeof(PhraseModuleSettingView), [])
    {
    }

    protected override Phrase CreateItem() => new();
}

[JsonObject(MemberSerialization.OptIn)]
public class Phrase : IEquatable<Phrase>
{
    [JsonProperty("name")]
    public Observable<string> Name { get; set; } = new("New Phrase");

    [JsonProperty("text")]
    public Observable<string> Text { get; set; } = new(string.Empty);

    [JsonProperty("parameters")]
    public ObservableCollection<Parameter> Parameters { get; set; } = [];

    [JsonConstructor]
    public Phrase()
    {
    }

    public bool Equals(Phrase? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name.Equals(other.Name) && Text.Equals(other.Text) && Parameters.SequenceEqual(other.Parameters);
    }
}