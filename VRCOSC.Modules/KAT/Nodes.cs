// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.KAT;

[Node("Set KAT Text")]
public sealed class SetTextKATNode() : ActionValueConsumeNode<string>("Text"), IModuleNode<KATModule>
{
    public KATModule Module { get; set; } = null!;

    protected override void ConsumeValue(string text, IPulseContext c) => Module.TargetText = text;
}

[Node("Set KAT Visibility")]
public sealed class SetVisibilityKATNode() : ActionValueConsumeNode<bool>("Visible"), IModuleNode<KATModule>
{
    public KATModule Module { get; set; } = null!;

    protected override void ConsumeValue(bool visible, IPulseContext c) => Module.SetVisiblity(visible);
}