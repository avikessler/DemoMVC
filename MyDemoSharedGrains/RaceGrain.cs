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
    public List<string> ChildrensRaces { get; set; }
    public int DescendedCarsCount { get; set; }
    public string RaceName { get; set; }
    public double TotalRaceKM { get; set; }
  }
  [StorageProvider(ProviderName = "MongoStore")]
  [Reentrant]
  public class RaceGrain : Grain<RaceState>, MyDemoSharedGrainInterfaces.IRaceGrain
  {
    public int coint { get { return State.DescendedCarsCount; } }

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
    public virtual int MaxCarPerRaceGrain// for mocking purpose 
    {
      get
      {
        return 10;
      }
    }
    public Task<IEnumerable<ICarRaceRecord>> GetCarsStatus()
    {
      return GetCarsStatus(false);
    }
    public async Task<IEnumerable<ICarRaceRecord>> GetCarsStatus(bool exludeChildCalc)
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
      if (exludeChildCalc) return State.Cars;

      IEnumerable<IEnumerable<ICarRaceRecord>> results = await Task.WhenAll<IEnumerable<ICarRaceRecord>>(
           State.ChildrensRaces.Select(
              cr => this.getRaceGrain(cr).GetCarsStatus()  // create task for each child and get it's Status
              ));

      return State.Cars.Concat(results.SelectMany(i => i)).OrderByDescending(c => c.CarKMPassed).ThenBy(c => c.CarLastKMReported).ToList();


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
      if (cr.CarKMPassed < State.TotalRaceKM)
      {
        ICarGrain car = getCarGrain(cr.CarId);
        double km = await car.GetKMPassed();
        if (km < State.TotalRaceKM)
        {
          cr.CarKMPassed = km;
          cr.CarLastKMReported = Time.Now;
        }
        else
        {
          cr.CarKMPassed = State.TotalRaceKM;
          cr.CarLastKMReported = Time.Now;
        }
      }
    }


    public virtual ICarGrain getCarGrain(long carId) // for mocking purpose 
    {
      return this.GrainFactory.GetGrain<ICarGrain>(carId);
    }
    public virtual IRaceGrain getRaceGrain(string raceId) // for mocking purpose 
    {
      return this.GrainFactory.GetGrain<IRaceGrain>(raceId);
    }


    public async Task Init(string raceName, double TotalKM)
    {
      if (State == null)
      {
        State = new RaceState();  // in case we are int testing mode
        testingMode = true;
      }
      State.Cars = new List<CarRaceRecord>();
      State.ChildrensRaces = new List<string>();
      State.DescendedCarsCount = 0;
      State.RaceName = raceName;
      State.TotalRaceKM = TotalKM;
      if (!testingMode) await base.WriteStateAsync();
    }

    public async Task<bool> IsRaceActive()
    {
      if (!State.Cars.Any()) throw new InvalidOperationException("Race don't have cars");
      await GetCarsStatus(true);

    

      IEnumerable<bool> results = await Task.WhenAll<bool>(
          State.ChildrensRaces.Select(
             cr => this.getRaceGrain(cr).IsRaceActive()  // create task for each child and get it's Status
             ));
      return (State.Cars.Select(c => c.CarKMPassed).Min() < State.TotalRaceKM) || results.Any(b => b);


    }


    public int CalcChildRaceNum(int decendentCarsCount, int maxCarPerRace)
    {
      if (decendentCarsCount < maxCarPerRace) return -1;
      var result = decendentCarsCount;
      result -= maxCarPerRace;
      result = result / maxCarPerRace;

      return result % maxCarPerRace;


    }
    public async Task joinCarToRace(long carId)
    {

      if (State.DescendedCarsCount < MaxCarPerRaceGrain)
      {
        State.Cars.Add(new CarRaceRecord
        {
          CarId = carId,
          CarKMPassed = 0,
          CarLastKMReported = Time.Now
        });
      }
      else
      {
        string childRaceID = this.State.RaceName + '.' + this.CalcChildRaceNum(State.DescendedCarsCount, MaxCarPerRaceGrain).ToString();
        IRaceGrain childRace = getRaceGrain(childRaceID);

        if (!State.ChildrensRaces.Contains(childRaceID))
        { // if we need to create new child race
          await childRace.Init(childRaceID, State.TotalRaceKM);
          State.ChildrensRaces.Add(childRaceID);
        }
        await childRace.joinCarToRace(carId); // add the car to the child race to be handled by the child
      }
      State.DescendedCarsCount++;
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
