// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.KAT;

[Node("Set KAT Text")]
public sealed class SetTextKATNode : ModuleNode<KATModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<string> Text = new();

    protected override Task Process(PulseContext c)
    {
        Module.TargetText = Text.Read(c);
        return Next.Execute(c);
    }
}

[Node("Set KAT Visibility")]
public sealed class SetVisibilityKATNode : ModuleNode<KATModule>, IFlowInput
{
    public FlowContinuation Next = new("Next");

    public ValueInput<bool> Visible = new();

    protected override Task Process(PulseContext c)
    {
        Module.SetVisiblity(Visible.Read(c));
        return Next.Execute(c);
    }
}