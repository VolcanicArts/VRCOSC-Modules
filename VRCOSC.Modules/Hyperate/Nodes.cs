// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Hyperate;

[Node("Hyperate Source")]
public sealed class HyperateSourceNode() : ValueSourceNode<int>("Heartrate"), IModuleNode<HypeRateModule>
{
    public HypeRateModule Module { get; set; } = null!;

    protected override int ComputeValue(IPulseContext c) => Module.TargetValue;
}