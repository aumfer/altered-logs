using Altered.Aws;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Altered.Logs
{
    public class Proxy : AlteredPipeline<HttpRequestMessage, HttpResponseMessage>
    {
        public Proxy() : this(new HttpClient().SendAsync) { }
        public Proxy(Func<HttpRequestMessage, Task<HttpResponseMessage>> http) : base(request => http(request))
        { }
    }

    public static class AlteredApiExtensions
    {
        // todo https://docs.aws.amazon.com/general/latest/gr/sigv4-signed-request-examples.html
        // https://docs.aws.amazon.com/general/latest/gr/sigv4_elements.html
        public static AlteredApiRequest WithAws(this AlteredApiRequest request)
        {
            var amzDate = DateTime.UtcNow.ToString("YYYYMMDD'T'HHMMSS'Z'");
            request.Headers["x-amz-date"] = amzDate;
            return request;
        }
    }

    public class Client : AlteredPipeline<AlteredApiRequest, AlteredApiResponse>
    {
        public Client(Uri uri, Proxy proxy) : base(async (apiRequest) =>
        {
            // todo put this somewhere
            apiRequest = apiRequest.WithAws();

            var httpRquest = new HttpRequestMessage
            {
                RequestUri = new Uri(QueryHelpers.AddQueryString($"{uri}{apiRequest.Path?.TrimStart('/')}", apiRequest.QueryStringParameters.ToDictionary(kvp => kvp.Key, kvp => $"{kvp.Value}"))),
                Method = new HttpMethod(apiRequest.HttpMethod),
                Content = new StringContent(apiRequest.Body)
            };
            foreach (var kvp in apiRequest.Headers)
            {
                try
                {
                    httpRquest.Headers.Add(kvp.Key, kvp.Value.AsEnumerable());
                }
                catch (InvalidOperationException) { }
            }
            httpRquest.Headers.Host = uri.Host;

            if (apiRequest.Headers.TryGetValue(HeaderNames.ContentType, out StringValues contentType))
            {
                httpRquest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);
            }

            var httpResponse = await proxy.Execute(httpRquest);

            var apiResponse = new AlteredApiResponse();
            apiResponse.StatusCode = (int)httpResponse.StatusCode;
            foreach (var kvp in httpResponse.Headers)
            {
                apiResponse.Headers[kvp.Key] = kvp.Value?.FirstOrDefault();
            }
            //foreach (var kvp in httpResponse.Content.Headers)
            //{
            //    //ignore content length, since we may be de/re compressing
            //    if (kvp.Key == "Content-Length") continue;
            //
            //    apiResponse.Headers[kvp.Key] = kvp.Value?.FirstOrDefault();
            //}
            apiResponse.Headers[HeaderNames.ContentType] = $"{httpResponse.Content.Headers.ContentType}";
            var stream = await httpResponse.Content.ReadAsStreamAsync();
            var contentEncoding = httpResponse.Content.Headers.ContentEncoding;
            var isGzip = contentEncoding?.Any(t => t?.Contains("gzip", StringComparison.OrdinalIgnoreCase) == true) == true;
            var isDefalt = contentEncoding?.Any(t => t?.Contains("deflate", StringComparison.OrdinalIgnoreCase) == true) == true;
            var newStream = isGzip ? new GZipStream(stream, CompressionMode.Decompress) : (isDefalt ? new DeflateStream(stream, CompressionMode.Decompress) : stream);
            //var newStream = stream;
            using (var reader = new StreamReader(newStream))
            {
                apiResponse.Body = await reader.ReadToEndAsync();
            }
            return apiResponse;
        })
        { }
    }
}
