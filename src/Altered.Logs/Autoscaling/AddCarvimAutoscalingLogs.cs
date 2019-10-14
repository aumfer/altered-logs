using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Autoscaling
{
    public static class AddAlteredAutoscalingLogsExtensions
    {
        public static IServiceCollection AddAlteredAutoscalingLogs(this IServiceCollection services) => services
            .AddSingleton<LogAutoscalingEvent>();
    }
}
