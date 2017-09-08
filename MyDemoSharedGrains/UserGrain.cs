using System.Threading.Tasks;
using Orleans;
using MyDemoSharedGrainInterfaces;
namespace MyDemoSharedGrains
{
  /// <summary>
  /// Grain implementation class Grain1.
  /// </summary>
  public class UserGrain : Grain, IUserGrain
  {

    internal  string Email { get; set; }

    public Task SetUserEmail(string userEmail)
    {
      this.Email = userEmail;
      return TaskDone.Done;
    }

    public Task<bool> Login(string password)
    {

      return Task.FromResult<bool>(Email == "avi.kessler@gmail.com" && password == "1");
    }


  }
}
