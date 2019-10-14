using Altered.Aws;
using Altered.Aws.Cloudwatch;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Altered.Logs.Dash
{
    // hopefully we can use apigateway to log us in instead
    public sealed class ProxyDashboard : Client
    {
        public ProxyDashboard(HttpClient httpClient, Uri uri = null) : base(uri ?? DefaultDashboardUri, new Proxy(httpClient.SendAsync))
        { }

        public static readonly string ElasticsearchDomain = Environment.GetEnvironmentVariable("elasticsearch_domain");
        public static readonly Uri DefaultDashboardUri = new Uri(ElasticsearchDomain);
    }

    public static class RouteProxyDashboardExtensions
    {
        public static IRouteBuilder RouteProxyDashboard(this IRouteBuilder routes, IServiceProvider services) => routes
            .MapRoute("/", (context) =>
            {
                var request = context.Request;
                var response = context.Response;
                var location = $"{request.Scheme}://{request.Host}{request.Path}{HomePath}";
                response.Redirect(location, false);
                return Task.CompletedTask;
            })
            .MapRoute("{*pathInfo}", services
                .GetService<ProxyDashboard>()
                .WithAlteredLogsApi()
                .WithCloudwatchMetrics(services.GetService<CloudwatchMetricsSink>(), nameof(ProxyDashboard))
                .WithCloudwatchLogs(services.GetService<CloudwatchLogsSink>(), nameof(ProxyDashboard))
                .ToAlteredAspNet());

        static readonly string HomePath = "_plugin/kibana/app/kibana#/dashboard/1c9e0ad0-42c9-11e9-b3cf-d9353c9ce009?_g=(refreshInterval%3A(pause%3A!f%2Cvalue%3A60000)%2Ctime%3A(from%3Anow-4h%2Cmode%3Aquick%2Cto%3Anow))";
    }
}
