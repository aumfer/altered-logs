using Altered.Aws;
using Altered.Aws.Cloudwatch;
using Altered.Logs.Alb;
using Altered.Logs.Autoscaling;
using Altered.Logs.Cloudwatch;
using Altered.Logs.Codebuild;
using Altered.Logs.Dash;
using Altered.Logs.Ecs;
using Altered.Logs.Elb;
using Altered.Logs.Es;
using Altered.Logs.Template;
using Altered.Logs.Terraform;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs
{
    public static class AddAlteredLogsExtensions
    {
        // todo client, mock, etc
        public static IServiceCollection AddAlteredLogsService(this IServiceCollection services, Uri elasticsearchUri = null) => services
            .AddAlteredAws(elasticSearchUri: elasticsearchUri ?? DefaultElasticSearchUri)
            .AddCloudwatchLogs()
            .AddCloudwatchMetrics()
            // the following microservices are code-only parts of altered-logs
            .AddSingleton<LogToElasticsearch>()
            .AddSingleton<StoreToElasticsearch>()
            // the following microservices are lambda and/or aspnet, deployed as part of altered-logs
            .AddAlteredAlbLogs()
            .AddAlteredAutoscalingLogs()
            .AddAlteredCloudwatchLogs()
            .AddAlteredCodebuildLogs()
            .AddAlteredEcsLogs()
            .AddAlteredElbLogs()
            .AddTemplateFoo()
            .AddHttpClient<ProxyDashboard>().Services
            .AddSingleton<LogTerraformS3Event>();

        static readonly Uri DefaultElasticSearchUri = new Uri("https://search-altered-logs-bguqgsgczqwijgrmxahh6wkdpa.us-east-1.es.amazonaws.com");
    }
}
