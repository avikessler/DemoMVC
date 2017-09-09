using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Orleans;
namespace MyDemoSite.Controllers
{

  public class RaceAPIController : ApiController
  {



    public async Task<long> StartNewRace([FromUri] string name, [FromUri] double KM)
    {
      Random rand = new Random(DateTime.Now.Millisecond);
      long raceId = rand.Next();
      var race = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.IRaceGrain>(raceId);
      await race.Init(name, KM);
      return raceId;
    }


    public async Task<long> AddCarToRace(string carName, long raceId)
    {
      Random rand = new Random(DateTime.Now.Millisecond);
      long carId = rand.Next();
      var car = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.ICarGrain>(carId);
      await car.Init(carName);
      await car.AttendInRace(raceId);

      return carId;

    }


    public async Task ReportCarSpeed(long carId, double speed)
    {

      var car = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.ICarGrain>(carId);
      await car.SetSpeed(speed);

    }



    public async Task<Object> RaceStatus(long raceId)
    {

      var race = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.IRaceGrain>(raceId);

      bool active = await race.IsRaceActive();
      var carStatuses = await race.GetCarsStatus();
      var result = new
      {
        raceActive = active,
        carStatuses = carStatuses

      };

      return result;
    }

  }
}
