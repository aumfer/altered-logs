using Altered.Aws;
using Altered.Shared;
using Elasticsearch.Net;
using MoreLinq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altered.Logs.Es
{
    public sealed class StoreToElasticsearchRequest
    {
        public string Id { get; set; }
        public JObject Data { get; set; }
    }
    public sealed class StoreToElasticsearch : AlteredPipeline<StoreToElasticsearchRequest, StringResponse>
    {
        public StoreToElasticsearch(IElasticLowLevelClient es, string indexName = "terraform") : base(async (request) =>
        {
            var logBatch = JObject.FromObject(request.Data, AlteredJson.DefaultJsonSerializer);
            var multiJson = logBatch.ToEsMultiJson(indexName);
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


    public static class StoreToElasticSearchExtensions
    {
        // elasticsearch proprietary jsonish request format
        public static string ToEsMultiJson(this JObject esLog, string indexName) => ToEsMultiJson(new[] { esLog }, indexName);
        public static string ToEsMultiJson(this IEnumerable<JObject> esLogs, string indexName) =>
            (from log in esLogs
             let meta = new
             {
                 index = new
                 {
                     _index = indexName,
                     _type = indexName
                 }
             }
             select new object[] { meta, log })
            .SelectMany(m => m)
            .Aggregate(new StringBuilder(), (sb, o) => sb
                .AppendLine(AlteredJson.SerializeObject(o)))
            .ToString();
    }
}
