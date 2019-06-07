using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio;

namespace Serverless
{
    public static class SendWhatsAppMessage
    {
        [FunctionName("SendWhatsAppMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "whatsapp")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("WhatsApp HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            
            // Based on https://www.twilio.com/docs/sms/whatsapp/quickstart/csharp
            string accountSid = Environment.GetEnvironmentVariable("TwilioAccountSid");
            string authToken = Environment.GetEnvironmentVariable("TwilioAuthToken");

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: (string)data.message,
                from: new Twilio.Types.PhoneNumber($"whatsapp:{Environment.GetEnvironmentVariable("TwilioFromNumber")}"),
                to: new Twilio.Types.PhoneNumber($"whatsapp:{(string)data.receiver}")
            );

            return new OkObjectResult("WhatsApp message sent successfully.");
        }
    }
}
