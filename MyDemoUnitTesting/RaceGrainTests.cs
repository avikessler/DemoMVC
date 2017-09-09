using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDemoSharedGrains;
using Moq;
using System.Linq;
namespace MyDemoUnitTesting
{
  [TestClass]
  public class RaceGrainTests
  {
    [TestMethod]
    public void RaceTest()
    {

      Mock<CarGrain> car1 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car1.SetupGet(c => c.carId).Returns(1);
      car1.Object.Init("speedy gonzales").Wait();


      Mock<CarGrain> car2 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car2.SetupGet(c => c.carId).Returns(2);
      car2.Object.Init("Bimba").Wait();

      Mock<CarGrain> car3 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car3.SetupGet(c => c.carId).Returns(3);
      car3.Object.Init("speed racer").Wait();


      RaceGrain race = new RaceGrain();
      race.Init("Formula 1", 0.3).Wait();
      car1.SetupGet(c => c.race).Returns(race);
      car2.SetupGet(c => c.race).Returns(race);
      car3.SetupGet(c => c.race).Returns(race);



      car1.Object.AttendInRace(0);
      car2.Object.AttendInRace(0);
      car3.Object.AttendInRace(0);

      car1.Object.SetSpeed(300);
      car2.Object.SetSpeed(200);
      car3.Object.SetSpeed(100);

      System.Threading.Thread.Sleep(1000);

      car1.Object.SetSpeed(300);
      car2.Object.SetSpeed(400);
      car3.Object.SetSpeed(200);
      Assert.IsTrue(race.IsRaceActive().Result, "check if the race is still active");
      System.Threading.Thread.Sleep(1000);

      car1.Object.SetSpeed(400);
      car2.Object.SetSpeed(400);
      car3.Object.SetSpeed(200);
      Assert.IsTrue(race.IsRaceActive().Result, "check if the race is still active");
      System.Threading.Thread.Sleep(1000);

      car1.Object.SetSpeed(800);
      car2.Object.SetSpeed(100);
      car3.Object.SetSpeed(200);
      Assert.IsTrue(race.IsRaceActive().Result, "check if the race is still active");
      System.Threading.Thread.Sleep(1000);

      car1.Object.SetSpeed(600);
      car2.Object.SetSpeed(400);
      car3.Object.SetSpeed(500);

      Assert.IsTrue(race.IsRaceActive().Result, "check if the race is still active");

      System.Threading.Thread.Sleep(1000);

      car1.Object.SetSpeed(800);
      car2.Object.SetSpeed(400);
      car3.Object.SetSpeed(400);

      Assert.IsFalse(race.IsRaceActive().Result, "check if the race is not active");

      Assert.AreEqual <long>(race.GetCarsStatus().Result.ElementAt(0).Key, 1,"the first car should win");



      /// should be driving for 1 sec (plus minus) in an average speed of 125MK meaning the car should pass (1 / 60 / 60) * 125) = ~ 0.0347222222222222
      // Assert.IsTrue(car1.GetKMPassed().Result > 0.03 && car.GetKMPassed().Result < 0.04);




    }



  }
}
