using Amazon.AutoScaling;
using Amazon.AutoScaling.Model;
using Altered.Aws;
using Elasticsearch.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Altered.Logs.Es;
using Altered.Shared;

namespace Altered.Logs.Autoscaling
{
    public class LogAutoscalingEvent : AlteredPipeline<JObject, StringResponse>
    {
        public LogAutoscalingEvent(IElasticLowLevelClient es, IAmazonAutoScaling asg, LogToElasticsearch logToEs) : base(request =>
            (from asgEvent in Observable.Return(request)
             let asgName = asgEvent.SelectToken("detail.AutoScalingGroupName")?.Value<string>()
             where !string.IsNullOrEmpty(asgName)
             let describeRequest = new DescribeAutoScalingGroupsRequest
             {
                 AutoScalingGroupNames = new List<string> { asgName }
             }
             from describeResponse in asg.DescribeAutoScalingGroupsAsync(describeRequest)
             from description in describeResponse.AutoScalingGroups
             let tags = description.Tags
             let app = tags.GetValue("repo") ?? tags.GetValue("Application") ?? tags.GetValue("app")
             let env = tags.GetValue("env") ?? tags.GetValue("Environment")
             let sha = tags.GetValue("sha")
             let log = new
             {
                 Name = asgEvent["detail-type"]?.Value<string>(),
                 RequestId = asgName,
                 Message = asgEvent
             }
             let time = asgEvent["time"].Value<DateTime>()
             let alteredLog = new AlteredLog
             {
                 Time = time,
                 App = app,
                 Env = env,
                 Sha = sha,
                 Log = JObject.FromObject(log, AlteredJson.DefaultJsonSerializer)
             }
             from response in logToEs.Execute(alteredLog)
             select response).ToTask())
        {

        }
    }
}
