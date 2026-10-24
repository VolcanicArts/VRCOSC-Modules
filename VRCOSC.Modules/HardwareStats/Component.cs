// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using LibreHardwareMonitor.Hardware;

namespace VRCOSC.Modules.HardwareStats;

public class Component
{
    public string Id { get; }
    public string Name { get; }
    public ComponentType Type { get; }
    public Dictionary<SensorType, Sensor[]> Sensors { get; } = [];

    public Component(string id, string name, ComponentType type)
    {
        Id = id;
        Name = name;
        Type = type;
    }

    public void Init(IHardware hardware)
    {
        foreach (var group in hardware.Sensors.GroupBy(sensor => sensor.SensorType))
        {
            var sensorType = group.Key.ToGeneric;
            var sensors = group.ToArray();

            var sensorList = new List<Sensor>();
            var maxIndex = 0;

            foreach (var sensor in sensors)
            {
                var id = sensor.Identifier.ToString();
                var index = int.Parse(id[(id.LastIndexOf('/') + 1)..]);
                maxIndex = int.Max(index, maxIndex);
                sensorList.Add(new Sensor(index, sensor.Name, sensorType));
            }

            var sensorArray = new Sensor[maxIndex + 1];
            Array.Fill(sensorArray, new Sensor());

            foreach (var sensor in sensorList)
            {
                sensorArray[sensor.Index] = sensor;
            }

            Sensors[sensorType] = sensorArray;
        }
    }

    public void Update(IHardware hardware)
    {
        foreach (var sensor in hardware.Sensors)
        {
            var sensorType = sensor.SensorType.ToGeneric;
            var id = sensor.Identifier.ToString();
            var index = int.Parse(id[(id.LastIndexOf('/') + 1)..]);
            Sensors[sensorType][index].Value = sensor.Value.GetValueOrDefault(0f);
        }
    }
}