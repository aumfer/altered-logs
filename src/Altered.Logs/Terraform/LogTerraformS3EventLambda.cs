using Altered.Aws;
using Altered.Aws.Cloudwatch;
using Amazon.S3.Util;
using Altered.Logs.Template;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Terraform
{
    public static class LogTerraformS3EventLambda
    {
        static readonly ServiceProvider services;
        static readonly LogTerraformS3Event logTerraformS3Event;

        static LogTerraformS3EventLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logTerraformS3Event = services.GetService<LogTerraformS3Event>();
        }

        public static Task<StringResponse> Execute(S3EventNotification request) =>
            logTerraformS3Event.Execute(request);
    }
}
