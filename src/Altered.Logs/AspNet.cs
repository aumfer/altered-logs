using Altered.Aws;
using Altered.Aws.Cloudwatch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs
{
    public static class AspNet
    {
        // translate MvcRequest/Response into aspnet RequestDelegate
        public static RequestDelegate ToAlteredAspNet(this IAlteredPipeline<AlteredApiRequest, AlteredApiResponse> pipeline) =>
            async (httpContext) =>
            {
                var request = httpContext.Request;
                using (var bodyReader = new StreamReader(request.Body))
                {
                    var mvcRequest = new AlteredApiRequest
                    {
                        Path = request.Path,
                        HttpMethod = request.Method,
                        Headers = request.Headers,
                        QueryStringParameters = QueryHelpers.ParseQuery(request.QueryString.Value)
                    };

                    if (request.Body?.CanRead == true)
                    {
                        mvcRequest.Body = await bodyReader.ReadToEndAsync();
                    }
                    var mvcResponse = await pipeline.Execute(mvcRequest);

                    var response = httpContext.Response;
                    response.StatusCode = mvcResponse.StatusCode;

                    foreach (var kvp in mvcResponse.Headers)
                    {
                        response.Headers[kvp.Key] = kvp.Value;
                    }

                    if (!string.IsNullOrEmpty(mvcResponse.Body))
                    {
                        //response.ContentLength = mvcResponse.Body.Length;

                        response.ContentType = response.Headers[HeaderNames.ContentType];
                        using (var bodyWriter = new StreamWriter(response.Body))
                        {
                            await bodyWriter.WriteAsync(mvcResponse.Body);
                        }
                    }
                }
            };

        // this will build a mock ApiRequest/Response and record the logs/metrics of the underlying mvc middleware
        public static RequestDelegate WithAlteredAspNet(RequestDelegate aspnet, CloudwatchLogsSink cloudwatchLogs, CloudwatchMetricsSink cloudwatchMetrics)
        {
            return async (httpContext) =>
            {
                var request = httpContext.Request;
                var mvcRequest = new AlteredApiRequest
                {
                    Path = request.Path,
                    HttpMethod = request.Method,
                    Headers = request.Headers,
                    QueryStringParameters = QueryHelpers.ParseQuery(request.QueryString.Value)
                };

                var pipeline = new AlteredPipeline<AlteredApiRequest, AlteredApiResponse>(async (_) =>
                {
                    await aspnet(httpContext);
                    var mvcResponse = new AlteredApiResponse
                    {
                        StatusCode = httpContext.Response.StatusCode,
                        Headers = httpContext.Response?.Headers
                    };
                    return mvcResponse;
                })
                .WithCloudwatchLogs(cloudwatchLogs, mvcRequest.Path)
                .WithCloudwatchMetrics(cloudwatchMetrics, mvcRequest.Path);

                var response = await pipeline.Execute(mvcRequest);
            };
        }
    }
}
