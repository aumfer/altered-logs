using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Codebuild
{
    public static class AddAlteredCodebuildLogsExtensions
    {
        public static IServiceCollection AddAlteredCodebuildLogs(this IServiceCollection services) => services
            .AddSingleton<LogCodebuildEvent>();
    }
}
