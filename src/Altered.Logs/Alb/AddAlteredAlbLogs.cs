using Altered.Logs.Es;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Alb
{
    public static class AddAlteredAlbLogsExtensions
    {
        public static IServiceCollection AddAlteredAlbLogs(this IServiceCollection services) => services
            .AddSingleton<GetAlbLog>()
            .AddSingleton<AlbLogToAlteredLog>()
            .AddSingleton<LogToElasticsearch>()
            .AddSingleton<LogAlbS3Event>();
    }
}
