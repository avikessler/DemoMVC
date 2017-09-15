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
    static SiloHost siloHost;
    static void Main(string[] args)
    {


      // First, configure and start a local silo
      siloHost = new SiloHost(System.Net.Dns.GetHostName());
      // The Cluster config is quirky and weird to configure in code, so we're going to use a config file
      siloHost.ConfigFileName = "OrleansConfiguration.xml";

      siloHost.InitializeOrleansSilo();
      siloHost.StartOrleansSilo();

      Console.WriteLine("Silo started.");



     

      //
      // This is the place for your test code.
      //

      Console.WriteLine("\nPress Enter to terminate...");
      Console.ReadLine();

      // Shut down
      //client.Close();
      siloHost.ShutdownOrleansSilo();
    }
  }
}
