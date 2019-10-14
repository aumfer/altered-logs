using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Cloudwatch
{
    public static class AddAlteredCloudwatchLogsExtensions
    {
        public static IServiceCollection AddAlteredCloudwatchLogs(this IServiceCollection services) => services
            .AddSingleton<LogCloudwatchLog>();
    }
}
