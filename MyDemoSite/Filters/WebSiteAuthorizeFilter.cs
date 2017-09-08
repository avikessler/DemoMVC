using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MyDemoSite.Filters
{
  public class WebSiteAuthorizeFilter : System.Web.Mvc.AuthorizeAttribute
  {

    protected override bool AuthorizeCore(HttpContextBase httpContext)
    {
      return (!string.IsNullOrWhiteSpace(httpContext.Request.Cookies["user"]?.Value));
    }
    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
      filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
         new { action = "Index", controller = "Login" }));    

      

    }

  }
}