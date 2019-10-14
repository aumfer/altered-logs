using Altered.Pipeline;
using Amazon.AutoScaling;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Autoscaling
{
    public static class LogAutoscalingEventLambda
    {
        static readonly ServiceProvider services;
        static readonly LogAutoscalingEvent logAutoscalingEvent;

        static LogAutoscalingEventLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logAutoscalingEvent = services.GetService<LogAutoscalingEvent>();
        }

        public static Task<StringResponse> Execute(JObject asgEvent) =>
            logAutoscalingEvent.Execute(asgEvent);
    }
}
