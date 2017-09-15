using Orleans;
using Orleans.Runtime.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyDemoSite
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      initClientSilo();
    }

    private void initClientSilo(int reccount = 10)
    {
      try
      {
        var clientConfig = ClientConfiguration.LocalhostSilo(30000);
        GrainClient.Initialize(clientConfig);
      }
      catch (Exception ex)
      {
        if (reccount > 0)
        {
          System.Threading.Thread.Sleep(1000);
          initClientSilo(reccount - 1);
        }
        else throw;
      }

    }
  }
}
