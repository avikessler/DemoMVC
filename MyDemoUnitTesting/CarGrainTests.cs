using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MyDemoUnitTesting
{
  [TestClass]
  public class CarGrainTests
  {
    [TestMethod]
    public void CarDriveTest()
    {
      var car = new MyDemoSharedGrains.CarGrain();
      car.Init("speedy gonzales");
      car.SetSpeed(100).Wait();
      System.Threading.Thread.Sleep(1000);
      car.SetSpeed(150).Wait();


      /// should be driving for 1 sec (plus minus) in an average speed of 125MK meaning the car should pass (1 / 60 / 60) * 125) = ~ 0.0347222222222222
      Assert.IsTrue(car.GetKMPassed().Result > 0.03 && car.GetKMPassed().Result < 0.04);




    }

    

  }
}
