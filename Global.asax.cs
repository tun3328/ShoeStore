using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace EshopperMCV
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;

            FormsAuthenticationTicket ticket =
                FormsAuthentication.Decrypt(authCookie.Value);

            if (ticket == null) return;

            string[] roles = ticket.UserData.Split(',');

            HttpContext.Current.User = new GenericPrincipal(
                new FormsIdentity(ticket),
                roles
            );
        }

    }
}
