using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace EmployeeService
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Found);
                string schemeValue = actionContext.Request.Headers.Authorization.Scheme.ToString();
                string parameterValue = actionContext.Request.Headers.Authorization.Parameter.ToString();
                string extraValue = schemeValue + " : " + parameterValue;
                actionContext.Response.Content = new StringContent
                    ("<p>Use https instead of http</p>", Encoding.UTF8, "text/html");

                UriBuilder uriBuilder = new UriBuilder(actionContext.Request.RequestUri);
                uriBuilder.Scheme = Uri.UriSchemeHttps;
                uriBuilder.Port = 44349;

                //UriBuilder uriBuilder = new UriBuilder(Uri.UriSchemeHttps, "", 44349, "", extraValue);

                actionContext.Response.Headers.Location = uriBuilder.Uri;
                actionContext.Response.Headers.Add("SchemeValue", schemeValue);
                actionContext.Response.Headers.Add("ParameterValue", parameterValue);

            }
            else
            {
                base.OnAuthorization(actionContext);
            }

        }
    }
}