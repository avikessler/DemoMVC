using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyDemoSite.Controllers
{
  public class LoginController : Controller
  {


    [HttpGet]
    public ActionResult Index()
    {
      return View();
    }



    [HttpPost]
    [ActionName("Index")]
    public ActionResult IndexPost(Models.SiteLoginResponse res)
    {

      if (res.email == "avi.kessler@gmail.com" && res.password == "1")
      {
        Response.SetCookie(new HttpCookie("user", res.email));
        return RedirectToAction("Index", "Home");
      }
      else return View(new Models.SiteLoginRequest { email = res.email, message = "worng user name or password" });
    }
  }
}