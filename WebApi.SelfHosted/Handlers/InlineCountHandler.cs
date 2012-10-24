using System.Net.Http;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace WebApi.SelfHosted.Handlers
{
    // NOTE: This needs review after the upgrade from Beta to Release bits. There might be a better way to do it now, and we've hacked it up a bit.
    // ALSO NOTE: We are looking for _inlinecount instead of $inlinecount, because the OData Nuget package will preempt us with a "$inlinecount not supported" exception before
    // we even get a chance to run --- again probably a better way to do this now (Marcin noted on original blog post that the Web API team is looking at doing it another way).
    public class InlineCountHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (!ShouldInlineCount(request))
                return base.SendAsync(request, cancellationToken);

            // Otherwise, we have a continuation to work our magic...
            return base.SendAsync(request, cancellationToken).ContinueWith(
                t =>
                {
                    var response = t.Result;

                    // Is this a response we can work with?
                    if (!ResponseIsValid(response)) return response;

                    var pagedResultsValue = this.GetValueFromObjectContent(response.Content);
                    Type queriedType;

                    // Can we find the underlying type of the results?
                    if (pagedResultsValue is IQueryable)
                        queriedType = ((IQueryable)pagedResultsValue).ElementType;
                    else
                        return response;

                    // Reissue the request without a skip/take to get our count. This will preserve filtering which
                    // could affect the count
                    var newRequest = new HttpRequestMessage(
                        request.Method,
                        request.RequestUri.AbsoluteUri.Replace("$skip=", "_skip=").Replace("$top=", "_top="));
                    request.Headers.ToList().ForEach(h => newRequest.Headers.Add(h.Key, h.Value));
                    newRequest.Content = request.Content;

                    // Reissue the request without a skip/take to get our count. This will preserve filtering which
                    // could affect the count
                    var unpagedResult = base.SendAsync(newRequest, cancellationToken).Result;
                    var unpagedResultsValue = this.GetValueFromObjectContent(unpagedResult.Content);

                    var resultsValueMethod =
                        this.GetType().GetMethod(
                            "CreateResultValue", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(
                                new[] { queriedType });
                    // Create the result value with dynamic type
                    var resultValue = resultsValueMethod.Invoke(
                        this, new[] { unpagedResultsValue, pagedResultsValue });

                    // Push the new content and return the response
                    response.Content = new ObjectContent(resultValue.GetType(), resultValue, ((ObjectContent)response.Content).Formatter);
                    return response;
                });
        }

        private bool ResponseIsValid(HttpResponseMessage response)
        {
            // Only do work if the response is OK
            if (response == null || response.StatusCode != HttpStatusCode.OK) return false;

            // Only do work if we are an ObjectContent
            return response.Content is ObjectContent;
        }

        private bool ShouldInlineCount(HttpRequestMessage request)
        {
            var queryParams = request.RequestUri.ParseQueryString();
            var inlinecount = queryParams["_inlinecount"];
            return string.Compare(inlinecount, "allpages", true) == 0;
        }

        // Dynamically invoked for the T returned by the resulting ApiController
        private ResultValue<T> CreateResultValue<T>(IQueryable<T> unpagedResults, IQueryable<T> pagedResults)
        {
            var genericType = typeof(ResultValue<>);
            var constructedType = genericType.MakeGenericType(new[] { typeof(T) });

            var ctor = constructedType
                .GetConstructors().First();

            var instance = ctor.Invoke(null);

            var countProperty = constructedType.GetProperty("Count");
            countProperty.SetValue(instance, unpagedResults.Count(), null);

            var resultsProperty = constructedType.GetProperty("Results");
            resultsProperty.SetValue(instance, pagedResults.ToArray(), null);

            return instance as ResultValue<T>;
        }

        // We need this because ObjectContent's Value property is internal
        private object GetValueFromObjectContent(HttpContent content)
        {
            if (!(content is ObjectContent)) return null;

            return ((ObjectContent)content).Value;
        }

        // We need this because ObjectContent's constructors are internal
        private ObjectContent CreateObjectContent(object value, MediaTypeHeaderValue mthv)
        {
            if (value == null) return null;

            var ctor = typeof(ObjectContent).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(
                ci =>
                {
                    var parameters = ci.GetParameters();
                    if (parameters.Length != 3) return false;
                    if (parameters[0].ParameterType != typeof(Type)) return false;
                    if (parameters[1].ParameterType != typeof(object)) return false;
                    if (parameters[2].ParameterType != typeof(MediaTypeHeaderValue)) return false;
                    return true;
                });

            if (ctor == null) return null;

            return ctor.Invoke(new[] { value.GetType(), value, mthv }) as ObjectContent;
        }

    }
}
