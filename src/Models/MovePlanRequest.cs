using System;
using System.Collections.Generic;
using System.Text;

namespace PlanMover.Models
{
    public class MovePlanRequest
    {
        public string appName { get; set; }
        public string subscriptionId { get; set; }
        public string resourceGroup { get; set; }
        public string startingPlanName { get; set; }
        public string destinationPlanName { get; set; }
    }
}
