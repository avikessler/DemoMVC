using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyDemoUnitTesting
{
  [TestClass]
  public class UserGrianTests
  {
    [TestMethod]
    public void LoginSeccessTest()
    {
      var userBL = new MyDemoSharedGrains.UserGrain();
      userBL.SetUserEmail("avi.kessler@gmail.com");
      Assert.IsTrue(userBL.Login("1").Result);

    }

    [TestMethod]
    public void LoginFailedTest()
    {
      var userBL = new MyDemoSharedGrains.UserGrain();
      userBL.SetUserEmail("avi.kessler@gmail.com");
      Assert.IsFalse(userBL.Login("2").Result);

    }

  }
}
