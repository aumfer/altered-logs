using Altered.Shared;
using Amazon.Lambda.CloudWatchLogsEvents;
using Amazon.Lambda.Core;
using Elasticsearch.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Altered.Logs
{
    public static class Program
    {
        // this little entry point allows us to call any of the lambdas contained in this build,
        // or to run the ASP.NET MVC Web Host

        // dotnet run -p src/Altered.Logs Altered.Logs.Api.Lambda Execute
        // dotnet run -p src/Altered.Logs Altered.Logs.Mvc Host
        public static int Main(string[] args) =>
            (from name in (args.Skip(0).Take(1).Concat(new[] { DefaultName }).Take(1)).ToObservable()
             let envHost = args.Skip(1).Take(1).FirstOrDefault() ?? Environment.GetEnvironmentVariable("TF_VAR_env_host") ?? DefaultHost
             let type = typeof(Program).Assembly.GetType(name, true, true)
             let method = type.GetMethod(envHost, BindingFlags.Static | BindingFlags.Public)
             //let x = method.CreateDelegate(method.
             from requestJson in args.Length != 0 ? JToken.ReadFromAsync(new JsonTextReader(Console.In)) : Task.FromResult(DefaultParams)
                 //let _ = AlteredConsole.WriteLine($"{requestJson}")
             from requestParameter in method.GetParameters().Skip(0).Take(1)
                 //let __ = AlteredConsole.WriteLine($"{requestParameter}")
             let request = requestJson.ToObject(requestParameter.ParameterType)
             from response in method.Invoke(null, new[] { request }) as Task<StringResponse>
             let responseJson = AlteredJson.SerializeObject(response)
             select AlteredConsole.WriteLine(responseJson))
            .ToTask().Result;

        public static readonly string DefaultName = typeof(Mvc).FullName;
        public static readonly string DefaultHost = "Host";
        public static readonly JToken DefaultParams = new JArray();

        //public static readonly string DefaultName = typeof(Template.Lambda).FullName;
        //public static readonly string DefaultHost = "Execute";
        //public static readonly JToken DefaultParams = JObject.FromObject(new 
        //{
        //});
    }
}
