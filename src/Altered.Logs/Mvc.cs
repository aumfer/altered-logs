using Altered.Aws;
using Altered.Mvc.Components;
using Altered.Pipeline;
using Altered.Aws.Cloudwatch;
using Altered.Logs.Template;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Altered.Logs.Dash;

namespace Altered.Logs
{
    public static class Mvc
    {     
        public static void Host(params string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseAlteredAppConfiguration()
                .UseAlteredConfigure()
                .EnsureAwsRegion()
                // todo UseMvc instead of MvcCore for swagger
                .UseAlteredMvcCore(
                    routeBuilder: (services, routes) => routes
                        .RouteMySampleApiCallApi(services)
                        .RouteProxyDashboard(services)
                , assemblies: typeof(Mvc).Assembly)
                .ConfigureServices(services => services
                    .AddAlteredLogsService())
                    .Build()
                    .Run();
    }
}
