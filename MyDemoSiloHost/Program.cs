using System;

using Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace MyDemoSiloHost
{
  /// <summary>
  /// Orleans test silo host
  /// </summary>
  public class Program
  {
    static void Main(string[] args)
    {


      // First, configure and start a local silo
      var siloConfig = ClusterConfiguration.LocalhostPrimarySilo(22222);

      var silo = new SiloHost("TestSilo", siloConfig);
      silo.InitializeOrleansSilo();
      silo.StartOrleansSilo();

      Console.WriteLine("Silo started.");

      // Then configure and connect a client.
      var clientConfig = ClientConfiguration.LocalhostSilo();

      Console.WriteLine("Client connected.");

      GrainClient.Initialize(clientConfig);

      var userBL = GrainClient.GrainFactory.GetGrain<MyDemoSharedGrainInterfaces.IUserGrain>("avi.kessler@gmail.com");
      userBL.SetUserEmail("avi.kessler@gmail.com").Wait(); ;
      Console.WriteLine("\n\n{0}\n\n", userBL.Login("1").Result);

      //
      // This is the place for your test code.
      //

      Console.WriteLine("\nPress Enter to terminate...");
      Console.ReadLine();

      // Shut down
      //client.Close();
      silo.ShutdownOrleansSilo();
    }
  }
}
