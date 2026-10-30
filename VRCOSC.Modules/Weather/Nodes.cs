// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using UnitsNet;
using UnitsNet.Units;
using VRCOSC.App.Nodes;
using VRCOSC.App.Nodes.Types;
using VRCOSC.App.SDK.Nodes;

namespace VRCOSC.Modules.Weather;

[Node("Weather Source")]
public sealed class WeatherSourceNode : ValueSourceNode<Weather?>, IModuleNode<WeatherModule>
{
    public WeatherModule Module { get; set; } = null!;

    protected override Weather? ComputeValue(IPulseContext c) => Module.CurrentWeather;
}

public abstract class WeatherConsumeBase : ValueConsumeNode<Weather?>, IModuleNode<WeatherModule>
{
    public WeatherModule Module { get; set; } = null!;

    protected override void ConsumeValue(Weather? weather, IPulseContext c)
    {
        if (weather is null) return;

        ConsumeWeather(weather, c);
    }

    protected abstract void ConsumeWeather(Weather weather, IPulseContext c);
}

[Node("Temperature", "Data")]
public sealed class WeatherTemperatureNode : WeatherConsumeBase
{
    public ValueOutput<IQuantity<TemperatureUnit>> Temperature = new();
    public ValueOutput<IQuantity<TemperatureUnit>> FeelsLike = new();
    public ValueOutput<IQuantity<TemperatureUnit>> WindChill = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Temperature.Write(weather.Temperature, c);
        FeelsLike.Write(weather.FeelsLike, c);
        WindChill.Write(weather.WindChill, c);
    }
}

[Node("Humidity", "Data")]
public sealed class WeatherHumidityNode : WeatherConsumeBase
{
    public ValueOutput<IQuantity<RelativeHumidityUnit>> Humidity = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Humidity.Write(weather.Humidity, c);
    }
}

[Node("Wind", "Data")]
public sealed class WeatherWindNode : WeatherConsumeBase
{
    public ValueOutput<IQuantity<SpeedUnit>> Speed = new();
    public ValueOutput<string> Direction = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Speed.Write(weather.WindSpeed, c);
        Direction.Write(weather.WindDirection, c);
    }
}

[Node("Atmosphere", "Data")]
public sealed class WeatherAtmosphereNode : WeatherConsumeBase
{
    public ValueOutput<IQuantity<PressureUnit>> Pressure = new();
    public ValueOutput<IQuantity<LengthUnit>> Visibility = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Pressure.Write(weather.Pressure, c);
        Visibility.Write(weather.Visibility, c);
    }
}

[Node("Rain", "Data")]
public sealed class WeatherRainNode : WeatherConsumeBase
{
    public ValueOutput<IQuantity<LengthUnit>> Precipitation = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Precipitation.Write(weather.Precipitation, c);
    }
}

[Node("Sun", "Data")]
public sealed class WeatherSunNode : WeatherConsumeBase
{
    public ValueOutput<DateTime> Rise = new();
    public ValueOutput<DateTime> Set = new();
    public ValueOutput<bool> IsUp = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Rise.Write(weather.Sun.Rise, c);
        Set.Write(weather.Sun.Set, c);
        IsUp.Write(weather.Sun.IsUp, c);
    }
}

[Node("Moon", "Data")]
public sealed class WeatherMoonNode : WeatherConsumeBase
{
    public ValueOutput<DateTime> Rise = new();
    public ValueOutput<DateTime> Set = new();
    public ValueOutput<bool> IsUp = new();
    public ValueOutput<string> Phase = new();

    protected override void ConsumeWeather(Weather weather, IPulseContext c)
    {
        Rise.Write(weather.Moon.Rise, c);
        Set.Write(weather.Moon.Set, c);
        IsUp.Write(weather.Moon.IsUp, c);
        Phase.Write(weather.Moon.Phase, c);
    }
}