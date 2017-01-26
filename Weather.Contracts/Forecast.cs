using System.Linq;

namespace Weather.Contracts
{
    public class Forecast
    {
        public string ZipCode { get; set; }
        public int Temperature { get; set; }
    }
}
