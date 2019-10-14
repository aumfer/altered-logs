using Altered.Shared;
using Altered.Aws;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Routing;
using Altered.Aws.Cloudwatch;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Altered.Logs.Template
{
        /**
         * STEP 3
         * 
         * this is your Mvc Controller Action
         * but without the nice [FromHeader] [FromBody] stuff
         * instead we can write extension methods off of MvcRequest/Response, if needed
         * 
         * we can't generate swagger without controllers though
         * i don't mind hand-writing swagger endpoints, but i don't want to write swagger dtos
         * could have swagger w/o dto definition, or just use controller
         */
    public sealed class MySampleApiCallApi : AlteredPipeline<AlteredApiRequest, AlteredApiResponse>
    {
        public MySampleApiCallApi(IMySampleApiCall mySampleApiCall) : base(async (request) =>
        {
            var response = new AlteredApiResponse();

            var myDataIn = JObject.Parse(request.Body);

            var mySampleApiCallRequest = new MySampleApiCallRequest
            {
                RequestId = request.RequestId,
                //BearerToken = 
                MyDataIn = myDataIn
            };
            var mySampleApiCallResponse = await mySampleApiCall.Execute(mySampleApiCallRequest);

            response.StatusCode = mySampleApiCallResponse.StatusCode;
            response.Headers[HeaderNames.ContentType] = "application/json";
            response.Body = AlteredJson.SerializeObject(mySampleApiCallResponse.MyDataOut);

            return response;
        })
        {
        }
    }

    public static class MySampleApiCallApiExtensions
    {
        public static IRouteBuilder RouteMySampleApiCallApi(this IRouteBuilder routes, IServiceProvider services) => routes
            .MapRoute("template/{*pathInfo}", services
                .GetService<MySampleApiCallApi>()
                // this function could populate the Entity property of your request
                // by reading the EntityId property and calling cdom
                // that code would be written once, and work with code/lambda/api/exe/etc
                // as long as its request shape implements IEntity
                //.WithCdomEntity()
                .WithAlteredLogsApi()
                .WithCloudwatchMetrics(services.GetService<CloudwatchMetricsSink>(), nameof(MySampleApiCallApi))
                .WithCloudwatchLogs(services.GetService<CloudwatchLogsSink>(), nameof(MySampleApiCallApi))
                .ToAspNet());
    }
}
