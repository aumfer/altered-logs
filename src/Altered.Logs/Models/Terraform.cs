using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altered.Logs.Models
{
    public sealed class TerraformOutput
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public sealed class TerraformResource
    {
        public string Type { get; set; }

        public string Id { get; set; }

        public Dictionary<string, string> Attributes { get; set; }
    }

    public sealed class TerraformModel
    {
        public TerraformOutput[] Outputs { get; set; }

        public TerraformResource[] Resources { get; set; }
    }

    public static class TerraformExtensions
    {
        public static TerraformModel ToTerraformModel(this JToken tfState)
        {
            var outputs = from output in tfState.SelectTokens("$...outputs")
                          from p in ((JObject)output).Properties()
                          let outputName = p.Name
                          let outputValue = p.Value?["value"]?.ToString()
                          select new TerraformOutput
                          {
                              Name = outputName,
                              Value = outputValue
                          };
            var resources = from resource in tfState.SelectTokens("$...resources.*")
                            let resourceType = resource["type"]?.ToObject<string>()
                            let resourceId = resource.SelectToken("primary.id")?.ToObject<string>()
                            let resourceAttributes = resource.SelectToken("primary.attributes")
                                // had some issues with attributes getting wonky in elasticsearch
                                // stringifying values below this level
                                .Value<JObject>().Properties()
                                .ToDictionary(p => p.Name, p => p.Value?.ToString())
                            select new TerraformResource
                            {
                                Type = resourceType,
                                Id = resourceId,
                                Attributes = resourceAttributes
                            };

            var tfModel = new TerraformModel
            {
                Outputs = outputs.ToArray(),
                Resources = resources.ToArray()
            };
            return tfModel;
        }
    }
}
