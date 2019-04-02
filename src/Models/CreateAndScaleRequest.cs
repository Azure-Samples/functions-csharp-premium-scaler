using System;
using System.Collections.Generic;
using System.Text;

namespace PlanMover.Models
{
    public class CreateAndScaleRequest
    {
        public string subscriptionId { get; set; }
        public string basePlanName { get; set; }
        public string scaledPlanName { get; set; }
        public int scaledPrewarmedInstances { get; set; }
        public string scaledSku { get; set; }
        public string resourceGroup { get; set; }
        public string location { get; set; }
        public int baseCosmosDbRequestUnit { get; set; }
        public int scaledCosmosDbRequestUnit { get; set; }
        public string databaseName { get; set; }
        public string appName { get; set; }
        public string containerName { get; set; }
    }
}
