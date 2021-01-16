using PX.Data;
using System;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace CustomController.Helpers
{
    public sealed class BasicAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public BasicAuthorizeAttribute()
        {
        }
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            bool authorized = false,
                allowAnonymous = false,
                missingCredentials = false;
            var anonActionAttributes = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true);
            var anonControllerAttributes = actionContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true);

            if (anonActionAttributes.Count > 0 || anonControllerAttributes.Count > 0)
                allowAnonymous = true;
            var authorizeHeader = actionContext.Request.Headers.Authorization;
            if (authorizeHeader != null && authorizeHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(authorizeHeader.Parameter))
            {
                var encoding = Encoding.GetEncoding("ISO-8859-1");
                var credintials = encoding.GetString(Convert.FromBase64String(authorizeHeader.Parameter));

                var splitted = credintials.Split(':');
                if (splitted.Length < 2)
                    actionContext.Response = new Util.UnauthorizedMessage();
                else
                {
                    string company = string.Empty;
                    string username = string.Empty;
                    string password = string.Empty;
                    if (splitted.Length>2)
                    {
                        company = splitted[0].ToUpper();
                        username = splitted[1].ToUpper();
                        password = splitted[2];
                    }
                    else if(splitted.Length==2)
                    {
                        username = splitted[0].ToUpper();
                        password = splitted[1];
                    }
                    if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                    {
                        var userName = PXLogin.ConcatLogin(username, company);
                        var retVal = PXLogin.LoginUser(ref userName,password);
                        if (retVal)
                        {
                            authorized = true;
                        }
                    }
                    else
                        missingCredentials = true;
                }
            }
            else
                missingCredentials = true;
            if(!allowAnonymous && missingCredentials)
                actionContext.Response = new Util.ErrorMessage { Message = "Credentials are missing (protocol error)." };
            else if (!allowAnonymous && !authorized)
                actionContext.Response = new Util.UnauthorizedMessage();
        }
    }
}
