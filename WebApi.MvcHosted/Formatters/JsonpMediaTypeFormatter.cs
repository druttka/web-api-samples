// Adapted from git://gist.github.com/2967547.git, jonathanhunt
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebApi.Shared
{
    /// <summary>
    /// Custom WebAPI media type formatter for the streaming JSONP data to the API consumer
    /// </summary>
    public class JsonpMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private readonly string _callbackQueryParameter;

        public JsonpMediaTypeFormatter(string callbackQueryParameter)
        {
            _callbackQueryParameter = callbackQueryParameter ?? "callback";
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/javascript"));
        }

        private string CallbackFunction { get; set; }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var isJsonp = !string.IsNullOrEmpty(CallbackFunction);

            if (!isJsonp)
                return base.WriteToStreamAsync(type, value, writeStream, content, transportContext);

            return Task.Factory.StartNew(() =>
            {
                using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(writeStream, Encoding.UTF8)) { CloseOutput = false })
                {
                    jsonTextWriter.WriteRaw(CallbackFunction + "(");

                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonTextWriter, value);

                    jsonTextWriter.WriteRaw(")");

                    jsonTextWriter.Flush();
                }
            });
        }

        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            var formatter = new JsonpMediaTypeFormatter(_callbackQueryParameter)
            {
                CallbackFunction = GetJsonCallbackFunction(request)
            };

            return formatter;
        }

        private string GetJsonCallbackFunction(HttpRequestMessage request)
        {
            if (request.Method != HttpMethod.Get) return null;

            var query = request.RequestUri.ParseQueryString();
            var queryVal = query[_callbackQueryParameter];

            if (string.IsNullOrEmpty(queryVal)) return null;

            return queryVal;
        }
    }
}