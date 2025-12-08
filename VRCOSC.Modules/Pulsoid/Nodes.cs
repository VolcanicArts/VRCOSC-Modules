// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Pulsoid;

[Node("Pulsoid Source")]
[NodeForceReprocess]
public sealed class PulsoidSourceNode : ModuleNode<PulsoidModule>, IActiveUpdateNode
{
    public int UpdateOffset => 0;
    public ValueOutput<int> Heartrate = new();

    protected override Task Process(PulseContext c)
    {
        Heartrate.Write(Module.TargetValue, c);
        return Task.CompletedTask;
    }

    public Task<bool> OnUpdate(PulseContext c) => Task.FromResult(true);
}