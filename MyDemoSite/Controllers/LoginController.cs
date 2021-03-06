﻿using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


    [HttpGet]
    public ActionResult Welcome(string UID, string UIDSig, string timestamp)
    {

      // UID, UIDSig, timestamp, loginProvider, loginProviderUID, nickname, photoURL, thumbnailURL, firstName, lastName, gender, birthDay, birthMonth, birthYear, email, country, state, city, zip, profileURL

      //TODO not working as the gigya account is disabled :(

      return View();



    }





    [HttpPost]
    [ActionName("Index")]
    public async Task<ActionResult> IndexPost(Models.SiteLoginResponse res)
    {

      var userBL = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.IUserGrain>(res.email);

      bool result = await userBL.ValidateUIDSignature(res.UID , res.timestamp, res.UIDSignature);
      if (result)
      // if (await userBL.Login(res.password))
      {
        Response.SetCookie(new HttpCookie("user", res.email));
        return RedirectToAction("Index", "Home");
      }
      else return View(new Models.SiteLoginRequest { email = res.email, message = "wrong user name or password" });
    }

  }
}
