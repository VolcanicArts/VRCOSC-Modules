// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Pulsoid;

[Node("Pulsoid Source")]
[NodeForceReprocess]
public sealed class PulsoidSourceNode() : ValueSourceNode<int>("Heartrate"), IModuleNode<PulsoidModule>
{
    public PulsoidModule Module { get; set; } = null!;

    protected override int ComputeValue(IPulseContext c) => Module.TargetValue;
}