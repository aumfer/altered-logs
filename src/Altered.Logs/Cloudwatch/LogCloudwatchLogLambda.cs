using Amazon.Lambda.CloudWatchLogsEvents;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Cloudwatch 
{
    public static class LogCloudwatchLogLambda
    {
        // lambda context
        static readonly ServiceProvider services;
        static readonly LogCloudwatchLog logCloudwatchLog;

        // lambda initializer
        static LogCloudwatchLogLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logCloudwatchLog = services.GetService<LogCloudwatchLog>();
        }

        // lambda entry point
        public static Task<StringResponse> Execute(CloudWatchLogsEvent e) =>
            logCloudwatchLog.Execute(e);
    }
}
