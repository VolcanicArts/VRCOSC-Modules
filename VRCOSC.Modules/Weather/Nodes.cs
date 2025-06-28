// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using VRCOSC.App.Nodes;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Weather;

[Node("Weather Source")]
public sealed class WeatherSourceNode : ModuleNode<WeatherModule>
{
    public ValueOutput<int> Code = new();
    public ValueOutput<float> TempC = new();
    public ValueOutput<float> TempF = new();
    public ValueOutput<int> Humidity = new();
    public ValueOutput<string> Condition = new();

    protected override Task Process(PulseContext c)
    {
        var currentWeather = Module.CurrentWeather;
        if (currentWeather is null) return Task.CompletedTask;

        Code.Write(currentWeather.Condition.Code, c);
        TempC.Write(currentWeather.TempC, c);
        TempF.Write(currentWeather.TempF, c);
        Humidity.Write(currentWeather.Humidity, c);
        Condition.Write(currentWeather.ConditionString, c);
        return Task.CompletedTask;
    }
}