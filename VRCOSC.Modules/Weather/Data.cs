// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using UnitsNet;
using UnitsNet.Units;

namespace VRCOSC.Modules.Weather;

public sealed class Weather
{
    public int Code { get; }
    public string Condition { get; }
    public IQuantity<TemperatureUnit> Temperature { get; }
    public IQuantity<TemperatureUnit> FeelsLike { get; }
    public IQuantity<RelativeHumidityUnit> Humidity { get; }
    public IQuantity<SpeedUnit> WindSpeed { get; }
    public IQuantity<TemperatureUnit> WindChill { get; }
    public string WindDirection { get; }
    public IQuantity<PressureUnit> Pressure { get; }
    public IQuantity<LengthUnit> Precipitation { get; }
    public IQuantity<LengthUnit> Visibility { get; }
    public Sun Sun { get; }
    public Moon Moon { get; }

    public Weather(CurrentWeather weather, Astro astronomy)
    {
        Code = weather.Condition.Code;
        Condition = weather.ConditionString;
        Temperature = UnitsNet.Temperature.FromDegreesCelsius(weather.TempC);
        FeelsLike = UnitsNet.Temperature.FromDegreesCelsius(weather.FeelsLikeC);
        Humidity = RelativeHumidity.FromPercent(weather.Humidity);
        WindSpeed = Speed.FromKilometersPerHour(weather.WindKph);
        WindChill = UnitsNet.Temperature.FromDegreesCelsius(weather.WindChillC);
        WindDirection = weather.WindDirection;
        Pressure = UnitsNet.Pressure.FromMillibars(weather.PressureMb);
        Precipitation = Length.FromMillimeters(weather.PrecipitationMm);
        Visibility = Length.FromKilometers(weather.VisibilityKm);
        Sun = new Sun(DateTime.Parse(astronomy.Sunrise), DateTime.Parse(astronomy.Sunset), astronomy.IsSunUp == 1);
        Moon = new Moon(DateTime.Parse(astronomy.Moonrise), DateTime.Parse(astronomy.Moonset), astronomy.IsMoonUp == 1, astronomy.MoonPhase);
    }
}

public abstract class CelestialBody
{
    public DateTime Rise { get; }
    public DateTime Set { get; }
    public bool IsUp { get; }

    protected CelestialBody(DateTime rise, DateTime set, bool isUp)
    {
        Rise = rise;
        Set = set;
        IsUp = isUp;
    }
}

public sealed class Sun : CelestialBody
{
    public Sun(DateTime rise, DateTime set, bool isUp)
        : base(rise, set, isUp)
    {
    }
}

public sealed class Moon : CelestialBody
{
    public string Phase { get; }

    public Moon(DateTime rise, DateTime set, bool isUp, string phase)
        : base(rise, set, isUp)
    {
        Phase = phase;
    }
}