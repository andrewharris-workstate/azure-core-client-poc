using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
//using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using MyWebApi;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    internal class AzureCoreHttpClient
    {
        private readonly Uri _apiUri;
        private readonly HttpPipeline _pipeline;
        //private readonly ClientDiagnostics

        /// <summary>
        /// Initialize for mocking
        /// </summary>
        protected AzureCoreHttpClient()
        {

        }

        public AzureCoreHttpClient(Uri apiUri, ClientOptions options)
        {
            _apiUri = apiUri;
            _pipeline = HttpPipelineBuilder.Build(options, new MyWebApiPolicy());
        }

        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts() => await GetResultAsync<IEnumerable<WeatherForecast>>("/weatherforecast");

        public async Task<T> GetResultAsync<T>(string endpoint)
        {
            try
            {
                //using DiagnosticScope scope = _pipeline.CreateScope

                var uri = new Uri(_apiUri, endpoint);
                var request = CreateRequest(RequestMethod.Get, uri);

                // todo: what is the default?
                var cancellationToken = new CancellationToken();
                var response = await _pipeline.SendRequestAsync(request, cancellationToken).ConfigureAwait(false);

                switch (response.Status)
                {
                    case 200:
                    case 201:
                    case 202:
                    case 204:
                    {
                        using var reader = new StreamReader(response.ContentStream);
                        var resultString = reader.ReadToEnd();

                        return JsonConvert.DeserializeObject<T>(resultString);
                    }
                    default:
                        throw new Exception(response.Status.ToString());
                }
            }
            catch (Exception e)
            {
                // todo: log
                throw;
            }
        }

        private Request CreateRequest(RequestMethod method, Uri uri)
        {
            var request = _pipeline.CreateRequest();

            request.Headers.Add(HttpHeader.Common.JsonContentType);
            request.Headers.Add(HttpHeader.Common.JsonAccept);
            request.Method = method;
            request.Uri.Reset(uri);

            return request;
        }
    }
}
