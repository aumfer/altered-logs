using Altered.Logs.Es;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Elb
{
    public static class AddAlteredAlbLogsExtensions
    {
        public static IServiceCollection AddAlteredElbLogs(this IServiceCollection services) => services
            .AddSingleton<GetElbLog>()
            .AddSingleton<ElbLogToAlteredLog>()
            .AddSingleton<LogToElasticsearch>()
            .AddSingleton<LogElbS3Event>();
    }
}
