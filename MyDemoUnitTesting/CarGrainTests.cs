using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MyDemoUnitTesting
{
  [TestClass]
  public class CarGrainTests
  {
    [TestMethod]
    public async Task CarDriveTest()
    {
      var car = new MyDemoSharedGrains.CarGrain();
      await car.Init("speedy gonzales");
      await car.SetSpeed(100);
      await Task.Delay(1000);
      await car.SetSpeed(150);


      /// should be driving for 1 sec (plus minus) in an average speed of 125MK meaning the car should pass (1 / 60 / 60) * 125) = ~ 0.0347222222222222
      Assert.IsTrue(await car.GetKMPassed() > 0.03 && await car.GetKMPassed() < 0.04);




    }



  }
}
