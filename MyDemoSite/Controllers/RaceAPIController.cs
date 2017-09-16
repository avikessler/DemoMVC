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


    static Random rand = new Random(DateTime.Now.Millisecond);
    public async Task<string> StartNewRace([FromUri] string name, [FromUri] double KM)
    {

    
      var race = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.IRaceGrain>(name);
      await race.Init(name, KM);
      return name;
    }


    public async Task<long> AddCarToRace(string carName, string raceId)
    {

      long carId = carName.GetHashCode();
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



    public async Task<Object> RaceStatus(string raceId)
    {
      try
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
      catch (Exception  ex)
      {

        throw;
      }

    
    }

  }
}
