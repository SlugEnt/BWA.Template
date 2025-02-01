namespace SlugEnt.BWA.Client.Weather;

public interface IWeatherForecaster
{
    Task<IEnumerable<WeatherForecast>> GetWeatherForecastAsync();
}
