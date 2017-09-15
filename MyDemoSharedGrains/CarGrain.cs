using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDemoSharedGrainInterfaces;
namespace MyDemoSharedGrains
{
  public class CarGrain : Grain, ICarGrain
  {

    internal double CurrentSpeed { get; set; }
    internal DateTime? LastTimeSpeedReported { get; set; }
    internal double KMPassd { get; set; }
    internal string Name { get; set; }
    internal long? attendraceID { get; set; }
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
        return this.GrainFactory.GetGrain<IRaceGrain>(attendraceID.Value);
      }
    }

    public async Task AttendInRace(long raceId)
    {
      attendraceID = raceId;
      await race.joinCarToRace(this.carId);

    }

    public Task<double> GetKMPassed()
    {
      return Task.FromResult<double>(KMPassd);
    }

    public Task Init(string carName)
    {
      KMPassd = 0;
      CurrentSpeed = 0;
      LastTimeSpeedReported = null;
      Name = carName;
      return Task.CompletedTask;

    }

    public async Task SetSpeed(double speed)
    {
      if (LastTimeSpeedReported.HasValue)
      { // only if the car have started all ready
        KMPassd += Time.Now.Subtract(LastTimeSpeedReported.Value).TotalHours * ((CurrentSpeed + speed) / 2);
      }

      // set values
      CurrentSpeed = speed;
      LastTimeSpeedReported = Time.Now;


      if (attendraceID.HasValue)
      {

        await race.reportCarKMPassed(this.carId, this.KMPassd);

      }

    }
  }
}
