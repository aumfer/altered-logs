using Altered.Aws;
using Altered.Aws.S3;
using Amazon.S3;
using Amazon.S3.Util;
using Altered.Logs.Es;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reactive.Linq;
using System.Linq;
using Altered.Logs.Models;
using Altered.Shared;

namespace Altered.Logs.Terraform
{
    public sealed class LogTerraformS3Event : AlteredPipelineEx<S3EventNotification, StringResponse>
    {
        public LogTerraformS3Event(IAmazonS3 s3, StoreToElasticsearch storeToElasticsearch) : base(events =>
            from bo in s3.GetBucketObjects(events)
            from tfState in JToken.ReadFromAsync(new JsonTextReader(new StreamReader(bo.ResponseStream)))
            let tfModel = tfState.ToTerraformModel()
            let store = new StoreToElasticsearchRequest
            {
                Id = bo.Key,
                Data = JObject.FromObject(tfModel, AlteredJson.DefaultJsonSerializer)
            }
            from response in storeToElasticsearch.Execute(store)
            select response)
        { }
    }
}
