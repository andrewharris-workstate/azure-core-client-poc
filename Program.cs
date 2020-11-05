using System;
using System.Collections.Generic;
using Azure.Core;
using MyWebApi;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var forecast = GetWeatherForecast();

            Console.WriteLine(forecast);
        }

        static IEnumerable<WeatherForecast> GetWeatherForecast()
        {
            var opts = new MyWebApiClientOptions();
            var client = new AzureCoreHttpClient(new Uri("https://localhost:5001/"), opts);

            return client.GetWeatherForecasts().Result;
        }
    }
}
