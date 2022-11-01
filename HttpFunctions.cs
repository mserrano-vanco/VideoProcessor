using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace VideoProcessor
{
    /// <summary>
    /// 01
    /// </summary>
    public static class HttpFunctions
    {
        /// <summary>
        /// 01 - This is where everyting starts
        /// </summary>
        /// <param name="req"></param>
        /// <param name="starter"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(ProcessVideoStarter))]
        public static async Task<IActionResult> ProcessVideoStarter(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient starter,
           ILogger log)
        {
            var video = req.GetQueryParameterDictionary()["video"];
            if (video == null)
            {
                return new BadRequestObjectResult("Please pass the video location of the query string");
            }
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("ProcessVideoOrchestrator", null, video);

            log.LogInformation($"Here we go! " +
                $"Started orchestration for processing the video with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }

}