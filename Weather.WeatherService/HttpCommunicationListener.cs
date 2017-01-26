using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Description;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;

namespace Weather.WeatherService
{
    public class HttpCommunicationListener : ICommunicationListener
    {
        private readonly ServiceContext _serviceContext;
        private readonly HttpListener _httpListener = new HttpListener();

        public HttpCommunicationListener(ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var endpoint = _serviceContext.CodePackageActivationContext.GetEndpoint("http");

            var uriPrefix = $"{endpoint.Protocol}://+:{endpoint.Port}/weather/";

            _httpListener.Prefixes.Add(uriPrefix);
            _httpListener.Start();

            var uriPublished = uriPrefix.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);

            return Task.FromResult(uriPublished);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _httpListener.Close();

            return Task.FromResult(0);
        }

        public void Abort()
        {
            _httpListener.Abort();
        }
    }
}
