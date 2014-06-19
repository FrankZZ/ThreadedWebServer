using System;
using System.Threading;

namespace WebServer.Models
{
	public static class Init
	{
		private static Server server = null;
		private static ControlServer controlServer = null;
		private static bool Terminate = false;

		public static void Main(string[] args)
		{
			new LoggerQueue();
			
			Configuration.Initialize();
			Configuration.Read();

			Console.BufferHeight = 600;// For Console size
			Console.BufferWidth = 900;// For Console size

			/*
			ConfigLoader.Entries.Host = "127.0.0.1";
			ConfigLoader.Entries.webPort = "3333";
			ConfigLoader.Entries.controlPort = "3334";
			ConfigLoader.Entries.directoryBrowsing = "true";
			ConfigLoader.Entries.defaultPage = "index.html";
			
			ConfigLoader.Write();
			*/

			Init.server = new Server(
				Configuration.Entries.Host,
				Convert.ToInt32(Configuration.Entries.webPort)
			);

			Init.controlServer = new ControlServer(
				Configuration.Entries.Host,
				Convert.ToInt32(Configuration.Entries.controlPort)
			);

			//new Database().RegisterUser("user", "tester123");

			server.Run();
			controlServer.Run();

			//new Thread(new ThreadStart(RestartServer)).Start();

		}
		private static void RestartServer()
		{
			while (Init.Terminate == false)
			{
				Init.server.Shutdown.WaitOne();
				
				Init.server = new Server(
					Configuration.Entries.Host,
					Convert.ToInt32(Configuration.Entries.webPort)
				);
				
				Init.controlServer.Shutdown.WaitOne();

				Init.controlServer = new ControlServer(
					Configuration.Entries.Host,
					Convert.ToInt32(Configuration.Entries.controlPort)
				);

				Configuration.ShutdownRequested = false;

				Init.server.Run();

				Init.controlServer.Run();

			}
		}
	}
}