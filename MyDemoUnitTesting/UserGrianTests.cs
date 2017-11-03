using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Moq;
using MyDemoSharedGrains;
namespace MyDemoUnitTesting
{
  [TestClass]
  public class UserGrianTests
  {
    [TestMethod]
    public async Task LoginSeccessTest()
    {
      Mock<UserGrain> user = new Moq.Mock<UserGrain>(Moq.MockBehavior.Loose);
      user.SetupGet( u => u.Email).Returns("avi.kessler@gmail.com");
      Assert.IsTrue(await user.Object.Login("1"));

    }

    [TestMethod]
    public async Task LoginFailedTest()
    {
      Mock<UserGrain> user = new Moq.Mock<UserGrain>(Moq.MockBehavior.Loose);
      user.SetupGet(u => u.Email).Returns("avi.kessler@gmail.com");
      Assert.IsFalse(await user.Object.Login("2"));

    }

    [TestMethod]
    public async Task RegisterTest()
    {
      Mock<UserGrain> user = new Moq.Mock<UserGrain>(Moq.MockBehavior.Loose);
      Guid g = Guid.NewGuid();
      user.SetupGet(u => u.Email).Returns($"avi.kessler+{g.ToString("N")}@gmail.com");
      Assert.IsTrue(await user.Object.Register(g.ToString("N")));

    }


  }
}
