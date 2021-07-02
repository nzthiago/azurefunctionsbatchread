# Azure Functions Batch Read
Small sample with single message sender console app and batch receiver C# Azure Function.

Notes for running locally:

* Create an Azure Service Bus namespace, a Service Bus topic in it, and a Service Bus subscription on the topic
* Create a local.settings.json file in the ConsumerFunctionApp folder with the following:

```json
{
    "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SBConnString": "<Service Bus Connection String>"
  }
}
```

* Update the following values:
  * `<Service Bus Connection String>` in the new `local.settings.json` you just created with the connection string to the Azure Service Bus namespace
  * `<Service Bus Topic Name>` and `<Service Bus Subscription Name>` in [ProcessEvents.cs](./ConsumerFunctionApp/ProcessEvents.cs)
  * `<Service Bus Connection String>` and `<Service Bus Topic Name>` in [appsettings.json](./producer/appsettings.json)
* Run the producer console app
* Run the Azure Function project
* Observe the telemetry to see the number of events per execution of the function (i.e., batch sizes)
