using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Ecs
{
    public static class AddAlteredEcsLogsExtensions
    {
        public static IServiceCollection AddAlteredEcsLogs(this IServiceCollection services) => services
            .AddSingleton<TaskStateChangeToAlteredLog>()
            .AddSingleton<LogEcsTaskStateChange>();
    }
}
