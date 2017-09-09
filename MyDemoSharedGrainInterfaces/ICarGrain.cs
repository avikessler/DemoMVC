using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrainInterfaces
{
 public interface ICarGrain : IGrainWithIntegerKey
  {

    Task Init(string name);
    Task SetSpeed(double speed);
    Task<double> GetKMPassed();

    Task AttendInRace(long raceId);
  
  }
}
