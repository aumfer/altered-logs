using Altered.Shared.Extensions;
using Amazon.ECS.Model;
using Amazon.Lambda.CloudWatchEvents.ECSEvents;
using Altered.Aws;
using Altered.Aws.Ecs;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using Altered.Shared;

namespace Altered.Logs.Ecs
{
    public sealed class TaskStateChangeToAlteredLog : AlteredPipeline<ECSTaskStateChangeEvent, AlteredLog>
    {
        public TaskStateChangeToAlteredLog(DescribeTasks describeTasks) : base(request =>
        (from taskStateChange in new[] { request }.ToObservable()
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
        select new AlteredLog
        {
            Time = taskStateChange.Time,
            App = app,
            Env = env,
            Sha = sha,
            Log = JObject.FromObject(log, AlteredJson.DefaultJsonSerializer)
        }).ToTask())
        { }
    }
}
