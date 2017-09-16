using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyDemoSharedGrains;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyDemoSharedGrainInterfaces;

namespace MyDemoUnitTesting
{
  [TestClass]
  public class RaceGrainTests
  {
    [TestMethod]
    public async Task RaceTest()
    {

      Dictionary<long, CarGrain> cars = new Dictionary<long, CarGrain>();

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
      race.Setup(r => r.getCarGrain(It.IsAny<long>())).Returns<long>(id =>
      {

        return cars[id];
      });


      string raceName = "Formula 1";
      await race.Object.Init(raceName, 30);
      car1.SetupGet(c => c.race).Returns(race.Object);
      car2.SetupGet(c => c.race).Returns(race.Object);
      car3.SetupGet(c => c.race).Returns(race.Object);

      await car1.Object.AttendInRace(raceName);
      await car2.Object.AttendInRace(raceName);
      await car3.Object.AttendInRace(raceName);



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

      Assert.AreEqual<long>((await race.Object.GetCarsStatus()).ElementAt(0).CarId, 1, "the first car should win");


    }

    [TestMethod]
    public void RaceChildNumTest()
    {

      RaceGrain race = new RaceGrain();



      Assert.AreEqual<int>(race.CalcChildRaceNum(1, 10), -1, "should not be valid as we are creating child only after the cars overflow the max car allowed ");
      Assert.AreEqual<int>(race.CalcChildRaceNum(11, 10), 0, "If the 11th car is added we want it on the first child(zero based)");
      Assert.AreEqual<int>(race.CalcChildRaceNum(15, 10), 0, "If the 15th car is added we still want it on the first child(zero based)");
      Assert.AreEqual<int>(race.CalcChildRaceNum(21, 10), 1, "If the 21th car is add we want it on the second child (zero based)");
      Assert.AreEqual<int>(race.CalcChildRaceNum(31, 10), 2, "If the 31th car is add we want it on the third child(zero based)");
      Assert.AreEqual<int>(race.CalcChildRaceNum(129, 10), 1, "If the 129th car is add we want it on the third child(zero based)");
      Assert.AreEqual<int>(race.CalcChildRaceNum(131, 10), 2, "If the 131th car is add we want it on the third child(zero based)");
      Assert.AreEqual<int>(race.CalcChildRaceNum(321, 10), 1, "If the 321th car is add we want it on the second child(zero based)");


    }


    [TestMethod]
    public async Task RaceMassiveAggregateTest()
    {

      Dictionary<long, Mock<CarGrain>> cars = new Dictionary<long, Mock<CarGrain>>();
      Dictionary<string, Mock<RaceGrain>> races = new Dictionary<string, Mock<RaceGrain>>();

      Mock<TimeProvider> time = new Mock<TimeProvider>();
      DateTime newTime = DateTime.Now;
      time.Setup(t => t.Now).Returns(newTime);

      int numOfCars = 400;
      int maxNumOfCarPerRaceObject = 10;
      int winningCar = 65;

      for (int i = 0; i < numOfCars; i++)
      {
        Mock<CarGrain> car = new Moq.Mock<CarGrain>(Moq.MockBehavior.Loose);
        car.SetupGet(c => c.carId).Returns(i);
        car.SetupGet(c => c.Time).Returns(time.Object);
        cars.Add(i, car);
      }
      await Task.WhenAll(cars.Select(c => c.Value.Object.Init($"Test car #{c.Key}")).ToArray());

      Mock<RaceGrain> rootRace = createRace(races, cars, time.Object, maxNumOfCarPerRaceObject);

      string rootRaceName = "Formula 1";
      races.Add(rootRaceName, rootRace);
      await rootRace.Object.Init(rootRaceName, 30);

      foreach (Mock<CarGrain> car in cars.Values)
      {
        car.SetupGet(c => c.race).Returns(rootRace.Object);
      }



      await Task.WhenAll(cars.Values.Select(c => c.Object.AttendInRace(rootRaceName)));

      Assert.AreEqual<int>(races.Count, numOfCars / maxNumOfCarPerRaceObject, "Should be (numOfCars / maxNumOfCarPerRaceObject) of race objects");
      Random rand = new Random(DateTime.Now.Millisecond);

      while (await rootRace.Object.IsRaceActive())
      {

        // drive the cars
        await Task.WhenAll(cars.Select(c =>
        {
          if (c.Key == winningCar) return c.Value.Object.SetSpeed(rand.Next(500, 600));
          return c.Value.Object.SetSpeed(rand.Next(50, 200));
        }));
        // lets move forward in time (we only need 1.21 gigawatts !!!!)
        newTime = newTime.AddSeconds(100);
        time.Setup(t => t.Now).Returns(newTime);
      }

      IEnumerable<ICarRaceRecord> results = await rootRace.Object.GetCarsStatus();
      Assert.AreEqual<int>((int)results.ElementAt(0).CarId, winningCar, "The winner should be the value of winningCar");


    }

    private Mock<RaceGrain> createRace(Dictionary<string, Mock<RaceGrain>> races, Dictionary<long, Mock<CarGrain>> cars, TimeProvider time, int maxNumOfCarPerRaceObject)
    {
      Mock<RaceGrain> raceResult = new Moq.Mock<RaceGrain>(Moq.MockBehavior.Loose);
      raceResult.Setup(r => r.Time).Returns(time);
      raceResult.Setup(r => r.getCarGrain(It.IsAny<long>())).Returns<long>(id => cars[id].Object);
      raceResult.Setup(r => r.MaxCarPerRaceGrain).Returns(maxNumOfCarPerRaceObject);
      raceResult.Setup(r => r.getRaceGrain(It.IsAny<string>())).Returns<string>(raceid =>
      {
        if (races.ContainsKey(raceid)) return races[raceid].Object;
        Mock<RaceGrain> race = createRace(races, cars, time, maxNumOfCarPerRaceObject);
        races.Add(raceid, race);
        return race.Object;
      });
      return raceResult;
    }

  }
}
