using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDemoSharedGrains
{
  public class TimeProvider
  {

    virtual public DateTime Now
    {
      get
      {
        return DateTime.Now;
      }
    }




  }
}
