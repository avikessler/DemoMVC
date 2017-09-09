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

    public async Task AttendInRace(int raceId)
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
        KMPassd += DateTime.Now.Subtract(LastTimeSpeedReported.Value).TotalHours * ((CurrentSpeed + speed) / 2);
      }

      // set values
      CurrentSpeed = speed;
      LastTimeSpeedReported = DateTime.Now;


      if (attendraceID.HasValue)
      {

        await race.reportCarKMPassed(this.carId, this.KMPassd);

      }

    }
  }
}
