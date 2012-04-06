using System.Net.Http;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApi.SelfHosted.Handlers
{
    public class InlineCountHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if(!ShouldInlineCount(request))
                return base.SendAsync(request, cancellationToken);

            // Otherwise, we have a continuation to work our magic...
            return base.SendAsync(request, cancellationToken).ContinueWith(
                t =>
                {
                    var response = t.Result;

                    // Only do work if the response is OK
                    if (response.StatusCode != HttpStatusCode.OK) return response;

                    // Only do work if we are an ObjectContent
                    var objectContent = response.Content as ObjectContent;
                    if (objectContent == null) return response;

                    // Only do work if the ObjectContent's value is IQueryable<>
                    var pagedResultsValue = this.GetValueFromObjectContent(response.Content);
                    var interfaceType =
                        pagedResultsValue.GetType().GetInterfaces().FirstOrDefault(
                            i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>));
                    if (interfaceType == null) return response;

                    // Reissue the request without a skip/take to get our count. This will preserve filtering which
                    // could affect the count
                    var newRequest = new HttpRequestMessage(
                        request.Method,
                        request.RequestUri.AbsoluteUri.Replace("$skip=", "$_skip=").Replace("$top=", "$_top="));

                    // Get the result with no paging
                    var unpagedTaskResult = base.SendAsync(newRequest, cancellationToken).Result;
                    var unpagedResultsValue = this.GetValueFromObjectContent(unpagedTaskResult.Content);

                    // Get a ResultValue<T> for our interfaceType
                    var genericType = interfaceType.GetGenericArguments().First();

                    var resultsValueMethod =
                        this.GetType().GetMethod("CreateResultValue", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(new[] { genericType });
                    // Create the result value with dynamic type
                    var resultValue = resultsValueMethod.Invoke(
                        this, new[] { unpagedResultsValue, pagedResultsValue });

                    // Push the new content and return the response
                    response.Content = CreateObjectContent(resultValue);
                    return response;
                });
        }

        private bool ShouldInlineCount(HttpRequestMessage request)
        {
            var queryParams = request.RequestUri.ParseQueryString();
            var inlinecount = queryParams["$inlinecount"];
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
            var property = typeof(ObjectContent).GetProperty("Value", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property == null) return null;

            return property.GetValue(content, null);
        }

        // We need this because ObjectContent's constructors are internal
        private ObjectContent CreateObjectContent(object value)
        {
            if (value == null) return null;

            var ctor = typeof(ObjectContent).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(
                ci =>
                {
                    var parameters = ci.GetParameters();
                    if (parameters.Length != 2) return false;
                    if (parameters[0].ParameterType != typeof(Type)) return false;
                    if (parameters[1].ParameterType != typeof(object)) return false;
                    return true;
                });

            if (ctor == null) return null;
            return ctor.Invoke(new[] { value.GetType(), value }) as ObjectContent;
        }

    }
}
