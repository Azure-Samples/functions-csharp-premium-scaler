using System;
using System.Collections.Generic;
using System.Text;

namespace PlanMover.Models
{
    public class PremiumPlanResource
    {
        public PremiumPlanResource(string location, int maximumElasticWorkerCount, string skuName, int skuCapacity)
        {
            this.location = location;
            properties.maximumElasticWorkerCount = maximumElasticWorkerCount;
            sku.name = skuName;
            sku.capacity = skuCapacity;
        }
        public string location { get; set; }
        public Properties properties { get; set; } = new Properties();
        public Sku sku { get; set; } = new Sku();

        public class Properties
        {
            public int maximumElasticWorkerCount { get; set; }
        }

        public class Sku
        {
            public string name { get; set; }
            public string tier { get; set; } = "ElasticPremium";
            public int capacity { get; set; }
        }
    }
}
