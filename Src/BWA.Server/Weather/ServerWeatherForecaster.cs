using SlugEnt.BWA.Client.Weather;

namespace BWA.Weather;

/// <summary>
///    A server-side weather forecaster that generates random weather forecasts.
/// </summary>
public class ServerWeatherForecaster() : IWeatherForecaster
{
    /// <summary>
    ///   A list of weather summaries to choose from when generating forecasts.
    /// </summary>
    public readonly string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];


    /// <summary>
    ///    Generates a sequence of weather forecasts.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync()
    {
        // Simulate asynchronous loading to demonstrate streaming rendering
        await Task.Delay(500);

        return Enumerable.Range(1, 5).Select(index =>
                                                 new WeatherForecast(
                                                                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                                                     Random.Shared.Next(-20, 55),
                                                                     summaries[Random.Shared.Next(summaries.Length)]
                                                                    ))
                         .ToArray();
    }
}