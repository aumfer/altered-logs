using Amazon.CodeBuild;
using Amazon.CodeBuild.Model;
using Altered.Aws;
using Elasticsearch.Net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using Altered.Logs.Es;

namespace Altered.Logs.Codebuild
{
    public sealed class LogCodebuildEvent : AlteredPipeline<JObject, StringResponse>
    {
        public LogCodebuildEvent(LogToElasticsearch logToEs, IAmazonCodeBuild cb) : base(request =>
        (from codebuildEvent in Observable.Return(request)
         let projectName = codebuildEvent.SelectToken("detail.project-name")?.ToObject<string>()
         where !string.IsNullOrEmpty(projectName)
         let batchGetProjectsRequest = new BatchGetProjectsRequest
         {
             Names = new List<string> { projectName }
         }
         from batchGetProjectsResponse in cb.BatchGetProjectsAsync(batchGetProjectsRequest)
         let time = codebuildEvent["time"].Value<DateTime>()
         from project in batchGetProjectsResponse.Projects
         let tags = project.Tags
         let app = tags.GetValue("repo") ?? tags.GetValue("Application") ?? tags.GetValue("app")
         let env = tags.GetValue("env") ?? tags.GetValue("Environment")
         let sha = tags.GetValue("sha")
         let log = new AlteredLog
         {
             Time = time,
             App = app,
             Env = env,
             Sha = sha,
             Log = codebuildEvent
         }
         from response in logToEs.Execute(log)
         select response).ToTask())
        { }
    }
}
