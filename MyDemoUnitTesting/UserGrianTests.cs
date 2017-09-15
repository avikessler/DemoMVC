using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MyDemoUnitTesting
{
  [TestClass]
  public class UserGrianTests
  {
    [TestMethod]
    public async Task LoginSeccessTest()
    {
      var userBL = new MyDemoSharedGrains.UserGrain();
      await userBL.SetUserEmail("avi.kessler@gmail.com");
      Assert.IsTrue(await userBL.Login("1"));

    }

    [TestMethod]
    public async Task LoginFailedTest()
    {
      var userBL = new MyDemoSharedGrains.UserGrain();
      await  userBL.SetUserEmail("avi.kessler@gmail.com");
      Assert.IsFalse(await userBL.Login("2"));

    }

  }
}
