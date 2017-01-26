using System.Threading.Tasks;

namespace Weather.Contracts
{
    public interface ILocationService
    {
        Task<Forecast> GetForecastAsync(string zipCode);
        Task AddForecastAsync(Forecast forecast);
    }
}