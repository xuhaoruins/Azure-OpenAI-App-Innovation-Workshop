using System.Collections.Generic;
using System.Net;
using HostEngine.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

//reference: https://colhountech.com/2022/12/07/creating-azure-functions-using-ioptions-with-dotnet-isolated-process-6-0/#:~:text=In%20order%20to%20make%20it%20easier%20to%20manage,the%20middleware%20pipeline.%20Here%20is%20a%20working%20config

namespace HostEngine.FunctionIsolated
{
    public class EngineFunction
    {
        private readonly ILogger _logger;
        private readonly IEngine _engine;
        public EngineFunction(ILoggerFactory loggerFactory, IEngine engine)
        {
            _logger = loggerFactory.CreateLogger<EngineFunction>();
            _engine = engine;
        }

        [Function("prompt")]
        public async Task<HttpResponseData> RunPropmt(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "prompt")] HttpRequestData req, FunctionContext executionContext, ILogger log)
        {
            var userId = executionContext.BindingContext.BindingData["userId"] as string;
            var prompt = executionContext.BindingContext.BindingData["prompt"] as string;
            var output = await _engine.GetCompletionAsync(userId, prompt);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(output);
            return response;
        }

        [Function("test")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "test")] HttpRequestData req, FunctionContext executionContext, ILogger log)
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync("OK");
            return response;
        }
    }
}
