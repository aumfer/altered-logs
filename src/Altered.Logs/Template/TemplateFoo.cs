using Altered.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Altered.Logs.Template
{
    /**
     * STEP 0
     * 
     * define a standard request/response shape for your service
     * used for common params (eg entityid, bearertoken)
     */
    public class TemplateFooStandardRequest : AlteredRequest
    {
        // todo define service request shape here
        public string BearerToken { get; set; }
    }

    public class TemplateFooStandardResponse : AlteredResponse
    {
        // todo define service response shape here
    }

    /**
     * STEP 4
     * 
     * define dependency injectors for your service
     * a different one for each context you want to run it in
     * (eg client, lambda, aspnet, test mock, etc)
     */
    public static class TemplateFoo
    {
        public static IServiceCollection AddTemplateFoo(this IServiceCollection services /*, MyTemplateApiSettings settings*/) => services
            //.AddMyDependencyClient(settings.MyDependency)
            .AddSingleton<IMySampleApiCall, MySampleApiCallService>()
            .AddSingleton<MySampleApiCallApi>()
            .AddSingleton<MySampleApiCallMvc>();

        public static IServiceCollection AddTemplateFooTest(this IServiceCollection services /*, MyTemplateApiSettings settings*/) => services
            //.AddMyDependencyTest(settings.MyDependency)
            ;//.AddSingleton<IMySampleApiCall MySampleApiCallTest>();
    }
}
