using Altered.Shared;
using Altered.Aws;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Es
{
    public sealed class LogToElasticsearch : AlteredPipeline<IEnumerable<AlteredLog>, StringResponse>
    {
        public LogToElasticsearch(IElasticLowLevelClient es) : base(async (logBatch) =>
        {
            var multiJson = logBatch.ToEsMultiJson();
            //let _ = Write(multiJson)
            var bulkRequest = PostData.String(multiJson);
            var bulkRequestParameters = new BulkRequestParameters
            {
                // todo
            };
            var bulkResponse = await es.BulkAsync<StringResponse>(bulkRequest, bulkRequestParameters);
            return bulkResponse;
            })
        {
        }
    }


    public static class LogToElasticsearchExtensions
    {
        // elasticsearch proprietary jsonish request format
        public static string ToEsMultiJson(this IEnumerable<AlteredLog> esLogs) =>
            (from log in esLogs
             let indexName = $"acl-{log.Time.ToString("yyyy.MM.dd")}"
             let meta = new
             {
                 index = new
                 {
                     _index = indexName,
                     // reusing the type from cw logs
                     // (otherwise we'll use a new index for elb stuff)
                     _type = "acl"
                 }
             }
             select new object[] { meta, log })
            .Flatten()
            .Aggregate(new StringBuilder(), (sb, o) => sb
                .AppendLine(JsonConvert.SerializeObject(o, AlteredJson.DefaultJsonSerializerSettings)))
            .ToString();
    }
}
