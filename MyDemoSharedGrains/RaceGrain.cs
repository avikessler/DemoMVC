using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrains
{
  public class RaceGrain : Grain, MyDemoSharedGrainInterfaces.IRaceGrain
  {

    internal Dictionary<long, CarRaceRecord> cars { get; set; }
    internal string RaceName { get; set; }
    internal double TotalRaceKM { get; set; }

    public Task<IEnumerable<KeyValuePair<long, double>>> GetCarsStatus()
    {
      return Task.FromResult<IEnumerable <KeyValuePair<long, double>>>(
        cars.OrderByDescending(c => c.Value.CarKMPassed).Select(c => new KeyValuePair<long, double>(c.Value.CarId, c.Value.CarKMPassed))
        );
    }

    public Task Init(string raceName, double TotalKM)
    {
      cars = new Dictionary<long, CarRaceRecord>();
      RaceName = raceName;
      TotalRaceKM = TotalKM;
      return Task.CompletedTask;
    }

    public Task<bool> IsRaceActive()
    {
      if (!cars.Any()) throw new InvalidOperationException("Race don't have cars");

      return Task.FromResult<bool>(
        cars.Values.Select(c => c.CarKMPassed).Min() <= TotalRaceKM
        );
    }

    public Task joinCarToRace(long carId)
    {
      cars.Add(carId, new CarRaceRecord
      {
        CarId = carId,
        CarKMPassed = 0,
        CarLastKMReported = DateTime.Now
      });

      return Task.CompletedTask;
    }

    public Task<bool> reportCarKMPassed(long carId, double KM)
    {
      cars[carId].CarLastKMReported = DateTime.Now;
      cars[carId].CarKMPassed = KM;

      return Task.FromResult<bool>(TotalRaceKM < KM);

    }
  }

  internal class CarRaceRecord
  {
    public long CarId { get; set; }
    public double CarKMPassed { get; set; }
    public DateTime CarLastKMReported { get; set; }
  }
}
