using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrainInterfaces
{
  public interface IRaceGrain : Orleans.IGrainWithStringKey
  {



    Task Init(string raceName, double TotalKM);
    Task joinCarToRace(long carId);

    Task<IEnumerable<ICarRaceRecord>> GetCarsStatus();

    Task<bool> IsRaceActive();



  }
  public interface ICarRaceRecord
  {
    long CarId { get; set; }
    double CarKMPassed { get; set; }
    DateTime CarLastKMReported { get; set; }
  }
}
