using System.Threading.Tasks;
using Orleans;

namespace MyDemoSharedGrainInterfaces
{
  /// <summary>
  /// Grain interface IGrain1
  /// </summary>
  public interface IUserGrain : IGrainWithStringKey
  {


    Task<bool> Login(string password);

    Task<bool> ValidateUIDSignature(string UID, string timestamp, string signature);

  }
}
