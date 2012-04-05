using System.Net.Http;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApi.SelfHosted.Api.Controllers;

namespace WebApi.SelfHosted.Handlers
{
    public class InlineCountHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var queryParams = request.RequestUri.ParseQueryString();
            var inlinecount = queryParams["$inlinecount"];

            // No inline count, just do what they asked
            if (string.Compare(inlinecount, "allpages", true) != 0) return base.SendAsync(request, cancellationToken);

            return base.SendAsync(request, cancellationToken).ContinueWith(
                t =>
                {
                    var response = t.Result;
                    // Only do work if the response is OK
                    if (response.StatusCode != HttpStatusCode.OK) return response;

                    // Only do work if we are an ObjectContent of IQueryable
                    var objectContent = response.Content as ObjectContent;
                    if (objectContent == null) return response;

                    var pagedResults = GetValue(response.Content) as IQueryable<object>;
                    if (pagedResults == null) return response;

                    // Clone the response
                    var newResponse = CloneResponse(response);

                    // Reissue the request without a skip/take, because that is our count. Then return a 
                    // response that has both pieces of data
                    var newRequest = new HttpRequestMessage(
                        request.Method,
                        request.RequestUri.AbsoluteUri.Replace("$skip=", "$_skip=").Replace("$top=", "$_top="));

                    // Get the result with no paging
                    var unpagedTaskResult = base.SendAsync(newRequest, cancellationToken).Result;
                    var unpagedResults = GetValue(unpagedTaskResult.Content) as IQueryable<object>;

                    // Use the total count, but the paged results
                    var pagedArray = pagedResults.ToArray();
                    var resultValue = GetResultValue(unpagedResults.Count(), pagedArray);

                    SetContent(newResponse, resultValue);

                    return newResponse;
                });
        }

        private object GetResultValue(int count, object[] pagedArray)
        {
            var genericType = typeof(ResultValue<>);
            var t = pagedArray.First().GetType();
            var constructedType = genericType.MakeGenericType(new[] { t });

            var ctor = constructedType
                .GetConstructors().First();

            var instance = ctor.Invoke(null);
            var countProperty = constructedType.GetProperty("Count");
            countProperty.SetValue(instance, count, null);

            var setResultsMethod = constructedType.GetMethod("SetResults");
            setResultsMethod.Invoke(instance, new[] { pagedArray });

            return instance;
        }

        private HttpResponseMessage CloneResponse(HttpResponseMessage response)
        {
            var clone = new HttpResponseMessage
            {
                ReasonPhrase = response.ReasonPhrase,
                RequestMessage = response.RequestMessage,
                Version = response.Version,
                StatusCode = response.StatusCode
            };

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }

            return clone;
        }

        public class ResultValue<T>
        {
            public int Count { get; set; }
            public T[] Results { get; set; }

            public void SetResults(object[] results)
            {
                Results = results.Select(x => (T)x).ToArray();
            }
        }

        private object GetValue(HttpContent content)
        {
            var property = typeof(ObjectContent).GetProperty("Value", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property == null) return null;

            return property.GetValue(content, null);
        }

        private void SetContent(HttpResponseMessage newResponse, object value)
        {
            if (value == null) return;

            var ctor = typeof(ObjectContent).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Where(
                ci =>
                {
                    var parameters = ci.GetParameters();
                    if (parameters.Length != 2) return false;
                    if (parameters[0].ParameterType != typeof(Type)) return false;
                    if (parameters[1].ParameterType != typeof(object)) return false;
                    return true;
                }).FirstOrDefault();

            if (ctor == null) return;
            newResponse.Content = ctor.Invoke(new[] { value.GetType(), value }) as ObjectContent;
        }

    }
}
