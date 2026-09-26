// Copyright (c) VolcanicArts. Licensed under the GPL-3.0 License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics;
using VRCOSC.App.SDK.Modules;
using VRCOSC.App.SDK.Parameters;
using VRCOSC.App.SDK.Providers.Weather;
using VRCOSC.App.SDK.VRChat;

namespace VRCOSC.Modules.Weather;

[ModuleTitle("Weather")]
[ModuleDescription("Retrieves weather information for a specific area")]
[ModuleType(ModuleType.Generic)]
[ModuleInfo("https://vrcosc.com/docs/V2/Modules/weather")]
public class WeatherModule : Module
{
    private WeatherProvider? weatherProvider;

    public CurrentWeather? CurrentWeather { get; private set; }

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
        CreateVariable<float>(WeatherVariable.FeelsLikeC, "Feels Like C");
        CreateVariable<float>(WeatherVariable.FeelsLikeF, "Feels Like F");
        CreateVariable<float>(WeatherVariable.WindKph, "Wind KPH");
        CreateVariable<float>(WeatherVariable.WindMph, "Wind MPH");
        CreateVariable<string>(WeatherVariable.WindDir, "Wind Direction");
        CreateVariable<string>(WeatherVariable.Emoji, "Emoji");

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

    protected override void OnAvatarChange(Avatar? avatar)
    {
        updateParameters();
    }

    [ModuleUpdate(ModuleUpdateMode.Custom, true, 60000)]
    private async void updateParameters()
    {
        Debug.Assert(weatherProvider is not null);

        CurrentWeather = await weatherProvider.RetrieveFor(GetSettingValue<string>(WeatherSetting.Location)!, DateTime.Now);

        if (CurrentWeather is null)
        {
            Log("Cannot retrieve weather for provided location");
            Log("This could mean the weather service is down. Please wait for it to return");
            return;
        }

        SendParameter(WeatherParameter.Code, getConvertedWeatherCode(CurrentWeather));

        SetVariableValue(WeatherVariable.TempC, CurrentWeather.TempC);
        SetVariableValue(WeatherVariable.TempF, CurrentWeather.TempF);
        SetVariableValue(WeatherVariable.Humidity, CurrentWeather.Humidity);
        SetVariableValue(WeatherVariable.Condition, CurrentWeather.ConditionString);
        SetVariableValue(WeatherVariable.FeelsLikeC, CurrentWeather.FeelsLikeC);
        SetVariableValue(WeatherVariable.FeelsLikeF, CurrentWeather.FeelsLikeF);
        SetVariableValue(WeatherVariable.WindKph, CurrentWeather.WindKph);
        SetVariableValue(WeatherVariable.WindMph, CurrentWeather.WindMph);
        SetVariableValue(WeatherVariable.WindDir, CurrentWeather.WindDir);
        SetVariableValue(WeatherVariable.Emoji, getWeatherEmoji(CurrentWeather));
    }

    private static string getWeatherEmoji(CurrentWeather currentWeather)
    {
        var isDay = currentWeather.IsDay == 1;

        return currentWeather.Condition.Code switch
        {
            1000 => isDay ? "\u2600\uFE0F" : "\U0001F319",
            1003 => isDay ? "\u26C5" : "\U0001F319\u2601\uFE0F",
            1006 => "\u2601\uFE0F",
            1009 => "\u2601\uFE0F",
            1030 or 1135 or 1147 => "\U0001F32B\uFE0F",
            1063 or 1150 or 1153 or 1180 or 1183 or 1240 => isDay ? "\U0001F326\uFE0F" : "\U0001F327\uFE0F",
            1186 or 1189 or 1192 or 1195 or 1243 or 1246 => "\U0001F327\uFE0F",
            1072 or 1168 or 1171 or 1198 or 1201 => "\U0001F327\uFE0F\u2744\uFE0F",
            1069 or 1204 or 1207 or 1237 or 1249 or 1252 or 1261 or 1264 => "\U0001F328\uFE0F",
            1066 or 1114 or 1117 or 1210 or 1213 or 1216 or 1219 or 1222 or 1225 or 1255 or 1258 => "\u2744\uFE0F",
            1087 or 1273 or 1276 => "\u26C8\uFE0F",
            1279 or 1282 => "\u26C8\uFE0F\u2744\uFE0F",
            _ => "\u2753"
        };
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
        Condition,
        FeelsLikeC,
        FeelsLikeF,
        WindKph,
        WindMph,
        WindDir,
        Emoji
    }
}