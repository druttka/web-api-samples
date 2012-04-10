using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;

namespace WebApi.MvcHosted.Filters
{
    public class LogErrorFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            // TODO: Log however you want to
            
            // Maybe we want to change the status and content, we can do that here.
            var response = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)418
            };
            response.CreateContent("Out of coffee :(");

            actionExecutedContext.Result = response;
            base.OnException(actionExecutedContext);
        }
    }
}