using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDemoSharedGrains;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace MyDemoUnitTesting
{
  [TestClass]
  public class RaceGrainTests
  {
    [TestMethod]
    public async Task RaceTest()
    {

      Mock<CarGrain> car1 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car1.SetupGet(c => c.carId).Returns(1);
      await car1.Object.Init("speedy gonzales");


      Mock<CarGrain> car2 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car2.SetupGet(c => c.carId).Returns(2);
      await car2.Object.Init("Bimba");

      Mock<CarGrain> car3 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car3.SetupGet(c => c.carId).Returns(3);
      await car3.Object.Init("speed racer");


      RaceGrain race = new RaceGrain();
      race.Init("Formula 1", 0.3).Wait();
      car1.SetupGet(c => c.race).Returns(race);
      car2.SetupGet(c => c.race).Returns(race);
      car3.SetupGet(c => c.race).Returns(race);



      await car1.Object.AttendInRace(0);
      await car2.Object.AttendInRace(0);
      await car3.Object.AttendInRace(0);

      await car1.Object.SetSpeed(300);
      await car2.Object.SetSpeed(200);
      await car3.Object.SetSpeed(100);

      await Task.Delay(1000);

      await car1.Object.SetSpeed(300);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(200);
      Assert.IsTrue(await race.IsRaceActive(), "check if the race is still active");
      await Task.Delay(1000);

      await car1.Object.SetSpeed(400);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(200);
      Assert.IsTrue(await race.IsRaceActive(), "check if the race is still active");
      await Task.Delay(1000);

      await car1.Object.SetSpeed(800);
      await car2.Object.SetSpeed(100);
      await car3.Object.SetSpeed(200);
      Assert.IsTrue(await race.IsRaceActive(), "check if the race is still active");
      await Task.Delay(1000);

      await car1.Object.SetSpeed(600);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(500);

      Assert.IsTrue(await race.IsRaceActive(), "check if the race is still active");

      await Task.Delay(1000);

      await car1.Object.SetSpeed(800);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(400);

      Assert.IsFalse(await race.IsRaceActive(), "check if the race is not active");

      Assert.AreEqual<long>((await race.GetCarsStatus()).ElementAt(0).Key, 1, "the first car should win");



      /// should be driving for 1 sec (plus minus) in an average speed of 125MK meaning the car should pass (1 / 60 / 60) * 125) = ~ 0.0347222222222222
      // Assert.IsTrue(car1.GetKMPassed().Result > 0.03 && car.GetKMPassed().Result < 0.04);




    }



  }
}
