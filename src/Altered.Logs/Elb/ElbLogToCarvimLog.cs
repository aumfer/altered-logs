using Amazon.ElasticLoadBalancing.Model;
using Altered.Aws;
using Altered.Aws.Elb;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using Altered.Shared;

namespace Altered.Logs.Elb
{
    public sealed class ElbLogToAlteredLog : AlteredPipeline<CsvReader, IList<AlteredLog>>
    {
        public ElbLogToAlteredLog(DescribeTags describeTags) : base((csv) =>
            (from r in csv.GetRecords<ElbLogEntry>()
                // run each log entry in parallel
                .ToObservable()
                // todo try 'group by' to hit the cache less
                .SubscribeOn(Scheduler.Default)
             let elbName = r.Elb
             let describeTagsRequest = new DescribeTagsRequest
             {
                 LoadBalancerNames = new List<string> { elbName }
             }
             from describeTagsResponse in describeTags.Execute(describeTagsRequest)
             let tags = describeTagsResponse.TagDescriptions
             let app = tags.GetValue("repo") ?? tags.GetValue("Application") ?? tags.GetValue("app")
             let env = tags.GetValue("env") ?? tags.GetValue("Environment")
             let sha = tags.GetValue("sha")
             let duration = TimeSpan.FromSeconds(Math.Max(0, r.RequestProcessingTime)
                + Math.Max(0, r.BackendProcessingTime)
                + Math.Max(0, r.ResponseProcessingTime)).TotalMilliseconds
             let log = new
             {
                 Name = r.Request,
                 RequestId = elbName,
                 Response = new
                 {
                     StatusCode = r.ElbStatusCode,
                     RequestDuration = duration,
                     Elb = r
                 }
             }
             select new AlteredLog
             {
                 Time = r.Timestamp,
                 App = app,
                 Env = env,
                 Sha = sha,
                 Log = JObject.FromObject(log, AlteredJson.DefaultJsonSerializer)
             }).ToList().ToTask())
        {
        }
    }

    public static class AlbLogToAlteredLogExtensions
    {
    }
}
