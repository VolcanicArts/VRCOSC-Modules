// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

namespace VRCOSC.Modules.HardwareStats;

public class Sensor
{
    public int Index { get; }
    public string Name { get; }
    public SensorType Type { get; }
    public float Value { get; set; }

    public Sensor()
    {
        Index = -1;
        Name = "BLANK";
        Type = SensorType.Unknown;
    }

    public Sensor(int index, string name, SensorType type)
    {
        Index = index;
        Name = name;
        Type = type;
    }
}
