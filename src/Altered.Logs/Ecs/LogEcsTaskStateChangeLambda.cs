using Altered.Pipeline;
using Amazon.ECS;
using Amazon.Lambda.CloudWatchEvents.ECSEvents;
using Altered.Aws.Ecs;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Ecs
{
    public static class LogEcsTaskStateChangeLambda
    {
        static readonly ServiceProvider services;
        static readonly LogEcsTaskStateChange logEcsTaskStateChange;

        static LogEcsTaskStateChangeLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logEcsTaskStateChange = services.GetService<LogEcsTaskStateChange>();
        }

        public static System.Threading.Tasks.Task<StringResponse> Execute(ECSTaskStateChangeEvent e) =>
            logEcsTaskStateChange.Execute(e);
    }
}
