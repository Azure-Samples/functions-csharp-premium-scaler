# Azure Functions plan switcher

.NET Function that uses Azure Durable Functions to schedule the scale of a function app + CosmosDB to a pre-warmed premium plan for 4 hours.  After the 4 hours it moves the app back to the consumption plan, and scales down CosmosDB database.

## Scenario

Contoso is building an app to process weekly timecards for employees.  While timecards may be submitted at anytime during the week, the bulk of requests are made late Friday afternoon.  To ensure high performance even with large bursts of data, Contoso can use this sample code to pre-emptively scale their serverless components on Friday afternoons, and scale down on Friday evenings.  This gets the app warmed and ready in advance of the burst of data.

## Setup

⚠ The Azure Functions Premium plan is only available in a sub-set of infrastructure in each region.  Internally we call these "webspaces" or "stamps."  You will only be able to move your function between plans if the webspace supports both consumption and premium.  To make sure your consumption and premium functions land in an enabled webspace you should [create a premium plan](https://aka.ms/funcpremiumpreview) in a new resource group.  Then create a consumption plan in the same resource group.  You can then remove the premium plan.  This will ensure the consumption function is in a premium-enabled webspace. ⚠

Before deploying the project you should have an:
* Azure Function deployed to a consumption plan (the plan and app should be in the same resource group - see note above as well)
* Azure CosmosDB database

You can clone and run the project locally, or deploy to Azure to run.  The following app settings should be configured (in `local.settings.json` locally or in the Application Settings in the cloud).

|Setting name|Example|Description|
|--|--|--|
|SUBSCRIPTION_ID|80d4fe69-xxxx-xxxx-a938-9250f1c8ab03|Azure Subscription Id|
|RESOURCE_GROUP|functions-premium|Resource Group with deployed function and consumption plan.  This will be used to create your new temporary premium plan in|
|LOCATION|southcentralus|Azure region of the resource group and function / plan|
|APPNAME|my-function|Name of the Azure Function that should be moved|
|AzureWebJobsStorage|AccountEndpoint=.....|Connection string for the storage account for state of the durable function|
|BASE_PLAN_NAME|SouthCentralUSPlan|Name of the Azure Function consumption plan that will be the starting and ending destination of the function|
|SCALED_PLAN_NAME|my-premium-plan|Name of the Azure Function premium plan you want the app to create and move the function to during the scale duration|
|SCALED_SKU|EP1|The Azure Functions premium plan SKU to use for the instance size. EP1, EP2, or EP3|
|SCALED_PREWARMED_INSTANCES|1|How many app instances to pre-warm during the scale operation|
|COSMOSDB_DATABASENAME|timecardsDb|Name of the CosmosDB database to scale|
|COSMOSDB_CONTAINERNAME|timecards|Name of the CosmosDB container to scale|
|COSMOSDB_CONNECTIONSTRING|AccountEndpoint=.....|Connection string to manage the CosmosDB database|
|BASE_COSMOSDB_RU|400|Initial RUs as the starting and finishing point for CosmosDB|
|SCALED_COSMOSDB_RU|1200|The number to temporarily scale CosmosDB RUs during the scale duration|
|DURATION_SECONDS|14400|The number of seconds to keep the app in a scaled out premium state.  NOTE: CosmosDB will only allow scale operations once every 4 hours, so if less than 4 hours make sure base and scaled CosmosDB are the same|
|SERVICE_PRINCIPAL_APP_ID|b91f48f9-xxxx-xxxx-xxxx-614df8f4ca1a|Optional: App ID of a service principal to authenticate and perform operations on the resource group|
|SERVICE_PRINCIPAL_CLIENT_SECRET|as35235a|Optional: Client secret of a service principal to authenticate and perform operations on the resource group|
|SERVICE_PRINCIPAL_TENANT_ID|72f988bf-xxxx-xxxx-xxxx-2d7cd011db47|Optional: Tenant ID of a service principal to authenticate and perform operations on the resource group|

## Authentication

By default the app will attempt to use [Managed Identities for Azure Services](https://docs.microsoft.com/azure/app-service/overview-managed-identity) to authenticate and perform operations on your subscription.  You will need to make sure the managed identity has access to contribute to the resource group.  Alternative and for local testing you can use a service principal and provide an app ID and client secret in the application settings.

## Testing

To trigger this functionality locally you can call the admin API of the function to start a run.  When running locally the request would look something like:

```http
POST http://localhost:7071/admin/functions/Timer

{
	"input": "test"
}
```
