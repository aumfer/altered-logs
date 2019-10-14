using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs.Template
{
    public sealed class TemplateFooController : ControllerBase
    {
        public async Task MySampleApiCall([FromServices] MySampleApiCallMvc mySampleApiCall)
        {
            // todo
            await mySampleApiCall.Execute(null);
        }
    }
}
