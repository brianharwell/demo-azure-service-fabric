using System.Threading.Tasks;

namespace Weather.Contracts
{
    public interface ILocationService : Microsoft.ServiceFabric.Services.Remoting.IService
    {
        Task<Forecast> GetForecastAsync(string zipCode);
        Task AddForecastAsync(Forecast forecast);
    }
}