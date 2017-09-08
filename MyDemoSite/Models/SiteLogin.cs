using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDemoSite.Models
{
  public class SiteLoginResponse
  {
    public string email { get; set; }

    public string password { get; set; }
  }


  public class SiteLoginRequest
  {
    public string email { get; set; }

    public string message { get; set; }
  }
}