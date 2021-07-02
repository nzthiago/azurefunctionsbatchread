using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;

namespace Producer
{
    class Program
    {
        private static ServiceBus _serviceBus;

        static async Task Main(string[] args)
        {
             IConfiguration Configuration = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json", optional: false)
                                                .AddJsonFile("appsettings.Local.json", optional: true)
                                                .AddEnvironmentVariables()
                                                .AddCommandLine(args)
                                                .Build();

            _serviceBus = Configuration.GetSection("ServiceBus").Get<ServiceBus>();


            await using (var producerClient = new ServiceBusClient(_serviceBus.ConnectionString))
            {
                var sender = producerClient.CreateSender(_serviceBus.TopicName);
                var r = new Random();
                var sensorCnt = r.Next(1, 10);
                var message = await CreatePayload(Convert.ToInt32(sensorCnt));
                Console.WriteLine($"Test Payload: {message}");

                for (int i = 1;i < 1000; i++)
                { 
                    await sender.SendMessageAsync(new ServiceBusMessage(message));
                    Console.WriteLine($"message {i} sent.");
                }
            }
        }

        private static async Task<string> CreatePayload(int numOfSensors) {
            List<string> randomWords = new List<string>();
            using (var httpClient = new HttpClient()) {
                var json = await httpClient.GetStringAsync("https://random-word-api.herokuapp.com/word?number=10&swear=0");
                randomWords = JsonConvert.DeserializeObject<List<string>>(json);
            }

            StringBuilder sb = new StringBuilder(
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                <devices>");
            sb.AppendLine();
            Enumerable.Range(1, numOfSensors).ToList().ForEach(s => sb.AppendLine(CreateSensor(randomWords)));
            sb.AppendLine("  </devices>");

            return Regex.Replace(sb.ToString(), @"[ ]{5,}", "  ");
        }

        private static string CreateSensor(List<string> randomWords) {
            var colors = new string[] { "blue", "green", "yellow", "red", "black", "white", "purple" };
            var random = new Random();
            var id = random.Next(100, 110);
            var color = colors[random.Next(0,6)];
            var timestamp = Convert.ToInt32((DateTime.Now.ToUniversalTime() - new DateTime (1970, 1, 1)).TotalSeconds + new Random().Next(0, 10000));
            var randomPhrase = string.Join(string.Empty,randomWords.OrderBy(x => Guid.NewGuid()).Take(3));
            return $"    <sensor id='{id}' type='{color}' timestamp='{timestamp}'><value>{randomPhrase}</value></sensor>".Replace("'", "\"");
        }

    }
}
