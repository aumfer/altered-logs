using Altered.Pipeline.Pipelines;
using Altered.Shared;
using Altered.Aws;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Altered.Shared.Interfaces;

namespace Altered.Logs
{
    // common api/mvc/service/client/etc request/response logic
    public sealed class AlteredLogsPipeline<TRequest, TResponse> : AlteredPipeline<TRequest, TResponse>
        where TRequest : IRequestId
        where TResponse : IRequestId, IRequestDuration, IStatusCode
    {
        public AlteredLogsPipeline(IAlteredPipeline<TRequest, TResponse> incomingPipeline) : base(incomingPipeline
            .AsFunc()
            .WithCopyRequestId()
            .WithMeasureRequestDuration())
        {
        }
    }

    public static class AlteredLogsPipelineExtensions
    {
        public static IAlteredPipeline<TRequest, TResponse> WithAlteredLogsApi<TRequest, TResponse>(this IAlteredPipeline<TRequest, TResponse> pipeline)
            where TRequest : IRequestId
        where TResponse : IRequestId, IRequestDuration, IStatusCode =>
            new AlteredLogsPipeline<TRequest, TResponse>(pipeline);
    }
}
