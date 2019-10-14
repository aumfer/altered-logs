using Altered.Pipeline;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Altered.Aws;
using Altered.Aws.Cloudwatch;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Threading.Tasks;

namespace Altered.Logs.Template
{
    /**
     * Lambda is just a static entry point, that configures & injects your service & dependencies
     * Mvc is the same, just configured at the deploy/host level (see Altered.Logs.Mvc.cs)
     */
    public static class MySampleApiCallLambda
    {
        static readonly ServiceProvider services;
        static readonly ILambda mySampleApiCall;

        static MySampleApiCallLambda()
        {
            services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

            mySampleApiCall = services.GetService<MySampleApiCallApi>()
                .WithAlteredLogsApi()
                .WithCloudwatchMetrics(services.GetService<CloudwatchMetricsSink>(), nameof(MySampleApiCallLambda))
                .WithCloudwatchLogs(services.GetService<CloudwatchLogsSink>(), nameof(MySampleApiCallLambda))
                .WithLambda();
        }

        public static Task<ApplicationLoadBalancerResponse> Execute(ApplicationLoadBalancerRequest request) =>
            mySampleApiCall.Execute(request);
    }
}

