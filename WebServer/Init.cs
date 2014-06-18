using System;

namespace WebServer.Models
{
	public static class Init
	{
		public static void Main(string[] args)
		{
			new LoggerQueue();
			
			Configuration.Initialize();
			Configuration.Read();
			
			/*
			ConfigLoader.Entries.Host = "127.0.0.1";
			ConfigLoader.Entries.webPort = "3333";
			ConfigLoader.Entries.controlPort = "3334";
			ConfigLoader.Entries.directoryBrowsing = "true";
			ConfigLoader.Entries.defaultPage = "index.html";
			
			ConfigLoader.Write();
			*/

			var server = new Server(
				Configuration.Entries.Host,
				Convert.ToInt32(Configuration.Entries.webPort)
			);

			var controlServer = new ControlServer(
				Configuration.Entries.Host,
				Convert.ToInt32(Configuration.Entries.controlPort)
			);

			// new Database().RegisterUser("frank", "tester");

			server.Run();
			controlServer.Run();

		}
	}
}