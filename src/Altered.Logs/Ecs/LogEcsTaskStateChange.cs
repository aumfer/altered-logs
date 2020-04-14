using Amazon.Lambda.CloudWatchEvents.ECSEvents;
using Altered.Aws.Ecs;
using Altered.Aws;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Linq;
using Amazon.ECS.Model;
using Altered.Shared.Extensions;
using System.Reactive.Threading.Tasks;
using Altered.Logs.Es;
using Newtonsoft.Json.Linq;
using Altered.Shared;

namespace Altered.Logs.Ecs
{
    public class LogEcsTaskStateChange : AlteredPipeline<ECSTaskStateChangeEvent, StringResponse>
    {
        public LogEcsTaskStateChange(LogToElasticsearch logToEs, DescribeTasks describeTasks) : base(r =>
            (from taskStateChange in new[] { r }.ToObservable()
             let clusterArn = taskStateChange.Detail?.ClusterArn
             let taskArn = taskStateChange.Detail?.TaskArn
             where !string.IsNullOrEmpty(clusterArn) && !string.IsNullOrEmpty(taskArn)
             let describeTasksRequest = new DescribeTasksRequest
             {
                 Cluster = clusterArn,
                 Tasks = new List<string> { taskArn },
                 Include = new List<string> { "TAGS" }
             }
             from describeTasksResponse in describeTasks.Execute(describeTasksRequest)
                 // lambda will error if this where clause is false
             where describeTasksResponse.HttpStatusCode.Is2XX()
             // once c# 8 comes out, switch expressions solve
             from task in describeTasksResponse.Tasks
             let tags = task.Tags
             let app = tags.GetValue("repo") ?? tags.GetValue("Application") ?? tags.GetValue("app")
             let env = tags.GetValue("env") ?? tags.GetValue("Environment")
             let sha = tags.GetValue("sha")
             let log = new
             {
                 Name = taskStateChange.DetailType,
                 RequestId = taskArn,
                 Message = taskStateChange
             }
             let alteredLog = new AlteredLog
             {
                 Time = taskStateChange.Time,
                 Repo = app,
                 Env = env,
                 Sha = sha,
                 Log = JObject.FromObject(log, AlteredJson.DefaultJsonSerializer)
             }
             from response in logToEs.Execute(alteredLog)
             select response).ToTask())
        { }
        public LogEcsTaskStateChange(IAlteredPipeline<ECSTaskStateChangeEvent, StringResponse> incomingPipeline) : base(incomingPipeline)
        {
        }
    }
}
