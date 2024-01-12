// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using VRCOSC.Graphics.UI.Text;
using VRCOSC.SDK.Attributes.Settings;
using VRCOSC.SDK.Graphics.Settings.Lists;

namespace VRCOSC.Modules.PiShock;

public class Group : IEquatable<Group>
{
    [JsonProperty("keys")]
    public Bindable<string> Names = new(string.Empty);

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

public partial class DrawableGroup : DrawableListModuleSettingItem<Group>
{
    public DrawableGroup(Group item)
        : base(item)
    {
        StringTextBox positionTextBox;

        Add(new GridContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            ColumnDimensions = new[]
            {
                new Dimension(maxSize: 50),
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
                    positionTextBox = new StringTextBox
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 35,
                        ReadOnly = true
                    },
                    null,
                    new StringTextBox
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        RelativeSizeAxes = Axes.X,
                        Height = 35,
                        ValidCurrent = Item.Names.GetBoundCopy(),
                        PlaceholderText = "Name,Name2,Name3",
                        EmptyIsValid = false
                    }
                }
            }
        });

        OnPositionChanged += index => Scheduler.Add(() => positionTextBox.Current.Value = index.ToString());
    }
}

public class GroupListModuleSetting : ListModuleSetting<Group>
{
    public GroupListModuleSetting(ListModuleSettingMetadata metadata, IEnumerable<Group> defaultValues)
        : base(metadata, defaultValues)
    {
    }

    protected override Group CloneValue(Group value) => new(value);
    protected override Group ConstructValue(JToken token) => token.ToObject<Group>()!;
    protected override Group CreateNewItem() => new();
}

public partial class DrawableGroupListModuleSetting : DrawableListModuleSetting<GroupListModuleSetting, Group>
{
    public DrawableGroupListModuleSetting(GroupListModuleSetting moduleSetting)
        : base(moduleSetting)
    {
    }
}
