using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using PlanMover.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlanMover.Functions
{
    public class CosmosDbFunctions
    {
        //TODO: Refactor to use DI when that gets released
        private CosmosClient _client = new CosmosClient(Constants.COSMOSDB_CONNECTIONSTRING);
        public CosmosDbFunctions()
        {
        }

        [FunctionName(nameof(Durable_ScaleCosmosDB))]
        public async Task Durable_ScaleCosmosDB(
            [ActivityTrigger] CosmosDbScaleRequest request, 
            ILogger log)
        {
            log.LogInformation($"Scaling up database {request.databaseName} collection {request.containerName} to {request.desiredRus}");
            await _client.Databases[request.databaseName].Containers[request.containerName].ReplaceProvisionedThroughputAsync(request.desiredRus);
        }
    }
}
