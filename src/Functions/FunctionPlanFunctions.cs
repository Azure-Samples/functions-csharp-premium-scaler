using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PlanMover.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlanMover.Functions
{
    public class FunctionPlanFunctions
    {
        //TODO: refactor to use DI when that gets released
        private static IArmClient _armClient = new ArmClient(new System.Net.Http.HttpClient());
        public FunctionPlanFunctions()
        {
        }

        [FunctionName(nameof(Durable_CreatePlan))]
        public async Task<string> Durable_CreatePlan(
            [ActivityTrigger] Plan plan, 
            ILogger log)
        {
            string resourceId = $"/subscriptions/{plan.subscriptionId}/resourceGroups/{plan.resourceGroup}/providers/Microsoft.Web/serverfarms/{plan.name}";

            var response = await _armClient.PutAsync(
                url: $"https://management.azure.com{resourceId}?api-version=2018-02-01",
                body: JObject.FromObject(new PremiumPlanResource(plan.location, 20, plan.sku, plan.prewarmedInstances)),
                log: log);
            if (response.IsSuccessStatusCode)
                return resourceId;
            else
                throw new ArgumentException($"HTTP Response was not successful. Status code {response.StatusCode}:{response.ReasonPhrase} -- {await response.Content.ReadAsStringAsync()}");
        }

        [FunctionName(nameof(Durable_MoveApp))]
        public async Task Durable_MoveApp(
            [ActivityTrigger] MovePlanRequest request, 
            ILogger log)
        {
            log.LogInformation($"Moving app {request.appName} from plan {request.startingPlanName} to {request.destinationPlanName}");

            string appResourceId = $"/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroup}/providers/Microsoft.Web/sites/{request.appName}";
            string planResourceId = $"/subscriptions/{request.subscriptionId}/resourceGroups/{request.resourceGroup}/providers/Microsoft.Web/serverfarms/{request.destinationPlanName}";
            var response = await _armClient.PatchAsync(
                            url: $"https://management.azure.com{appResourceId}?api-version=2018-02-01",
                            body: JObject.FromObject(new {
                                properties = new
                                {
                                    serverFarmId = planResourceId
                                }
                            }),
                            log: log);
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException($"HTTP Response was not successful. Status code {response.StatusCode}:{response.ReasonPhrase} -- {await response.Content.ReadAsStringAsync()}");
        }

        [FunctionName(nameof(Durable_DeletePlan))]
        public async Task Durable_DeletePlan(
            [ActivityTrigger] string planId,
            ILogger log)
        {
            var response = await _armClient.DeleteAsync($"https://management.azure.com{planId}?api-version=2018-02-01", log);
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException($"HTTP Response was not successful. Status code {response.StatusCode}:{response.ReasonPhrase} -- {await response.Content.ReadAsStringAsync()}");
        }
    }
}
