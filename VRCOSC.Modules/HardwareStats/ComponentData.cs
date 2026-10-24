// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using UnitsNet;

namespace VRCOSC.Modules.HardwareStats;

public class ComponentData
{
    public string Id { get; private set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; } = true;
    public ComponentType Type { get; set; }
    public List<ComponentDataMapping> Mappings { get; init; } = [];

    public ComponentData(Component component)
    {
        Id = component.Id;
        Name = component.Name;
        Type = component.Type;
    }
}

public class ComponentDataMapping
{
    public SensorType Type { get; set; }
    public UnitInfo Unit { get; set; }
    public int Index { get; set; }
    public string Name { get; set; }

    public ComponentDataMapping(SensorType type, UnitInfo unit, int index, string name)
    {
        Type = type;
        Unit = unit;
        Index = index;
        Name = name;
    }
}