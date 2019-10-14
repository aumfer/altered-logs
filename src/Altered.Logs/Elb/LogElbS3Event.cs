using Altered.Aws;
using Amazon.S3.Util;
using Altered.Logs.Es;
using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Elb
{
    public sealed class LogElbS3Event : AlteredPipeline<S3EventNotification, StringResponse>
    {
        public LogElbS3Event(GetElbLog getElbLog, ElbLogToAlteredLog elbLogToAlteredLog, LogToElasticsearch logToEs) : base(async (r) =>
        {
            var elbLog = await getElbLog.Execute(r);
            var alteredLog = await elbLogToAlteredLog.Execute(elbLog);
            var response = await logToEs.Execute(alteredLog);
            return response;
        })
        { }
    }
}
