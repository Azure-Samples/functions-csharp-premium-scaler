using System;
using System.Collections.Generic;
using System.Text;

namespace PlanMover
{
    internal static class Constants
    {
        internal static string SUBSCRIPTION_ID = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
        internal static string BASE_PLAN_NAME = Environment.GetEnvironmentVariable("BASE_PLAN_NAME");
        internal static string SCALED_PLAN_NAME = Environment.GetEnvironmentVariable("SCALED_PLAN_NAME");
        internal static int SCALED_PREWARMED_INSTANCES = int.Parse(Environment.GetEnvironmentVariable("SCALED_PREWARMED_INSTANCES"));
        internal static string SCALED_SKU = Environment.GetEnvironmentVariable("SCALED_SKU");
        internal static int BASE_COSMOSDB_RU = int.Parse(Environment.GetEnvironmentVariable("BASE_COSMOSDB_RU"));
        internal static int SCALED_COSMOSDB_RU = int.Parse(Environment.GetEnvironmentVariable("SCALED_COSMOSDB_RU"));
        internal static string LOCATION = Environment.GetEnvironmentVariable("LOCATION");
        internal static string RESOURCE_GROUP = Environment.GetEnvironmentVariable("RESOURCE_GROUP");
        internal static string COSMOSDB_CONNECTIONSTRING = Environment.GetEnvironmentVariable("COSMOSDB_CONNECTIONSTRING");
        internal static string COSMOSDB_DATABASENAME = Environment.GetEnvironmentVariable("COSMOSDB_DATABASENAME");
        internal static string COSMOSDB_CONTAINERNAME = Environment.GetEnvironmentVariable("COSMOSDB_CONTAINERNAME");
        internal static string APPNAME = Environment.GetEnvironmentVariable("APPNAME");
        internal static string SERVICE_PRINCIPAL_APP_ID = Environment.GetEnvironmentVariable("SERVICE_PRINCIPAL_APP_ID");
        internal static string SERVICE_PRINCIPAL_CLIENT_SECRET = Environment.GetEnvironmentVariable("SERVICE_PRINCIPAL_CLIENT_SECRET");
        internal static string SERVICE_PRINCIPAL_TENANT_ID = Environment.GetEnvironmentVariable("SERVICE_PRINCIPAL_TENANT_ID");
        internal static int DURATION_SECONDS = int.Parse(Environment.GetEnvironmentVariable("DURATION_SECONDS"));
    }
}
