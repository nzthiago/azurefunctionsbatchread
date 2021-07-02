using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ConsumerFunctionApp
{
    public static class ProcessEvents
    {
        [FunctionName("ProcessEvents")]
        public static void Run([ServiceBusTrigger("<Service Bus Topic Name>", "<Service Bus Subscription Name>", Connection = "SBConnString")]string[] myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed {myQueueItem.Length} messages");
        }
    }
}
