using Altered.Aws;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Logs
{
    // in addition to lambda, alb+lambda, and mvc, (and nuget and exe)
    // you can also host your deployable artifact as an sns subscription
    public sealed class AlteredSns<TRequest, TResponse> : AlteredPipeline<AlteredApiRequest, AlteredApiRequest>
    {
        public AlteredSns(IAlteredPipeline<TRequest, TResponse> operation) : base(async (request) =>
        {
            throw new NotImplementedException();
        }) {}
    }
}
