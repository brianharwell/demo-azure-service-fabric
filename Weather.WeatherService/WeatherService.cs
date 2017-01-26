using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Weather.Contracts;

namespace Weather.WeatherService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class WeatherService : StatefulService, ILocationService
    {
        public WeatherService(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
                       {
                           //new ServiceReplicaListener(context => new HttpCommunicationListener(context)),
                           new ServiceReplicaListener(this.CreateServiceRemotingListener)
                           //new ServiceReplicaListener(context => new FabricTransportServiceRemotingListener(context, this)), 
                       };
        }

        public async Task<Forecast> GetForecastAsync(string zipCode)
        {
            ServiceEventSource.Current.ServiceMessage(Context, "Retrieving forecast for zip code: {0}", zipCode);

            var forecasts = await StateManager.GetOrAddAsync<IReliableDictionary<string, Forecast>>("Forecasts")
                .ConfigureAwait(false);

            Forecast forecast;

            using (var tx = StateManager.CreateTransaction())
            {
                var conditionalValue = await forecasts.TryGetValueAsync(tx, zipCode)
                    .ConfigureAwait(false);

                ServiceEventSource.Current.ServiceMessage(Context, "Temperature for zip code: {0} is {1}", zipCode, conditionalValue.HasValue ? conditionalValue.Value.Temperature.ToString() : "unknown");

                await tx.CommitAsync();

                forecast = conditionalValue.HasValue ? conditionalValue.Value : null;
            }

            return forecast;
        }

        public async Task AddForecastAsync(Forecast forecast)
        {
            ServiceEventSource.Current.ServiceMessage(Context, "Adding forecast for zip code: {0}", forecast.ZipCode);

            var forecasts = await StateManager.GetOrAddAsync<IReliableDictionary<string, Forecast>>("Forecasts")
                .ConfigureAwait(false);

            using (var tx = StateManager.CreateTransaction())
            {
                await forecasts.TryAddAsync(tx, forecast.ZipCode, forecast)
                    .ConfigureAwait(false);
                
                ServiceEventSource.Current.ServiceMessage(Context, "Added forecast for zip code: {0}", forecast.ZipCode);

                await tx.CommitAsync();
            }
        }
    }
}
