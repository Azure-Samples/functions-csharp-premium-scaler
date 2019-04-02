using System;
using System.Collections.Generic;
using System.Text;

namespace PlanMover.Models
{
    public class CosmosDbScaleRequest
    {
        public string databaseName { get; set; }
        public int desiredRus { get; set; }
        public string containerName { get; set; }
    }
}
