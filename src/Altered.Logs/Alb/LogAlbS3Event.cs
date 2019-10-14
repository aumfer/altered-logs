using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Util;
using Altered.Aws;
using Altered.Aws.Alb;
using Elasticsearch.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Altered.Logs.Es;

namespace Altered.Logs.Alb
{
    public sealed class LogAlbS3Event : AlteredPipeline<S3EventNotification, StringResponse>
    {
        public LogAlbS3Event(GetAlbLog getAlbLog, AlbLogToAlteredLog albLogToAlteredLog, LogToElasticsearch logToEs) : base(async (r) =>
            {
                var albLog = await getAlbLog.Execute(r);
                var alteredLog = await albLogToAlteredLog.Execute(albLog);
                var response = await logToEs.Execute(alteredLog);
                return response;
            })
        { }
    }
}
