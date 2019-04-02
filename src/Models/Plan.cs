using System;
using System.Collections.Generic;
using System.Text;

namespace PlanMover.Models
{
    public class Plan
    {
        public string name { get; set; }
        public string resourceGroup { get; set; }
        public int prewarmedInstances { get; set; } = 1;
        public string sku { get; set; } = "EP1";
        public string location { get; set; } = "southcentralus";
        public string subscriptionId { get; set; }
    }
}
