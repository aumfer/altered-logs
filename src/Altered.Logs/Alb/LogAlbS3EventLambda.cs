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

namespace Altered.Logs.Alb
{
    public static class LogAlbS3EventLambda
    {
        static readonly ServiceProvider services;
        static readonly LogAlbS3Event logAlbS3Event;

        static LogAlbS3EventLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logAlbS3Event = services.GetService<LogAlbS3Event>();
        }

        public static Task<StringResponse> Execute(S3Event e) =>
           logAlbS3Event.Execute(e);
    }
}
