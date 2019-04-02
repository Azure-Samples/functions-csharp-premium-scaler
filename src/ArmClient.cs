using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using PlanMover.Models;

namespace PlanMover
{
    public class ArmClient : IArmClient
    {
        private static string _accessToken;
        private HttpClient _httpClient;

        public ArmClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<HttpResponseMessage> GetAsync(string url, ILogger log)
        {
            await CheckToken();
            var result = await _httpClient.GetAsync(url);
            return result;
        }

        public async Task<HttpResponseMessage> PatchAsync(string url, JObject body, ILogger log)
        {
            log.LogInformation($"PATCH URL: {url} \n Request body: {body.ToString()} \n");
            await CheckToken();
            var content = new StringContent(body.ToString(Newtonsoft.Json.Formatting.None));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var result = await _httpClient.PatchAsync(url, content);
            return result;
        }

        public async Task<HttpResponseMessage> PutAsync(string url, JObject body, ILogger log)
        {
            log.LogInformation($"PUT URL: {url} \n Request body: {body.ToString()} \n");
            await CheckToken();
            var content = new StringContent(body.ToString(Newtonsoft.Json.Formatting.None));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            var result = await _httpClient.PutAsync(url, content);
            return result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url, ILogger log)
        {
            log.LogInformation($"DELETE {url}");
            await CheckToken();
            var result = await _httpClient.DeleteAsync(url);
            return result;
        }

        private async Task CheckToken()
        {
            if (_accessToken == null && Constants.SERVICE_PRINCIPAL_APP_ID == null)
            {
                _accessToken = await GetTokenManagedIdentities();
            }
            else if (_accessToken == null)
            {
                _accessToken = await GetTokenServicePrincipal();
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        private async Task<string> GetTokenManagedIdentities()
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.core.windows.net/");
            return accessToken;
        }

        private async Task<string> GetTokenServicePrincipal()
        {
            AuthenticationContext ac = new AuthenticationContext($"https://login.microsoftonline.com/{Constants.SERVICE_PRINCIPAL_TENANT_ID}", true);
            var ar = await ac.AcquireTokenAsync("https://management.core.windows.net/", new ClientCredential(Constants.SERVICE_PRINCIPAL_APP_ID, Constants.SERVICE_PRINCIPAL_CLIENT_SECRET));
            return ar.AccessToken;
        }

        public void SetAccessToken(string accessToken)
        {
            _accessToken = accessToken;
        }
    }
}