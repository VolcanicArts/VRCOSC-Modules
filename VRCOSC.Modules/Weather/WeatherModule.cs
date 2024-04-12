// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Providers.Weather;

namespace VRCOSC.Modules.Weather;

[ModuleTitle("Weather")]
[ModuleDescription("Retrieves weather information for a specific area")]
[ModuleType(ModuleType.Generic)]
public class WeatherModule : ChatBoxModule
{
    private WeatherProvider? weatherProvider;

    protected override void OnPreLoad()
    {
        CreateTextBox(WeatherSetting.Location, "Location", "The location to retrieve weather data for\nThis can be a city name, UK/US/Canada postcode, or IP address", string.Empty);

        RegisterParameter<int>(WeatherParameter.Code, "VRCOSC/Weather/Code", ParameterMode.Write, "Weather Code", "The current weather's code");
    }

    protected override void OnPostLoad()
    {
        var tempCReference = CreateVariable<float>(WeatherVariable.TempC, "Temp C")!;
        var tempFReference = CreateVariable<float>(WeatherVariable.TempF, "Temp F")!;
        CreateVariable<int>(WeatherVariable.Humidity, "Humidity");
        var conditionReference = CreateVariable<string>(WeatherVariable.Condition, "Condition")!;

        CreateState(WeatherState.Default, "Default", "Local Weather\n{0}\n{1}C - {2}F", new[] { conditionReference, tempCReference, tempFReference });
    }

    protected override Task<bool> OnModuleStart()
    {
        if (string.IsNullOrEmpty(GetSettingValue<string>(WeatherSetting.Location)))
        {
            Log("Please provide a post/zip code or city name");
            return Task.FromResult(false);
        }

        weatherProvider ??= new WeatherProvider(OfficialModuleSecrets.GetSecret(OfficialModuleSecretsKeys.Weather));

        ChangeState(WeatherState.Default);

        return Task.FromResult(true);
    }

    protected override void OnAvatarChange()
    {
        updateParameters();
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 60000)]
    private async void updateParameters()
    {
        Debug.Assert(weatherProvider is not null);

        var weather = await weatherProvider.RetrieveFor(GetSettingValue<string>(WeatherSetting.Location)!, DateTime.Now);

        if (weather is null)
        {
            Log("Cannot retrieve weather for provided location");
            return;
        }

        SendParameter(WeatherParameter.Code, getConvertedWeatherCode(weather));

        SetVariableValue(WeatherVariable.TempC, weather.TempC);
        SetVariableValue(WeatherVariable.TempF, weather.TempF);
        SetVariableValue(WeatherVariable.Humidity, weather.Humidity);
        SetVariableValue(WeatherVariable.Condition, weather.ConditionString);
    }

    private static int getConvertedWeatherCode(CurrentWeather currentWeather) => currentWeather.Condition.Code switch
    {
        1000 => 1,
        1003 => 2,
        1006 => 3,
        1009 => 4,
        1030 => 5,
        1063 => 6,
        1066 => 7,
        1069 => 8,
        1072 => 9,
        1087 => 10,
        1114 => 11,
        1117 => 12,
        1135 => 13,
        1147 => 14,
        1150 => 15,
        1153 => 16,
        1168 => 17,
        1171 => 18,
        1180 => 19,
        1183 => 20,
        1186 => 21,
        1189 => 22,
        1192 => 23,
        1195 => 24,
        1198 => 25,
        1201 => 26,
        1204 => 27,
        1207 => 28,
        1210 => 29,
        1213 => 30,
        1216 => 31,
        1219 => 32,
        1222 => 33,
        1225 => 34,
        1237 => 35,
        1240 => 36,
        1243 => 37,
        1246 => 38,
        1249 => 39,
        1252 => 40,
        1255 => 41,
        1258 => 42,
        1261 => 43,
        1264 => 44,
        1273 => 45,
        1276 => 46,
        1279 => 47,
        1282 => 48,
        _ => 0
    };

    private enum WeatherSetting
    {
        Location
    }

    private enum WeatherParameter
    {
        Code
    }

    private enum WeatherState
    {
        Default
    }

    private enum WeatherVariable
    {
        TempC,
        TempF,
        Humidity,
        Condition
    }
}
