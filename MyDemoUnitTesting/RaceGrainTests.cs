using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDemoSharedGrains;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace MyDemoUnitTesting
{
  [TestClass]
  public class RaceGrainTests
  {
    [TestMethod]
    public async Task RaceTest()
    {
 
      Dictionary<long,CarGrain> cars = new Dictionary<long,CarGrain>();

      Mock<TimeProvider> time = new Mock<TimeProvider>();
      DateTime newTime = DateTime.Now;
      time.Setup(t => t.Now).Returns(newTime);

      Mock<CarGrain> car1 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car1.SetupGet(c => c.carId).Returns(1);
      car1.SetupGet(c => c.Time).Returns(time.Object);
      cars.Add(1, car1.Object);
      await car1.Object.Init("speedy gonzales");


      Mock<CarGrain> car2 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car2.SetupGet(c => c.carId).Returns(2);
      car2.SetupGet(c => c.Time).Returns(time.Object);
      cars.Add(2, car2.Object);
      await car2.Object.Init("Bimba");

      Mock<CarGrain> car3 = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
      car3.SetupGet(c => c.carId).Returns(3);
      car3.SetupGet(c => c.Time).Returns(time.Object);
      cars.Add(3, car3.Object);
      await car3.Object.Init("speed racer");


      Mock<RaceGrain> race = new Moq.Mock<RaceGrain>(Moq.MockBehavior.Loose);
      race.Setup(r => r.Time).Returns(time.Object);
      race.Setup(r => r.getCarGrain(It.IsAny<long>())).Returns<long>(id => {

        return cars[id];
        });
        
  

      await race.Object.Init("Formula 1", 30);
      car1.SetupGet(c => c.race).Returns(race.Object);
      car2.SetupGet(c => c.race).Returns(race.Object);
      car3.SetupGet(c => c.race).Returns(race.Object);

      await car1.Object.AttendInRace(0);
      await car2.Object.AttendInRace(0);
      await car3.Object.AttendInRace(0);

      await car1.Object.SetSpeed(300);
      await car2.Object.SetSpeed(200);
      await car3.Object.SetSpeed(100);

      newTime = newTime.AddSeconds(100);
      time.Setup(t => t.Now).Returns(newTime);

      await car1.Object.SetSpeed(300);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(200);
      Assert.IsTrue(await race.Object.IsRaceActive(), "check if the race is still active");
      newTime = newTime.AddSeconds(100);
      time.Setup(t => t.Now).Returns(newTime);


      await car1.Object.SetSpeed(400);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(200);
      Assert.IsTrue(await race.Object.IsRaceActive(), "check if the race is still active");
      newTime = newTime.AddSeconds(100);
      time.Setup(t => t.Now).Returns(newTime);


      await car1.Object.SetSpeed(800);
      await car2.Object.SetSpeed(100);
      await car3.Object.SetSpeed(200);
      Assert.IsTrue(await race.Object.IsRaceActive(), "check if the race is still active");
      newTime = newTime.AddSeconds(100);
      time.Setup(t => t.Now).Returns(newTime);


      await car1.Object.SetSpeed(600);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(500);

      Assert.IsTrue(await race.Object.IsRaceActive(), "check if the race is still active");
      newTime = newTime.AddSeconds(100);
      time.Setup(t => t.Now).Returns(newTime);

      await car1.Object.SetSpeed(800);
      await car2.Object.SetSpeed(400);
      await car3.Object.SetSpeed(400);

      Assert.IsFalse(await race.Object.IsRaceActive(), "check if the race is not active");

      Assert.AreEqual<long>((await race.Object.GetCarsStatus()).ElementAt(0).CarId , 1, "the first car should win");


    }



  }
}
