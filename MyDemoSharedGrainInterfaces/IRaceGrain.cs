using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrainInterfaces
{
  public interface IRaceGrain : Orleans.IGrainWithIntegerKey
  {



    Task Init(string raceName, double TotalKM);
    Task joinCarToRace(long carId);

    Task<bool> reportCarKMPassed(long carId, double KM);

    Task<IEnumerable<KeyValuePair<long, double>>> GetCarsStatus();

    Task<bool> IsRaceActive();
  }
}
