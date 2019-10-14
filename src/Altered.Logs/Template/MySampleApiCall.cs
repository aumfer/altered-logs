using Altered.Shared;
using Altered.Aws;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json.Linq;

namespace Altered.Logs.Template
{
    /**
     * STEP 1
     * 
     * define the request/response shapes of MySampleApiCall
     */
    public class MySampleApiCallRequest : TemplateFooStandardRequest
    {
        public JObject MyDataIn { get; set; }
    }
    public class MySampleApiCallResponse : TemplateFooStandardResponse
    {
        public JObject MyDataOut { get; set; }
    }
    
    public interface IMySampleApiCall : IAlteredPipeline<MySampleApiCallRequest, MySampleApiCallResponse> { }
    /**
     * STEP 2
     * 
     * declare an implementation MySampleApiCall and its dependencies
     */
    public sealed class MySampleApiCallService : AlteredPipeline<MySampleApiCallRequest, MySampleApiCallResponse>,
        IMySampleApiCall
    {
        public MySampleApiCallService(/*MyDependency foo*/) : base(async request =>
        {
            if (request.MyDataIn == null)
            {
                return new MySampleApiCallResponse
                {
                    StatusCode = 400
                };
            }

            var someWorkRequest = new
            {
                request.RequestId
            };
            //var someWorkResponse = await foo.Execute(request);

            var myDataOut = JObject.Parse($"{request.MyDataIn}");
            return new MySampleApiCallResponse
            {
                StatusCode = 200,
                MyDataOut = myDataOut
            };
        })
        { }
    }
    public sealed class MySampleApiCallClient : AlteredPipeline<MySampleApiCallRequest, MySampleApiCallResponse>,
        IMySampleApiCall
    {
        static MySampleApiCallClient()
        {
            FlurlHttp.Configure(c => c.JsonSerializer = new NewtonsoftJsonSerializer(AlteredJson.DefaultJsonSerializerSettings));
        }

        public MySampleApiCallClient(string uri /*, ISwaggerGenClient client OR HttpClient client etc */) : base(async request =>
        {
            var httpResponse = await uri.AppendPathSegment("template")
                .WithOAuthBearerToken(request.BearerToken)
                .PostStringAsync(AlteredJson.SerializeObject(request.MyDataIn));

            var json = await httpResponse.Content.ReadAsStringAsync();
            var myDataOut = JObject.Parse(json);

            var response = new MySampleApiCallResponse
            {
                StatusCode = httpResponse.StatusCode,
                MyDataOut = myDataOut
            };
            return response;
        })
        { }
    }
}
