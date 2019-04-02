using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PlanMover.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlanMover
{
    public interface IArmClient
    {
        Task<HttpResponseMessage> GetAsync(string url, ILogger log);
        Task<HttpResponseMessage> PutAsync(string url, JObject body, ILogger log);

        Task<HttpResponseMessage> PatchAsync(string url, JObject body, ILogger log);
        Task<HttpResponseMessage> DeleteAsync(string url, ILogger log);
    }
}