using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;

namespace WebApi.MvcHosted.Filters
{
    public class LogErrorFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            // TODO: What have you.
            
            base.OnException(actionExecutedContext);
        }
    }
}