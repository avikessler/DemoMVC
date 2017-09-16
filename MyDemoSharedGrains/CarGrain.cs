using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDemoSharedGrainInterfaces;
using Orleans.Providers;
using Orleans.Concurrency;

namespace MyDemoSharedGrains
{

  public class CarState
  {
    public double CurrentSpeed { get; set; }
    public DateTime? LastTimeSpeedReported { get; set; }
    public double KMPassd { get; set; }
    public string Name { get; set; }
    public long? attendraceID { get; set; }
  }

  [StorageProvider(ProviderName = "MongoStore")]
  [Reentrant]
  public class CarGrain : Grain<CarState>, ICarGrain
  {

    private bool testingMode = false;
    TimeProvider _time = new TimeProvider();
    public virtual TimeProvider Time
    {
      get
      {
        return _time;
      }
    }

    public virtual long carId
    {
      get
      {
        return this.GetPrimaryKeyLong();
      }
    }


    public virtual IRaceGrain race
    {
      get
      {
        return this.GrainFactory.GetGrain<IRaceGrain>(State.attendraceID.Value);
      }
    }

    public async Task AttendInRace(long raceId)
    {
      State.attendraceID = raceId;
      await race.joinCarToRace(this.carId);
      if (!testingMode) await base.WriteStateAsync();
    }

    public Task<double> GetKMPassed()
    {
      return Task.FromResult<double>(State.KMPassd);
    }

    public async Task Init(string carName)
    {
      if (State == null)
      {
        State = new CarState(); // in case we are int testing mode
        testingMode = true;
      }
      State.KMPassd = 0;
      State.CurrentSpeed = 0;
      State.LastTimeSpeedReported = null;
      State.Name = carName;

      if (!testingMode) await base.WriteStateAsync();
      return;

    }

    public Task SetSpeed(double speed)
    {
      if (State.LastTimeSpeedReported.HasValue)
      { // only if the car have started all ready
        State.KMPassd += Time.Now.Subtract(State.LastTimeSpeedReported.Value).TotalHours * ((State.CurrentSpeed + speed) / 2);
      }

      // set values
      State.CurrentSpeed = speed;
      State.LastTimeSpeedReported = Time.Now;



      if (!testingMode) base.WriteStateAsync();

      return Task.CompletedTask;
    }
  }
}
