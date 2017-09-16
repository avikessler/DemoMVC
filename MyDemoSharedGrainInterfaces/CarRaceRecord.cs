using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrainInterfaces
{

  public class CarRaceRecord : ICarRaceRecord
  {
    public long CarId { get; set; }
    public double CarKMPassed { get; set; }
    public DateTime CarLastKMReported { get; set; }
  }
}
