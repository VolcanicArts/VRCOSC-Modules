// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using VRCOSC.Graphics.UI.Text;
using VRCOSC.SDK.Attributes.Settings;
using VRCOSC.SDK.Graphics.Settings.Lists;

namespace VRCOSC.Modules.Counter;

public class Milestone : IEquatable<Milestone>
{
    [JsonProperty("counter_key")]
    public Bindable<string> CounterKey = new(string.Empty);

    [JsonProperty("required_count")]
    public Bindable<int> RequiredCount = new();

    [JsonProperty("parameter_name")]
    public Bindable<string> ParameterName = new(string.Empty);

    [JsonConstructor]
    public Milestone()
    {
    }

    public Milestone(Milestone other)
    {
        CounterKey.Value = other.CounterKey.Value;
        RequiredCount.Value = other.RequiredCount.Value;
        ParameterName.Value = other.ParameterName.Value;
    }

    public bool Equals(Milestone? other)
    {
        if (ReferenceEquals(null, other)) return false;

        return CounterKey.Value.Equals(other.CounterKey.Value) && RequiredCount.Value.Equals(other.RequiredCount.Value) && ParameterName.Value.Equals(other.ParameterName.Value);
    }
}

public partial class DrawableMilestone : DrawableListModuleSettingItem<Milestone>
{
    public DrawableMilestone(Milestone item)
        : base(item)
    {
        Add(new GridContainer
        {
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            ColumnDimensions = new[]
            {
                new Dimension(),
                new Dimension(GridSizeMode.Absolute, 5),
                new Dimension(),
                new Dimension(GridSizeMode.Absolute, 5),
                new Dimension()
            },
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize)
            },
            Content = new[]
            {
                new Drawable?[]
                {
                    new StringTextBox
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 35,
                        ValidCurrent = item.CounterKey.GetBoundCopy(),
                        PlaceholderText = "Counter Key",
                        EmptyIsValid = false
                    },
                    null,
                    new IntTextBox
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 35,
                        ValidCurrent = item.RequiredCount.GetBoundCopy(),
                        PlaceholderText = "Required Count",
                    },
                    null,
                    new StringTextBox
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 35,
                        ValidCurrent = item.ParameterName.GetBoundCopy(),
                        PlaceholderText = "Parameter Name",
                        EmptyIsValid = true
                    }
                }
            }
        });
    }
}

public class MilestoneListModuleSetting : ListModuleSetting<Milestone>
{
    public MilestoneListModuleSetting(ListModuleSettingMetadata metadata, IEnumerable<Milestone> defaultValues)
        : base(metadata, defaultValues)
    {
    }

    protected override Milestone CloneValue(Milestone value) => new(value);
    protected override Milestone ConstructValue(JToken token) => token.ToObject<Milestone>()!;
    protected override Milestone CreateNewItem() => new();
}

public partial class DrawableMilestoneList : DrawableListModuleSetting<MilestoneListModuleSetting, Milestone>
{
    public DrawableMilestoneList(MilestoneListModuleSetting moduleSetting)
        : base(moduleSetting)
    {
    }

    protected override Drawable Header => new GridContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        ColumnDimensions = new[]
        {
            new Dimension(),
            new Dimension(GridSizeMode.Absolute, 5),
            new Dimension(),
            new Dimension(GridSizeMode.Absolute, 5),
            new Dimension()
        },
        RowDimensions = new[]
        {
            new Dimension(GridSizeMode.AutoSize)
        },
        Content = new[]
        {
            new Drawable?[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Child = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "Parameter Name",
                        Font = FrameworkFont.Regular.With(size: 20)
                    }
                },
                null,
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Child = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "Counter Key",
                        Font = FrameworkFont.Regular.With(size: 20)
                    }
                },
                null,
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Child = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = "Required Count",
                        Font = FrameworkFont.Regular.With(size: 20)
                    }
                }
            }
        }
    };
}
