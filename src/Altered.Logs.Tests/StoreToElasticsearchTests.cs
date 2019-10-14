using Altered.Shared.Extensions;
using Amazon.S3;
using Altered.Logs.Es;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Altered.Aws;
using System.Linq;
using Altered.Shared;
using Altered.Logs.Models;

namespace Altered.Logs.Tests
{
    public class StoreToElasticSearchTests
    {
        static readonly ServiceProvider services = new ServiceCollection()
                .AddAlteredLogsService()
                .BuildServiceProvider();

        static readonly IAmazonS3 s3 = services.GetService<IAmazonS3>();
        static readonly StoreToElasticsearch storeToElasticsearch = services.GetService<StoreToElasticsearch>();

        [Fact]
        async Task StoreToElasticsearch_StoresTerraform()
        {
            var id = "altered-logs/master";

            var bucketObject = await s3.GetObjectAsync("altered-terraform", id);

            Assert.True(bucketObject.HttpStatusCode.Is2XX());

            var tfState = JToken.ReadFrom(new JsonTextReader(new StreamReader(bucketObject.ResponseStream))) as JObject;

            Assert.NotNull(tfState);

            var tfModel = tfState.ToTerraformModel();

            var store = new StoreToElasticsearchRequest
            {
                Id = id,
                Data = JObject.FromObject(tfModel, AlteredJson.DefaultJsonSerializer)
            };
            var storedObject = await storeToElasticsearch.Execute(store);

            Assert.True(storedObject.Success);

            var actions = tfState.SelectTokens("$...resources.*.primary.attributes.action");

            await Task.CompletedTask;
        }
    }
}
