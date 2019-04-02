using System;
using System.Threading.Tasks;
using PlanMover.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace PlanMover.Functions
{
    public class DurableScalerFunctions
    {
        public DurableScalerFunctions()
        {
        }

        [FunctionName("Timer")]
        public async Task Durable_TimerStart(
            [TimerTrigger("0 0 16 * * FRI")]TimerInfo timerInfo,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation($"Timer fired at: {DateTime.UtcNow} \n\n {nameof(Durable_TimerStart)}");

            // Create the move request
            var moveRequest = new CreateAndScaleRequest
            {
                appName = Constants.APPNAME,
                subscriptionId = Constants.SUBSCRIPTION_ID,
                basePlanName = Constants.BASE_PLAN_NAME,
                scaledPlanName = Constants.SCALED_PLAN_NAME,
                scaledPrewarmedInstances = Constants.SCALED_PREWARMED_INSTANCES,
                scaledSku = Constants.SCALED_SKU,
                baseCosmosDbRequestUnit = Constants.BASE_COSMOSDB_RU,
                scaledCosmosDbRequestUnit = Constants.SCALED_COSMOSDB_RU,
                location = Constants.LOCATION,
                resourceGroup = Constants.RESOURCE_GROUP,
                databaseName = Constants.COSMOSDB_DATABASENAME,
                containerName = Constants.COSMOSDB_CONTAINERNAME
            };
            
            // Kick off the orchestration
            string instanceId = await starter.StartNewAsync(nameof(Durable_Orchestrator), moveRequest);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }

        [FunctionName(nameof(Durable_Orchestrator))]
        public async Task<JObject> Durable_Orchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context,
            ILogger log,
            CancellationToken ctx)
        {
            if (!context.IsReplaying) log.LogInformation("Starting the plan mover orchestration");

            var input = context.GetInput<CreateAndScaleRequest>();

            // Create premium plan
            string premiumPlanId = await context.CallActivityAsync<string>(nameof(FunctionPlanFunctions.Durable_CreatePlan),
                new Plan
                {
                    subscriptionId = input.subscriptionId,
                    location = input.location,
                    resourceGroup = input.resourceGroup,
                    name = input.scaledPlanName,
                    prewarmedInstances = input.scaledPrewarmedInstances,
                    sku = input.scaledSku
                });
            if (!context.IsReplaying) log.LogInformation($"Created plan with ID {premiumPlanId}");

            // Scale up cosmos DB
            await context.CallActivityAsync(nameof(CosmosDbFunctions.Durable_ScaleCosmosDB), 
                new CosmosDbScaleRequest
                {
                    databaseName = input.databaseName,
                    containerName = input.containerName,
                    desiredRus = input.scaledCosmosDbRequestUnit
                });
            if (!context.IsReplaying) log.LogInformation($"Scaled CosmosDB database {input.databaseName} to {input.scaledCosmosDbRequestUnit} RUs");

            // Move function app to premium plan
            await context.CallActivityAsync(nameof(FunctionPlanFunctions.Durable_MoveApp), 
                new MovePlanRequest
                {
                    appName = input.appName,
                    resourceGroup = input.resourceGroup,
                    subscriptionId = input.subscriptionId,
                    startingPlanName = input.basePlanName,
                    destinationPlanName = input.scaledPlanName
                });
            if (!context.IsReplaying) log.LogInformation($"Moved function {input.appName} to plan {input.scaledPlanName}");

            // Wait for 3 hours
            if (!context.IsReplaying) log.LogInformation($"Waiting for 4 hours...");
            await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(Constants.DURATION_SECONDS), ctx);
            if (!context.IsReplaying) log.LogInformation($"Waited for 4 hours. Now scaling down...");

            // Move function app to consumption plan
            await context.CallActivityAsync(nameof(FunctionPlanFunctions.Durable_MoveApp),
                new MovePlanRequest
                {
                    appName = input.appName,
                    resourceGroup = input.resourceGroup,
                    subscriptionId = input.subscriptionId,
                    startingPlanName = input.scaledPlanName,
                    destinationPlanName = input.basePlanName
                });
            if (!context.IsReplaying) log.LogInformation($"Moved function {input.appName} back to plan {input.basePlanName}");

            // Scale down cosmos DB
            await context.CallActivityAsync(nameof(CosmosDbFunctions.Durable_ScaleCosmosDB),
                new CosmosDbScaleRequest
                {
                    databaseName = input.databaseName,
                    containerName = input.containerName,
                    desiredRus = input.baseCosmosDbRequestUnit
                });
            if (!context.IsReplaying) log.LogInformation($"Scaled CosmosDB database {input.databaseName} back to {input.baseCosmosDbRequestUnit} RUs");

            // Delete premium plan
            await context.CallActivityAsync(nameof(FunctionPlanFunctions.Durable_DeletePlan), premiumPlanId);
            if (!context.IsReplaying) log.LogInformation($"Premium plan deleted");

            return JObject.FromObject(new
            {
                status = "completed",
                request = input
            });
        }
    }
}