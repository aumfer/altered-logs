using Altered.Pipeline;
using Amazon.CodeBuild;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Codebuild
{
    public static class LogCodebuildEventLambda
    {
        static readonly ServiceProvider services;
        static readonly LogCodebuildEvent logCodebuildEvent;

        static LogCodebuildEventLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            logCodebuildEvent = services.GetService<LogCodebuildEvent>();
        }

        public static Task<StringResponse> Execute(JObject codebuildEvent) =>
            logCodebuildEvent.Execute(codebuildEvent);
    }
}
