using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using System.Net;
using System.Net.Http;

namespace WebApi.MvcHosted.Filters
{
    public class AllYourErrorsAreTeapotsFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // actionExecutedContext.Exception, if you want to do something with it.
            
            // Change the status and message to prevent information leakage!
            var response = actionExecutedContext.Request.CreateResponse((HttpStatusCode)418, "Out of coffee :(");
            actionExecutedContext.Response = response;

            base.OnException(actionExecutedContext);
        }
    }
}