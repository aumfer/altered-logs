using Altered.Aws;
using Altered.Aws.Cloudwatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs.Template
{
    // see TemplateFooController.cs
    // this is a pipeline to use in place of MySampleApiCallApi if u need mvc controllers
    public sealed class MySampleApiCallMvc : AlteredPipeline<MySampleApiCallRequest, MySampleApiCallResponse>
    {
        public MySampleApiCallMvc(IMySampleApiCall mySampleApiCall, CloudwatchLogsSink logs, CloudwatchMetricsSink metrics)
            : base(mySampleApiCall
                  .WithAlteredLogsApi()
                  .WithCloudwatchMetrics(metrics, nameof(MySampleApiCallMvc))
                  .WithCloudwatchLogs(logs, nameof(MySampleApiCallMvc)))
                    // this function could populate the Entity property of your request
                    // by reading the EntityId property and calling cdom
                    // that code would be written once, and work with code/lambda/api/exe/etc
                    // as long as its request shape implements IEntity
                    //.WithCdomEntity()
        { }
    }
}
