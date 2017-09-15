using Orleans;
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
    public Dictionary<string, CarRaceRecord> Cars { get; set; }
    public string RaceName { get; set; }
    public double TotalRaceKM { get; set; }
  }
  [StorageProvider(ProviderName = "MongoStore")]
  public class RaceGrain : Grain<RaceState>, MyDemoSharedGrainInterfaces.IRaceGrain
  {



    TimeProvider _time = new TimeProvider();
    public virtual TimeProvider Time
    {
      get
      {
        return _time;
      }
    }

    public Task<IEnumerable<KeyValuePair<long, double>>> GetCarsStatus()
    {
      return Task.FromResult<IEnumerable<KeyValuePair<long, double>>>(
       State.Cars.OrderByDescending(c => c.Value.CarKMPassed).ThenBy(c => c.Value.CarLastKMReported)
        .Select(c => new KeyValuePair<long, double>(c.Value.CarId, c.Value.CarKMPassed)).ToArray()
        );
    }

    public async Task Init(string raceName, double TotalKM)
    {

      State.Cars = new Dictionary<string, CarRaceRecord>();
      State.RaceName = raceName;
      State.TotalRaceKM = TotalKM;
      await base.WriteStateAsync();

    }

    public Task<bool> IsRaceActive()
    {
      if (!State.Cars.Any()) throw new InvalidOperationException("Race don't have cars");

      return Task.FromResult<bool>(
         State.Cars.Values.Select(c => c.CarKMPassed).Min() < State.TotalRaceKM
        );
    }

    public async Task joinCarToRace(long carId)
    {
      State.Cars.Add(carId.ToString(), new CarRaceRecord
      {
        CarId = carId,
        CarKMPassed = 0,
        CarLastKMReported = Time.Now
      });
      await base.WriteStateAsync();

    }

    public Task<bool> reportCarKMPassed(long carId, double KM)
    {

      if (State.TotalRaceKM > KM)
      {
        State.Cars[carId.ToString()].CarLastKMReported = Time.Now;
        State.Cars[carId.ToString()].CarKMPassed = KM;
        base.WriteStateAsync();
        return Task.FromResult<bool>(true);
      }
      else
      {
        if (State.Cars[carId.ToString()].CarKMPassed != State.TotalRaceKM)
        {
          State.Cars[carId.ToString()].CarKMPassed = State.TotalRaceKM;
          base.WriteStateAsync();
        }

        return Task.FromResult<bool>(false);
      }


    }
  }

  public class CarRaceRecord
  {
    public long CarId { get; set; }
    public double CarKMPassed { get; set; }
    public DateTime CarLastKMReported { get; set; }
  }
}
