using MyDemoSharedGrainInterfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrains
{
  public class RaceState
  {
    public DateTime LastGatherTime;
    public List<CarRaceRecord> Cars { get; set; }
    public string RaceName { get; set; }
    public double TotalRaceKM { get; set; }
  }
  [StorageProvider(ProviderName = "MongoStore")]
  [Reentrant]
  public class RaceGrain : Grain<RaceState>, MyDemoSharedGrainInterfaces.IRaceGrain
  {
    const int CacheTimemSec = 1000;
    private Task gatheringTask = null; // used for the gathering the information from all binded cars, in-order to prevent parallel gathering process in the same time
    private bool testingMode = false;

    TimeProvider _time = new TimeProvider();
    public virtual TimeProvider Time
    {
      get
      {
        return _time;
      }
    }


    public async Task<IEnumerable<ICarRaceRecord>> GetCarsStatus()
    {
      if (Time.Now.Subtract(State.LastGatherTime).TotalMilliseconds > CacheTimemSec) // if there was a request in the cache period then just return the cached version
      {

        if (gatheringTask != null) await gatheringTask; // check if there is already a gathering process running, if so then just return the same answer for the original process.
        else
        { // nop this there isn't a process running, the lets create one.
          gatheringTask = getherCarsStatus();
          await gatheringTask;
          gatheringTask = null; // after finish lets clear the task(process) variable .
        }

      }
      return
       State.Cars;
    }
    private async Task getherCarsStatus()
    {
      await Task.WhenAll(State.Cars.Select(cr => updateCarRecord(cr)));
      State.Cars = State.Cars.OrderByDescending(c => c.CarKMPassed).ThenBy(c => c.CarLastKMReported).ToList();
      State.LastGatherTime = Time.Now;
      if (!testingMode) await base.WriteStateAsync();
    }

    private async Task updateCarRecord(CarRaceRecord cr)
    {
      ICarGrain car = getCarGrain(cr.CarId);
      cr.CarKMPassed = await car.GetKMPassed();
      cr.CarLastKMReported = Time.Now;
    }


    public virtual ICarGrain getCarGrain(long carId) // for mocking purpose 
    {
      return this.GrainFactory.GetGrain<ICarGrain>(carId); 
    }



    public async Task Init(string raceName, double TotalKM)
    {
      if (State == null)
      {
        State = new RaceState();  // in case we are int testing mode
        testingMode = true;
      }
      State.Cars = new List<CarRaceRecord>();
      State.RaceName = raceName;
      State.TotalRaceKM = TotalKM;
      if (!testingMode) await base.WriteStateAsync();
    }

    public async Task<bool> IsRaceActive()
    {
      if (!State.Cars.Any()) throw new InvalidOperationException("Race don't have cars");
      await GetCarsStatus();
      return State.Cars.Select(c => c.CarKMPassed).Min() < State.TotalRaceKM;
    }

    public async Task joinCarToRace(long carId)
    {
      State.Cars.Add(new CarRaceRecord
      {
        CarId = carId,
        CarKMPassed = 0,
        CarLastKMReported = Time.Now
      });
      if (!testingMode) await base.WriteStateAsync();

    }


  }

  public class CarRaceRecord : ICarRaceRecord
  {
    public long CarId { get; set; }
    public double CarKMPassed { get; set; }
    public DateTime CarLastKMReported { get; set; }
  }
}
