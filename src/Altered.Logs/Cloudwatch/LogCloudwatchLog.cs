using Altered.Shared;
using Amazon.Lambda.CloudWatchLogsEvents;
using Altered.Aws;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using Altered.Logs.Es;

namespace Altered.Logs.Cloudwatch
{
    public sealed class LogCloudwatchLog : AlteredPipeline<CloudWatchLogsEvent, StringResponse>
    {
        public LogCloudwatchLog(LogToElasticsearch logToEs) : base(request =>
        (from e in Observable.Return(request)
         let logString = e.Awslogs?.DecodeData()
         //let _ = AlteredConsole.WriteLine(logString)
         let payload = JObject.Parse(logString)
         from logEvent in payload["logEvents"]
         let alteredLog = logEvent["message"]?.Value<string>()
         let log = AlteredTime(JsonConvert.DeserializeObject<AlteredLog>(alteredLog))
         from response in logToEs.Execute(log)
         select response).ToTask())
        { }

        static AlteredLog AlteredTime(AlteredLog o)
        {
            o.AlteredTime = DateTime.UtcNow;
            return o;
        }
    }
}
