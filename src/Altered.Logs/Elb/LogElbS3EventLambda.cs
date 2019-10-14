using Altered.Pipeline;
using Altered.Shared;
using Amazon.ElasticLoadBalancingV2;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Altered.Aws.Alb;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Elb
{
    public static class LogElbS3EventLambda
    {
        static readonly ServiceProvider services;
        static readonly LogElbS3Event logElbS3Event;

        static LogElbS3EventLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logElbS3Event = services.GetService<LogElbS3Event>();
        }

        public static Task<StringResponse> Execute(S3Event e) =>
           logElbS3Event.Execute(e);
    }
}
