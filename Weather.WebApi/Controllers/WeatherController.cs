using System;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Weather.Contracts;

namespace Weather.WebApi.Controllers
{
    public class WeatherController : ApiController, ILocationService
    {
        private readonly Uri _weatherServiceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/WeatherService");

        public async Task<Forecast> GetForecastAsync(string zipCode)
        {
            var client = ServiceProxy.Create<ILocationService>(_weatherServiceUri, new ServicePartitionKey(1));
            var forecast = await client.GetForecastAsync(zipCode);

            return forecast;
        }

        //public async Task<Forecast> GetForecastAsync(string zipCode)
        //{
        //    var tokenSource = new CancellationTokenSource();
        //    var servicePartitionResolver = ServicePartitionResolver.GetDefault();
        //    var httpClient = new HttpClient();

        //    var partition = await servicePartitionResolver.ResolveAsync(_weatherServiceUri, new ServicePartitionKey(1), tokenSource.Token);
        //    var endpoint = partition.GetEndpoint();
        //    var addresses = JObject.Parse(endpoint.Address);
        //    var primaryReplicaAddress = (string)addresses["Endpoints"].First;

        //    var primaryReplicaUriBuilder = new UriBuilder(primaryReplicaAddress);
        //    primaryReplicaUriBuilder.Query = $"ZipCode={zipCode}";

        //    var result = await httpClient.GetStringAsync(primaryReplicaUriBuilder.Uri);
        //    var forecast = JsonConvert.DeserializeObject<Forecast>(result);

        //    return forecast;
        //}

        [HttpPost]
        public async Task AddForecastAsync(Forecast forecast)
        {
            var client = ServiceProxy.Create<ILocationService>(_weatherServiceUri, new ServicePartitionKey(1));

            await client.AddForecastAsync(forecast);
        }
    }
}
