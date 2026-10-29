// Copyright (c) VolcanicArts. Licensed under the LGPL License.
// See the LICENSE file in the repository root for full license text.

using System.Net.Http;
using Newtonsoft.Json;

namespace VRCOSC.Modules.Weather;

public class WeatherProvider
{
    private const string api_base_url = "https://api.weatherapi.com/v1/";
    private const string current_url_format = api_base_url + "current.json?key={0}&q={1}";
    private const string astronomy_url_format = api_base_url + "astronomy.json?key={0}&q={1}&dt={2}";

    private const string condition_url = "https://www.weatherapi.com/docs/weather_conditions.json";

    private static readonly TimeSpan cache_expire_time = TimeSpan.FromMinutes(20);

    private readonly HttpClient httpClient = new();
    private readonly string apiKey;

    private DateTimeOffset lastUpdate = DateTimeOffset.MinValue;
    private string? lastLocation;
    private Weather? weather;

    private Dictionary<int, WeatherCondition>? conditions;

    public WeatherProvider(string apiKey)
    {
        this.apiKey = apiKey;
    }

    public async Task<Weather?> RetrieveFor(string location, DateTime locationDateTime)
    {
        try
        {
            if (lastUpdate + cache_expire_time > DateTimeOffset.Now && location == lastLocation) return weather;

            lastUpdate = DateTimeOffset.Now;
            lastLocation = location;

            var currentUrl = string.Format(current_url_format, apiKey, location);
            var currentResponseData = await httpClient.GetAsync(new Uri(currentUrl));
            if (!currentResponseData.IsSuccessStatusCode) return weather;

            var currentResponseString = await currentResponseData.Content.ReadAsStringAsync();
            var currentResponse = JsonConvert.DeserializeObject<WeatherCurrentResponse>(currentResponseString)?.Current;
            if (currentResponse is null) return weather;

            var astronomyUrl = string.Format(astronomy_url_format, apiKey, location, DateTime.Now.ToString("yyyy-MM-dd"));
            var astronomyResponseData = await httpClient.GetAsync(new Uri(astronomyUrl));
            if (!astronomyResponseData.IsSuccessStatusCode) return weather;

            var astronomyResponseString = await astronomyResponseData.Content.ReadAsStringAsync();
            var astronomyResponse = JsonConvert.DeserializeObject<WeatherAstronomyResponse>(astronomyResponseString)?.Astronomy.Astro;
            if (astronomyResponse is null) return weather;

            currentResponse.ConditionString = "Unknown";

            if (!DateTime.TryParse(astronomyResponse.Sunrise, out var sunriseParsed)) return weather;
            if (!DateTime.TryParse(astronomyResponse.Sunset, out var sunsetParsed)) return weather;

            await retrieveConditions();

            if (conditions is not null)
            {
                var conditionResponse = conditions[currentResponse.Condition.Code];
                currentResponse.ConditionString = locationDateTime >= sunriseParsed && locationDateTime < sunsetParsed ? conditionResponse.Day : conditionResponse.Night;
            }

            weather = new Weather(currentResponse, astronomyResponse);
        }
        catch (Exception e)
        {
            weather = null;
        }

        return weather;
    }

    private async Task retrieveConditions()
    {
        try
        {
            if (conditions is not null) return;

            var conditionResponseData = await httpClient.GetAsync(condition_url);

            if (!conditionResponseData.IsSuccessStatusCode)
            {
                return;
            }

            var conditionResponseString = await conditionResponseData.Content.ReadAsStringAsync();
            var conditionResponse = JsonConvert.DeserializeObject<List<WeatherCondition>>(conditionResponseString);

            if (conditionResponse is null)
            {
                return;
            }

            conditions = new Dictionary<int, WeatherCondition>();
            conditionResponse.ForEach(condition => conditions.Add(condition.Code, condition));
        }
        catch (Exception e)
        {
            conditions = null;
        }
    }
}

public class WeatherCurrentResponse
{
    [JsonProperty("current")]
    public CurrentWeather Current = null!;
}

public class WeatherAstronomyResponse
{
    [JsonProperty("astronomy")]
    public Astronomy Astronomy = null!;
}

public class CurrentWeather
{
    [JsonProperty("temp_c")]
    public float TempC;

    [JsonProperty("feelslike_c")]
    public float FeelsLikeC;

    [JsonProperty("humidity")]
    public int Humidity;

    [JsonProperty("wind_kph")]
    public float WindKph;

    [JsonProperty("windchill_c")]
    public float WindChillC;

    [JsonProperty("wind_dir")]
    public string WindDirection = string.Empty;

    [JsonProperty("pressure_mb")]
    public float PressureMb;

    [JsonProperty("precip_mm")]
    public float PrecipitationMm;

    [JsonProperty("vis_km")]
    public float VisibilityKm;

    [JsonProperty("condition")]
    public Condition Condition = null!;

    [JsonIgnore]
    public string ConditionString = null!;
}

public class Astronomy
{
    [JsonProperty("astro")]
    public Astro Astro = null!;
}

public class Astro
{
    [JsonProperty("sunrise")]
    public string Sunrise = null!;

    [JsonProperty("sunset")]
    public string Sunset = null!;

    [JsonProperty("moonrise")]
    public string Moonrise = null!;

    [JsonProperty("moonset")]
    public string Moonset = null!;

    [JsonProperty("is_sun_up")]
    public int IsSunUp;

    [JsonProperty("is_moon_up")]
    public int IsMoonUp;

    [JsonProperty("moon_phase")]
    public string MoonPhase = string.Empty;
}

public class Condition
{
    [JsonProperty("code")]
    public int Code;
}

public class WeatherCondition
{
    [JsonProperty("code")]
    public int Code;

    [JsonProperty("day")]
    public string Day = null!;

    [JsonProperty("night")]
    public string Night = null!;

    [JsonProperty("icon")]
    public int Icon;
}
