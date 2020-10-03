using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace integration_point
{
    public class RFPDescriptor {
        public string id { get; set; }
        public string xml { get; set; }
    }

    public class EventGridValidation
    {
        public string validationUrl { get; set; }
        public string validationCode { get; set; }
    }

    public static class RFPNotificationOrchestrator
    {
        private static HttpClient _httpClient = new HttpClient();

        [FunctionName("RFPNotificationOrchestrator")]
        public static async Task<dynamic> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            List<EventGridEvent> events = context.GetInput<List<EventGridEvent>>();
            var tasks = new List<Task<bool>>();

            foreach (EventGridEvent e in events) 
            {
                tasks.Add(context.CallActivityAsync<bool>("DownloadRFP", e));
            }
            
            await Task.WhenAll(tasks);
            return from t in tasks select new { TaskId = t.Id, Status = t.Result };
        }

        [FunctionName("DownloadRFP")]
        public static async Task<bool> DownloadRFP([ActivityTrigger] EventGridEvent e,
        [Blob("northwind-rfp-collection", Connection = "RFPStorageConnection")] CloudBlobContainer blobContainer,
        ILogger log)
        {
            log.LogInformation("Processing event {0}.",e.Id);
            // Check if event is for Webhook registration
            if (e.EventType == "Microsoft.EventGrid.SubscriptionValidationEvent") {
                EventGridValidation validationObject = (e.Data as JObject).ToObject<EventGridValidation>();
                log.LogInformation("Validation URL: {0}", validationObject.validationUrl);
                return true;
            }

            RFPDescriptor descriptor = (e.Data as JObject).ToObject<RFPDescriptor>();
            log.LogInformation("Downloading RFP document ID {0} from North Wind", descriptor.id);
            var response = await _httpClient.GetAsync(descriptor.xml);
            var responseString = await response.Content.ReadAsStringAsync();

            // Store RFP to Blob Storage
            string blobName = String.Format("{0}.xml", descriptor.id);
            log.LogInformation("Uploading RFP document ID {0} to internal storage", descriptor.id);
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);
            await blob.UploadTextAsync(responseString);

            return true;
        }
    }
}